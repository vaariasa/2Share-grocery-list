using GroceryList.Domain.Entities;

namespace GroceryList.Application.Interfaces;

/// <summary>
/// Creates the access token (JWT) that the client sends with every request after login.
/// </summary>
public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAt) CreateToken(User user);
}
