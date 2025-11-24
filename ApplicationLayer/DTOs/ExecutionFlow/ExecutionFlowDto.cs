namespace ApplicationLayer.DTOs.ExecutionFlow;

public class ExecutionFlowDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Flow { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

