using ApplicationLayer.DTOs.PaymentMethod;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using Moq;

namespace Tests.Unit.ApplicationLayer.Services;

public class PaymentMethodServiceTests
{
    private readonly Mock<IPaymentMethodRepository> _paymentMethodRepositoryMock;
    private readonly IPaymentMethodService _paymentMethodService;

    public PaymentMethodServiceTests()
    {
        _paymentMethodRepositoryMock = new Mock<IPaymentMethodRepository>();
        _paymentMethodService = new PaymentMethodService(_paymentMethodRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ShouldCreatePaymentMethod()
    {
        // Arrange
        var dto = new CreatePaymentMethodDto
        {
            Name = "Credit Card",
            Type = PaymentMethodType.CreditCard
        };

        _paymentMethodRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<PaymentMethod>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentMethod pm, CancellationToken ct) => pm);

        // Act
        var result = await _paymentMethodService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Type.Should().Be(dto.Type);
    }

    [Fact]
    public async Task ActivateAsync_ShouldActivatePaymentMethod()
    {
        // Arrange
        var paymentMethod = new PaymentMethod("Test", PaymentMethodType.CreditCard);
        paymentMethod.Deactivate();

        _paymentMethodRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethod);

        _paymentMethodRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<PaymentMethod>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentMethod pm, CancellationToken ct) => pm);

        // Act
        var result = await _paymentMethodService.ActivateAsync(paymentMethod.Id);

        // Assert
        result.Inactive.Should().BeFalse();
    }
}

