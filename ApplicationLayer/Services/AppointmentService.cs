using ApplicationLayer.DTOs.Appointment;
using ApplicationLayer.DTOs.AppointmentService;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;

namespace ApplicationLayer.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentServiceRepository _appointmentServiceRepository;
    private readonly IServiceRepository _serviceRepository;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IAppointmentServiceRepository appointmentServiceRepository,
        IServiceRepository serviceRepository)
    {
        _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        _appointmentServiceRepository = appointmentServiceRepository ?? throw new ArgumentNullException(nameof(appointmentServiceRepository));
        _serviceRepository = serviceRepository ?? throw new ArgumentNullException(nameof(serviceRepository));
    }

    public async Task<AppointmentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            throw new ArgumentException($"Appointment with id {id} not found", nameof(id));

        return MapToDto(appointment);
    }

    public async Task<AppointmentCompleteDto> GetByIdCompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            throw new ArgumentException($"Appointment with id {id} not found", nameof(id));

        var appointmentServices = await _appointmentServiceRepository.GetByAppointmentIdAsync(id, cancellationToken);
        var servicesSimple = new List<AppointmentServiceSimpleDto>();

        foreach (var appointmentService in appointmentServices)
        {
            // Buscar o Service para obter o ExecutionFlowId e Duration
            var service = await _serviceRepository.GetByIdAsync(appointmentService.ServiceId, cancellationToken);
            servicesSimple.Add(MapToSimpleDto(appointmentService, service?.ExecutionFlowId, service?.Duration));
        }
        
        return MapToCompleteDto(appointment, servicesSimple);
    }

    public async Task<IEnumerable<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);
        return appointments.Select(MapToDto);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return appointments.Select(MapToDto);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.GetByUserIdAsync(userId, cancellationToken);
        return appointments.Select(MapToDto);
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        // Validate UserId if provided (it's optional)
        if (dto.UserId.HasValue && dto.UserId.Value == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty when provided", nameof(dto.UserId));

        var appointment = new DomainLayer.Entities.Appointment(
            dto.CustomerId,
            dto.UserId,
            dto.InitDate,
            dto.EndDate,
            dto.Status
        );

        var createdAppointment = await _appointmentRepository.CreateAsync(appointment, cancellationToken);
        return MapToDto(createdAppointment);
    }

    public async Task<AppointmentDto> UpdateAsync(Guid id, UpdateAppointmentDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            throw new ArgumentException($"Appointment with id {id} not found", nameof(id));

        appointment.Update(
            dto.InitDate,
            dto.EndDate,
            dto.Status,
            dto.UserId
        );

        var updatedAppointment = await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        return MapToDto(updatedAppointment);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            throw new ArgumentException($"Appointment with id {id} not found", nameof(id));

        return await _appointmentRepository.DeleteAsync(id, cancellationToken);
    }

    private static AppointmentDto MapToDto(DomainLayer.Entities.Appointment appointment)
    {
        return new AppointmentDto
        {
            Id = appointment.Id,
            CustomerId = appointment.CustomerId,
            UserId = appointment.UserId,
            InitDate = appointment.InitDate,
            EndDate = appointment.EndDate,
            Duration = appointment.Duration,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt
        };
    }

    private static AppointmentServiceSimpleDto MapToSimpleDto(
        DomainLayer.Entities.AppointmentService appointmentService,
        Guid? executionFlowId,
        TimeSpan? duration)
    {
        return new AppointmentServiceSimpleDto
        {
            Id = appointmentService.Id,
            ServiceId = appointmentService.ServiceId,
            ExecutionFlowId = executionFlowId,
            SessionNumber = appointmentService.SessionNumber,
            SessionTotal = appointmentService.SessionTotal,
            Status = appointmentService.Status,
            Duration = duration ?? TimeSpan.Zero
        };
    }

    private static AppointmentCompleteDto MapToCompleteDto(
        DomainLayer.Entities.Appointment appointment,
        List<AppointmentServiceSimpleDto> services)
    {
        // Calcular a duração total como a soma das durações dos serviços
        var totalDuration = services.Aggregate(TimeSpan.Zero, (sum, service) => sum + service.Duration);
        
        return new AppointmentCompleteDto
        {
            Id = appointment.Id,
            CustomerId = appointment.CustomerId,
            UserId = appointment.UserId,
            InitDate = appointment.InitDate,
            EndDate = appointment.EndDate,
            Duration = totalDuration,
            Status = appointment.Status,
            Services = services.OrderBy(s => s.SessionNumber).ToList()
        };
    }
}

