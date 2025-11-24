using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.PaymentMethod;

public class PaymentMethodDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PaymentMethodType Type { get; set; }
    public bool Inactive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

