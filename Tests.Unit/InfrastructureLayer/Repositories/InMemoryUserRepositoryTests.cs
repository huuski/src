using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.ValueObjects;
using FluentAssertions;
using InfrastructureLayer.Repositories;
using InfrastructureLayer.Services;
using Moq;

namespace Tests.Unit.InfrastructureLayer.Repositories;

public class InMemoryUserRepositoryTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly InMemoryUserRepository _repository;

    public InMemoryUserRepositoryTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _passwordHasherMock
            .Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns<string>(p => $"$2a$12${p}");

        _repository = new InMemoryUserRepository(_passwordHasherMock.Object);
    }

    [Fact]
    public async Task GetByEmailAsync_ExistingUser_ShouldReturnUser()
    {
        // Arrange
        var email = "joao.silva@example.com";

        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Value.Should().Be(email);
    }

    [Fact]
    public async Task GetByEmailAsync_NonExistentUser_ShouldReturnNull()
    {
        // Arrange
        var email = "nonexistent@example.com";

        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ExistingUser_ShouldReturnUser()
    {
        // Arrange
        var email = "joao.silva@example.com";
        var user = await _repository.GetByEmailAsync(email);
        var userId = user!.Id;

        // Act
        var result = await _repository.GetByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentUser_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_NewUser_ShouldAddUser()
    {
        // Arrange
        var user = CreateTestUser("newuser@example.com");

        // Act
        var result = await _repository.CreateAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);

        var retrieved = await _repository.GetByEmailAsync(user.Email.Value);
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var email = "joao.silva@example.com";
        var user = CreateTestUser(email);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.CreateAsync(user)
        );
    }

    [Fact]
    public async Task UpdateAsync_ExistingUser_ShouldUpdateUser()
    {
        // Arrange
        var email = "joao.silva@example.com";
        var user = await _repository.GetByEmailAsync(email);
        var newName = Name.Create("João Silva Updated");

        // Act
        user!.UpdateName(newName);
        var result = await _repository.UpdateAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Name.Value.Should().Be(newName.Value);

        var retrieved = await _repository.GetByEmailAsync(email);
        retrieved!.Name.Value.Should().Be(newName.Value);
    }

    [Fact]
    public async Task UpdateAsync_NonExistentUser_ShouldThrowException()
    {
        // Arrange
        var user = CreateTestUser("nonexistent@example.com");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.UpdateAsync(user)
        );
    }

    [Fact]
    public async Task ExistsByEmailAsync_ExistingUser_ShouldReturnTrue()
    {
        // Arrange
        var email = "joao.silva@example.com";

        // Act
        var result = await _repository.ExistsByEmailAsync(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_NonExistentUser_ShouldReturnFalse()
    {
        // Arrange
        var email = "nonexistent@example.com";

        // Act
        var result = await _repository.ExistsByEmailAsync(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldInitializeDefaultUsers()
    {
        // Act
        var repo = new InMemoryUserRepository(_passwordHasherMock.Object);

        // Assert
        // Verify that default users are created
        // This is verified by the fact that we can retrieve them
        var user1 = repo.GetByEmailAsync("joao.silva@example.com").Result;
        var user2 = repo.GetByEmailAsync("maria.santos@example.com").Result;

        user1.Should().NotBeNull();
        user2.Should().NotBeNull();
    }

    private User CreateTestUser(string email)
    {
        var name = Name.Create("Test User");
        var emailValue = Email.Create(email);
        var address = Address.Create("Street", "City", "State", "12345");
        var phoneNumber = PhoneNumber.Create("+5511999999999");
        var password = Password.FromHash("$2a$12$HashedPassword");

        return new User(
            name,
            "12345678900",
            new DateTime(1990, 1, 1),
            emailValue,
            address,
            phoneNumber,
            password
        );
    }
}

