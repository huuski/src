using ApplicationLayer.DTOs.Reminder;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;

namespace ApplicationLayer.Services;

public class ReminderService : IReminderService
{
    private readonly IReminderRepository _reminderRepository;

    public ReminderService(IReminderRepository reminderRepository)
    {
        _reminderRepository = reminderRepository ?? throw new ArgumentNullException(nameof(reminderRepository));
    }

    public async Task<ReminderDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reminder = await _reminderRepository.GetByIdAsync(id, cancellationToken);
        if (reminder == null)
            throw new ArgumentException($"Reminder with id {id} not found", nameof(id));

        return MapToDto(reminder);
    }

    public async Task<IEnumerable<ReminderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var reminders = await _reminderRepository.GetAllAsync(cancellationToken);
        return reminders.Select(MapToDto);
    }

    public async Task<ReminderDto> CreateAsync(CreateReminderDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var reminder = new Reminder(
            dto.Title,
            dto.Description,
            dto.Priority
        );

        var createdReminder = await _reminderRepository.CreateAsync(reminder, cancellationToken);
        return MapToDto(createdReminder);
    }

    public async Task<ReminderDto> UpdateAsync(Guid id, UpdateReminderDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var reminder = await _reminderRepository.GetByIdAsync(id, cancellationToken);
        if (reminder == null)
            throw new ArgumentException($"Reminder with id {id} not found", nameof(id));

        reminder.Update(
            dto.Title,
            dto.Description,
            dto.Priority
        );

        var updatedReminder = await _reminderRepository.UpdateAsync(reminder, cancellationToken);
        return MapToDto(updatedReminder);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reminder = await _reminderRepository.GetByIdAsync(id, cancellationToken);
        if (reminder == null)
            throw new ArgumentException($"Reminder with id {id} not found", nameof(id));

        return await _reminderRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<ReminderDto> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reminder = await _reminderRepository.GetByIdAsync(id, cancellationToken);
        if (reminder == null)
            throw new ArgumentException($"Reminder with id {id} not found", nameof(id));

        reminder.MarkAsRead();
        var updatedReminder = await _reminderRepository.UpdateAsync(reminder, cancellationToken);
        return MapToDto(updatedReminder);
    }

    public async Task<ReminderDto> MarkAsUnreadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reminder = await _reminderRepository.GetByIdAsync(id, cancellationToken);
        if (reminder == null)
            throw new ArgumentException($"Reminder with id {id} not found", nameof(id));

        reminder.MarkAsUnread();
        var updatedReminder = await _reminderRepository.UpdateAsync(reminder, cancellationToken);
        return MapToDto(updatedReminder);
    }

    private static ReminderDto MapToDto(Reminder reminder)
    {
        return new ReminderDto
        {
            Id = reminder.Id,
            Title = reminder.Title,
            Description = reminder.Description,
            Priority = reminder.Priority,
            ReadAt = reminder.ReadAt,
            IsRead = reminder.IsRead,
            CreatedAt = reminder.CreatedAt,
            UpdatedAt = reminder.UpdatedAt
        };
    }
}

