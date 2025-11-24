using ApplicationLayer.DTOs.NegotiationItem;
using ApplicationLayer.DTOs.NegotiationPaymentMethod;

namespace ApplicationLayer.DTOs.Negotiation;

public class NegotiationCompleteDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public decimal GrossValue { get; set; }
    public decimal NetValue { get; set; }
    public decimal DiscountValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<NegotiationItemDto> Items { get; set; } = new();
    public List<NegotiationPaymentMethodDto> PaymentMethods { get; set; } = new();
}

