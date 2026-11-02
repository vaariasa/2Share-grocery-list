# Grocery List – Concept Demo Web App

A small web application where users register, log in and manage their own grocery list:
add products, remove them, and tag / un-tag them while shopping (put them in or out of the basket).

It is built as two independent parts that only talk to each other through a REST/JSON API:

| Part | Technology | Folder |
|---|---|---|
| Frontend | Angular 22, TypeScript, SCSS | [`frontend/`](frontend/) |
| Backend | ASP.NET Core Web API (.NET 10, C#), Clean Architecture, JWT | [`backend/`](backend/) |
| Database | SQL Server (LocalDB) | [`database/`](database/) |

Because of this split, a new frontend (e.g. a mobile app) can use the same API, and the backend can be
replaced by another implementation as long as it respects the [API contract](docs/api.md).

## Documentation

| Document | What you will find |
|---|---|
| [docs/architecture.md](docs/architecture.md) | Functional, logical and technical design. **System and software architecture diagrams**, data model, sequence diagrams, design decisions |
| [docs/api.md](docs/api.md) | REST API contract: every endpoint with request/response examples and status codes |
| [docs/handover.md](docs/handover.md) | Guide for the next developer: where things are, how to add a feature, known limitations, next steps |

## Features (user stories)

1. Register with username, full name and password
2. Log in with username and password
3. See your own grocery list (never the products of other users)
4. Add a product
5. Remove a product
6. Tag a product (put it in the basket)
7. Un-tag a product (put it back)
8. Log out
9. Without an active session no products can be seen – login is mandatory

## Repository structure

```
2Share-grocery-list/
├── README.md               ← you are here
├── docs/                   ← architecture, API and handover documentation
├── database/
│   └── create-tables.sql   ← creates the database and tables
├── backend/
│   ├── GroceryList.sln
│   └── src/
│       ├── GroceryList.Domain/          ← entities
│       ├── GroceryList.Application/     ← use cases, DTOs, interfaces
│       ├── GroceryList.Infrastructure/  ← EF Core, repositories, JWT, password hashing
│       └── GroceryList.Api/             ← controllers, configuration (entry point)
└── frontend/
    └── src/app/
        ├── core/           ← models, services, interceptor, guard
        └── features/       ← pages: login, register, grocery list
```

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 22.22+ or 24.15+ (required by Angular 22)
- SQL Server LocalDB (installed with Visual Studio or SQL Server Express) and `sqlcmd`

### 1. Create the database

```bash
# Create and start a LocalDB instance called "GroceryProject"
sqllocaldb create GroceryProject -s

# Create the database and the tables (safe to run more than once)
sqlcmd -S "(localdb)\GroceryProject" -E -i database/create-tables.sql
```

The database starts empty: create your user from the Register page.

<details>
<summary>Using SQL Server Express or another SQL Server instead of LocalDB</summary>

Run the script against that server instead, for example:

```bash
sqlcmd -S .\SQLEXPRESS -E -i database/create-tables.sql
```

Then point the backend to the same server: change `ConnectionStrings:GroceryDb` in
[`backend/src/GroceryList.Api/appsettings.json`](backend/src/GroceryList.Api/appsettings.json).
Only the `Server=` part changes, the rest stays the same:

```diff
- "GroceryDb": "Server=(localdb)\\GroceryProject;Database=GroceryProject;Trusted_Connection=True;TrustServerCertificate=True"
+ "GroceryDb": "Server=.\\SQLEXPRESS;Database=GroceryProject;Trusted_Connection=True;TrustServerCertificate=True"
```

The `-S` used with `sqlcmd` and the `Server=` used in the connection string must point to the same instance.

</details>

### 2. Run the backend

```bash
dotnet run --project backend/src/GroceryList.Api
```

The API listens on **http://localhost:5157**. You can try every endpoint with
[`GroceryList.Api.http`](backend/src/GroceryList.Api/GroceryList.Api.http)
(Visual Studio, or VS Code with the REST Client extension).

### 3. Run the frontend

```bash
cd frontend
npm install
npm start
```

Open **http://localhost:4200**, register a user and log in.

## Configuration

| Setting | Where | Default |
|---|---|---|
| Database connection | `backend/src/GroceryList.Api/appsettings.json` → `ConnectionStrings:GroceryDb` | `(localdb)\GroceryProject` |
| JWT issuer, audience, key, lifetime | same file → `Jwt` | 60 minutes |
| Allowed frontend origins (CORS) | same file → `Cors:AllowedOrigins` | `http://localhost:4200` |
| API URL used by the frontend | `frontend/src/environments/environment.ts` | `http://localhost:5157/api` |

> The JWT key in `appsettings.json` is for local development only. See [docs/handover.md](docs/handover.md#known-limitations).
