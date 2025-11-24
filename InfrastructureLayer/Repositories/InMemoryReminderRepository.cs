using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryReminderRepository : IReminderRepository
{
    private readonly Dictionary<Guid, Reminder> _reminders = new();
    private readonly object _lock = new();

    public InMemoryReminderRepository(SeedDataService? seedDataService = null)
    {
        bool remindersLoaded = false;
        
        if (seedDataService != null)
        {
            var reminders = seedDataService.GetReminders();
            foreach (var reminder in reminders)
            {
                try
                {
                    _reminders[reminder.Id] = reminder;
                    remindersLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default reminders if SeedDataService is not available or no reminders were loaded
        if (!remindersLoaded)
        {
            InitializeDefaultReminders();
        }
    }

    private void InitializeSeedData(SeedDataService seedDataService)
    {
        var reminders = seedDataService.GetReminders();
        foreach (var reminder in reminders)
        {
            try
            {
                _reminders[reminder.Id] = reminder;
            }
            catch
            {
                continue;
            }
        }
    }

    private void InitializeDefaultReminders()
    {
    }

    public Task<Reminder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _reminders.TryGetValue(id, out var reminder);
            return Task.FromResult<Reminder?>(reminder?.IsDeleted == false ? reminder : null);
        }
    }

    public Task<IEnumerable<Reminder>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Reminder>>(
                _reminders.Values.Where(r => !r.IsDeleted).ToList()
            );
        }
    }

    public Task<Reminder> CreateAsync(Reminder reminder, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_reminders.ContainsKey(reminder.Id))
                throw new InvalidOperationException($"Reminder with id {reminder.Id} already exists");

            _reminders[reminder.Id] = reminder;
            return Task.FromResult(reminder);
        }
    }

    public Task<Reminder> UpdateAsync(Reminder reminder, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_reminders.ContainsKey(reminder.Id))
                throw new InvalidOperationException($"Reminder with id {reminder.Id} not found");

            _reminders[reminder.Id] = reminder;
            return Task.FromResult(reminder);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reminder = await GetByIdAsync(id, cancellationToken);
        if (reminder == null)
            return false;

        lock (_lock)
        {
            reminder.MarkAsDeleted();
            _reminders[reminder.Id] = reminder;
            return true;
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _reminders.Clear();
            
            bool remindersLoaded = false;
            if (seedDataService != null)
            {
                var reminders = seedDataService.GetReminders();
                foreach (var reminder in reminders)
                {
                    try
                    {
                        _reminders[reminder.Id] = reminder;
                        remindersLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!remindersLoaded)
            {
                InitializeDefaultReminders();
            }
        }
    }
}

