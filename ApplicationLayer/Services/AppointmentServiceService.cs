using ApplicationLayer.DTOs.AppointmentService;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;

namespace ApplicationLayer.Services;

public class AppointmentServiceService : IAppointmentServiceService
{
    private readonly IAppointmentServiceRepository _appointmentServiceRepository;

    public AppointmentServiceService(IAppointmentServiceRepository appointmentServiceRepository)
    {
        _appointmentServiceRepository = appointmentServiceRepository ?? throw new ArgumentNullException(nameof(appointmentServiceRepository));
    }

    public async Task<AppointmentServiceDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointmentService = await _appointmentServiceRepository.GetByIdAsync(id, cancellationToken);
        if (appointmentService == null)
            throw new ArgumentException($"AppointmentService with id {id} not found", nameof(id));

        return MapToDto(appointmentService);
    }

    public async Task<IEnumerable<AppointmentServiceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var appointmentServices = await _appointmentServiceRepository.GetAllAsync(cancellationToken);
        return appointmentServices.Select(MapToDto);
    }

    public async Task<IEnumerable<AppointmentServiceDto>> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointmentServices = await _appointmentServiceRepository.GetByAppointmentIdAsync(appointmentId, cancellationToken);
        return appointmentServices.Select(MapToDto);
    }

    public async Task<AppointmentServiceDto> CreateAsync(CreateAppointmentServiceDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var appointmentService = new DomainLayer.Entities.AppointmentService(
            dto.AppointmentId,
            dto.ServiceId,
            dto.SessionNumber,
            dto.SessionTotal,
            dto.Status,
            dto.Notes
        );

        var createdAppointmentService = await _appointmentServiceRepository.CreateAsync(appointmentService, cancellationToken);
        return MapToDto(createdAppointmentService);
    }

    public async Task<AppointmentServiceDto> UpdateAsync(Guid id, UpdateAppointmentServiceDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var appointmentService = await _appointmentServiceRepository.GetByIdAsync(id, cancellationToken);
        if (appointmentService == null)
            throw new ArgumentException($"AppointmentService with id {id} not found", nameof(id));

        appointmentService.Update(
            dto.SessionNumber,
            dto.SessionTotal,
            dto.Status,
            dto.Notes
        );

        var updatedAppointmentService = await _appointmentServiceRepository.UpdateAsync(appointmentService, cancellationToken);
        return MapToDto(updatedAppointmentService);
    }

    public async Task<AppointmentServiceDto> UpdateStatusAsync(UpdateAppointmentServiceStatusDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var appointmentService = await _appointmentServiceRepository.GetByIdAsync(dto.AppointmentServiceId, cancellationToken);
        if (appointmentService == null)
            throw new ArgumentException($"AppointmentService with id {dto.AppointmentServiceId} not found", nameof(dto.AppointmentServiceId));

        // Validar que cancelation reason é obrigatório quando status é Canceled
        if (dto.AppointmentServiceStatus == AppointmentServiceStatus.Canceled && 
            string.IsNullOrWhiteSpace(dto.AppointmentServiceCancelationReason))
        {
            throw new ArgumentException("AppointmentServiceCancelationReason is required when status is Canceled", nameof(dto));
        }

        appointmentService.UpdateStatus(dto.AppointmentServiceStatus, dto.AppointmentServiceCancelationReason);

        var updatedAppointmentService = await _appointmentServiceRepository.UpdateAsync(appointmentService, cancellationToken);
        return MapToDto(updatedAppointmentService);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointmentService = await _appointmentServiceRepository.GetByIdAsync(id, cancellationToken);
        if (appointmentService == null)
            throw new ArgumentException($"AppointmentService with id {id} not found", nameof(id));

        return await _appointmentServiceRepository.DeleteAsync(id, cancellationToken);
    }

    private static AppointmentServiceDto MapToDto(DomainLayer.Entities.AppointmentService appointmentService)
    {
        return new AppointmentServiceDto
        {
            Id = appointmentService.Id,
            AppointmentId = appointmentService.AppointmentId,
            ServiceId = appointmentService.ServiceId,
            SessionNumber = appointmentService.SessionNumber,
            SessionTotal = appointmentService.SessionTotal,
            Status = appointmentService.Status,
            Notes = appointmentService.Notes,
            CancelationReason = appointmentService.CancelationReason,
            CreatedAt = appointmentService.CreatedAt,
            UpdatedAt = appointmentService.UpdatedAt
        };
    }
}

