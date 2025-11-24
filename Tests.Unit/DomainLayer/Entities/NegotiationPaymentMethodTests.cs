using DomainLayer.Entities;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.Entities;

public class NegotiationPaymentMethodTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateNegotiationPaymentMethod()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var paymentMethodId = Guid.NewGuid();
        var value = 500.00m;

        // Act
        var negotiationPaymentMethod = new NegotiationPaymentMethod(negotiationId, paymentMethodId, value);

        // Assert
        negotiationPaymentMethod.Should().NotBeNull();
        negotiationPaymentMethod.NegotiationId.Should().Be(negotiationId);
        negotiationPaymentMethod.PaymentMethodId.Should().Be(paymentMethodId);
        negotiationPaymentMethod.Value.Should().Be(value);
    }

    [Fact]
    public void Constructor_ZeroValue_ShouldThrowArgumentException()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var paymentMethodId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new NegotiationPaymentMethod(negotiationId, paymentMethodId, 0m)
        );
    }

    [Fact]
    public void Update_ValidValue_ShouldUpdate()
    {
        // Arrange
        var negotiationPaymentMethod = new NegotiationPaymentMethod(
            Guid.NewGuid(),
            Guid.NewGuid(),
            500m
        );

        // Act
        negotiationPaymentMethod.Update(750m);

        // Assert
        negotiationPaymentMethod.Value.Should().Be(750m);
    }
}

