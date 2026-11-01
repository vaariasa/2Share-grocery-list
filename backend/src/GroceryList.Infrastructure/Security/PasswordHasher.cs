using GroceryList.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GroceryList.Infrastructure.Security;

/// <summary>
/// Uses the ASP.NET Core Identity password hasher (PBKDF2 with a random salt).
/// We only use the hasher, not the full Identity system.
/// </summary>
public class PasswordHasher : Application.Interfaces.IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    // The default Identity hasher does not use the user object, so we pass null.
    public string Hash(string password) =>
        _hasher.HashPassword(null!, password);

    public bool Verify(string passwordHash, string password) =>
        _hasher.VerifyHashedPassword(null!, passwordHash, password) != PasswordVerificationResult.Failed;
}
