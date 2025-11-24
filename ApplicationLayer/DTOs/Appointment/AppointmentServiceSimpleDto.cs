using System.Text.Json.Serialization;
using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.Appointment;

public class AppointmentServiceSimpleDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("serviceId")]
    public Guid ServiceId { get; set; }
    
    [JsonPropertyName("executionflowid")]
    public Guid? ExecutionFlowId { get; set; }
    
    [JsonPropertyName("sessionNumber")]
    public int SessionNumber { get; set; }
    
    [JsonPropertyName("sessionTotal")]
    public int SessionTotal { get; set; }
    
    [JsonPropertyName("status")]
    public AppointmentServiceStatus Status { get; set; }
    
    [JsonPropertyName("duration")]
    public TimeSpan Duration { get; set; }
}

