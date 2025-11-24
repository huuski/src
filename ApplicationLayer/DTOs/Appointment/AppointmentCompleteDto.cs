using System.Text.Json.Serialization;
using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.Appointment;

public class AppointmentCompleteDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("customerId")]
    public Guid CustomerId { get; set; }
    
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }
    
    [JsonPropertyName("initDate")]
    public DateTime InitDate { get; set; }
    
    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; set; }
    
    [JsonPropertyName("duration")]
    public TimeSpan Duration { get; set; }
    
    [JsonPropertyName("status")]
    public AppointmentStatus Status { get; set; }
    
    [JsonPropertyName("services")]
    public List<AppointmentServiceSimpleDto> Services { get; set; } = new();
}

