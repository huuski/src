using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryNotificationRepository : INotificationRepository
{
    private readonly Dictionary<Guid, Notification> _notifications = new();
    private readonly object _lock = new();

    public InMemoryNotificationRepository(SeedDataService? seedDataService = null)
    {
        bool notificationsLoaded = false;
        
        if (seedDataService != null)
        {
            var notifications = seedDataService.GetNotifications();
            foreach (var notification in notifications)
            {
                try
                {
                    _notifications[notification.Id] = notification;
                    notificationsLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default notifications if SeedDataService is not available or no notifications were loaded
        if (!notificationsLoaded)
        {
            InitializeDefaultNotifications();
        }
    }

    private void InitializeSeedData(SeedDataService seedDataService)
    {
        var notifications = seedDataService.GetNotifications();
        foreach (var notification in notifications)
        {
            try
            {
                _notifications[notification.Id] = notification;
            }
            catch
            {
                continue;
            }
        }
    }

    private void InitializeDefaultNotifications()
    {
        var notif1 = new Notification(
            "Bem-vindo ao sistema!",
            "Sua conta foi criada com sucesso. Explore nossos serviços e produtos.",
            "welcome-icon"
        );
        _notifications[notif1.Id] = notif1;

        var notif2 = new Notification(
            "Negociação finalizada",
            "Consulte suas estatísticas e resultados para verificar o comissionamento previsto.",
            "negotiation-icon"
        );

        _notifications[notif2.Id] = notif2;
    }

    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _notifications.TryGetValue(id, out var notification);
            return Task.FromResult<Notification?>(notification?.IsDeleted == false ? notification : null);
        }
    }

    public Task<IEnumerable<Notification>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Notification>>(
                _notifications.Values.Where(n => !n.IsDeleted).ToList()
            );
        }
    }

    public Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_notifications.ContainsKey(notification.Id))
                throw new InvalidOperationException($"Notification with id {notification.Id} already exists");

            _notifications[notification.Id] = notification;
            return Task.FromResult(notification);
        }
    }

    public Task<Notification> UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_notifications.ContainsKey(notification.Id))
                throw new InvalidOperationException($"Notification with id {notification.Id} not found");

            _notifications[notification.Id] = notification;
            return Task.FromResult(notification);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await GetByIdAsync(id, cancellationToken);
        if (notification == null)
            return false;

        lock (_lock)
        {
            notification.MarkAsDeleted();
            _notifications[notification.Id] = notification;
            return true;
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _notifications.Clear();
            
            bool notificationsLoaded = false;
            if (seedDataService != null)
            {
                var notifications = seedDataService.GetNotifications();
                foreach (var notification in notifications)
                {
                    try
                    {
                        _notifications[notification.Id] = notification;
                        notificationsLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!notificationsLoaded)
            {
                InitializeDefaultNotifications();
            }
        }
    }
}

