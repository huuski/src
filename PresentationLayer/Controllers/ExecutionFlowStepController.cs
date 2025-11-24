using ApplicationLayer.DTOs.ExecutionFlowStep;
using ApplicationLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExecutionFlowStepController : ControllerBase
{
    private readonly IExecutionFlowStepService _executionFlowStepService;
    private readonly ILogger<ExecutionFlowStepController> _logger;

    public ExecutionFlowStepController(
        IExecutionFlowStepService executionFlowStepService,
        ILogger<ExecutionFlowStepController> logger)
    {
        _executionFlowStepService = executionFlowStepService ?? throw new ArgumentNullException(nameof(executionFlowStepService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExecutionFlowStepDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _executionFlowStepService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "ExecutionFlowStep not found: {ExecutionFlowStepId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting execution flow step: {ExecutionFlowStepId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExecutionFlowStepDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _executionFlowStepService.GetAllAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all execution flow steps");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet("executionflow/{executionFlowId}")]
    [ProducesResponseType(typeof(IEnumerable<ExecutionFlowStepDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByExecutionFlowId(Guid executionFlowId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _executionFlowStepService.GetByExecutionFlowIdAsync(executionFlowId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting execution flow steps for execution flow: {ExecutionFlowId}", executionFlowId);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ExecutionFlowStepDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateExecutionFlowStepDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _executionFlowStepService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid execution flow step creation request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating execution flow step");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ExecutionFlowStepDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExecutionFlowStepDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _executionFlowStepService.UpdateAsync(id, dto, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "ExecutionFlowStep not found or invalid update request: {ExecutionFlowStepId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating execution flow step: {ExecutionFlowStepId}", id);
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
            var result = await _executionFlowStepService.DeleteAsync(id, cancellationToken);
            if (result)
                return NoContent();

            return NotFound(new { message = $"ExecutionFlowStep with id {id} not found" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "ExecutionFlowStep not found: {ExecutionFlowStepId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting execution flow step: {ExecutionFlowStepId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }
}

