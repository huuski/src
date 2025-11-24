using ApplicationLayer.DTOs.Notification;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;

namespace ApplicationLayer.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
    }

    public async Task<NotificationDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (notification == null)
            throw new ArgumentException($"Notification with id {id} not found", nameof(id));

        return MapToDto(notification);
    }

    public async Task<IEnumerable<NotificationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetAllAsync(cancellationToken);
        return notifications.Select(MapToDto);
    }

    public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var notification = new Notification(
            dto.Title,
            dto.Description,
            dto.Icon
        );

        var createdNotification = await _notificationRepository.CreateAsync(notification, cancellationToken);
        return MapToDto(createdNotification);
    }

    public async Task<NotificationDto> UpdateAsync(Guid id, UpdateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (notification == null)
            throw new ArgumentException($"Notification with id {id} not found", nameof(id));

        notification.Update(
            dto.Title,
            dto.Description,
            dto.Icon
        );

        var updatedNotification = await _notificationRepository.UpdateAsync(notification, cancellationToken);
        return MapToDto(updatedNotification);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (notification == null)
            throw new ArgumentException($"Notification with id {id} not found", nameof(id));

        return await _notificationRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<NotificationDto> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (notification == null)
            throw new ArgumentException($"Notification with id {id} not found", nameof(id));

        notification.MarkAsRead();
        var updatedNotification = await _notificationRepository.UpdateAsync(notification, cancellationToken);
        return MapToDto(updatedNotification);
    }

    public async Task<NotificationDto> MarkAsUnreadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (notification == null)
            throw new ArgumentException($"Notification with id {id} not found", nameof(id));

        notification.MarkAsUnread();
        var updatedNotification = await _notificationRepository.UpdateAsync(notification, cancellationToken);
        return MapToDto(updatedNotification);
    }

    private static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Description = notification.Description,
            Icon = notification.Icon,
            ReadAt = notification.ReadAt,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            UpdatedAt = notification.UpdatedAt
        };
    }
}

