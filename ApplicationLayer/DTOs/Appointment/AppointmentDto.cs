using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.Appointment;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime InitDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan Duration { get; set; }
    public AppointmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

