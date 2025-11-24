using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces.Services;

public class TokenResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public interface ITokenService
{
    Task<TokenResult> GenerateTokensAsync(User user, CancellationToken cancellationToken = default);
    Task<string?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Guid? GetUserIdFromToken(string token);
}
