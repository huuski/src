using System.ComponentModel.DataAnnotations;
using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.AppointmentService;

public class UpdateAppointmentServiceStatusDto : IValidatableObject
{
    [Required]
    public Guid AppointmentServiceId { get; set; }

    [Required]
    public AppointmentServiceStatus AppointmentServiceStatus { get; set; }

    public string? AppointmentServiceCancelationReason { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (AppointmentServiceStatus == AppointmentServiceStatus.Canceled && 
            string.IsNullOrWhiteSpace(AppointmentServiceCancelationReason))
        {
            results.Add(new ValidationResult(
                "AppointmentServiceCancelationReason is required when status is Canceled",
                new[] { nameof(AppointmentServiceCancelationReason) }
            ));
        }

        return results;
    }
}

