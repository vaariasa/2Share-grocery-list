namespace GroceryList.Domain.Entities;

/// <summary>
/// A product on a user's grocery list.
/// </summary>
public class GroceryItem
{
    public Guid Id { get; set; }

    /// <summary>The owner of the item. Users can only see and change their own items.</summary>
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>True when the item is tagged (put in the basket while shopping).</summary>
    public bool IsInBasket { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
