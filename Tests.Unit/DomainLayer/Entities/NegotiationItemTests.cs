using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.Entities;

public class NegotiationItemTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateNegotiationItem()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var type = NegotiationItemType.Service;
        var quantity = 2;
        var grossValueUnit = 100.00m;
        var netValue = 180.00m;
        var discountValue = 20.00m;

        // Act
        var negotiationItem = new NegotiationItem(negotiationId, type, quantity, grossValueUnit, netValue, discountValue);

        // Assert
        negotiationItem.Should().NotBeNull();
        negotiationItem.NegotiationId.Should().Be(negotiationId);
        negotiationItem.Type.Should().Be(type);
        negotiationItem.Quantity.Should().Be(quantity);
        negotiationItem.GrossValueUnit.Should().Be(grossValueUnit);
        negotiationItem.GrossValue.Should().Be(grossValueUnit * quantity);
        negotiationItem.NetValue.Should().Be(netValue);
        negotiationItem.DiscountValue.Should().Be(discountValue);
    }

    [Fact]
    public void Constructor_ShouldCalculateGrossValue()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var quantity = 3;
        var grossValueUnit = 50.00m;

        // Act
        var negotiationItem = new NegotiationItem(negotiationId, NegotiationItemType.Product, quantity, grossValueUnit, 140m, 10m);

        // Assert
        negotiationItem.GrossValue.Should().Be(150.00m);
    }

    [Fact]
    public void Update_ShouldRecalculateGrossValue()
    {
        // Arrange
        var negotiationItem = new NegotiationItem(
            Guid.NewGuid(),
            NegotiationItemType.Service,
            2,
            100m,
            180m,
            20m
        );

        // Act
        negotiationItem.Update(NegotiationItemType.Product, 5, 50m, 240m, 10m);

        // Assert
        negotiationItem.GrossValue.Should().Be(250m);
        negotiationItem.Quantity.Should().Be(5);
    }
}

