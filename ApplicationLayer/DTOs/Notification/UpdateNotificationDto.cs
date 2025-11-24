namespace ApplicationLayer.DTOs.Notification;

public class UpdateNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Icon { get; set; }
}

