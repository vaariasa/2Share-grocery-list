# Handover guide

This guide is for the developer who continues this project. It explains where things are,
the conventions used, how to add a feature, and what is still missing.

Start with the [README](../README.md) to run the app, then read [architecture.md](architecture.md)
for the big picture.

## Where do I find…?

| I want to change… | Go to |
|---|---|
| A database table | `database/create-tables.sql` **and** `backend/src/GroceryList.Infrastructure/Persistence/AppDbContext.cs` |
| An entity | `backend/src/GroceryList.Domain/Entities/` |
| A business rule or validation | `backend/src/GroceryList.Application/Services/` |
| The JSON shape of a request/response | `backend/src/GroceryList.Application/DTOs/` **and** `frontend/src/app/core/models/` |
| A database query | `backend/src/GroceryList.Infrastructure/Repositories/` |
| An endpoint URL or HTTP status | `backend/src/GroceryList.Api/Controllers/` |
| How errors map to HTTP codes | `backend/src/GroceryList.Api/Middleware/ExceptionHandlingMiddleware.cs` |
| JWT, CORS, dependency injection setup | `backend/src/GroceryList.Api/Program.cs` and `appsettings.json` |
| How the frontend calls the API | `frontend/src/app/core/services/` |
| The token header / 401 handling | `frontend/src/app/core/interceptors/auth.interceptor.ts` |
| Which pages need login | `frontend/src/app/app.routes.ts` (`canActivate: [authGuard]`) |
| A page | `frontend/src/app/features/<page>/` |
| Colors and shared styles | `frontend/src/styles.scss` |

## Conventions

- **Language:** English for code, UI, database and documentation.
- **Backend**
  - Dependencies point inwards: `Api → Application → Domain`, `Infrastructure → Application`.
    Never reference Infrastructure from Application or Domain.
  - Controllers are thin: they read the request, take the user id from the token, call a service and
    return the result. No business logic, no try/catch.
  - Services throw the exceptions in `Application/Exceptions` (`ValidationException`, `NotFoundException`…).
    The middleware turns them into HTTP responses.
  - Every grocery item query **must** filter by `UserId`. The user id **must** come from the token
    (`CurrentUserId` in the controller), never from the request body or URL.
  - DTOs are C# `record`s. Entities are never returned directly by the API.
  - Each layer registers its services in its own `DependencyInjection.cs`.
- **Frontend**
  - Standalone components, signals for state, reactive forms, the new control flow (`@if`, `@for`).
  - Pages (`features/`) never use `HttpClient` directly: they call a service in `core/services/`.
  - Models in `core/models/` mirror the backend DTOs. Keep them in sync.
  - File naming follows the Angular CLI default (`login.ts`, class `Login`).
- **Git:** small commits with self-explanatory messages (`Add …`, `Fix …`, `Update …`).

## How to add a feature (example: rename an item)

Follow the same path a request takes, from the inside out:

1. **Domain** – nothing to do (`GroceryItem.Name` already exists). Add properties here if the data is new,
   and update `create-tables.sql` + `AppDbContext`.
2. **Application**
   - DTO: `public record RenameGroceryItemRequest(string Name);` in `DTOs/GroceryItemDtos.cs`.
   - Service: add `RenameAsync(Guid userId, Guid itemId, string name)` to `GroceryItemService`.
     Reuse `GetOwnItemAsync` (ownership check) and validate the name like `AddAsync` does.
   - If you need a new kind of query, add it to `IGroceryItemRepository`.
3. **Infrastructure** – implement any new repository method in `GroceryItemRepository` (filter by `UserId`).
4. **Api** – add `[HttpPatch("{id:guid}/name")]` to `GroceryItemsController`, using `CurrentUserId`.
   Add a request to `GroceryList.Api.http` and document it in `docs/api.md`.
5. **Frontend**
   - Model: `RenameGroceryItemRequest` in `core/models/grocery-item.model.ts`.
   - Service: `rename(id, name)` in `GroceryItemService`.
   - Page: call it from `GroceryList` and update the `items` signal with the response.

## Known limitations

These were left out on purpose to keep the demo simple. They should be addressed before production.

| Limitation | Recommendation |
|---|---|
| The JWT signing key is in `appsettings.json` | Use [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) locally and environment variables / Azure Key Vault in production |
| The token is stored in `localStorage` (readable by JavaScript if the site had an XSS bug) | Consider an `HttpOnly` cookie (needs CSRF protection) |
| No refresh token: the user must log in again after 60 minutes | Add refresh tokens if longer sessions are needed |
| Logout does not invalidate the token on the server (it stays valid until it expires) | Short token lifetime, or a server-side deny list if needed |
| HTTP (not HTTPS) in local development | HTTPS is enabled automatically outside Development (`UseHttpsRedirection`). Host behind HTTPS |
| No integration tests for the API (only unit tests for the Application services, see below) | Add them with `WebApplicationFactory` if the project grows |
| One environment file in the frontend | Add `environment.production.ts` with `fileReplacements` in `angular.json` when deploying |
| No login rate limiting | Use ASP.NET Core rate limiting on `/api/auth/login` |
| No pagination on the item list | Fine for a grocery list. Add paging if lists can grow large |

## Tests

Unit tests for the Application layer live in
[`backend/tests/GroceryList.Application.Tests`](../backend/tests/GroceryList.Application.Tests/)
(xUnit + Moq). They cover `AuthService` and `GroceryItemService` with the repositories, password
hasher and token service mocked, so no database is needed. Run them with:

```bash
dotnet test backend/GroceryList.sln
```

They check the business rules behind each user story: registration validation, a taken username,
wrong login credentials, adding an item, and — the most important one — that an item belonging to
another user is always reported as "not found", never loaded or changed.
