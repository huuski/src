namespace ApplicationLayer.DTOs.NegotiationPaymentMethod;

public class CreateNegotiationPaymentMethodDto
{
    public Guid NegotiationId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public decimal Value { get; set; }
}

