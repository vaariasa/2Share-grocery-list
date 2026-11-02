namespace GroceryList.Application.DTOs;

public record CreateGroceryItemRequest(string Name);

public record UpdateBasketRequest(bool IsInBasket);

public record GroceryItemResponse(Guid Id, string Name, bool IsInBasket, DateTimeOffset CreatedAt);
