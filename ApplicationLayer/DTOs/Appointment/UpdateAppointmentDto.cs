using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.Appointment;

public class UpdateAppointmentDto
{
    public DateTime InitDate { get; set; }
    public DateTime EndDate { get; set; }
    public AppointmentStatus? Status { get; set; }
    public Guid UserId { get; set; }
}

