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
    TokenResult GenerateTokens(User user);
    string? ValidateRefreshToken(string refreshToken);
    Guid? GetUserIdFromToken(string token);
}
