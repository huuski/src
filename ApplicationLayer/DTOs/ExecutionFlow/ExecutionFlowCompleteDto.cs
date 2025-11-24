using System.Text.Json;

namespace ApplicationLayer.DTOs.ExecutionFlow;

public class ExecutionFlowCompleteDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public JsonElement Steps { get; set; }
}

