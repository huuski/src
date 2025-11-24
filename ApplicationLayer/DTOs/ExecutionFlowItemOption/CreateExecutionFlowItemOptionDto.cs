namespace ApplicationLayer.DTOs.ExecutionFlowItemOption;

public class CreateExecutionFlowItemOptionDto
{
    public Guid ExecutionFlowStepItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool EnableExtraAnswer { get; set; }
    public int? ExtraAnswerMaxLength { get; set; }
}
