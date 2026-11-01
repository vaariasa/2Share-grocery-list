using System.Security.Claims;
using System.Text;
using GroceryList.Application.Interfaces;
using GroceryList.Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GroceryList.Infrastructure.Security;

/// <summary>
/// Creates a signed JWT that identifies the user.
/// The "sub" (subject) claim holds the user id; the API reads it to know who is calling.
/// </summary>
public class JwtTokenService(JwtSettings settings) : ITokenService
{
    public (string Token, DateTimeOffset ExpiresAt) CreateToken(User user)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(settings.ExpirationMinutes);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = settings.Issuer,
            Audience = settings.Audience,
            Expires = expiresAt.UtcDateTime,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
            ]),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return (token, expiresAt);
    }
}
