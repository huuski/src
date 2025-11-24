using ApplicationLayer.DTOs.Notification;

namespace ApplicationLayer.Interfaces.Services;

public interface INotificationService
{
    Task<NotificationDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<NotificationDto> CreateAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default);
    Task<NotificationDto> UpdateAsync(Guid id, UpdateNotificationDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NotificationDto> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NotificationDto> MarkAsUnreadAsync(Guid id, CancellationToken cancellationToken = default);
}

