using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.NegotiationItem;

public class UpdateNegotiationItemDto
{
    public NegotiationItemType Type { get; set; }
    public int Quantity { get; set; }
    public decimal GrossValueUnit { get; set; }
    public decimal NetValue { get; set; }
    public decimal DiscountValue { get; set; }
}

