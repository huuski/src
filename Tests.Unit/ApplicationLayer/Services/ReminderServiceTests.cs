using ApplicationLayer.DTOs.Reminder;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using Moq;

namespace Tests.Unit.ApplicationLayer.Services;

public class ReminderServiceTests
{
    private readonly Mock<IReminderRepository> _reminderRepositoryMock;
    private readonly IReminderService _reminderService;

    public ReminderServiceTests()
    {
        _reminderRepositoryMock = new Mock<IReminderRepository>();
        _reminderService = new ReminderService(_reminderRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnReminderDto()
    {
        // Arrange
        var reminderId = Guid.NewGuid();
        var reminder = CreateTestReminder(reminderId);

        _reminderRepositoryMock
            .Setup(x => x.GetByIdAsync(reminderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reminder);

        // Act
        var result = await _reminderService.GetByIdAsync(reminderId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(reminderId);
        result.Title.Should().Be(reminder.Title);
        result.Description.Should().Be(reminder.Description);
        result.Priority.Should().Be(reminder.Priority);
    }

    [Fact]
    public async Task GetByIdAsync_ReminderNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var reminderId = Guid.NewGuid();

        _reminderRepositoryMock
            .Setup(x => x.GetByIdAsync(reminderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reminder?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _reminderService.GetByIdAsync(reminderId)
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllReminders()
    {
        // Arrange
        var reminders = new List<Reminder>
        {
            CreateTestReminder(Guid.NewGuid()),
            CreateTestReminder(Guid.NewGuid())
        };

        _reminderRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(reminders);

        // Act
        var result = await _reminderService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ShouldCreateReminder()
    {
        // Arrange
        var dto = new CreateReminderDto
        {
            Title = "Meeting Reminder",
            Description = "Team meeting at 3 PM",
            Priority = Priority.High
        };

        _reminderRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Reminder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reminder r, CancellationToken ct) => r);

        // Act
        var result = await _reminderService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
        result.Description.Should().Be(dto.Description);
        result.Priority.Should().Be(dto.Priority);
        _reminderRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Reminder>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_ShouldUpdateReminder()
    {
        // Arrange
        var reminderId = Guid.NewGuid();
        var existingReminder = CreateTestReminder(reminderId);

        var dto = new UpdateReminderDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = Priority.Low
        };

        _reminderRepositoryMock
            .Setup(x => x.GetByIdAsync(reminderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReminder);

        _reminderRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Reminder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reminder r, CancellationToken ct) => r);

        // Act
        var result = await _reminderService.UpdateAsync(reminderId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
        result.Description.Should().Be(dto.Description);
        result.Priority.Should().Be(dto.Priority);
        _reminderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Reminder>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidId_ShouldDeleteReminder()
    {
        // Arrange
        var reminderId = Guid.NewGuid();
        var reminder = CreateTestReminder(reminderId);

        _reminderRepositoryMock
            .Setup(x => x.GetByIdAsync(reminderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reminder);

        _reminderRepositoryMock
            .Setup(x => x.DeleteAsync(reminderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _reminderService.DeleteAsync(reminderId);

        // Assert
        result.Should().BeTrue();
        _reminderRepositoryMock.Verify(x => x.DeleteAsync(reminderId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsReadAsync_ValidId_ShouldMarkAsRead()
    {
        // Arrange
        var reminderId = Guid.NewGuid();
        var reminder = CreateTestReminder(reminderId);

        _reminderRepositoryMock
            .Setup(x => x.GetByIdAsync(reminderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reminder);

        _reminderRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Reminder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reminder r, CancellationToken ct) => r);

        // Act
        var result = await _reminderService.MarkAsReadAsync(reminderId);

        // Assert
        result.Should().NotBeNull();
        result.IsRead.Should().BeTrue();
        _reminderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Reminder>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsUnreadAsync_ValidId_ShouldMarkAsUnread()
    {
        // Arrange
        var reminderId = Guid.NewGuid();
        var reminder = CreateTestReminder(reminderId);
        reminder.MarkAsRead();

        _reminderRepositoryMock
            .Setup(x => x.GetByIdAsync(reminderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reminder);

        _reminderRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Reminder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reminder r, CancellationToken ct) => r);

        // Act
        var result = await _reminderService.MarkAsUnreadAsync(reminderId);

        // Assert
        result.Should().NotBeNull();
        result.IsRead.Should().BeFalse();
        _reminderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Reminder>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private Reminder CreateTestReminder(
        Guid id,
        string title = "Test Reminder",
        string description = "Test Description",
        Priority priority = Priority.Normal)
    {
        var reminder = new Reminder(
            title,
            description,
            priority
        );

        // Set the ID using reflection
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(reminder, id);
        }

        return reminder;
    }
}

