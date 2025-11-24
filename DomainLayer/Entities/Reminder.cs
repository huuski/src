using DomainLayer.Enums;

namespace DomainLayer.Entities;

public class Reminder : Entity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Priority Priority { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private Reminder()
    {
        // For ORM
    }

    public Reminder(
        string title,
        string description,
        Priority priority)
        : base()
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Priority = priority;

        Validate();
    }

    public void Update(
        string title,
        string description,
        Priority priority)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Priority = priority;

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

