using DomainLayer.Entities;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.Entities;

public class NegotiationTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateNegotiation()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(30);
        var grossValue = 1000.00m;
        var netValue = 900.00m;
        var discountValue = 100.00m;

        // Act
        var negotiation = new Negotiation(customerId, userId, expirationDate, grossValue, netValue, discountValue);

        // Assert
        negotiation.Should().NotBeNull();
        negotiation.CustomerId.Should().Be(customerId);
        negotiation.UserId.Should().Be(userId);
        negotiation.Code.Should().NotBeNullOrEmpty();
        negotiation.Code.Length.Should().Be(10);
        negotiation.ExpirationDate.Should().Be(expirationDate);
        negotiation.GrossValue.Should().Be(grossValue);
        negotiation.NetValue.Should().Be(netValue);
        negotiation.DiscountValue.Should().Be(discountValue);
    }

    [Fact]
    public void Constructor_CodeShouldBeUppercase()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(30);

        // Act
        var negotiation = new Negotiation(customerId, Guid.NewGuid(), expirationDate, 1000m, 900m, 100m);

        // Assert
        negotiation.Code.Should().MatchRegex("^[A-Z0-9]{10}$");
    }

    [Fact]
    public void Constructor_EmptyCustomerId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Negotiation(Guid.Empty, Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 1000m, 900m, 100m)
        );
    }

    [Fact]
    public void Constructor_EmptyUserId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Negotiation(Guid.NewGuid(), Guid.Empty, DateTime.UtcNow.AddDays(30), 1000m, 900m, 100m)
        );
    }

    [Fact]
    public void Constructor_PastExpirationDate_ShouldThrowArgumentException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Negotiation(customerId, Guid.NewGuid(), pastDate, 1000m, 900m, 100m)
        );
    }

    [Fact]
    public void Constructor_NetValueGreaterThanGrossValue_ShouldThrowArgumentException()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Negotiation(customerId, Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 1000m, 1100m, 100m)
        );
    }
}

