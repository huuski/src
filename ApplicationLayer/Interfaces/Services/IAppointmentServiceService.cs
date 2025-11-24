using ApplicationLayer.DTOs.AppointmentService;

namespace ApplicationLayer.Interfaces.Services;

public interface IAppointmentServiceService
{
    Task<AppointmentServiceDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentServiceDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentServiceDto>> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<AppointmentServiceDto> CreateAsync(CreateAppointmentServiceDto dto, CancellationToken cancellationToken = default);
    Task<AppointmentServiceDto> UpdateAsync(Guid id, UpdateAppointmentServiceDto dto, CancellationToken cancellationToken = default);
    Task<AppointmentServiceDto> UpdateStatusAsync(UpdateAppointmentServiceStatusDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

