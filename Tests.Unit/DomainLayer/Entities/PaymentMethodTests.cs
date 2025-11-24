using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.Entities;

public class PaymentMethodTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreatePaymentMethod()
    {
        // Arrange
        var name = "Credit Card";
        var type = PaymentMethodType.CreditCard;

        // Act
        var paymentMethod = new PaymentMethod(name, type);

        // Assert
        paymentMethod.Should().NotBeNull();
        paymentMethod.Name.Should().Be(name);
        paymentMethod.Type.Should().Be(type);
        paymentMethod.Inactive.Should().BeFalse();
        paymentMethod.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_NullName_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new PaymentMethod(null!, PaymentMethodType.CreditCard)
        );
    }

    [Fact]
    public void Activate_ShouldSetInactiveToFalse()
    {
        // Arrange
        var paymentMethod = new PaymentMethod("Test", PaymentMethodType.CreditCard);
        paymentMethod.Deactivate();

        // Act
        paymentMethod.Activate();

        // Assert
        paymentMethod.Inactive.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_ShouldSetInactiveToTrue()
    {
        // Arrange
        var paymentMethod = new PaymentMethod("Test", PaymentMethodType.CreditCard);

        // Act
        paymentMethod.Deactivate();

        // Assert
        paymentMethod.Inactive.Should().BeTrue();
    }
}

