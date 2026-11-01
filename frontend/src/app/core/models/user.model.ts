// TypeScript shapes of the JSON exchanged with the /api/auth endpoints.
// They mirror the backend DTOs (GroceryList.Application/DTOs/AuthDtos.cs).

export interface RegisterRequest {
  username: string;
  fullName: string;
  password: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  username: string;
  fullName: string;
}

export interface UserResponse {
  id: string;
  username: string;
  fullName: string;
}
