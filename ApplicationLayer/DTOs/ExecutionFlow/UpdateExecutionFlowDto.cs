using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApplicationLayer.DTOs.ExecutionFlow;

public class UpdateExecutionFlowDto
{
    [Required]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [JsonPropertyName("flow")]
    public string Flow { get; set; } = string.Empty;
}

