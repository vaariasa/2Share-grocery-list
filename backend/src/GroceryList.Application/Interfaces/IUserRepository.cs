using GroceryList.Domain.Entities;

namespace GroceryList.Application.Interfaces;

/// <summary>
/// Data access for users. Implemented in the Infrastructure layer.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);

    Task<bool> UsernameExistsAsync(string username);

    Task AddAsync(User user);
}
