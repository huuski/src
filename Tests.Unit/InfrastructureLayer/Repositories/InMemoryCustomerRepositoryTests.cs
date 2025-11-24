using DomainLayer.Entities;
using DomainLayer.ValueObjects;
using FluentAssertions;
using InfrastructureLayer.Repositories;

namespace Tests.Unit.InfrastructureLayer.Repositories;

public class InMemoryCustomerRepositoryTests
{
    private readonly InMemoryCustomerRepository _repository;

    public InMemoryCustomerRepositoryTests()
    {
        _repository = new InMemoryCustomerRepository();
    }

    [Fact]
    public async Task GetByDocumentAsync_WithFormattedDocument_ShouldFindCustomer()
    {
        // Arrange
        var customer = new Customer(
            Name.Create("Test Customer"),
            "123.456.789-00",
            new DateTime(1990, 1, 1),
            Email.Create("test@example.com"),
            Address.Create("Street", "City", "State", "12345"),
            PhoneNumber.Create("+5511999999999")
        );
        await _repository.CreateAsync(customer);

        // Act - Search with formatted document
        var result = await _repository.GetByDocumentAsync("123.456.789-00");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(customer.Id);
    }

    [Fact]
    public async Task GetByDocumentAsync_WithUnformattedDocument_ShouldFindCustomer()
    {
        // Arrange
        var customer = new Customer(
            Name.Create("Test Customer"),
            "123.456.789-00",
            new DateTime(1990, 1, 1),
            Email.Create("test@example.com"),
            Address.Create("Street", "City", "State", "12345"),
            PhoneNumber.Create("+5511999999999")
        );
        await _repository.CreateAsync(customer);

        // Act - Search with unformatted document (no special characters)
        var result = await _repository.GetByDocumentAsync("12345678900");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(customer.Id);
    }

    [Fact]
    public async Task GetByDocumentAsync_WithDifferentFormat_ShouldFindCustomer()
    {
        // Arrange
        var customer = new Customer(
            Name.Create("Test Customer"),
            "98765432100",
            new DateTime(1990, 1, 1),
            Email.Create("test2@example.com"),
            Address.Create("Street", "City", "State", "12345"),
            PhoneNumber.Create("+5511999999998")
        );
        await _repository.CreateAsync(customer);

        // Act - Search with different format
        var result = await _repository.GetByDocumentAsync("987.654.321-00");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(customer.Id);
    }

    [Fact]
    public async Task CreateAsync_DuplicateDocumentWithDifferentFormat_ShouldThrowException()
    {
        // Arrange
        var customer1 = new Customer(
            Name.Create("Customer 1"),
            "123.456.789-00",
            new DateTime(1990, 1, 1),
            Email.Create("customer1@example.com"),
            Address.Create("Street", "City", "State", "12345"),
            PhoneNumber.Create("+5511999999999")
        );
        await _repository.CreateAsync(customer1);

        var customer2 = new Customer(
            Name.Create("Customer 2"),
            "12345678900", // Same document, different format
            new DateTime(1990, 1, 1),
            Email.Create("customer2@example.com"),
            Address.Create("Street", "City", "State", "12345"),
            PhoneNumber.Create("+5511999999998")
        );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.CreateAsync(customer2)
        );
    }
}

