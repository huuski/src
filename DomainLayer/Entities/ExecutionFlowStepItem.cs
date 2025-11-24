using DomainLayer.Enums;

namespace DomainLayer.Entities;

public class ExecutionFlowStepItem : Entity
{
    public Guid ExecutionFlowStepId { get; private set; }
    public int Order { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Subtitle { get; private set; }
    public string? Description { get; private set; }
    public AnswerType AnswerType { get; private set; }
    public int? MaxLength { get; private set; }
    public bool Required { get; private set; }
    public int? MaxImages { get; private set; }
    public List<string>? AcceptedImageTypes { get; private set; }

    private ExecutionFlowStepItem()
    {
        // For ORM
    }

    public ExecutionFlowStepItem(
        Guid executionFlowStepId,
        int order,
        string title,
        AnswerType answerType,
        bool required,
        string? subtitle = null,
        string? description = null,
        int? maxLength = null,
        int? maxImages = null,
        List<string>? acceptedImageTypes = null)
        : base()
    {
        ExecutionFlowStepId = executionFlowStepId;
        Order = order;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Subtitle = subtitle;
        Description = description;
        AnswerType = answerType;
        Required = required;
        MaxLength = maxLength;
        MaxImages = maxImages;
        AcceptedImageTypes = acceptedImageTypes;

        Validate();
    }

    public void Update(
        int order,
        string title,
        AnswerType answerType,
        bool required,
        string? subtitle = null,
        string? description = null,
        int? maxLength = null,
        int? maxImages = null,
        List<string>? acceptedImageTypes = null)
    {
        Order = order;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Subtitle = subtitle;
        Description = description;
        AnswerType = answerType;
        Required = required;
        MaxLength = maxLength;
        MaxImages = maxImages;
        AcceptedImageTypes = acceptedImageTypes;

        Validate();
        MarkAsUpdated();
    }

    private void Validate()
    {
        if (ExecutionFlowStepId == Guid.Empty)
            throw new ArgumentException("ExecutionFlowStepId cannot be empty", nameof(ExecutionFlowStepId));

        if (Order < 0)
            throw new ArgumentException("Order must be greater than or equal to zero", nameof(Order));

        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title cannot be empty", nameof(Title));

        if (MaxLength.HasValue && MaxLength.Value <= 0)
            throw new ArgumentException("MaxLength must be greater than zero if provided", nameof(MaxLength));

        if (MaxImages.HasValue && MaxImages.Value <= 0)
            throw new ArgumentException("MaxImages must be greater than zero if provided", nameof(MaxImages));
    }
}

