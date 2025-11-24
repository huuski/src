using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.Appointment;

public class CreateAppointmentDto
{
    [Required]
    [JsonPropertyName("customerId")]
    public Guid CustomerId { get; set; }
    
    [Required]
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }
    
    [Required]
    [JsonPropertyName("initDate")]
    public DateTime InitDate { get; set; }
    
    [Required]
    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; set; }
    
    [JsonPropertyName("status")]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
}

