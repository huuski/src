using ApplicationLayer.DTOs.Notification;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Services;
using DomainLayer.Entities;
using FluentAssertions;
using Moq;

namespace Tests.Unit.ApplicationLayer.Services;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepositoryMock;
    private readonly INotificationService _notificationService;

    public NotificationServiceTests()
    {
        _notificationRepositoryMock = new Mock<INotificationRepository>();
        _notificationService = new NotificationService(_notificationRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnNotificationDto()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var notification = CreateTestNotification(notificationId);

        _notificationRepositoryMock
            .Setup(x => x.GetByIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        // Act
        var result = await _notificationService.GetByIdAsync(notificationId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(notificationId);
        result.Title.Should().Be(notification.Title);
        result.Description.Should().Be(notification.Description);
    }

    [Fact]
    public async Task GetByIdAsync_NotificationNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var notificationId = Guid.NewGuid();

        _notificationRepositoryMock
            .Setup(x => x.GetByIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _notificationService.GetByIdAsync(notificationId)
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllNotifications()
    {
        // Arrange
        var notifications = new List<Notification>
        {
            CreateTestNotification(Guid.NewGuid()),
            CreateTestNotification(Guid.NewGuid())
        };

        _notificationRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        // Act
        var result = await _notificationService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ShouldCreateNotification()
    {
        // Arrange
        var dto = new CreateNotificationDto
        {
            Title = "New Message",
            Description = "You have a new message",
            Icon = "message-icon"
        };

        _notificationRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification n, CancellationToken ct) => n);

        // Act
        var result = await _notificationService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
        result.Description.Should().Be(dto.Description);
        result.Icon.Should().Be(dto.Icon);
        _notificationRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_ShouldUpdateNotification()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var existingNotification = CreateTestNotification(notificationId);

        var dto = new UpdateNotificationDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Icon = "updated-icon"
        };

        _notificationRepositoryMock
            .Setup(x => x.GetByIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingNotification);

        _notificationRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification n, CancellationToken ct) => n);

        // Act
        var result = await _notificationService.UpdateAsync(notificationId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
        result.Description.Should().Be(dto.Description);
        _notificationRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidId_ShouldDeleteNotification()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var notification = CreateTestNotification(notificationId);

        _notificationRepositoryMock
            .Setup(x => x.GetByIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _notificationRepositoryMock
            .Setup(x => x.DeleteAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _notificationService.DeleteAsync(notificationId);

        // Assert
        result.Should().BeTrue();
        _notificationRepositoryMock.Verify(x => x.DeleteAsync(notificationId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsReadAsync_ValidId_ShouldMarkAsRead()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var notification = CreateTestNotification(notificationId);

        _notificationRepositoryMock
            .Setup(x => x.GetByIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _notificationRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification n, CancellationToken ct) => n);

        // Act
        var result = await _notificationService.MarkAsReadAsync(notificationId);

        // Assert
        result.Should().NotBeNull();
        result.IsRead.Should().BeTrue();
        _notificationRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsUnreadAsync_ValidId_ShouldMarkAsUnread()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var notification = CreateTestNotification(notificationId);
        notification.MarkAsRead();

        _notificationRepositoryMock
            .Setup(x => x.GetByIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        _notificationRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification n, CancellationToken ct) => n);

        // Act
        var result = await _notificationService.MarkAsUnreadAsync(notificationId);

        // Assert
        result.Should().NotBeNull();
        result.IsRead.Should().BeFalse();
        _notificationRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private Notification CreateTestNotification(
        Guid id,
        string title = "Test Notification",
        string description = "Test Description",
        string? icon = null)
    {
        var notification = new Notification(
            title,
            description,
            icon
        );

        // Set the ID using reflection
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(notification, id);
        }

        return notification;
    }
}

