using DomainLayer.Entities;
using FluentAssertions;
using InfrastructureLayer.Repositories;

namespace Tests.Unit.InfrastructureLayer.Repositories;

public class InMemoryNotificationRepositoryTests
{
    private readonly InMemoryNotificationRepository _repository;

    public InMemoryNotificationRepositoryTests()
    {
        _repository = new InMemoryNotificationRepository();
    }

    [Fact]
    public async Task CreateAsync_ValidNotification_ShouldCreateNotification()
    {
        // Arrange
        var notification = new Notification(
            "Test Notification",
            "Test Description"
        );

        // Act
        var result = await _repository.CreateAsync(notification);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(notification.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingNotification_ShouldReturnNotification()
    {
        // Arrange
        var notification = new Notification(
            "Test Notification",
            "Test Description"
        );
        await _repository.CreateAsync(notification);

        // Act
        var result = await _repository.GetByIdAsync(notification.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(notification.Id);
        result.Title.Should().Be(notification.Title);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentNotification_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllNotifications()
    {
        // Arrange
        var notification1 = new Notification(
            "Notification 1",
            "Description 1"
        );
        var notification2 = new Notification(
            "Notification 2",
            "Description 2"
        );

        await _repository.CreateAsync(notification1);
        await _repository.CreateAsync(notification2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ExistingNotification_ShouldUpdateNotification()
    {
        // Arrange
        var notification = new Notification(
            "Old Title",
            "Old Description"
        );
        await _repository.CreateAsync(notification);

        notification.Update(
            "New Title",
            "New Description"
        );

        // Act
        var result = await _repository.UpdateAsync(notification);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Title");
        result.Description.Should().Be("New Description");
    }

    [Fact]
    public async Task DeleteAsync_ExistingNotification_ShouldMarkAsDeleted()
    {
        // Arrange
        var notification = new Notification(
            "Test Notification",
            "Test Description"
        );
        await _repository.CreateAsync(notification);

        // Act
        var result = await _repository.DeleteAsync(notification.Id);

        // Assert
        result.Should().BeTrue();
        var deletedNotification = await _repository.GetByIdAsync(notification.Id);
        deletedNotification.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentNotification_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }
}

