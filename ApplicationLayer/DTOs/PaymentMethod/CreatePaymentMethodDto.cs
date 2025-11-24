using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.PaymentMethod;

public class CreatePaymentMethodDto
{
    public string Name { get; set; } = string.Empty;
    public PaymentMethodType Type { get; set; }
}

