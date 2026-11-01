namespace GroceryList.Application.DTOs;

public record CreateGroceryItemRequest(string Name, int Quantity = 1);

public record UpdateBasketRequest(bool IsInBasket);

public record GroceryItemResponse(Guid Id, string Name, int Quantity, bool IsInBasket, DateTimeOffset CreatedAt);
