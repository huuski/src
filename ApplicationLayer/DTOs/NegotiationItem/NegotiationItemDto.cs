using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.NegotiationItem;

public class NegotiationItemDto
{
    public Guid Id { get; set; }
    public Guid NegotiationId { get; set; }
    public NegotiationItemType Type { get; set; }
    public int Quantity { get; set; }
    public decimal GrossValueUnit { get; set; }
    public decimal GrossValue { get; set; }
    public decimal NetValue { get; set; }
    public decimal DiscountValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

