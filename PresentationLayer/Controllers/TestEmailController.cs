using ApplicationLayer.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // Allow anonymous access for testing purposes
public class TestEmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<TestEmailController> _logger;

    public TestEmailController(
        IEmailService emailService,
        ILogger<TestEmailController> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Test email sending without attachment
    /// </summary>
    [HttpPost("send-test-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendTestEmail([FromBody] TestEmailDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.To))
                return BadRequest(new { message = "Email 'to' is required" });

            var subject = dto.Subject ?? "Teste de Email - Sistema";
            var body = dto.Body ?? @"
                <html>
                <body>
                    <h2>Email de Teste</h2>
                    <p>Este é um email de teste enviado pelo sistema.</p>
                    <p>Se você recebeu este email, a configuração está funcionando corretamente!</p>
                    <p>Data/Hora: " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss") + @"</p>
                </body>
                </html>";

            await _emailService.SendEmailAsync(dto.To, subject, body, null, null, cancellationToken);

            return Ok(new 
            { 
                message = "Email de teste enviado com sucesso",
                to = dto.To,
                subject = subject
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email de teste para: {Email}", dto.To);
            return StatusCode(500, new { message = "Erro ao enviar email", error = ex.Message });
        }
    }

    /// <summary>
    /// Test email sending with PDF attachment
    /// </summary>
    [HttpPost("send-test-email-with-pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendTestEmailWithPdf([FromBody] TestEmailDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.To))
                return BadRequest(new { message = "Email 'to' is required" });

            var subject = dto.Subject ?? "Teste de Email com PDF - Sistema";
            var body = dto.Body ?? @"
                <html>
                <body>
                    <h2>Email de Teste com Anexo PDF</h2>
                    <p>Este é um email de teste com um PDF anexado.</p>
                    <p>Se você recebeu este email com o PDF, a configuração está funcionando corretamente!</p>
                    <p>Data/Hora: " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss") + @"</p>
                </body>
                </html>";

            // Criar um PDF de teste simples
            var pdfBytes = CreateTestPdf();

            await _emailService.SendEmailAsync(
                dto.To, 
                subject, 
                body, 
                pdfBytes, 
                $"teste_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf", 
                cancellationToken
            );

            return Ok(new 
            { 
                message = "Email de teste com PDF enviado com sucesso",
                to = dto.To,
                subject = subject,
                attachmentSize = pdfBytes.Length
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email de teste com PDF para: {Email}", dto.To);
            return StatusCode(500, new { message = "Erro ao enviar email", error = ex.Message });
        }
    }

    private byte[] CreateTestPdf()
    {
        // Criar um PDF simples de teste
        // Usando uma estrutura PDF básica válida
        var timestamp = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss");
        var pdfContent = $@"%PDF-1.4
1 0 obj
<< /Type /Catalog /Pages 2 0 R >>
endobj
2 0 obj
<< /Type /Pages /Kids [3 0 R] /Count 1 >>
endobj
3 0 obj
<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> >> >> >>
endobj
4 0 obj
<< /Length 200 >>
stream
BT
/F1 16 Tf
100 700 Td
(Documento de Teste) Tj
0 -20 Td
/F1 12 Tf
(Data: {timestamp}) Tj
0 -20 Td
(Se voce consegue ver este PDF, o sistema esta funcionando!) Tj
ET
endstream
endobj
xref
0 5
0000000000 65535 f 
0000000009 00000 n 
0000000058 00000 n 
0000000115 00000 n 
0000000300 00000 n 
trailer
<< /Size 5 /Root 1 0 R >>
startxref
400
%%EOF";

        return System.Text.Encoding.UTF8.GetBytes(pdfContent);
    }
}

public class TestEmailDto
{
    public string To { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string? Body { get; set; }
}

