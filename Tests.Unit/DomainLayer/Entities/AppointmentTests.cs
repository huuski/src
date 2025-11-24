using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.DomainLayer.Entities;

public class AppointmentTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateAppointment()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var initDate = DateTime.UtcNow.AddDays(1);
        var endDate = initDate.AddHours(1);

        // Act
        var appointment = new Appointment(customerId, userId, initDate, endDate);

        // Assert
        appointment.Should().NotBeNull();
        appointment.CustomerId.Should().Be(customerId);
        appointment.UserId.Should().Be(userId);
        appointment.InitDate.Should().Be(initDate);
        appointment.EndDate.Should().Be(endDate);
        appointment.Status.Should().Be(AppointmentStatus.Scheduled);
        appointment.Duration.Should().Be(endDate - initDate);
    }

    [Fact]
    public void Constructor_EmptyCustomerId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Appointment(Guid.Empty, Guid.NewGuid(), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1))
        );
    }

    [Fact]
    public void Constructor_EmptyUserId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Appointment(Guid.NewGuid(), Guid.Empty, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1))
        );
    }

    [Fact]
    public void Constructor_EndDateBeforeInitDate_ShouldThrowArgumentException()
    {
        // Arrange
        var initDate = DateTime.UtcNow.AddDays(1);
        var endDate = initDate.AddHours(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Appointment(Guid.NewGuid(), Guid.NewGuid(), initDate, endDate)
        );
    }

    [Fact]
    public void Constructor_EndDateEqualToInitDate_ShouldThrowArgumentException()
    {
        // Arrange
        var initDate = DateTime.UtcNow.AddDays(1);
        var endDate = initDate;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Appointment(Guid.NewGuid(), Guid.NewGuid(), initDate, endDate)
        );
    }

    [Fact]
    public void Constructor_InitDateTooFarInPast_ShouldThrowArgumentException()
    {
        // Arrange
        var initDate = DateTime.UtcNow.AddHours(-2);
        var endDate = initDate.AddHours(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Appointment(Guid.NewGuid(), Guid.NewGuid(), initDate, endDate)
        );
    }

    [Fact]
    public void Update_ValidParameters_ShouldUpdateAppointment()
    {
        // Arrange
        var appointment = new Appointment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(1)
        );

        var newInitDate = DateTime.UtcNow.AddDays(2);
        var newEndDate = newInitDate.AddHours(2);

        // Act
        appointment.Update(newInitDate, newEndDate, AppointmentStatus.Confirmed);

        // Assert
        appointment.InitDate.Should().Be(newInitDate);
        appointment.EndDate.Should().Be(newEndDate);
        appointment.Status.Should().Be(AppointmentStatus.Confirmed);
        appointment.Duration.Should().Be(newEndDate - newInitDate);
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateStatus()
    {
        // Arrange
        var appointment = new Appointment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(1)
        );

        // Act
        appointment.UpdateStatus(AppointmentStatus.Confirmed);

        // Assert
        appointment.Status.Should().Be(AppointmentStatus.Confirmed);
    }

    [Fact]
    public void Duration_ShouldCalculateCorrectly()
    {
        // Arrange
        var initDate = DateTime.UtcNow.AddDays(1);
        var endDate = initDate.AddHours(2).AddMinutes(30);

        // Act
        var appointment = new Appointment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            initDate,
            endDate
        );

        // Assert
        appointment.Duration.Should().Be(TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(30)));
    }
}

