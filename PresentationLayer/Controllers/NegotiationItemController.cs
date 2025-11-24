using ApplicationLayer.DTOs.NegotiationItem;
using ApplicationLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NegotiationItemController : ControllerBase
{
    private readonly INegotiationItemService _negotiationItemService;
    private readonly ILogger<NegotiationItemController> _logger;

    public NegotiationItemController(INegotiationItemService negotiationItemService, ILogger<NegotiationItemController> logger)
    {
        _negotiationItemService = negotiationItemService ?? throw new ArgumentNullException(nameof(negotiationItemService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NegotiationItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _negotiationItemService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "NegotiationItem not found: {NegotiationItemId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting negotiation item: {NegotiationItemId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NegotiationItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _negotiationItemService.GetAllAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all negotiation items");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet("negotiation/{negotiationId}")]
    [ProducesResponseType(typeof(IEnumerable<NegotiationItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByNegotiationId(Guid negotiationId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _negotiationItemService.GetByNegotiationIdAsync(negotiationId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting negotiation items by negotiation: {NegotiationId}", negotiationId);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(NegotiationItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateNegotiationItemDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _negotiationItemService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid negotiation item creation request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating negotiation item");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(NegotiationItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNegotiationItemDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _negotiationItemService.UpdateAsync(id, dto, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "NegotiationItem not found or invalid update request: {NegotiationItemId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating negotiation item: {NegotiationItemId}", id);
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
            var result = await _negotiationItemService.DeleteAsync(id, cancellationToken);
            if (result)
                return NoContent();

            return NotFound(new { message = $"NegotiationItem with id {id} not found" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "NegotiationItem not found: {NegotiationItemId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting negotiation item: {NegotiationItemId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }
}

