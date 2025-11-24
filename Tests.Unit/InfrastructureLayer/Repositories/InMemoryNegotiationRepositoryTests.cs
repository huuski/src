using DomainLayer.Entities;
using FluentAssertions;
using InfrastructureLayer.Repositories;

namespace Tests.Unit.InfrastructureLayer.Repositories;

public class InMemoryNegotiationRepositoryTests
{
    private readonly InMemoryNegotiationRepository _repository;

    public InMemoryNegotiationRepositoryTests()
    {
        _repository = new InMemoryNegotiationRepository();
    }

    [Fact]
    public async Task CreateAsync_ValidNegotiation_ShouldCreate()
    {
        // Arrange
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );

        // Act
        var result = await _repository.CreateAsync(negotiation);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetByCodeAsync_ExistingCode_ShouldReturn()
    {
        // Arrange
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );
        await _repository.CreateAsync(negotiation);

        // Act
        var result = await _repository.GetByCodeAsync(negotiation.Code);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(negotiation.Code);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ShouldReturnNegotiation()
    {
        // Arrange
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );
        await _repository.CreateAsync(negotiation);

        // Act
        var result = await _repository.GetByIdAsync(negotiation.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(negotiation.Id);
        result.Code.Should().Be(negotiation.Code);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCodeAsync_NonExistentCode_ShouldReturnNull()
    {
        // Arrange
        var nonExistentCode = "NONEXISTENT";

        // Act
        var result = await _repository.GetByCodeAsync(nonExistentCode);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllNegotiations()
    {
        // Arrange
        var negotiation1 = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );
        var negotiation2 = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            2000m,
            1800m,
            200m
        );

        await _repository.CreateAsync(negotiation1);
        await _repository.CreateAsync(negotiation2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByCustomerIdAsync_ExistingCustomerId_ShouldReturnNegotiations()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var negotiation1 = new Negotiation(
            customerId,
            userId,
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );
        var negotiation2 = new Negotiation(
            customerId,
            userId,
            DateTime.UtcNow.AddDays(30),
            2000m,
            1800m,
            200m
        );
        var negotiation3 = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            3000m,
            2700m,
            300m
        );

        await _repository.CreateAsync(negotiation1);
        await _repository.CreateAsync(negotiation2);
        await _repository.CreateAsync(negotiation3);

        // Act
        var result = await _repository.GetByCustomerIdAsync(customerId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(n => n.CustomerId == customerId).Should().BeTrue();
    }

    [Fact]
    public async Task GetByCustomerIdAsync_NonExistentCustomerId_ShouldReturnEmpty()
    {
        // Arrange
        var nonExistentCustomerId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByCustomerIdAsync(nonExistentCustomerId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByUserIdAsync_ExistingUserId_ShouldReturnNegotiations()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var negotiation1 = new Negotiation(
            Guid.NewGuid(),
            userId,
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );
        var negotiation2 = new Negotiation(
            Guid.NewGuid(),
            userId,
            DateTime.UtcNow.AddDays(30),
            2000m,
            1800m,
            200m
        );
        var negotiation3 = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            3000m,
            2700m,
            300m
        );

        await _repository.CreateAsync(negotiation1);
        await _repository.CreateAsync(negotiation2);
        await _repository.CreateAsync(negotiation3);

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(n => n.UserId == userId).Should().BeTrue();
    }

    [Fact]
    public async Task GetByUserIdAsync_NonExistentUserId_ShouldReturnEmpty()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByUserIdAsync(nonExistentUserId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ExistingNegotiation_ShouldUpdate()
    {
        // Arrange
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );
        await _repository.CreateAsync(negotiation);

        negotiation.Update(
            DateTime.UtcNow.AddDays(60),
            2000m,
            1800m,
            200m
        );

        // Act
        var result = await _repository.UpdateAsync(negotiation);

        // Assert
        result.Should().NotBeNull();
        result.GrossValue.Should().Be(2000m);
        result.NetValue.Should().Be(1800m);
        result.DiscountValue.Should().Be(200m);
    }

    [Fact]
    public async Task UpdateAsync_NonExistentNegotiation_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.UpdateAsync(negotiation)
        );
    }

    [Fact]
    public async Task DeleteAsync_ExistingNegotiation_ShouldMarkAsDeleted()
    {
        // Arrange
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );
        await _repository.CreateAsync(negotiation);

        // Act
        var result = await _repository.DeleteAsync(negotiation.Id);

        // Assert
        result.Should().BeTrue();
        var deletedNegotiation = await _repository.GetByIdAsync(negotiation.Id);
        deletedNegotiation.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentNegotiation_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_DeletedNegotiation_ShouldNotAppearInGetAll()
    {
        // Arrange
        var negotiation1 = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );
        var negotiation2 = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            2000m,
            1800m,
            200m
        );

        await _repository.CreateAsync(negotiation1);
        await _repository.CreateAsync(negotiation2);

        // Act
        await _repository.DeleteAsync(negotiation1.Id);
        var allNegotiations = await _repository.GetAllAsync();

        // Assert
        allNegotiations.Should().HaveCount(1);
        allNegotiations.First().Id.Should().Be(negotiation2.Id);
    }

    [Fact]
    public async Task CreateAsync_DuplicateId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );
        await _repository.CreateAsync(negotiation);

        var duplicateNegotiation = new Negotiation(
            negotiation.CustomerId,
            negotiation.UserId,
            DateTime.UtcNow.AddDays(30),
            2000m,
            1800m,
            200m
        );

        // Set the same ID using reflection
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(duplicateNegotiation, negotiation.Id);
        }

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.CreateAsync(duplicateNegotiation)
        );
    }

    [Fact]
    public async Task CreateAsync_DuplicateCode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var negotiation1 = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );
        await _repository.CreateAsync(negotiation1);

        // Try to create another negotiation with the same code (though it's randomly generated, this tests the validation)
        // Since codes are randomly generated, we'll use reflection to set the same code
        var negotiation2 = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            2000m,
            1800m,
            200m
        );

        var codeProperty = typeof(Negotiation).GetProperty("Code");
        if (codeProperty != null && codeProperty.CanWrite)
        {
            codeProperty.SetValue(negotiation2, negotiation1.Code);
        }

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.CreateAsync(negotiation2)
        );
    }
}

