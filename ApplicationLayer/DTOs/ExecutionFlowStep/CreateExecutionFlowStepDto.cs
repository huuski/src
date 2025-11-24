using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApplicationLayer.DTOs.ExecutionFlowStep;

public class CreateExecutionFlowStepDto
{
    [Required]
    [JsonPropertyName("executionFlowId")]
    public Guid ExecutionFlowId { get; set; }
    
    [Required]
    [JsonPropertyName("stepNumber")]
    public int StepNumber { get; set; }
    
    [Required]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }
    
    [Required]
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [JsonPropertyName("displayStepNumber")]
    public int DisplayStepNumber { get; set; }
    
    [Required]
    [JsonPropertyName("displayTitle")]
    public string DisplayTitle { get; set; } = string.Empty;
}

