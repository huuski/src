using ApplicationLayer.DTOs.Negotiation;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NegotiationController : ControllerBase
{
    private readonly INegotiationService _negotiationService;
    private readonly IPdfService _pdfService;
    private readonly IEmailService _emailService;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<NegotiationController> _logger;

    public NegotiationController(
        INegotiationService negotiationService,
        IPdfService pdfService,
        IEmailService emailService,
        ICustomerRepository customerRepository,
        ILogger<NegotiationController> logger)
    {
        _negotiationService = negotiationService ?? throw new ArgumentNullException(nameof(negotiationService));
        _pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NegotiationCompleteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _negotiationService.GetByIdCompleteAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Negotiation not found: {NegotiationId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting negotiation: {NegotiationId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(NegotiationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _negotiationService.GetByCodeAsync(code, cancellationToken);
            if (result == null)
                return NotFound(new { message = $"Negotiation with code {code} not found" });

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid code: {Code}", code);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting negotiation by code: {Code}", code);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NegotiationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _negotiationService.GetAllAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all negotiations");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet("customer/{customerId}")]
    [ProducesResponseType(typeof(IEnumerable<NegotiationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomerId(Guid customerId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _negotiationService.GetByCustomerIdAsync(customerId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting negotiations by customer: {CustomerId}", customerId);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Get all negotiations by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<NegotiationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _negotiationService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting negotiations by user: {UserId}", userId);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(NegotiationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateNegotiationDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Log the received UserId for debugging
            _logger.LogInformation("Creating negotiation with UserId: {UserId}", dto.UserId);

            var result = await _negotiationService.CreateAsync(dto, cancellationToken);
            
            // Log the created UserId for debugging
            _logger.LogInformation("Created negotiation with UserId: {UserId}", result.UserId);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid negotiation creation request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating negotiation");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(NegotiationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNegotiationDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _negotiationService.UpdateAsync(id, dto, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Negotiation not found or invalid update request: {NegotiationId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating negotiation: {NegotiationId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _negotiationService.DeleteAsync(id, cancellationToken);
            if (result)
                return NoContent();

            return NotFound(new { message = $"Negotiation with id {id} not found" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Negotiation not found: {NegotiationId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting negotiation: {NegotiationId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Generate PDF for a negotiation
    /// </summary>
    [HttpGet("{id}/pdf")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GeneratePdf(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            // Verify negotiation exists
            var negotiation = await _negotiationService.GetByIdCompleteAsync(id, cancellationToken);

            // Generate PDF
            var pdfBytes = await _pdfService.GenerateNegotiationPdfAsync(id, cancellationToken);

            return File(pdfBytes, "application/pdf", $"Negotiation_{negotiation.Code}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Negotiation not found for PDF generation: {NegotiationId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating PDF for negotiation: {NegotiationId}", id);
            return StatusCode(500, new { message = "An internal server error occurred while generating PDF" });
        }
    }

    /// <summary>
    /// Send PDF by email for a negotiation
    /// </summary>
    [HttpPost("{id}/send-pdf-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendPdfByEmail(Guid id, [FromBody] SendPdfEmailDto? dto = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify negotiation exists
            var negotiation = await _negotiationService.GetByIdCompleteAsync(id, cancellationToken);
            if (negotiation == null)
                return NotFound(new { message = $"Negotiation with id {id} not found" });

            // Get customer email
            string emailTo;
            if (!string.IsNullOrWhiteSpace(dto?.Email))
            {
                emailTo = dto.Email;
            }
            else
            {
                var customer = await _customerRepository.GetByIdAsync(negotiation.CustomerId, cancellationToken);
                if (customer == null)
                    return NotFound(new { message = $"Customer with id {negotiation.CustomerId} not found" });

                emailTo = customer.Email.Value;
            }

            // Generate PDF
            var pdfBytes = await _pdfService.GenerateNegotiationPdfAsync(id, cancellationToken);
            var pdfFileName = $"Negotiation_{negotiation.Code}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

            // Prepare email content
            var subject = $"Proposta Comercial - {negotiation.Code}";
            var body = $@"
                <html>
                <body>
                    <h2>Proposta Comercial</h2>
                    <p>Prezado(a) cliente,</p>
                    <p>Segue em anexo a proposta comercial referente ao código <strong>{negotiation.Code}</strong>.</p>
                    <p>Valor total: R$ {negotiation.NetValue:F2}</p>
                    <p>Data de expiração: {negotiation.ExpirationDate:dd/MM/yyyy}</p>
                    <p>Atenciosamente,<br/>Equipe</p>
                </body>
                </html>";

            // Send email
            await _emailService.SendEmailAsync(emailTo, subject, body, pdfBytes, pdfFileName, cancellationToken);

            return Ok(new { message = "PDF enviado por email com sucesso", email = emailTo });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Negotiation not found for sending PDF by email: {NegotiationId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending PDF by email for negotiation: {NegotiationId}", id);
            return StatusCode(500, new { message = "An internal server error occurred while sending PDF by email" });
        }
    }
}

public class SendPdfEmailDto
{
    public string? Email { get; set; }
}

