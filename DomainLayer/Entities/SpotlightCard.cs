namespace DomainLayer.Entities;

public class SpotlightCard : Entity
{
    public string Title { get; private set; } = string.Empty;
    public string ShortDescription { get; private set; } = string.Empty;
    public string LongDescription { get; private set; } = string.Empty;
    public string? Image { get; private set; }
    public string? ButtonTitle { get; private set; }
    public string? ButtonLink { get; private set; }
    public DateTime InitDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool Inactive { get; private set; }

    private SpotlightCard()
    {
        // For ORM
    }

    public SpotlightCard(
        string title,
        string shortDescription,
        string longDescription,
        DateTime initDate,
        DateTime endDate,
        string? image = null,
        string? buttonTitle = null,
        string? buttonLink = null)
        : base()
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        ShortDescription = shortDescription ?? throw new ArgumentNullException(nameof(shortDescription));
        LongDescription = longDescription ?? throw new ArgumentNullException(nameof(longDescription));
        InitDate = initDate;
        EndDate = endDate;
        Image = image;
        ButtonTitle = buttonTitle;
        ButtonLink = buttonLink;

        Validate();
    }

    public void Update(
        string title,
        string shortDescription,
        string longDescription,
        DateTime initDate,
        DateTime endDate,
        string? image = null,
        string? buttonTitle = null,
        string? buttonLink = null)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        ShortDescription = shortDescription ?? throw new ArgumentNullException(nameof(shortDescription));
        LongDescription = longDescription ?? throw new ArgumentNullException(nameof(longDescription));
        InitDate = initDate;
        EndDate = endDate;
        Image = image;
        ButtonTitle = buttonTitle;
        ButtonLink = buttonLink;

        Validate();
        MarkAsUpdated();
    }

    public void Activate()
    {
        if (Inactive)
        {
            Inactive = false;
            MarkAsUpdated();
        }
    }

    public void Deactivate()
    {
        if (!Inactive)
        {
            Inactive = true;
            MarkAsUpdated();
        }
    }

    public bool IsActive => !Inactive && DateTime.UtcNow >= InitDate && DateTime.UtcNow <= EndDate;

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title cannot be empty", nameof(Title));

        if (string.IsNullOrWhiteSpace(ShortDescription))
            throw new ArgumentException("ShortDescription cannot be empty", nameof(ShortDescription));

        if (string.IsNullOrWhiteSpace(LongDescription))
            throw new ArgumentException("LongDescription cannot be empty", nameof(LongDescription));

        if (EndDate < InitDate)
            throw new ArgumentException("EndDate cannot be before InitDate", nameof(EndDate));
    }
}

