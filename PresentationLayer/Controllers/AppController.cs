using ApplicationLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[ApiController]
[Route("api/app")]
public class AppController : ControllerBase
{
    private readonly IResetService _resetService;
    private readonly ILogger<AppController> _logger;

    public AppController(IResetService resetService, ILogger<AppController> logger)
    {
        _resetService = resetService ?? throw new ArgumentNullException(nameof(resetService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Reset all demo data to initial seed state
    /// </summary>
    [HttpPost("resetdemo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetDemo(CancellationToken cancellationToken)
    {
        try
        {
            await _resetService.ResetAllDataAsync(cancellationToken);
            _logger.LogInformation("Demo data reset successfully");
            return Ok(new { message = "Demo data reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while resetting demo data");
            return StatusCode(500, new { message = "An internal server error occurred while resetting demo data", error = ex.Message });
        }
    }
}

