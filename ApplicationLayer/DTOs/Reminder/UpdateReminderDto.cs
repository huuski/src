using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.Reminder;

public class UpdateReminderDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; }
}

