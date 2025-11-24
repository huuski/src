using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.AppointmentService;

public class CreateAppointmentServiceDto
{
    public Guid AppointmentId { get; set; }
    public Guid ServiceId { get; set; }
    public int SessionNumber { get; set; }
    public int SessionTotal { get; set; }
    public AppointmentServiceStatus Status { get; set; } = AppointmentServiceStatus.Pending;
    public string? Notes { get; set; }
}

