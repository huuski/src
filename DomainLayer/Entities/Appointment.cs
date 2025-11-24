using DomainLayer.Enums;

namespace DomainLayer.Entities;

public class Appointment : Entity
{
    public Guid CustomerId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime InitDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public TimeSpan Duration => EndDate - InitDate;
    public AppointmentStatus Status { get; private set; }

    private Appointment()
    {
        // For ORM
    }

    public Appointment(
        Guid customerId,
        Guid userId,
        DateTime initDate,
        DateTime endDate,
        AppointmentStatus status = AppointmentStatus.Scheduled)
        : base()
    {
        CustomerId = customerId;
        UserId = userId;
        InitDate = initDate;
        EndDate = endDate;
        Status = status;

        Validate();
    }

    public void Update(
        DateTime initDate,
        DateTime endDate,
        AppointmentStatus? status = null)
    {
        InitDate = initDate;
        EndDate = endDate;
        if (status.HasValue)
        {
            Status = status.Value;
        }

        Validate();
        MarkAsUpdated();
    }

    public void UpdateStatus(AppointmentStatus status)
    {
        Status = status;
        MarkAsUpdated();
    }

    private void Validate()
    {
        if (CustomerId == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(CustomerId));

        if (UserId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(UserId));

        if (EndDate <= InitDate)
            throw new ArgumentException("EndDate must be after InitDate", nameof(EndDate));

        if (InitDate < DateTime.UtcNow.AddHours(-1))
            throw new ArgumentException("InitDate cannot be more than 1 hour in the past", nameof(InitDate));
    }
}

