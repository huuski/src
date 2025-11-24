using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApplicationLayer.DTOs.Negotiation;

public class CreateNegotiationDto
{
    [Required]
    [JsonPropertyName("customerId")]
    public Guid CustomerId { get; set; }
    
    [Required]
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }
    
    [Required]
    [JsonPropertyName("expirationDate")]
    public DateTime ExpirationDate { get; set; }
    
    [JsonPropertyName("grossValue")]
    public decimal GrossValue { get; set; }
    
    [JsonPropertyName("netValue")]
    public decimal NetValue { get; set; }
    
    [JsonPropertyName("discountValue")]
    public decimal DiscountValue { get; set; }
    
    [JsonPropertyName("items")]
    public List<NegotiationItemInputDto> Items { get; set; } = new();
}

