namespace GroceryList.Infrastructure.Security;

/// <summary>
/// JWT settings, read from the "Jwt" section of appsettings.json.
/// Used to create tokens (JwtTokenService) and to validate them (Program.cs).
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>Secret key used to sign tokens. Must be at least 32 characters.</summary>
    public string Key { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; } = 60;
}
