using ApplicationLayer.DTOs.Auth;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.ValueObjects;

namespace ApplicationLayer.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required", nameof(request));

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password is required", nameof(request));

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password");

        if (!_passwordHasher.VerifyPassword(request.Password, user.Password.Hash))
            throw new UnauthorizedAccessException("Invalid email or password");

        // Revoke all existing refresh tokens for this user (optional: for security, only allow one active session)
        await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id, cancellationToken);

        var tokens = await _tokenService.GenerateTokensAsync(user, cancellationToken);

        return new LoginResponseDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            ExpiresAt = tokens.ExpiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name.Value,
                Email = user.Email.Value,
                Avatar = user.Avatar
            }
        };
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required", nameof(request));

        if (string.IsNullOrWhiteSpace(request.NewPassword))
            throw new ArgumentException("New password is required", nameof(request));

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new ArgumentException("User not found", nameof(request));

        // TODO: Implement token validation for password reset
        // For now, we'll just update the password
        var hashedPassword = _passwordHasher.HashPassword(request.NewPassword);
        var newPassword = Password.FromHash(hashedPassword);

        user.UpdatePassword(newPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return true;
    }

    public async Task<TokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token is required", nameof(refreshToken));

        // Validate refresh token and get stored token
        var userIdString = await _tokenService.ValidateRefreshTokenAsync(refreshToken, cancellationToken);
        if (userIdString == null || !Guid.TryParse(userIdString, out var userId))
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (storedToken == null || !storedToken.IsActive)
            throw new UnauthorizedAccessException("Invalid or revoked refresh token");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        // Token rotation: revoke old token and generate new tokens
        var newTokens = await _tokenService.GenerateTokensAsync(user, cancellationToken);
        storedToken.Revoke(newTokens.RefreshToken);
        await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);

        return newTokens;
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (storedToken == null || storedToken.IsRevoked)
            return false;

        storedToken.Revoke();
        await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);
        return true;
    }

    public async Task<bool> RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.RevokeAllUserTokensAsync(userId, cancellationToken);
        return true;
    }
}
