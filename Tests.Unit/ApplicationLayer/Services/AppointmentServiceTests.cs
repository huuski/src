using ApplicationLayer.DTOs.Appointment;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.ApplicationLayer.Services;

public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly Mock<IAppointmentServiceRepository> _appointmentServiceRepositoryMock;
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly IAppointmentService _appointmentService;

    public AppointmentServiceTests()
    {
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _appointmentServiceRepositoryMock = new Mock<IAppointmentServiceRepository>();
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _appointmentService = new global::ApplicationLayer.Services.AppointmentService(
            _appointmentRepositoryMock.Object,
            _appointmentServiceRepositoryMock.Object,
            _serviceRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ShouldCreateAppointment()
    {
        // Arrange
        var dto = new CreateAppointmentDto
        {
            CustomerId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            InitDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            Status = AppointmentStatus.Scheduled
        };

        _appointmentRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, CancellationToken ct) => a);

        // Act
        var result = await _appointmentService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(dto.CustomerId);
        result.UserId.Should().Be(dto.UserId);
        result.Status.Should().Be(dto.Status);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnAppointment()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(1)
        );

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var result = await _appointmentService.GetByIdAsync(appointmentId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(appointment.Id);
    }

    [Fact]
    public async Task GetByIdAsync_AppointmentNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _appointmentService.GetByIdAsync(appointmentId)
        );
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_ShouldUpdateAppointment()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(1)
        );

        var dto = new UpdateAppointmentDto
        {
            InitDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(2).AddHours(2),
            Status = AppointmentStatus.Confirmed
        };

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        _appointmentRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, CancellationToken ct) => a);

        // Act
        var result = await _appointmentService.UpdateAsync(appointmentId, dto);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ValidId_ShouldDeleteAppointment()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(1)
        );

        _appointmentRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        _appointmentRepositoryMock
            .Setup(x => x.DeleteAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _appointmentService.DeleteAsync(appointmentId);

        // Assert
        result.Should().BeTrue();
    }
}

