namespace DomainLayer.Entities;

public class Voucher : Entity
{
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal DiscountAmount { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidUntil { get; private set; }
    public bool IsActive { get; private set; }
    public decimal? MinimumPurchaseAmount { get; private set; }

    private Voucher()
    {
        // For ORM
    }

    public Voucher(
        string code,
        string description,
        decimal discountAmount,
        DateTime validFrom,
        DateTime validUntil,
        bool isActive = true,
        decimal? minimumPurchaseAmount = null)
        : base()
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        DiscountAmount = discountAmount;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        IsActive = isActive;
        MinimumPurchaseAmount = minimumPurchaseAmount;

        Validate();
    }

    public void Update(
        string code,
        string description,
        decimal discountAmount,
        DateTime validFrom,
        DateTime validUntil,
        bool isActive,
        decimal? minimumPurchaseAmount = null)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        DiscountAmount = discountAmount;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        IsActive = isActive;
        MinimumPurchaseAmount = minimumPurchaseAmount;

        Validate();
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public bool IsValid(DateTime? date = null)
    {
        var checkDate = date ?? DateTime.UtcNow;
        return IsActive && checkDate >= ValidFrom && checkDate <= ValidUntil;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Code))
            throw new ArgumentException("Code cannot be empty", nameof(Code));

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description cannot be empty", nameof(Description));

        if (DiscountAmount < 0)
            throw new ArgumentException("Discount amount cannot be negative", nameof(DiscountAmount));

        if (ValidUntil <= ValidFrom)
            throw new ArgumentException("ValidUntil must be after ValidFrom", nameof(ValidUntil));

        if (MinimumPurchaseAmount.HasValue && MinimumPurchaseAmount.Value < 0)
            throw new ArgumentException("Minimum purchase amount cannot be negative", nameof(MinimumPurchaseAmount));
    }
}

