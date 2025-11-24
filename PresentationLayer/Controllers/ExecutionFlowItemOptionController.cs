using ApplicationLayer.DTOs.ExecutionFlowItemOption;
using ApplicationLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExecutionFlowItemOptionController : ControllerBase
{
    private readonly IExecutionFlowItemOptionService _executionFlowItemOptionService;
    private readonly ILogger<ExecutionFlowItemOptionController> _logger;

    public ExecutionFlowItemOptionController(
        IExecutionFlowItemOptionService executionFlowItemOptionService,
        ILogger<ExecutionFlowItemOptionController> logger)
    {
        _executionFlowItemOptionService = executionFlowItemOptionService ?? throw new ArgumentNullException(nameof(executionFlowItemOptionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExecutionFlowItemOptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _executionFlowItemOptionService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "ExecutionFlowItemOption not found: {ExecutionFlowItemOptionId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting execution flow item option: {ExecutionFlowItemOptionId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExecutionFlowItemOptionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _executionFlowItemOptionService.GetAllAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all execution flow item options");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet("executionflowstepitem/{executionFlowStepItemId}")]
    [ProducesResponseType(typeof(IEnumerable<ExecutionFlowItemOptionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByExecutionFlowStepItemId(Guid executionFlowStepItemId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _executionFlowItemOptionService.GetByExecutionFlowStepItemIdAsync(executionFlowStepItemId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting execution flow item options for item: {ExecutionFlowStepItemId}", executionFlowStepItemId);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ExecutionFlowItemOptionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateExecutionFlowItemOptionDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _executionFlowItemOptionService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid execution flow item option creation request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating execution flow item option");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ExecutionFlowItemOptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExecutionFlowItemOptionDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _executionFlowItemOptionService.UpdateAsync(id, dto, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "ExecutionFlowItemOption not found or invalid update request: {ExecutionFlowItemOptionId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating execution flow item option: {ExecutionFlowItemOptionId}", id);
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
            var result = await _executionFlowItemOptionService.DeleteAsync(id, cancellationToken);
            if (result)
                return NoContent();

            return NotFound(new { message = $"ExecutionFlowItemOption with id {id} not found" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "ExecutionFlowItemOption not found: {ExecutionFlowItemOptionId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting execution flow item option: {ExecutionFlowItemOptionId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }
}

