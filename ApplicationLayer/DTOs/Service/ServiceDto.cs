using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.Service;

public class ServiceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ServiceCategory Category { get; set; }
    public decimal Amount { get; set; }
    public string? Image { get; set; }
    public TimeSpan Duration { get; set; }
    public Guid? ExecutionFlowId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

