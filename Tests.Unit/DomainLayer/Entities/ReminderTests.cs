using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.Entities;

public class ReminderTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateReminder()
    {
        // Arrange
        var title = "Meeting Reminder";
        var description = "Team meeting at 3 PM";
        var priority = Priority.High;

        // Act
        var reminder = new Reminder(title, description, priority);

        // Assert
        reminder.Should().NotBeNull();
        reminder.Title.Should().Be(title);
        reminder.Description.Should().Be(description);
        reminder.Priority.Should().Be(priority);
        reminder.ReadAt.Should().BeNull();
        reminder.IsRead.Should().BeFalse();
        reminder.Id.Should().NotBeEmpty();
        reminder.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_NullTitle_ShouldThrowArgumentNullException()
    {
        // Arrange
        var description = "Description";
        var priority = Priority.Normal;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Reminder(null!, description, priority)
        );
    }

    [Fact]
    public void Constructor_EmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var description = "Description";
        var priority = Priority.Normal;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Reminder("", description, priority)
        );
    }

    [Fact]
    public void Update_ValidParameters_ShouldUpdateReminder()
    {
        // Arrange
        var reminder = new Reminder(
            "Old Title",
            "Old Description",
            Priority.Low
        );

        var newTitle = "New Title";
        var newDescription = "New Description";
        var newPriority = Priority.High;

        // Act
        reminder.Update(newTitle, newDescription, newPriority);

        // Assert
        reminder.Title.Should().Be(newTitle);
        reminder.Description.Should().Be(newDescription);
        reminder.Priority.Should().Be(newPriority);
        reminder.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarkAsRead_ShouldSetReadAt()
    {
        // Arrange
        var reminder = new Reminder(
            "Title",
            "Description",
            Priority.Normal
        );

        // Act
        reminder.MarkAsRead();

        // Assert
        reminder.IsRead.Should().BeTrue();
        reminder.ReadAt.Should().NotBeNull();
        reminder.ReadAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarkAsRead_AlreadyRead_ShouldNotUpdateReadAt()
    {
        // Arrange
        var reminder = new Reminder(
            "Title",
            "Description",
            Priority.Normal
        );
        reminder.MarkAsRead();
        var firstReadAt = reminder.ReadAt;

        // Act
        reminder.MarkAsRead();

        // Assert
        reminder.ReadAt.Should().Be(firstReadAt);
    }

    [Fact]
    public void MarkAsUnread_ShouldClearReadAt()
    {
        // Arrange
        var reminder = new Reminder(
            "Title",
            "Description",
            Priority.Normal
        );
        reminder.MarkAsRead();

        // Act
        reminder.MarkAsUnread();

        // Assert
        reminder.IsRead.Should().BeFalse();
        reminder.ReadAt.Should().BeNull();
    }

    [Fact]
    public void MarkAsUnread_AlreadyUnread_ShouldNotChange()
    {
        // Arrange
        var reminder = new Reminder(
            "Title",
            "Description",
            Priority.Normal
        );

        // Act
        reminder.MarkAsUnread();

        // Assert
        reminder.IsRead.Should().BeFalse();
        reminder.ReadAt.Should().BeNull();
    }

    [Fact]
    public void MarkAsDeleted_ShouldSetDeletedAt()
    {
        // Arrange
        var reminder = new Reminder(
            "Title",
            "Description",
            Priority.Normal
        );

        // Act
        reminder.MarkAsDeleted();

        // Assert
        reminder.IsDeleted.Should().BeTrue();
        reminder.DeletedAt.Should().NotBeNull();
        reminder.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}

