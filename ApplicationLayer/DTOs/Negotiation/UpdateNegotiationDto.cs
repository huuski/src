namespace ApplicationLayer.DTOs.Negotiation;

public class UpdateNegotiationDto
{
    public DateTime ExpirationDate { get; set; }
    public decimal GrossValue { get; set; }
    public decimal NetValue { get; set; }
    public decimal DiscountValue { get; set; }
}

