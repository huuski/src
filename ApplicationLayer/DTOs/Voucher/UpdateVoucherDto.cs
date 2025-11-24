namespace ApplicationLayer.DTOs.Voucher;

public class UpdateVoucherDto
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public bool IsActive { get; set; }
    public decimal? MinimumPurchaseAmount { get; set; }
}

