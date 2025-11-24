using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;

namespace InfrastructureLayer.Repositories;

public class InMemoryRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly Dictionary<Guid, RefreshToken> _refreshTokens = new();
    private readonly Dictionary<string, Guid> _tokenToIdMap = new(); // For quick lookup by token string

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Task.FromResult<RefreshToken?>(null);

        if (!_tokenToIdMap.TryGetValue(token, out var tokenId))
            return Task.FromResult<RefreshToken?>(null);

        if (!_refreshTokens.TryGetValue(tokenId, out var refreshToken))
            return Task.FromResult<RefreshToken?>(null);

        return Task.FromResult<RefreshToken?>(refreshToken);
    }

    public Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = _refreshTokens.Values
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .ToList();

        return Task.FromResult<IEnumerable<RefreshToken>>(tokens);
    }

    public Task<RefreshToken> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        if (refreshToken == null)
            throw new ArgumentNullException(nameof(refreshToken));

        _refreshTokens[refreshToken.Id] = refreshToken;
        _tokenToIdMap[refreshToken.Token] = refreshToken.Id;

        return Task.FromResult(refreshToken);
    }

    public Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        if (refreshToken == null)
            throw new ArgumentNullException(nameof(refreshToken));

        if (!_refreshTokens.ContainsKey(refreshToken.Id))
            throw new InvalidOperationException($"RefreshToken with Id {refreshToken.Id} not found");

        _refreshTokens[refreshToken.Id] = refreshToken;
        
        // Update token mapping if token changed
        var oldEntry = _tokenToIdMap.FirstOrDefault(kvp => kvp.Value == refreshToken.Id);
        if (oldEntry.Key != null && oldEntry.Key != refreshToken.Token)
        {
            _tokenToIdMap.Remove(oldEntry.Key);
            _tokenToIdMap[refreshToken.Token] = refreshToken.Id;
        }
        else if (oldEntry.Key == null)
        {
            _tokenToIdMap[refreshToken.Token] = refreshToken.Id;
        }

        return Task.CompletedTask;
    }

    public Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userTokens = _refreshTokens.Values
            .Where(t => t.UserId == userId && !t.IsRevoked && !t.IsDeleted)
            .ToList();

        foreach (var token in userTokens)
        {
            token.Revoke();
        }

        return Task.CompletedTask;
    }

    public Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var expiredTokens = _refreshTokens.Values
            .Where(t => t.IsExpired && !t.IsDeleted)
            .ToList();

        foreach (var token in expiredTokens)
        {
            token.MarkAsDeleted();
        }

        return Task.CompletedTask;
    }
}

