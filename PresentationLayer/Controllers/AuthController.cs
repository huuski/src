using ApplicationLayer.DTOs.Auth;
using ApplicationLayer.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // Allow anonymous access to authentication endpoints
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Authenticates a user and returns access and refresh tokens
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication tokens and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.LoginAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Login failed for email: {Email}", request.Email);
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid login request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during login");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Resets a user's password
    /// </summary>
    /// <param name="request">Reset password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("resetpassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(request, cancellationToken);
            
            if (result)
                return Ok(new { message = "Password reset successfully" });
            
            return BadRequest(new { message = "Failed to reset password" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid reset password request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during password reset");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Refreshes access token using a refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New access and refresh tokens</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApplicationLayer.Interfaces.Services.TokenResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new { message = "Refresh token is required" });

            var result = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Invalid refresh token");
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid refresh token request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during token refresh");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Revokes a refresh token (logout)
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new { message = "Refresh token is required" });

            var result = await _authService.RevokeTokenAsync(request.RefreshToken, cancellationToken);
            
            if (result)
                return Ok(new { message = "Token revoked successfully" });
            
            return BadRequest(new { message = "Invalid or already revoked token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during logout");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Revokes all refresh tokens for the authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("logout-all")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user token" });

            var result = await _authService.RevokeAllUserTokensAsync(userId, cancellationToken);
            
            if (result)
                return Ok(new { message = "All tokens revoked successfully" });
            
            return BadRequest(new { message = "Failed to revoke tokens" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during logout all");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }
}
