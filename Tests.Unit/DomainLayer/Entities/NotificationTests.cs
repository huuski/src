using DomainLayer.Entities;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.Entities;

public class NotificationTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateNotification()
    {
        // Arrange
        var title = "New Message";
        var description = "You have a new message";
        var icon = "message-icon";

        // Act
        var notification = new Notification(title, description, icon);

        // Assert
        notification.Should().NotBeNull();
        notification.Title.Should().Be(title);
        notification.Description.Should().Be(description);
        notification.Icon.Should().Be(icon);
        notification.ReadAt.Should().BeNull();
        notification.IsRead.Should().BeFalse();
        notification.Id.Should().NotBeEmpty();
        notification.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_WithoutIcon_ShouldCreateNotification()
    {
        // Arrange
        var title = "New Message";
        var description = "You have a new message";

        // Act
        var notification = new Notification(title, description);

        // Assert
        notification.Should().NotBeNull();
        notification.Icon.Should().BeNull();
    }

    [Fact]
    public void Constructor_NullTitle_ShouldThrowArgumentNullException()
    {
        // Arrange
        var description = "Description";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Notification(null!, description)
        );
    }

    [Fact]
    public void Constructor_EmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var description = "Description";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Notification("", description)
        );
    }

    [Fact]
    public void Update_ValidParameters_ShouldUpdateNotification()
    {
        // Arrange
        var notification = new Notification(
            "Old Title",
            "Old Description"
        );

        var newTitle = "New Title";
        var newDescription = "New Description";
        var newIcon = "new-icon";

        // Act
        notification.Update(newTitle, newDescription, newIcon);

        // Assert
        notification.Title.Should().Be(newTitle);
        notification.Description.Should().Be(newDescription);
        notification.Icon.Should().Be(newIcon);
        notification.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarkAsRead_ShouldSetReadAt()
    {
        // Arrange
        var notification = new Notification(
            "Title",
            "Description"
        );

        // Act
        notification.MarkAsRead();

        // Assert
        notification.IsRead.Should().BeTrue();
        notification.ReadAt.Should().NotBeNull();
        notification.ReadAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarkAsRead_AlreadyRead_ShouldNotUpdateReadAt()
    {
        // Arrange
        var notification = new Notification(
            "Title",
            "Description"
        );
        notification.MarkAsRead();
        var firstReadAt = notification.ReadAt;

        // Act
        notification.MarkAsRead();

        // Assert
        notification.ReadAt.Should().Be(firstReadAt);
    }

    [Fact]
    public void MarkAsUnread_ShouldClearReadAt()
    {
        // Arrange
        var notification = new Notification(
            "Title",
            "Description"
        );
        notification.MarkAsRead();

        // Act
        notification.MarkAsUnread();

        // Assert
        notification.IsRead.Should().BeFalse();
        notification.ReadAt.Should().BeNull();
    }

    [Fact]
    public void MarkAsUnread_AlreadyUnread_ShouldNotChange()
    {
        // Arrange
        var notification = new Notification(
            "Title",
            "Description"
        );

        // Act
        notification.MarkAsUnread();

        // Assert
        notification.IsRead.Should().BeFalse();
        notification.ReadAt.Should().BeNull();
    }

    [Fact]
    public void MarkAsDeleted_ShouldSetDeletedAt()
    {
        // Arrange
        var notification = new Notification(
            "Title",
            "Description"
        );

        // Act
        notification.MarkAsDeleted();

        // Assert
        notification.IsDeleted.Should().BeTrue();
        notification.DeletedAt.Should().NotBeNull();
        notification.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}

