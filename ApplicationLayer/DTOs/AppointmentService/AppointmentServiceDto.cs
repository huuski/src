using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.AppointmentService;

public class AppointmentServiceDto
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid ServiceId { get; set; }
    public int SessionNumber { get; set; }
    public int SessionTotal { get; set; }
    public AppointmentServiceStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? CancelationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

