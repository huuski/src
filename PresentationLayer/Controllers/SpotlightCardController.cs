using ApplicationLayer.DTOs.SpotlightCard;
using ApplicationLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpotlightCardController : ControllerBase
{
    private readonly ISpotlightCardService _spotlightCardService;
    private readonly ILogger<SpotlightCardController> _logger;

    public SpotlightCardController(ISpotlightCardService spotlightCardService, ILogger<SpotlightCardController> logger)
    {
        _spotlightCardService = spotlightCardService ?? throw new ArgumentNullException(nameof(spotlightCardService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get a spotlight card by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SpotlightCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _spotlightCardService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "SpotlightCard not found: {SpotlightCardId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting spotlight card: {SpotlightCardId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Get all spotlight cards
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SpotlightCardDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _spotlightCardService.GetAllAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all spotlight cards");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Create a new spotlight card
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SpotlightCardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSpotlightCardDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _spotlightCardService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid spotlight card creation request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating spotlight card");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Update a spotlight card
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SpotlightCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSpotlightCardDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _spotlightCardService.UpdateAsync(id, dto, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "SpotlightCard not found or invalid update request: {SpotlightCardId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating spotlight card: {SpotlightCardId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Delete a spotlight card
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _spotlightCardService.DeleteAsync(id, cancellationToken);
            if (result)
                return NoContent();

            return NotFound(new { message = $"SpotlightCard with id {id} not found" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "SpotlightCard not found: {SpotlightCardId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting spotlight card: {SpotlightCardId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Activate a spotlight card
    /// </summary>
    [HttpPost("{id}/activate")]
    [ProducesResponseType(typeof(SpotlightCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _spotlightCardService.ActivateAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "SpotlightCard not found: {SpotlightCardId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while activating spotlight card: {SpotlightCardId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Deactivate a spotlight card
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [ProducesResponseType(typeof(SpotlightCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _spotlightCardService.DeactivateAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "SpotlightCard not found: {SpotlightCardId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deactivating spotlight card: {SpotlightCardId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }
}

