using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.PaymentMethod;

public class UpdatePaymentMethodDto
{
    public string Name { get; set; } = string.Empty;
    public PaymentMethodType Type { get; set; }
}

