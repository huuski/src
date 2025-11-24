using DomainLayer.Enums;

namespace DomainLayer.Entities;

public class Service : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ServiceCategory Category { get; private set; }
    public decimal Amount { get; private set; }
    public string? Image { get; private set; }
    public TimeSpan Duration { get; private set; }
    public Guid? ExecutionFlowId { get; private set; }

    private Service()
    {
        // For ORM
    }

    public Service(
        string name,
        string description,
        ServiceCategory category,
        decimal amount,
        TimeSpan duration,
        string? image = null,
        Guid? executionFlowId = null)
        : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category;
        Amount = amount;
        Duration = duration;
        Image = image;
        ExecutionFlowId = executionFlowId;

        Validate();
    }

    public void Update(
        string name,
        string description,
        ServiceCategory category,
        decimal amount,
        TimeSpan duration,
        string? image = null,
        Guid? executionFlowId = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category;
        Amount = amount;
        Duration = duration;
        Image = image;
        ExecutionFlowId = executionFlowId;

        Validate();
        MarkAsUpdated();
    }

    public void UpdateImage(string? image)
    {
        Image = image;
        MarkAsUpdated();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name cannot be empty", nameof(Name));

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description cannot be empty", nameof(Description));

        if (Amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(Amount));

        if (Duration <= TimeSpan.Zero)
            throw new ArgumentException("Duration must be greater than zero", nameof(Duration));
    }
}

