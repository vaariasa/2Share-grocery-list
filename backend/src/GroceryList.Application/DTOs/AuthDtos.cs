namespace GroceryList.Application.DTOs;

// DTOs (Data Transfer Objects) are the JSON shapes the API receives and returns.
// They keep the API contract separate from the domain entities.

public record RegisterRequest(string Username, string FullName, string Password);

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, DateTimeOffset ExpiresAt, string Username, string FullName);

public record UserResponse(Guid Id, string Username, string FullName);
