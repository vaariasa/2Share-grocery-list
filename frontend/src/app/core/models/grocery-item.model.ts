// TypeScript shapes of the JSON exchanged with the /api/grocery-items endpoints.
// They mirror the backend DTOs (GroceryList.Application/DTOs/GroceryItemDtos.cs).

export interface GroceryItem {
  id: string;
  name: string;
  isInBasket: boolean;
  createdAt: string;
}

export interface CreateGroceryItemRequest {
  name: string;
}
