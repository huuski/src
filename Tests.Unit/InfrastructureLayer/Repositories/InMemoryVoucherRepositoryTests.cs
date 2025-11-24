using DomainLayer.Entities;
using FluentAssertions;
using InfrastructureLayer.Repositories;

namespace Tests.Unit.InfrastructureLayer.Repositories;

public class InMemoryVoucherRepositoryTests
{
    private readonly InMemoryVoucherRepository _repository;

    public InMemoryVoucherRepositoryTests()
    {
        _repository = new InMemoryVoucherRepository();
    }

    [Fact]
    public async Task CreateAsync_ValidVoucher_ShouldCreateVoucher()
    {
        // Arrange
        var voucher = new Voucher(
            "DISCOUNT10",
            "Test Description",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        // Act
        var result = await _repository.CreateAsync(voucher);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(voucher.Id);
    }

    [Fact]
    public async Task CreateAsync_DuplicateCode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var voucher1 = new Voucher(
            "DISCOUNT10",
            "Test Description 1",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        var voucher2 = new Voucher(
            "DISCOUNT10",
            "Test Description 2",
            20.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        await _repository.CreateAsync(voucher1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.CreateAsync(voucher2)
        );
    }

    [Fact]
    public async Task GetByIdAsync_ExistingVoucher_ShouldReturnVoucher()
    {
        // Arrange
        var voucher = new Voucher(
            "DISCOUNT10",
            "Test Description",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        await _repository.CreateAsync(voucher);

        // Act
        var result = await _repository.GetByIdAsync(voucher.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(voucher.Id);
        result.Code.Should().Be(voucher.Code);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentVoucher_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCodeAsync_ExistingVoucher_ShouldReturnVoucher()
    {
        // Arrange
        var code = "DISCOUNT10";
        var voucher = new Voucher(
            code,
            "Test Description",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        await _repository.CreateAsync(voucher);

        // Act
        var result = await _repository.GetByCodeAsync(code);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(code);
    }

    [Fact]
    public async Task GetByCodeAsync_CaseInsensitive_ShouldReturnVoucher()
    {
        // Arrange
        var code = "DISCOUNT10";
        var voucher = new Voucher(
            code,
            "Test Description",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        await _repository.CreateAsync(voucher);

        // Act
        var result = await _repository.GetByCodeAsync("discount10");

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(code);
    }

    [Fact]
    public async Task GetByCodeAsync_NonExistentCode_ShouldReturnNull()
    {
        // Arrange
        var nonExistentCode = "INVALID_CODE";

        // Act
        var result = await _repository.GetByCodeAsync(nonExistentCode);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllVouchers()
    {
        // Arrange
        var voucher1 = new Voucher(
            "CODE1",
            "Description 1",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        var voucher2 = new Voucher(
            "CODE2",
            "Description 2",
            20.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        await _repository.CreateAsync(voucher1);
        await _repository.CreateAsync(voucher2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ExistingVoucher_ShouldUpdateVoucher()
    {
        // Arrange
        var voucher = new Voucher(
            "OLD_CODE",
            "Old Description",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        await _repository.CreateAsync(voucher);

        voucher.Update(
            "NEW_CODE",
            "New Description",
            20.00m,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(60),
            false,
            200.00m
        );

        // Act
        var result = await _repository.UpdateAsync(voucher);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be("NEW_CODE");
        result.Description.Should().Be("New Description");
        result.DiscountAmount.Should().Be(20.00m);
    }

    [Fact]
    public async Task UpdateAsync_ChangeCodeToExistingCode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var voucher1 = new Voucher(
            "CODE1",
            "Description 1",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        var voucher2 = new Voucher(
            "CODE2",
            "Description 2",
            20.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        await _repository.CreateAsync(voucher1);
        await _repository.CreateAsync(voucher2);

        voucher2.Update(
            "CODE1",
            "Updated Description",
            20.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30),
            true
        );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.UpdateAsync(voucher2)
        );
    }

    [Fact]
    public async Task DeleteAsync_ExistingVoucher_ShouldMarkAsDeleted()
    {
        // Arrange
        var voucher = new Voucher(
            "TEST_CODE",
            "Test Description",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        await _repository.CreateAsync(voucher);

        // Act
        var result = await _repository.DeleteAsync(voucher.Id);

        // Assert
        result.Should().BeTrue();
        var deletedVoucher = await _repository.GetByIdAsync(voucher.Id);
        deletedVoucher.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentVoucher_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_DeletedVoucher_ShouldNotAppearInGetAll()
    {
        // Arrange
        var voucher1 = new Voucher(
            "CODE1",
            "Description 1",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        var voucher2 = new Voucher(
            "CODE2",
            "Description 2",
            20.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        await _repository.CreateAsync(voucher1);
        await _repository.CreateAsync(voucher2);

        // Act
        await _repository.DeleteAsync(voucher1.Id);
        var allVouchers = await _repository.GetAllAsync();

        // Assert
        allVouchers.Should().HaveCount(1);
        allVouchers.First().Id.Should().Be(voucher2.Id);
    }
}

