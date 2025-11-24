using ApplicationLayer.DTOs.Auth;

namespace ApplicationLayer.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken = default);
    Task<TokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
