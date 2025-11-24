namespace DomainLayer.Entities;

public class ExecutionFlowStep : Entity
{
    public Guid ExecutionFlowId { get; private set; }
    public int StepNumber { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Subtitle { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int DisplayStepNumber { get; private set; }
    public string DisplayTitle { get; private set; } = string.Empty;

    private ExecutionFlowStep()
    {
        // For ORM
    }

    public ExecutionFlowStep(
        Guid executionFlowId,
        int stepNumber,
        string title,
        string description,
        int displayStepNumber,
        string displayTitle,
        string? subtitle = null)
        : base()
    {
        ExecutionFlowId = executionFlowId;
        StepNumber = stepNumber;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Subtitle = subtitle;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        DisplayStepNumber = displayStepNumber;
        DisplayTitle = displayTitle ?? throw new ArgumentNullException(nameof(displayTitle));

        Validate();
    }

    public void Update(
        int stepNumber,
        string title,
        string description,
        int displayStepNumber,
        string displayTitle,
        string? subtitle = null)
    {
        StepNumber = stepNumber;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Subtitle = subtitle;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        DisplayStepNumber = displayStepNumber;
        DisplayTitle = displayTitle ?? throw new ArgumentNullException(nameof(displayTitle));

        Validate();
        MarkAsUpdated();
    }

    private void Validate()
    {
        if (ExecutionFlowId == Guid.Empty)
            throw new ArgumentException("ExecutionFlowId cannot be empty", nameof(ExecutionFlowId));

        if (StepNumber <= 0)
            throw new ArgumentException("StepNumber must be greater than zero", nameof(StepNumber));

        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title cannot be empty", nameof(Title));

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description cannot be empty", nameof(Description));

        if (DisplayStepNumber <= 0)
            throw new ArgumentException("DisplayStepNumber must be greater than zero", nameof(DisplayStepNumber));

        if (string.IsNullOrWhiteSpace(DisplayTitle))
            throw new ArgumentException("DisplayTitle cannot be empty", nameof(DisplayTitle));
    }
}

