using DomainLayer.Enums;

namespace DomainLayer.Entities;

public class AppointmentService : Entity
{
    public Guid AppointmentId { get; private set; }
    public Guid ServiceId { get; private set; }
    public int SessionNumber { get; private set; }
    public int SessionTotal { get; private set; }
    public AppointmentServiceStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public string? CancelationReason { get; private set; }

    private AppointmentService()
    {
        // For ORM
    }

    public AppointmentService(
        Guid appointmentId,
        Guid serviceId,
        int sessionNumber,
        int sessionTotal,
        AppointmentServiceStatus status = AppointmentServiceStatus.Pending,
        string? notes = null)
        : base()
    {
        AppointmentId = appointmentId;
        ServiceId = serviceId;
        SessionNumber = sessionNumber;
        SessionTotal = sessionTotal;
        Status = status;
        Notes = notes;

        Validate();
    }

    public void Update(
        int sessionNumber,
        int sessionTotal,
        AppointmentServiceStatus? status = null,
        string? notes = null)
    {
        SessionNumber = sessionNumber;
        SessionTotal = sessionTotal;
        if (status.HasValue)
        {
            Status = status.Value;
        }
        Notes = notes;

        Validate();
        MarkAsUpdated();
    }

    public void UpdateStatus(AppointmentServiceStatus status, string? cancelationReason = null)
    {
        Status = status;
        if (status == AppointmentServiceStatus.Canceled)
        {
            CancelationReason = cancelationReason;
        }
        else
        {
            CancelationReason = null;
        }
        MarkAsUpdated();
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        MarkAsUpdated();
    }

    private void Validate()
    {
        if (AppointmentId == Guid.Empty)
            throw new ArgumentException("AppointmentId cannot be empty", nameof(AppointmentId));

        if (ServiceId == Guid.Empty)
            throw new ArgumentException("ServiceId cannot be empty", nameof(ServiceId));

        if (SessionNumber <= 0)
            throw new ArgumentException("SessionNumber must be greater than zero", nameof(SessionNumber));

        if (SessionTotal <= 0)
            throw new ArgumentException("SessionTotal must be greater than zero", nameof(SessionTotal));

        if (SessionNumber > SessionTotal)
            throw new ArgumentException("SessionNumber cannot be greater than SessionTotal", nameof(SessionNumber));
    }
}

