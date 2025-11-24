using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ApplicationLayer.DTOs.ExecutionFlowItemOption;
using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.ExecutionFlowStepItem;

public class CreateExecutionFlowStepItemDto : IValidatableObject
{
    [Required]
    [JsonPropertyName("executionFlowStepId")]
    public Guid ExecutionFlowStepId { get; set; }
    
    [Required]
    [JsonPropertyName("order")]
    public int Order { get; set; }
    
    [Required]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [Required]
    [JsonPropertyName("type")]
    public AnswerType Type { get; set; }
    
    [JsonPropertyName("maxLength")]
    public int? MaxLength { get; set; }
    
    [Required]
    [JsonPropertyName("required")]
    public bool Required { get; set; }
    
    [JsonPropertyName("maxImages")]
    public int? MaxImages { get; set; }
    
    [JsonPropertyName("acceptedImageTypes")]
    public List<string>? AcceptedImageTypes { get; set; }
    
    [JsonPropertyName("options")]
    public List<CreateExecutionFlowItemOptionDto>? Options { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // Se Type for Radio (2) ou Multiselect (3), Options é obrigatório
        if (Type == AnswerType.Radio || Type == AnswerType.Multiselect)
        {
            if (Options == null || Options.Count == 0)
            {
                results.Add(new ValidationResult(
                    "Options is required when Type is Radio or Multiselect",
                    new[] { nameof(Options) }));
            }
        }

        return results;
    }
}
