namespace GroceryList.Application.Interfaces;

/// <summary>
/// Turns a plain text password into a hash and checks a password against a stored hash.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string passwordHash, string password);
}
