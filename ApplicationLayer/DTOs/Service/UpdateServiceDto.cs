using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.Service;

public class UpdateServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ServiceCategory Category { get; set; }
    public decimal Amount { get; set; }
    public string? Image { get; set; }
    public TimeSpan Duration { get; set; }
    public Guid? ExecutionFlowId { get; set; }
}

