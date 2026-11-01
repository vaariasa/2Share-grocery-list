namespace GroceryList.Domain.Entities;

/// <summary>
/// A person who can log in and owns a grocery list.
/// </summary>
public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    /// <summary>Hashed password. The plain text password is never stored.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
}
