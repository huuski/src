using ApplicationLayer.DTOs.Voucher;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Services;
using DomainLayer.Entities;
using FluentAssertions;
using Moq;

namespace Tests.Unit.ApplicationLayer.Services;

public class VoucherServiceTests
{
    private readonly Mock<IVoucherRepository> _voucherRepositoryMock;
    private readonly IVoucherService _voucherService;

    public VoucherServiceTests()
    {
        _voucherRepositoryMock = new Mock<IVoucherRepository>();
        _voucherService = new VoucherService(_voucherRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnVoucherDto()
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var voucher = CreateTestVoucher(voucherId);

        _voucherRepositoryMock
            .Setup(x => x.GetByIdAsync(voucherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(voucher);

        // Act
        var result = await _voucherService.GetByIdAsync(voucherId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(voucherId);
        result.Code.Should().Be(voucher.Code);
        result.Description.Should().Be(voucher.Description);
        result.DiscountAmount.Should().Be(voucher.DiscountAmount);
    }

    [Fact]
    public async Task GetByIdAsync_VoucherNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var voucherId = Guid.NewGuid();

        _voucherRepositoryMock
            .Setup(x => x.GetByIdAsync(voucherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Voucher?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _voucherService.GetByIdAsync(voucherId)
        );
    }

    [Fact]
    public async Task GetByCodeAsync_ValidCode_ShouldReturnVoucherDto()
    {
        // Arrange
        var code = "DISCOUNT10";
        var voucher = CreateTestVoucher(Guid.NewGuid(), code);

        _voucherRepositoryMock
            .Setup(x => x.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(voucher);

        // Act
        var result = await _voucherService.GetByCodeAsync(code);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(code);
    }

    [Fact]
    public async Task GetByCodeAsync_VoucherNotFound_ShouldReturnNull()
    {
        // Arrange
        var code = "INVALID";

        _voucherRepositoryMock
            .Setup(x => x.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Voucher?)null);

        // Act
        var result = await _voucherService.GetByCodeAsync(code);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCodeAsync_EmptyCode_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _voucherService.GetByCodeAsync("")
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllVouchers()
    {
        // Arrange
        var vouchers = new List<Voucher>
        {
            CreateTestVoucher(Guid.NewGuid()),
            CreateTestVoucher(Guid.NewGuid())
        };

        _voucherRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(vouchers);

        // Act
        var result = await _voucherService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ShouldCreateVoucher()
    {
        // Arrange
        var dto = new CreateVoucherDto
        {
            Code = "DISCOUNT10",
            Description = "10% discount",
            DiscountAmount = 10.00m,
            ValidFrom = DateTime.UtcNow,
            ValidUntil = DateTime.UtcNow.AddDays(30),
            IsActive = true,
            MinimumPurchaseAmount = 100.00m
        };

        _voucherRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Voucher>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Voucher v, CancellationToken ct) => v);

        // Act
        var result = await _voucherService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be(dto.Code);
        result.Description.Should().Be(dto.Description);
        result.DiscountAmount.Should().Be(dto.DiscountAmount);
        result.ValidFrom.Should().Be(dto.ValidFrom);
        result.ValidUntil.Should().Be(dto.ValidUntil);
        result.IsActive.Should().Be(dto.IsActive);
        result.MinimumPurchaseAmount.Should().Be(dto.MinimumPurchaseAmount);
        _voucherRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Voucher>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_ShouldUpdateVoucher()
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var existingVoucher = CreateTestVoucher(voucherId);

        var dto = new UpdateVoucherDto
        {
            Code = "UPDATED_CODE",
            Description = "Updated Description",
            DiscountAmount = 20.00m,
            ValidFrom = DateTime.UtcNow.AddDays(1),
            ValidUntil = DateTime.UtcNow.AddDays(60),
            IsActive = false,
            MinimumPurchaseAmount = 200.00m
        };

        _voucherRepositoryMock
            .Setup(x => x.GetByIdAsync(voucherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingVoucher);

        _voucherRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Voucher>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Voucher v, CancellationToken ct) => v);

        // Act
        var result = await _voucherService.UpdateAsync(voucherId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be(dto.Code);
        result.Description.Should().Be(dto.Description);
        result.DiscountAmount.Should().Be(dto.DiscountAmount);
        _voucherRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Voucher>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_VoucherNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var dto = new UpdateVoucherDto
        {
            Code = "CODE",
            Description = "Description",
            DiscountAmount = 10.00m,
            ValidFrom = DateTime.UtcNow,
            ValidUntil = DateTime.UtcNow.AddDays(30),
            IsActive = true
        };

        _voucherRepositoryMock
            .Setup(x => x.GetByIdAsync(voucherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Voucher?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _voucherService.UpdateAsync(voucherId, dto)
        );
    }

    [Fact]
    public async Task DeleteAsync_ValidId_ShouldDeleteVoucher()
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var voucher = CreateTestVoucher(voucherId);

        _voucherRepositoryMock
            .Setup(x => x.GetByIdAsync(voucherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(voucher);

        _voucherRepositoryMock
            .Setup(x => x.DeleteAsync(voucherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _voucherService.DeleteAsync(voucherId);

        // Assert
        result.Should().BeTrue();
        _voucherRepositoryMock.Verify(x => x.DeleteAsync(voucherId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_VoucherNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var voucherId = Guid.NewGuid();

        _voucherRepositoryMock
            .Setup(x => x.GetByIdAsync(voucherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Voucher?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _voucherService.DeleteAsync(voucherId)
        );
    }

    private Voucher CreateTestVoucher(
        Guid id,
        string code = "TEST_CODE",
        string description = "Test Description",
        decimal discountAmount = 10.00m,
        DateTime? validFrom = null,
        DateTime? validUntil = null,
        bool isActive = true,
        decimal? minimumPurchaseAmount = null)
    {
        var voucher = new Voucher(
            code,
            description,
            discountAmount,
            validFrom ?? DateTime.UtcNow,
            validUntil ?? DateTime.UtcNow.AddDays(30),
            isActive,
            minimumPurchaseAmount
        );

        // Set the ID using reflection
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(voucher, id);
        }

        return voucher;
    }
}

