# FoodHub — Food Ordering Platform

A simplified food ordering platform: browse restaurants, build a cart, place orders and track their status in real time.

## Stack

- **Backend:** ASP.NET Core 8 Web API, Clean Architecture (Domain / Application / Infrastructure / Api)
- **Database:** SQL Server with Entity Framework Core (code-first, auto-created and seeded on startup)
- **Auth:** JWT bearer tokens, PBKDF2 password hashing
- **Frontend:** Vanilla HTML/CSS/JS single-page app (responsive, mobile-first), deployed on Vercel
- **Backend hosting:** MonsterASP.NET

## Project structure

```
FoodOrdering.Domain/          Entities (User, Restaurant, MenuItem, Order, OrderItem)
FoodOrdering.Application/     DTOs, service interfaces, business logic (Auth, Catalog, Orders)
FoodOrdering.Infrastructure/  EF Core DbContext, seed data, JWT + password hashing, DI
FoodOrdering.Api/             Controllers, Program.cs, Swagger
client/                       Static frontend (deployed to Vercel)
```

## API overview

| Method | Endpoint | Description |
| ------ | -------- | ----------- |
| POST | `/api/auth/register` | Create an account, returns JWT |
| POST | `/api/auth/login` | Sign in, returns JWT |
| GET | `/api/restaurants?search=` | List restaurants with menus |
| GET | `/api/restaurants/{id}` | Restaurant details |
| POST | `/api/orders` | Place an order (auth) |
| GET | `/api/orders/mine` | My orders with status (auth) |
| POST | `/api/orders/{id}/advance` | Advance order status (demo) |

Interactive docs are available at `/swagger` on the running API.

## Running locally

1. Set `ConnectionStrings:DefaultConnection` in `FoodOrdering.Api/appsettings.json` to a SQL Server instance.
2. `dotnet run --project FoodOrdering.Api` — the database is created and seeded automatically.
3. Serve `client/` with any static server and set `API_BASE` at the top of `client/app.js` to the API URL.
