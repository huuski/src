using ApplicationLayer.DTOs.AppointmentService;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using Moq;
using Xunit;
using AppointmentServiceEntity = DomainLayer.Entities.AppointmentService;

namespace Tests.Unit.ApplicationLayer.Services;

public class AppointmentServiceServiceTests
{
    private readonly Mock<IAppointmentServiceRepository> _appointmentServiceRepositoryMock;
    private readonly IAppointmentServiceService _appointmentServiceService;

    public AppointmentServiceServiceTests()
    {
        _appointmentServiceRepositoryMock = new Mock<IAppointmentServiceRepository>();
        _appointmentServiceService = new global::ApplicationLayer.Services.AppointmentServiceService(_appointmentServiceRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ShouldCreateAppointmentService()
    {
        // Arrange
        var dto = new CreateAppointmentServiceDto
        {
            AppointmentId = Guid.NewGuid(),
            ServiceId = Guid.NewGuid(),
            SessionNumber = 1,
            SessionTotal = 5,
            Status = AppointmentServiceStatus.Pending,
            Notes = "Test notes"
        };

        _appointmentServiceRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<AppointmentServiceEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppointmentServiceEntity a, CancellationToken ct) => a);

        // Act
        var result = await _appointmentServiceService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.AppointmentId.Should().Be(dto.AppointmentId);
        result.ServiceId.Should().Be(dto.ServiceId);
        result.SessionNumber.Should().Be(dto.SessionNumber);
        result.SessionTotal.Should().Be(dto.SessionTotal);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnAppointmentService()
    {
        // Arrange
        var appointmentServiceId = Guid.NewGuid();
        var entity = new AppointmentServiceEntity(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            5
        );

        _appointmentServiceRepositoryMock
            .Setup(x => x.GetByIdAsync(appointmentServiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _appointmentServiceService.GetByIdAsync(appointmentServiceId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
    }
}

