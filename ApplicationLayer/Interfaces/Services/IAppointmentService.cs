using ApplicationLayer.DTOs.Appointment;

namespace ApplicationLayer.Interfaces.Services;

public interface IAppointmentService
{
    Task<AppointmentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AppointmentCompleteDto> GetByIdCompleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, CancellationToken cancellationToken = default);
    Task<AppointmentDto> UpdateAsync(Guid id, UpdateAppointmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

