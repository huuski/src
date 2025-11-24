using ApplicationLayer.DTOs.ExecutionFlowStepItem;

namespace ApplicationLayer.DTOs.ExecutionFlowStep;

public class ExecutionFlowStepWithItemsDto
{
    public Guid Id { get; set; }
    public Guid ExecutionFlowId { get; set; }
    public int StepNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string Description { get; set; } = string.Empty;
    public int DisplayStepNumber { get; set; }
    public string DisplayTitle { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ExecutionFlowStepItemDto> Questions { get; set; } = new();
}

