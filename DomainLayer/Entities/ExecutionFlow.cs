namespace DomainLayer.Entities;

public class ExecutionFlow : Entity
{
    public string Title { get; private set; } = string.Empty;
    public string Flow { get; private set; } = string.Empty;

    private ExecutionFlow()
    {
        // For ORM
    }

    public ExecutionFlow(
        string title,
        string flow)
        : base()
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Flow = flow ?? throw new ArgumentNullException(nameof(flow));

        Validate();
    }

    public void Update(
        string title,
        string flow)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Flow = flow ?? throw new ArgumentNullException(nameof(flow));

        Validate();
        MarkAsUpdated();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title cannot be empty", nameof(Title));

        if (string.IsNullOrWhiteSpace(Flow))
            throw new ArgumentException("Flow cannot be empty", nameof(Flow));
    }
}

