using GroceryList.Application.DTOs;
using GroceryList.Application.Exceptions;
using GroceryList.Application.Interfaces;
using GroceryList.Domain.Entities;

namespace GroceryList.Application.Services;

/// <summary>
/// Use cases for user accounts: register (user story 1) and login (user story 2).
/// </summary>
public class AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
{
    private const int MinPasswordLength = 6;

    public async Task<UserResponse> RegisterAsync(RegisterRequest request)
    {
        var username = request.Username?.Trim() ?? string.Empty;
        var fullName = request.FullName?.Trim() ?? string.Empty;
        var password = request.Password ?? string.Empty;

        if (username.Length is 0 or > 50)
            throw new ValidationException("Username is required and must be at most 50 characters.");
        if (fullName.Length is 0 or > 100)
            throw new ValidationException("Full name is required and must be at most 100 characters.");
        if (password.Length < MinPasswordLength)
            throw new ValidationException($"Password must be at least {MinPasswordLength} characters.");

        if (await userRepository.UsernameExistsAsync(username))
            throw new ConflictException("This username is already taken.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            FullName = fullName,
            PasswordHash = passwordHasher.Hash(password),
            CreatedAt = DateTimeOffset.UtcNow
        };

        await userRepository.AddAsync(user);

        return new UserResponse(user.Id, user.Username, user.FullName);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var username = request.Username?.Trim() ?? string.Empty;
        var password = request.Password ?? string.Empty;

        var user = await userRepository.GetByUsernameAsync(username);

        // Same message for "unknown user" and "wrong password",
        // so nobody can find out which usernames exist.
        if (user is null || !passwordHasher.Verify(user.PasswordHash, password))
            throw new UnauthorizedException("Invalid username or password.");

        var (token, expiresAt) = tokenService.CreateToken(user);

        return new LoginResponse(token, expiresAt, user.Username, user.FullName);
    }
}
