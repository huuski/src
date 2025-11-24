namespace DomainLayer.Entities;

public class Notification : Entity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Icon { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private Notification()
    {
        // For ORM
    }

    public Notification(
        string title,
        string description,
        string? icon = null)
        : base()
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Icon = icon;

        Validate();
    }

    public void Update(
        string title,
        string description,
        string? icon = null)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Icon = icon;

        Validate();
        MarkAsUpdated();
    }

    public void MarkAsRead()
    {
        if (ReadAt == null)
        {
            ReadAt = DateTime.UtcNow;
            MarkAsUpdated();
        }
    }

    public void MarkAsUnread()
    {
        if (ReadAt != null)
        {
            ReadAt = null;
            MarkAsUpdated();
        }
    }

    public bool IsRead => ReadAt.HasValue;

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title cannot be empty", nameof(Title));

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description cannot be empty", nameof(Description));
    }
}

