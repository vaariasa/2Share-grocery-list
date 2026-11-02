using GroceryList.Application.DTOs;
using GroceryList.Application.Exceptions;
using GroceryList.Application.Interfaces;
using GroceryList.Domain.Entities;

namespace GroceryList.Application.Services;

/// <summary>
/// Use cases for the grocery list (user stories 3 to 7).
/// Every method receives the userId of the logged-in user, taken from the token by the API.
/// </summary>
public class GroceryItemService(IGroceryItemRepository groceryItemRepository)
{
    public async Task<List<GroceryItemResponse>> GetAllAsync(Guid userId)
    {
        var items = await groceryItemRepository.GetAllByUserAsync(userId);
        return items.Select(ToResponse).ToList();
    }

    public async Task<GroceryItemResponse> AddAsync(Guid userId, CreateGroceryItemRequest request)
    {
        var name = request.Name?.Trim() ?? string.Empty;

        if (name.Length is 0 or > 100)
            throw new ValidationException("Name is required and must be at most 100 characters.");

        var now = DateTimeOffset.UtcNow;
        var item = new GroceryItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            IsInBasket = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        await groceryItemRepository.AddAsync(item);

        return ToResponse(item);
    }

    public async Task DeleteAsync(Guid userId, Guid itemId)
    {
        var item = await GetOwnItemAsync(userId, itemId);
        await groceryItemRepository.DeleteAsync(item);
    }

    /// <summary>Tag (true) or un-tag (false) an item: put it in the basket or back on the shelf.</summary>
    public async Task<GroceryItemResponse> SetInBasketAsync(Guid userId, Guid itemId, bool isInBasket)
    {
        var item = await GetOwnItemAsync(userId, itemId);

        item.IsInBasket = isInBasket;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await groceryItemRepository.UpdateAsync(item);

        return ToResponse(item);
    }

    // An item of another user is treated as "not found", so its existence is never revealed.
    private async Task<GroceryItem> GetOwnItemAsync(Guid userId, Guid itemId) =>
        await groceryItemRepository.GetByIdAsync(itemId, userId)
        ?? throw new NotFoundException("Grocery item not found.");

    private static GroceryItemResponse ToResponse(GroceryItem item) =>
        new(item.Id, item.Name, item.IsInBasket, item.CreatedAt);
}
