namespace DomainLayer.Entities;

public class ExecutionFlowItemOption : Entity
{
    public Guid ExecutionFlowStepItemId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public bool EnableExtraAnswer { get; private set; }
    public int? ExtraAnswerMaxLength { get; private set; }

    private ExecutionFlowItemOption()
    {
        // For ORM
    }

    public ExecutionFlowItemOption(
        Guid executionFlowStepItemId,
        string title,
        string value,
        int order,
        bool enableExtraAnswer = false,
        int? extraAnswerMaxLength = null)
        : base()
    {
        ExecutionFlowStepItemId = executionFlowStepItemId;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Order = order;
        EnableExtraAnswer = enableExtraAnswer;
        ExtraAnswerMaxLength = extraAnswerMaxLength;

        Validate();
    }

    public void Update(
        string title,
        string value,
        int order,
        bool enableExtraAnswer = false,
        int? extraAnswerMaxLength = null)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Order = order;
        EnableExtraAnswer = enableExtraAnswer;
        ExtraAnswerMaxLength = extraAnswerMaxLength;

        Validate();
        MarkAsUpdated();
    }

    private void Validate()
    {
        if (ExecutionFlowStepItemId == Guid.Empty)
            throw new ArgumentException("ExecutionFlowStepItemId cannot be empty", nameof(ExecutionFlowStepItemId));

        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title cannot be empty", nameof(Title));

        if (string.IsNullOrWhiteSpace(Value))
            throw new ArgumentException("Value cannot be empty", nameof(Value));

        if (Order < 0)
            throw new ArgumentException("Order must be greater than or equal to zero", nameof(Order));

        if (ExtraAnswerMaxLength.HasValue && ExtraAnswerMaxLength.Value <= 0)
            throw new ArgumentException("ExtraAnswerMaxLength must be greater than zero if provided", nameof(ExtraAnswerMaxLength));
    }
}

