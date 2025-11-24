using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using InfrastructureLayer.Repositories;

namespace Tests.Unit.InfrastructureLayer.Repositories;

public class InMemoryReminderRepositoryTests
{
    private readonly InMemoryReminderRepository _repository;

    public InMemoryReminderRepositoryTests()
    {
        _repository = new InMemoryReminderRepository();
    }

    [Fact]
    public async Task CreateAsync_ValidReminder_ShouldCreateReminder()
    {
        // Arrange
        var reminder = new Reminder(
            "Test Reminder",
            "Test Description",
            Priority.Normal
        );

        // Act
        var result = await _repository.CreateAsync(reminder);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(reminder.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingReminder_ShouldReturnReminder()
    {
        // Arrange
        var reminder = new Reminder(
            "Test Reminder",
            "Test Description",
            Priority.Normal
        );
        await _repository.CreateAsync(reminder);

        // Act
        var result = await _repository.GetByIdAsync(reminder.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(reminder.Id);
        result.Title.Should().Be(reminder.Title);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentReminder_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllReminders()
    {
        // Arrange
        var reminder1 = new Reminder(
            "Reminder 1",
            "Description 1",
            Priority.Low
        );
        var reminder2 = new Reminder(
            "Reminder 2",
            "Description 2",
            Priority.High
        );

        await _repository.CreateAsync(reminder1);
        await _repository.CreateAsync(reminder2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ExistingReminder_ShouldUpdateReminder()
    {
        // Arrange
        var reminder = new Reminder(
            "Old Title",
            "Old Description",
            Priority.Low
        );
        await _repository.CreateAsync(reminder);

        reminder.Update(
            "New Title",
            "New Description",
            Priority.High
        );

        // Act
        var result = await _repository.UpdateAsync(reminder);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Title");
        result.Description.Should().Be("New Description");
        result.Priority.Should().Be(Priority.High);
    }

    [Fact]
    public async Task DeleteAsync_ExistingReminder_ShouldMarkAsDeleted()
    {
        // Arrange
        var reminder = new Reminder(
            "Test Reminder",
            "Test Description",
            Priority.Normal
        );
        await _repository.CreateAsync(reminder);

        // Act
        var result = await _repository.DeleteAsync(reminder.Id);

        // Assert
        result.Should().BeTrue();
        var deletedReminder = await _repository.GetByIdAsync(reminder.Id);
        deletedReminder.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentReminder_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }
}

