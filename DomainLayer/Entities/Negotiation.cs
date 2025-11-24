namespace DomainLayer.Entities;

public class Negotiation : Entity
{
    public Guid CustomerId { get; private set; }
    public Guid UserId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public DateTime ExpirationDate { get; private set; }
    public decimal GrossValue { get; private set; }
    public decimal NetValue { get; private set; }
    public decimal DiscountValue { get; private set; }

    private Negotiation()
    {
        // For ORM
    }

    public Negotiation(
        Guid customerId,
        Guid userId,
        DateTime expirationDate,
        decimal grossValue,
        decimal netValue,
        decimal discountValue)
        : base()
    {
        CustomerId = customerId;
        UserId = userId;
        ExpirationDate = expirationDate;
        GrossValue = grossValue;
        NetValue = netValue;
        DiscountValue = discountValue;
        Code = GenerateCode();

        Validate();
    }

    public void Update(
        DateTime expirationDate,
        decimal grossValue,
        decimal netValue,
        decimal discountValue)
    {
        ExpirationDate = expirationDate;
        GrossValue = grossValue;
        NetValue = netValue;
        DiscountValue = discountValue;

        Validate();
        MarkAsUpdated();
    }

    private static string GenerateCode()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 10)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private void Validate()
    {
        if (CustomerId == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(CustomerId));

        if (UserId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(UserId));

        if (ExpirationDate < DateTime.UtcNow.Date)
            throw new ArgumentException("ExpirationDate cannot be in the past", nameof(ExpirationDate));

        if (GrossValue < 0)
            throw new ArgumentException("GrossValue cannot be negative", nameof(GrossValue));

        if (NetValue < 0)
            throw new ArgumentException("NetValue cannot be negative", nameof(NetValue));

        if (DiscountValue < 0)
            throw new ArgumentException("DiscountValue cannot be negative", nameof(DiscountValue));

        if (NetValue > GrossValue)
            throw new ArgumentException("NetValue cannot be greater than GrossValue", nameof(NetValue));
    }
}

