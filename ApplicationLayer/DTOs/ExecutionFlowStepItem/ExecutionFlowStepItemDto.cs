using ApplicationLayer.DTOs.ExecutionFlowItemOption;
using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.ExecutionFlowStepItem;

public class ExecutionFlowStepItemDto
{
    public Guid Id { get; set; }
    public Guid ExecutionFlowStepId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public AnswerType Type { get; set; }
    public int? MaxLength { get; set; }
    public bool Required { get; set; }
    public int? MaxImages { get; set; }
    public List<string>? AcceptedImageTypes { get; set; }
    public List<ExecutionFlowItemOptionDto> Options { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

