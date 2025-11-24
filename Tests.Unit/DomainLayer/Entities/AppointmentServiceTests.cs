using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.DomainLayer.Entities;

public class AppointmentServiceTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateAppointmentService()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var sessionNumber = 1;
        var sessionTotal = 5;
        var notes = "Initial session notes";

        // Act
        var appointmentService = new AppointmentService(
            appointmentId,
            serviceId,
            sessionNumber,
            sessionTotal,
            AppointmentServiceStatus.Pending,
            notes
        );

        // Assert
        appointmentService.Should().NotBeNull();
        appointmentService.AppointmentId.Should().Be(appointmentId);
        appointmentService.ServiceId.Should().Be(serviceId);
        appointmentService.SessionNumber.Should().Be(sessionNumber);
        appointmentService.SessionTotal.Should().Be(sessionTotal);
        appointmentService.Status.Should().Be(AppointmentServiceStatus.Pending);
        appointmentService.Notes.Should().Be(notes);
    }

    [Fact]
    public void Constructor_EmptyAppointmentId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new AppointmentService(Guid.Empty, Guid.NewGuid(), 1, 5)
        );
    }

    [Fact]
    public void Constructor_EmptyServiceId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new AppointmentService(Guid.NewGuid(), Guid.Empty, 1, 5)
        );
    }

    [Fact]
    public void Constructor_SessionNumberZero_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new AppointmentService(Guid.NewGuid(), Guid.NewGuid(), 0, 5)
        );
    }

    [Fact]
    public void Constructor_SessionTotalZero_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new AppointmentService(Guid.NewGuid(), Guid.NewGuid(), 1, 0)
        );
    }

    [Fact]
    public void Constructor_SessionNumberGreaterThanSessionTotal_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new AppointmentService(Guid.NewGuid(), Guid.NewGuid(), 6, 5)
        );
    }

    [Fact]
    public void Update_ValidParameters_ShouldUpdateAppointmentService()
    {
        // Arrange
        var appointmentService = new AppointmentService(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            5,
            AppointmentServiceStatus.Pending
        );

        // Act
        appointmentService.Update(2, 5, AppointmentServiceStatus.InProgress, "Updated notes");

        // Assert
        appointmentService.SessionNumber.Should().Be(2);
        appointmentService.SessionTotal.Should().Be(5);
        appointmentService.Status.Should().Be(AppointmentServiceStatus.InProgress);
        appointmentService.Notes.Should().Be("Updated notes");
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateStatus()
    {
        // Arrange
        var appointmentService = new AppointmentService(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            5
        );

        // Act
        appointmentService.UpdateStatus(AppointmentServiceStatus.Finalized);

        // Assert
        appointmentService.Status.Should().Be(AppointmentServiceStatus.Finalized);
    }

    [Fact]
    public void UpdateNotes_ShouldUpdateNotes()
    {
        // Arrange
        var appointmentService = new AppointmentService(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            5
        );

        // Act
        appointmentService.UpdateNotes("New notes");

        // Assert
        appointmentService.Notes.Should().Be("New notes");
    }
}

