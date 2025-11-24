using ApplicationLayer.DTOs.Auth;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.ValueObjects;

namespace ApplicationLayer.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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

        var tokens = _tokenService.GenerateTokens(user);

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

        var userIdString = _tokenService.ValidateRefreshToken(refreshToken);
        if (userIdString == null || !Guid.TryParse(userIdString, out var userId))
            throw new UnauthorizedAccessException("Invalid refresh token");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        return _tokenService.GenerateTokens(user);
    }
}
