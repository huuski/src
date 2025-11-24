using ApplicationLayer.DTOs.ExecutionFlowStepItem;
using ApplicationLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExecutionFlowStepItemController : ControllerBase
{
    private readonly IExecutionFlowStepItemService _executionFlowStepItemService;
    private readonly ILogger<ExecutionFlowStepItemController> _logger;

    public ExecutionFlowStepItemController(
        IExecutionFlowStepItemService executionFlowStepItemService,
        ILogger<ExecutionFlowStepItemController> logger)
    {
        _executionFlowStepItemService = executionFlowStepItemService ?? throw new ArgumentNullException(nameof(executionFlowStepItemService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExecutionFlowStepItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _executionFlowStepItemService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "ExecutionFlowStepItem not found: {ExecutionFlowStepItemId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting execution flow step item: {ExecutionFlowStepItemId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExecutionFlowStepItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _executionFlowStepItemService.GetAllAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all execution flow step items");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet("executionflowstep/{executionFlowStepId}")]
    [ProducesResponseType(typeof(IEnumerable<ExecutionFlowStepItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByExecutionFlowStepId(Guid executionFlowStepId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _executionFlowStepItemService.GetByExecutionFlowStepIdAsync(executionFlowStepId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting execution flow step items for step: {ExecutionFlowStepId}", executionFlowStepId);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ExecutionFlowStepItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateExecutionFlowStepItemDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _executionFlowStepItemService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid execution flow step item creation request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating execution flow step item");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ExecutionFlowStepItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExecutionFlowStepItemDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _executionFlowStepItemService.UpdateAsync(id, dto, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "ExecutionFlowStepItem not found or invalid update request: {ExecutionFlowStepItemId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating execution flow step item: {ExecutionFlowStepItemId}", id);
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
            var result = await _executionFlowStepItemService.DeleteAsync(id, cancellationToken);
            if (result)
                return NoContent();

            return NotFound(new { message = $"ExecutionFlowStepItem with id {id} not found" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "ExecutionFlowStepItem not found: {ExecutionFlowStepItemId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting execution flow step item: {ExecutionFlowStepItemId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }
}

