using ApplicationLayer.DTOs.AppointmentService;
using ApplicationLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentServiceController : ControllerBase
{
    private readonly IAppointmentServiceService _appointmentServiceService;
    private readonly ILogger<AppointmentServiceController> _logger;

    public AppointmentServiceController(IAppointmentServiceService appointmentServiceService, ILogger<AppointmentServiceController> logger)
    {
        _appointmentServiceService = appointmentServiceService ?? throw new ArgumentNullException(nameof(appointmentServiceService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AppointmentServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _appointmentServiceService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "AppointmentService not found: {AppointmentServiceId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting appointment service: {AppointmentServiceId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AppointmentServiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _appointmentServiceService.GetAllAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all appointment services");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpGet("appointment/{appointmentId}")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentServiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAppointmentId(Guid appointmentId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _appointmentServiceService.GetByAppointmentIdAsync(appointmentId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting appointment services by appointment: {AppointmentId}", appointmentId);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(AppointmentServiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentServiceDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _appointmentServiceService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid appointment service creation request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating appointment service");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AppointmentServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentServiceDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _appointmentServiceService.UpdateAsync(id, dto, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "AppointmentService not found or invalid update request: {AppointmentServiceId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating appointment service: {AppointmentServiceId}", id);
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
            var result = await _appointmentServiceService.DeleteAsync(id, cancellationToken);
            if (result)
                return NoContent();

            return NotFound(new { message = $"AppointmentService with id {id} not found" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "AppointmentService not found: {AppointmentServiceId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting appointment service: {AppointmentServiceId}", id);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Update status of an AppointmentService
    /// </summary>
    [HttpPut("status")]
    [ProducesResponseType(typeof(AppointmentServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateAppointmentServiceStatusDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _appointmentServiceService.UpdateStatusAsync(dto, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "AppointmentService not found or invalid status update request: {AppointmentServiceId}", dto?.AppointmentServiceId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating appointment service status: {AppointmentServiceId}", dto?.AppointmentServiceId);
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }
}

