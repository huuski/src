namespace DomainLayer.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    private RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("ExpiresAt must be in the future", nameof(expiresAt));

        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        IsRevoked = false;
    }

    public void Revoke(string? replacedByToken = null)
    {
        if (IsRevoked)
            throw new InvalidOperationException("Token is already revoked");

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
        MarkAsUpdated();
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}

