using DomainLayer.Entities;
using DomainLayer.ValueObjects;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.Entities;

public class EntityTests
{
    private class TestEntity : Entity
    {
        public TestEntity() { }
    }

    [Fact]
    public void Constructor_ShouldInitializeId()
    {
        // Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_ShouldInitializeCreatedAt()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var entity = new TestEntity();

        // Arrange
        var afterCreation = DateTime.UtcNow;

        // Assert
        entity.CreatedAt.Should().BeOnOrAfter(beforeCreation);
        entity.CreatedAt.Should().BeOnOrBefore(afterCreation);
    }

    [Fact]
    public void Constructor_ShouldInitializeUpdatedAt()
    {
        // Act
        var entity = new TestEntity();

        // Assert
        entity.UpdatedAt.Should().Be(entity.CreatedAt);
    }

    [Fact]
    public void Constructor_ShouldSetDeletedAtAsNull()
    {
        // Act
        var entity = new TestEntity();

        // Assert
        entity.DeletedAt.Should().BeNull();
        entity.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void MarkAsUpdated_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var entity = new TestEntity();
        var originalUpdatedAt = entity.UpdatedAt;
        
        // Wait a bit to ensure time difference
        Thread.Sleep(10);

        // Act
        entity.MarkAsUpdated();

        // Assert
        entity.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void MarkAsDeleted_ShouldSetDeletedAt()
    {
        // Arrange
        var entity = new TestEntity();

        // Act
        entity.MarkAsDeleted();

        // Assert
        entity.DeletedAt.Should().NotBeNull();
        entity.IsDeleted.Should().BeTrue();
    }
}

