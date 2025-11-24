namespace ApplicationLayer.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, byte[]? attachment = null, string? attachmentFileName = null, CancellationToken cancellationToken = default);
}

