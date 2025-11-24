using System.Text.Json;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;
using DomainLayer.ValueObjects;

namespace InfrastructureLayer.Data;

public class SeedDataService
{
    private readonly IPasswordHasher _passwordHasher;

    public SeedDataService(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    private string? FindDataDirectory()
    {
        var possiblePaths = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"),
            Path.Combine(Directory.GetCurrentDirectory(), "InfrastructureLayer", "Data"),
            Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "InfrastructureLayer", "Data")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "InfrastructureLayer", "Data"))
        };

        foreach (var path in possiblePaths)
        {
            if (Directory.Exists(path))
            {
                return path;
            }
        }

        return null;
    }

    private T? LoadJsonFile<T>(string fileName)
    {
        try
        {
            var dataDir = FindDataDirectory();
            if (dataDir == null)
                return default(T);

            var filePath = Path.Combine(dataDir, fileName);
            if (!File.Exists(filePath))
                return default(T);

            var jsonContent = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return default(T);
        }
    }

    public SeedData? LoadSeedData()
    {
        try
        {
            return new SeedData
            {
                Customers = LoadJsonFile<List<CustomerSeedData>>("seed-customers.json"),
                Services = LoadJsonFile<List<ServiceSeedData>>("seed-services.json"),
                Products = LoadJsonFile<List<ProductSeedData>>("seed-products.json"),
                PaymentMethods = LoadJsonFile<List<PaymentMethodSeedData>>("seed-payment-methods.json"),
                Notifications = LoadJsonFile<List<NotificationSeedData>>("seed-notifications.json"),
                Reminders = LoadJsonFile<List<ReminderSeedData>>("seed-reminders.json"),
                SpotlightCards = LoadJsonFile<List<SpotlightCardSeedData>>("seed-spotlight-cards.json"),
                Vouchers = LoadJsonFile<List<VoucherSeedData>>("seed-vouchers.json"),
                Appointments = LoadJsonFile<List<AppointmentSeedData>>("seed-appointments.json"),
                AppointmentServices = LoadJsonFile<List<AppointmentServiceSeedData>>("seed-appointment-services.json"),
                ExecutionFlows = LoadJsonFile<List<ExecutionFlowSeedData>>("seed-execution-flows.json"),
                ExecutionFlowSteps = LoadJsonFile<List<ExecutionFlowStepSeedData>>("seed-execution-flow-steps.json"),
                ExecutionFlowStepItems = LoadJsonFile<List<ExecutionFlowStepItemSeedData>>("seed-execution-flow-step-items.json"),
                ExecutionFlowItemOptions = LoadJsonFile<List<ExecutionFlowItemOptionSeedData>>("seed-execution-flow-item-options.json")
            };
        }
        catch
        {
            // If files don't exist or can't be loaded, return null (seed data is optional)
            return null;
        }
    }

    public List<Customer> GetCustomers()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<Customer>();
    }

    public List<Service> GetServices()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<Service>();
    }

    public List<Product> GetProducts()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<Product>();
    }

    public List<PaymentMethod> GetPaymentMethods()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<PaymentMethod>();
    }

    public List<Notification> GetNotifications()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<Notification>();
    }

    public List<Reminder> GetReminders()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<Reminder>();
    }

    public List<SpotlightCard> GetSpotlightCards()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<SpotlightCard>();
    }

    public List<Voucher> GetVouchers()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<Voucher>();
    }

    public List<Appointment> GetAppointments()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<Appointment>();
    }

    public List<AppointmentService> GetAppointmentServices()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<AppointmentService>();
    }

    public List<ExecutionFlow> GetExecutionFlows()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<ExecutionFlow>();
    }

    public List<ExecutionFlowStep> GetExecutionFlowSteps()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<ExecutionFlowStep>();
    }

    public List<ExecutionFlowStepItem> GetExecutionFlowStepItems()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<ExecutionFlowStepItem>();
    }

    public List<ExecutionFlowItemOption> GetExecutionFlowItemOptions()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<ExecutionFlowItemOption>();
    }

    public List<User> GetUsers()
    {
        // Data is now loaded directly from repositories (hardcoded in C#)
        return new List<User>();
    }
}

// DTOs for JSON deserialization
public class SeedData
{
    public List<CustomerSeedData>? Customers { get; set; }
    public List<ServiceSeedData>? Services { get; set; }
    public List<ProductSeedData>? Products { get; set; }
    public List<PaymentMethodSeedData>? PaymentMethods { get; set; }
    public List<NotificationSeedData>? Notifications { get; set; }
    public List<ReminderSeedData>? Reminders { get; set; }
    public List<SpotlightCardSeedData>? SpotlightCards { get; set; }
    public List<VoucherSeedData>? Vouchers { get; set; }
    public List<AppointmentSeedData>? Appointments { get; set; }
    public List<AppointmentServiceSeedData>? AppointmentServices { get; set; }
    public List<ExecutionFlowSeedData>? ExecutionFlows { get; set; }
    public List<ExecutionFlowStepSeedData>? ExecutionFlowSteps { get; set; }
    public List<ExecutionFlowStepItemSeedData>? ExecutionFlowStepItems { get; set; }
    public List<ExecutionFlowItemOptionSeedData>? ExecutionFlowItemOptions { get; set; }
}

public class CustomerSeedData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? Complement { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}

public class ServiceSeedData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Category { get; set; }
    public decimal Amount { get; set; }
    public string Duration { get; set; } = string.Empty;
    public string? Image { get; set; }
    public Guid? ExecutionFlowId { get; set; }
}

public class ProductSeedData
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Category { get; set; }
    public decimal Amount { get; set; }
    public string? Image { get; set; }
}

public class PaymentMethodSeedData
{
    public string Name { get; set; } = string.Empty;
    public int Type { get; set; }
    public bool Inactive { get; set; }
}

public class NotificationSeedData
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public DateTime? ReadAt { get; set; }
}

public class ReminderSeedData
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; }
    public DateTime? ReadAt { get; set; }
}

public class SpotlightCardSeedData
{
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string? ButtonTitle { get; set; }
    public string? ButtonLink { get; set; }
    public DateTime InitDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Inactive { get; set; }
}

public class VoucherSeedData
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public bool IsActive { get; set; }
    public decimal? MinimumPurchaseAmount { get; set; }
}

public class AppointmentSeedData
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public DateTime InitDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Status { get; set; }
}

public class AppointmentServiceSeedData
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid ServiceId { get; set; }
    public int SessionNumber { get; set; }
    public int SessionTotal { get; set; }
    public int Status { get; set; }
    public string? Notes { get; set; }
}

public class ExecutionFlowSeedData
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ExecutionFlowStepSeedData
{
    public Guid Id { get; set; }
    public Guid ExecutionFlowId { get; set; }
    public int StepNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DisplayStepNumber { get; set; }
    public string DisplayTitle { get; set; } = string.Empty;
}

public class ExecutionFlowStepItemSeedData
{
    public Guid Id { get; set; }
    public Guid ExecutionFlowStepId { get; set; }
    public string Question { get; set; } = string.Empty;
    public int AnswerType { get; set; }
    public int? MaxLength { get; set; }
    public bool Required { get; set; }
    public bool DisplayExtraNotes { get; set; }
    public string? ExtraNotes { get; set; }
    public int? ExtraNotesLength { get; set; }
}

public class ExecutionFlowItemOptionSeedData
{
    public Guid Id { get; set; }
    public Guid ExecutionFlowStepItemId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class UserSeedData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? Complement { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Avatar { get; set; }
}

