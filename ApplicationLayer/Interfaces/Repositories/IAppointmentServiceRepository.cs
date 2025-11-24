using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Repositories;

public interface IAppointmentServiceRepository
{
    Task<AppointmentService?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentService>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentService>> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<AppointmentService> CreateAsync(AppointmentService appointmentService, CancellationToken cancellationToken = default);
    Task<AppointmentService> UpdateAsync(AppointmentService appointmentService, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

