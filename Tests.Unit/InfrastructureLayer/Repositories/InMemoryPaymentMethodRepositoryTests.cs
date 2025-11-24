using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using InfrastructureLayer.Repositories;

namespace Tests.Unit.InfrastructureLayer.Repositories;

public class InMemoryPaymentMethodRepositoryTests
{
    private readonly InMemoryPaymentMethodRepository _repository;

    public InMemoryPaymentMethodRepositoryTests()
    {
        _repository = new InMemoryPaymentMethodRepository();
    }

    [Fact]
    public async Task CreateAsync_ValidPaymentMethod_ShouldCreate()
    {
        // Arrange
        var paymentMethod = new PaymentMethod("Credit Card", PaymentMethodType.CreditCard);

        // Act
        var result = await _repository.CreateAsync(paymentMethod);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(paymentMethod.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingPaymentMethod_ShouldReturn()
    {
        // Arrange
        var paymentMethod = new PaymentMethod("Test", PaymentMethodType.CreditCard);
        await _repository.CreateAsync(paymentMethod);

        // Act
        var result = await _repository.GetByIdAsync(paymentMethod.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(paymentMethod.Name);
    }
}

