# REST API

This is the contract between any frontend and the backend. A new frontend only needs this document,
and a new backend implementation only needs to respect it.

- **Base URL (development):** `http://localhost:5157/api`
- **Format:** JSON (`Content-Type: application/json`), property names in camelCase
- **Authentication:** JWT bearer token, sent as `Authorization: Bearer <token>`
- **Machine-readable description:** OpenAPI document at `http://localhost:5157/openapi/v1.json` (development only)
- **Try it:** [`backend/src/GroceryList.Api/GroceryList.Api.http`](../backend/src/GroceryList.Api/GroceryList.Api.http)

## Endpoints overview

| Method | URL | Auth | Description |
|---|---|---|---|
| `POST` | `/auth/register` | – | Create a user |
| `POST` | `/auth/login` | – | Log in and get a token |
| `GET` | `/grocery-items` | ✔ | Get the items of the logged-in user |
| `POST` | `/grocery-items` | ✔ | Add an item |
| `DELETE` | `/grocery-items/{id}` | ✔ | Remove an item |
| `PATCH` | `/grocery-items/{id}/basket` | ✔ | Tag / un-tag an item |

There is no logout endpoint: the token is stateless, so the client logs out by deleting it.

## Errors

Errors use the standard **ProblemDetails** format (RFC 7807), with content type `application/problem+json`:

```json
{
  "title": "This username is already taken.",
  "status": 409
}
```

| Status | Meaning |
|---|---|
| `400 Bad Request` | Invalid data (e.g. empty name, password too short) |
| `401 Unauthorized` | Wrong credentials on login, or missing / invalid / expired token |
| `404 Not Found` | The item does not exist **or belongs to another user** |
| `409 Conflict` | The username is already taken |
| `500 Internal Server Error` | Unexpected error (details are logged on the server, not returned) |

---

## Auth

### `POST /auth/register`

Creates a new user.

Request:

```json
{
  "username": "jdoe",
  "fullName": "John Doe",
  "password": "Secret123"
}
```

| Field | Rules |
|---|---|
| `username` | required, 1–50 characters, unique |
| `fullName` | required, 1–100 characters |
| `password` | required, at least 6 characters |

Response `201 Created`:

```json
{
  "id": "d70a039c-9a3b-422a-aa27-b7f9308fa393",
  "username": "jdoe",
  "fullName": "John Doe"
}
```

Errors: `400` (invalid data), `409` (username already taken).

### `POST /auth/login`

Request:

```json
{
  "username": "jdoe",
  "password": "Secret123"
}
```

Response `200 OK`:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-11-01T22:40:37.45+00:00",
  "username": "jdoe",
  "fullName": "John Doe"
}
```

Errors: `401` with `"Invalid username or password."` (the same message for both cases, on purpose).

The token is valid for 60 minutes (configurable in `appsettings.json` → `Jwt:ExpirationMinutes`).
Its `sub` claim contains the user id.

---

## Grocery items

All these endpoints require `Authorization: Bearer <token>` and only work on the items of the user in the token.

### `GET /grocery-items`

Response `200 OK`, ordered by creation date:

```json
[
  {
    "id": "f3dd1acb-5c0c-40ab-a458-c64eb7f88f32",
    "name": "Milk",
    "isInBasket": false,
    "createdAt": "2026-11-01T21:40:37.68+00:00"
  }
]
```

### `POST /grocery-items`

Request:

```json
{
  "name": "Milk"
}
```

| Field | Rules |
|---|---|
| `name` | required, 1–100 characters |

Response `201 Created`: the created item (same shape as in `GET`). New items have `isInBasket: false`.

Errors: `400` (invalid data).

### `DELETE /grocery-items/{id}`

Response `204 No Content`.

Errors: `404` (not found or not yours).

### `PATCH /grocery-items/{id}/basket`

Tags (`true`) or un-tags (`false`) an item.

Request:

```json
{
  "isInBasket": true
}
```

Response `200 OK`: the updated item.

Errors: `404` (not found or not yours).
