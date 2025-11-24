using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.AppointmentService;

public class UpdateAppointmentServiceDto
{
    public int SessionNumber { get; set; }
    public int SessionTotal { get; set; }
    public AppointmentServiceStatus? Status { get; set; }
    public string? Notes { get; set; }
}

