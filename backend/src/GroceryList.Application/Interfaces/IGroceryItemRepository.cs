using GroceryList.Domain.Entities;

namespace GroceryList.Application.Interfaces;

/// <summary>
/// Data access for grocery items. Implemented in the Infrastructure layer.
/// Every read receives the userId, so a user can never load items of another user.
/// </summary>
public interface IGroceryItemRepository
{
    Task<List<GroceryItem>> GetAllByUserAsync(Guid userId);

    /// <summary>Returns the item only if it belongs to the given user, otherwise null.</summary>
    Task<GroceryItem?> GetByIdAsync(Guid id, Guid userId);

    Task AddAsync(GroceryItem item);

    Task UpdateAsync(GroceryItem item);

    Task DeleteAsync(GroceryItem item);
}
