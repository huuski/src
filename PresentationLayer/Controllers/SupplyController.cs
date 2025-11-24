using ApplicationLayer.DTOs.Supply;
using ApplicationLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplyController : ControllerBase
{
    private readonly ISupplyService _supplyService;
    private readonly ILogger<SupplyController> _logger;

    public SupplyController(ISupplyService supplyService, ILogger<SupplyController> logger)
    {
        _supplyService = supplyService ?? throw new ArgumentNullException(nameof(supplyService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get a supply by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SupplyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _supplyService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Supply not found: {SupplyId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting supply: {SupplyId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Get all supplies
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SupplyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _supplyService.GetAllAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all supplies");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Create a new supply
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SupplyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSupplyDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _supplyService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid supply creation request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating supply");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Update a supply
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SupplyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplyDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _supplyService.UpdateAsync(id, dto, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Supply not found or invalid update request: {SupplyId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating supply: {SupplyId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Delete a supply
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _supplyService.DeleteAsync(id, cancellationToken);
            if (result)
                return NoContent();

            return NotFound(new { message = $"Supply with id {id} not found" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Supply not found: {SupplyId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting supply: {SupplyId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }
}

