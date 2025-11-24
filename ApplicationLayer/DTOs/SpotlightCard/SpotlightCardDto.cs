namespace ApplicationLayer.DTOs.SpotlightCard;

public class SpotlightCardDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string? ButtonTitle { get; set; }
    public string? ButtonLink { get; set; }
    public DateTime InitDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Inactive { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

