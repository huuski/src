using ApplicationLayer.DTOs.Reminder;

namespace ApplicationLayer.Interfaces.Services;

public interface IReminderService
{
    Task<ReminderDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReminderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ReminderDto> CreateAsync(CreateReminderDto dto, CancellationToken cancellationToken = default);
    Task<ReminderDto> UpdateAsync(Guid id, UpdateReminderDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReminderDto> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReminderDto> MarkAsUnreadAsync(Guid id, CancellationToken cancellationToken = default);
}

