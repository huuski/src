using DomainLayer.Entities;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.Entities;

public class VoucherTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateVoucher()
    {
        // Arrange
        var code = "DISCOUNT10";
        var description = "10% discount voucher";
        var discountAmount = 10.00m;
        var validFrom = DateTime.UtcNow;
        var validUntil = DateTime.UtcNow.AddDays(30);
        var isActive = true;
        var minimumPurchaseAmount = 100.00m;

        // Act
        var voucher = new Voucher(code, description, discountAmount, validFrom, validUntil, isActive, minimumPurchaseAmount);

        // Assert
        voucher.Should().NotBeNull();
        voucher.Code.Should().Be(code);
        voucher.Description.Should().Be(description);
        voucher.DiscountAmount.Should().Be(discountAmount);
        voucher.ValidFrom.Should().Be(validFrom);
        voucher.ValidUntil.Should().Be(validUntil);
        voucher.IsActive.Should().Be(isActive);
        voucher.MinimumPurchaseAmount.Should().Be(minimumPurchaseAmount);
        voucher.Id.Should().NotBeEmpty();
        voucher.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_WithoutMinimumPurchaseAmount_ShouldCreateVoucher()
    {
        // Arrange
        var code = "DISCOUNT10";
        var description = "10% discount voucher";
        var discountAmount = 10.00m;
        var validFrom = DateTime.UtcNow;
        var validUntil = DateTime.UtcNow.AddDays(30);

        // Act
        var voucher = new Voucher(code, description, discountAmount, validFrom, validUntil);

        // Assert
        voucher.Should().NotBeNull();
        voucher.MinimumPurchaseAmount.Should().BeNull();
    }

    [Fact]
    public void Constructor_NullCode_ShouldThrowArgumentNullException()
    {
        // Arrange
        var description = "Description";
        var discountAmount = 10.00m;
        var validFrom = DateTime.UtcNow;
        var validUntil = DateTime.UtcNow.AddDays(30);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Voucher(null!, description, discountAmount, validFrom, validUntil)
        );
    }

    [Fact]
    public void Constructor_EmptyCode_ShouldThrowArgumentException()
    {
        // Arrange
        var description = "Description";
        var discountAmount = 10.00m;
        var validFrom = DateTime.UtcNow;
        var validUntil = DateTime.UtcNow.AddDays(30);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Voucher("", description, discountAmount, validFrom, validUntil)
        );
    }

    [Fact]
    public void Constructor_NegativeDiscountAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var code = "DISCOUNT10";
        var description = "Description";
        var discountAmount = -10.00m;
        var validFrom = DateTime.UtcNow;
        var validUntil = DateTime.UtcNow.AddDays(30);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Voucher(code, description, discountAmount, validFrom, validUntil)
        );
    }

    [Fact]
    public void Constructor_ValidUntilBeforeValidFrom_ShouldThrowArgumentException()
    {
        // Arrange
        var code = "DISCOUNT10";
        var description = "Description";
        var discountAmount = 10.00m;
        var validFrom = DateTime.UtcNow;
        var validUntil = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Voucher(code, description, discountAmount, validFrom, validUntil)
        );
    }

    [Fact]
    public void Constructor_NegativeMinimumPurchaseAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var code = "DISCOUNT10";
        var description = "Description";
        var discountAmount = 10.00m;
        var validFrom = DateTime.UtcNow;
        var validUntil = DateTime.UtcNow.AddDays(30);
        var minimumPurchaseAmount = -100.00m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Voucher(code, description, discountAmount, validFrom, validUntil, true, minimumPurchaseAmount)
        );
    }

    [Fact]
    public void Update_ValidParameters_ShouldUpdateVoucher()
    {
        // Arrange
        var voucher = new Voucher(
            "OLD_CODE",
            "Old Description",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        var newCode = "NEW_CODE";
        var newDescription = "New Description";
        var newDiscountAmount = 20.00m;
        var newValidFrom = DateTime.UtcNow.AddDays(1);
        var newValidUntil = DateTime.UtcNow.AddDays(60);
        var newIsActive = false;
        var newMinimumPurchaseAmount = 200.00m;

        // Act
        voucher.Update(newCode, newDescription, newDiscountAmount, newValidFrom, newValidUntil, newIsActive, newMinimumPurchaseAmount);

        // Assert
        voucher.Code.Should().Be(newCode);
        voucher.Description.Should().Be(newDescription);
        voucher.DiscountAmount.Should().Be(newDiscountAmount);
        voucher.ValidFrom.Should().Be(newValidFrom);
        voucher.ValidUntil.Should().Be(newValidUntil);
        voucher.IsActive.Should().Be(newIsActive);
        voucher.MinimumPurchaseAmount.Should().Be(newMinimumPurchaseAmount);
        voucher.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var voucher = new Voucher(
            "CODE",
            "Description",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30),
            false
        );

        // Act
        voucher.Activate();

        // Assert
        voucher.IsActive.Should().BeTrue();
        voucher.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var voucher = new Voucher(
            "CODE",
            "Description",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30),
            true
        );

        // Act
        voucher.Deactivate();

        // Assert
        voucher.IsActive.Should().BeFalse();
        voucher.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void IsValid_ActiveVoucherWithinValidityPeriod_ShouldReturnTrue()
    {
        // Arrange
        var voucher = new Voucher(
            "CODE",
            "Description",
            10.00m,
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow.AddDays(20),
            true
        );

        // Act
        var result = voucher.IsValid();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_InactiveVoucher_ShouldReturnFalse()
    {
        // Arrange
        var voucher = new Voucher(
            "CODE",
            "Description",
            10.00m,
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow.AddDays(20),
            false
        );

        // Act
        var result = voucher.IsValid();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_ExpiredVoucher_ShouldReturnFalse()
    {
        // Arrange
        var voucher = new Voucher(
            "CODE",
            "Description",
            10.00m,
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow.AddDays(-10),
            true
        );

        // Act
        var result = voucher.IsValid();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithSpecificDate_ShouldReturnCorrectValue()
    {
        // Arrange
        var checkDate = DateTime.UtcNow.AddDays(5);
        var voucher = new Voucher(
            "CODE",
            "Description",
            10.00m,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(10),
            true
        );

        // Act
        var result = voucher.IsValid(checkDate);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void MarkAsDeleted_ShouldSetDeletedAt()
    {
        // Arrange
        var voucher = new Voucher(
            "CODE",
            "Description",
            10.00m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        // Act
        voucher.MarkAsDeleted();

        // Assert
        voucher.IsDeleted.Should().BeTrue();
        voucher.DeletedAt.Should().NotBeNull();
        voucher.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}

