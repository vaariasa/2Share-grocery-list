using GroceryList.Application.DTOs;
using GroceryList.Application.Exceptions;
using GroceryList.Application.Interfaces;
using GroceryList.Application.Services;
using GroceryList.Domain.Entities;
using Moq;

namespace GroceryList.Application.Tests;

/// <summary>
/// Unit tests for AuthService (user stories 1 and 2).
/// The repository, password hasher and token service are mocked: these tests check business
/// rules only, with no real database and no real cryptography.
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly AuthService _sut; // "sut" = system under test

    public AuthServiceTests()
    {
        _sut = new AuthService(_userRepository.Object, _passwordHasher.Object, _tokenService.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesTheUserAndHashesThePassword()
    {
        _userRepository.Setup(r => r.UsernameExistsAsync("jdoe")).ReturnsAsync(false);
        _passwordHasher.Setup(h => h.Hash("Secret123")).Returns("hashed-password");

        var result = await _sut.RegisterAsync(new RegisterRequest("jdoe", "John Doe", "Secret123"));

        Assert.Equal("jdoe", result.Username);
        Assert.Equal("John Doe", result.FullName);
        // The hasher must be used, and the raw password must never reach the repository.
        _userRepository.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Username == "jdoe" && u.FullName == "John Doe" && u.PasswordHash == "hashed-password")));
    }

    [Fact]
    public async Task RegisterAsync_WithATakenUsername_ThrowsConflict()
    {
        _userRepository.Setup(r => r.UsernameExistsAsync("jdoe")).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() =>
            _sut.RegisterAsync(new RegisterRequest("jdoe", "John Doe", "Secret123")));

        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Theory]
    [InlineData("", "John Doe", "Secret123")] // empty username
    [InlineData("jdoe", "", "Secret123")] // empty full name
    [InlineData("jdoe", "John Doe", "123")] // password shorter than 6 characters
    public async Task RegisterAsync_WithInvalidData_ThrowsValidation(string username, string fullName, string password)
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.RegisterAsync(new RegisterRequest(username, fullName, password)));
    }

    [Fact]
    public async Task LoginAsync_WithCorrectCredentials_ReturnsAToken()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "jdoe", FullName = "John Doe", PasswordHash = "hashed" };
        _userRepository.Setup(r => r.GetByUsernameAsync("jdoe")).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify("hashed", "Secret123")).Returns(true);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(60);
        _tokenService.Setup(t => t.CreateToken(user)).Returns(("jwt-token", expiresAt));

        var result = await _sut.LoginAsync(new LoginRequest("jdoe", "Secret123"));

        Assert.Equal("jwt-token", result.Token);
        Assert.Equal("jdoe", result.Username);
    }

    [Fact]
    public async Task LoginAsync_WithAnUnknownUsername_ThrowsUnauthorized()
    {
        _userRepository.Setup(r => r.GetByUsernameAsync("ghost")).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _sut.LoginAsync(new LoginRequest("ghost", "whatever")));
    }

    [Fact]
    public async Task LoginAsync_WithTheWrongPassword_ThrowsUnauthorized()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "jdoe", PasswordHash = "hashed" };
        _userRepository.Setup(r => r.GetByUsernameAsync("jdoe")).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify("hashed", "wrong")).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _sut.LoginAsync(new LoginRequest("jdoe", "wrong")));
    }
}
