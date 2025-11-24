using ApplicationLayer.DTOs.NegotiationPaymentMethod;
using ApplicationLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NegotiationPaymentMethodController : ControllerBase
{
    private readonly INegotiationPaymentMethodService _negotiationPaymentMethodService;
    private readonly ILogger<NegotiationPaymentMethodController> _logger;

    public NegotiationPaymentMethodController(INegotiationPaymentMethodService negotiationPaymentMethodService, ILogger<NegotiationPaymentMethodController> logger)
    {
        _negotiationPaymentMethodService = negotiationPaymentMethodService ?? throw new ArgumentNullException(nameof(negotiationPaymentMethodService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NegotiationPaymentMethodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _negotiationPaymentMethodService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "NegotiationPaymentMethod not found: {NegotiationPaymentMethodId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting negotiation payment method: {NegotiationPaymentMethodId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NegotiationPaymentMethodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _negotiationPaymentMethodService.GetAllAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all negotiation payment methods");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet("negotiation/{negotiationId}")]
    [ProducesResponseType(typeof(IEnumerable<NegotiationPaymentMethodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByNegotiationId(Guid negotiationId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _negotiationPaymentMethodService.GetByNegotiationIdAsync(negotiationId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting negotiation payment methods by negotiation: {NegotiationId}", negotiationId);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(NegotiationPaymentMethodDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateNegotiationPaymentMethodDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _negotiationPaymentMethodService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid negotiation payment method creation request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating negotiation payment method");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(NegotiationPaymentMethodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNegotiationPaymentMethodDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _negotiationPaymentMethodService.UpdateAsync(id, dto, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "NegotiationPaymentMethod not found or invalid update request: {NegotiationPaymentMethodId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating negotiation payment method: {NegotiationPaymentMethodId}", id);
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
            var result = await _negotiationPaymentMethodService.DeleteAsync(id, cancellationToken);
            if (result)
                return NoContent();

            return NotFound(new { message = $"NegotiationPaymentMethod with id {id} not found" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "NegotiationPaymentMethod not found: {NegotiationPaymentMethodId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting negotiation payment method: {NegotiationPaymentMethodId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }
}

