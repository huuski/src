using ApplicationLayer.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace InfrastructureLayer.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string? _smtpHost;
    private readonly int _smtpPort;
    private readonly string? _smtpUsername;
    private readonly string? _smtpPassword;
    private readonly bool _enableSsl;
    private readonly string? _fromEmail;
    private readonly string? _fromName;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
        // Configurações SMTP (podem ser definidas em appsettings.json)
        _smtpHost = _configuration["Email:Smtp:Host"];
        _smtpPort = int.Parse(_configuration["Email:Smtp:Port"] ?? "587");
        _smtpUsername = _configuration["Email:Smtp:Username"];
        _smtpPassword = _configuration["Email:Smtp:Password"];
        _enableSsl = bool.Parse(_configuration["Email:Smtp:EnableSsl"] ?? "true");
        _fromEmail = _configuration["Email:From:Address"];
        _fromName = _configuration["Email:From:Name"];
    }

    public async Task SendEmailAsync(string to, string subject, string body, byte[]? attachment = null, string? attachmentFileName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Se não houver configuração SMTP, apenas loga (modo desenvolvimento)
            if (string.IsNullOrWhiteSpace(_smtpHost) || string.IsNullOrWhiteSpace(_smtpUsername))
            {
                _logger.LogWarning(
                    "Email não enviado (SMTP não configurado). Para: {To}, Assunto: {Subject}, Anexo: {AttachmentFileName}",
                    to,
                    subject,
                    attachmentFileName ?? "Nenhum"
                );
                return;
            }

            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_fromEmail ?? _smtpUsername, _fromName ?? "Sistema");
            mailMessage.To.Add(to);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            // Adicionar anexo se fornecido
            if (attachment != null && !string.IsNullOrWhiteSpace(attachmentFileName))
            {
                using var stream = new MemoryStream(attachment);
                var attachmentItem = new Attachment(stream, attachmentFileName);
                mailMessage.Attachments.Add(attachmentItem);
            }

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
            smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
            smtpClient.EnableSsl = _enableSsl;

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInformation(
                "Email enviado com sucesso para: {To}, Assunto: {Subject}, Anexo: {AttachmentFileName}",
                to,
                subject,
                attachmentFileName ?? "Nenhum"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao enviar email para: {To}, Assunto: {Subject}",
                to,
                subject
            );
            throw;
        }
    }
}

