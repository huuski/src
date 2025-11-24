using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.Negotiation;

public class NegotiationItemInputDto
{
    public NegotiationItemType Type { get; set; }
    public int Quantity { get; set; }
    public decimal GrossValueUnit { get; set; }
    public decimal NetValue { get; set; }
    public decimal DiscountValue { get; set; }
}

