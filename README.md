# StockCore

## Overview

StockCore is a web-based Inventory Management System built using **ASP.NET Core MVC**. The application is designed as a learning-focused project for **CSE 325: .NET Software Development (BYU-Idaho)** and follows standard enterprise MVC conventions.

---

## Technology Stack (Current Stage)

* **Framework:** ASP.NET Core (.NET 8)
* **Architecture:** MVC (Model–View–Controller)
* **View Engine:** Razor Views (.cshtml)
* **Routing:** Conventional MVC routing
* **Frontend:** Razor + Bootstrap (default template) + CSS

---

## Application Purpose

StockCore is designed for small business owners or team members responsible for managing inventory.
All users act as administrators of the system and can fully manage the available data.

The application allows users to:

Create and manage products

Organize products into categories

Register stock entries and outputs

View real-time inventory status

Track stock movement history

Securely log in and manage their session

The system centralizes inventory management and replaces manual spreadsheets or paper-based tracking with a structured digital solution.

## Core Features

### Users can:

Create an account and log in securely

Create, edit, and delete products

Create, edit, and delete categories

Register stock entries (incoming stock)

Register stock outputs (outgoing stock)

View current stock levels per product

View historical stock movements

---

## Controllers and Routes

All core areas of the application are represented by controllers, even if their internal logic is not yet implemented. This ensures the routing and navigation structure is complete.

### Home

| Route           | Action  | Description                          |
| --------------- | ------- | ------------------------------------ |
| `/`             | Index   | Landing page / dashboard placeholder |
| `/Home/Privacy` | Privacy | Static privacy page                  |

---

### Products

| Route                    | Action  | Description                       |
| ------------------------ | ------- | --------------------------------- |
| `/Products`              | Index   | List of products (placeholder)    |
| `/Products/Details/{id}` | Details | Product details (placeholder)     |
| `/Products/Create`       | Create  | Create product form (placeholder) |
| `/Products/Edit/{id}`    | Edit    | Edit product form (placeholder)   |
| `/Products/Delete/{id}`  | Delete  | Delete confirmation (placeholder) |

---

### Categories

| Route                     | Action | Description                                |
| ------------------------- | ------ | ------------------------------------------ |
| `/Categories`             | Index  | List of categories (placeholder)           |
| `/Categories/Create`      | Create | Create category form (placeholder)         |
| `/Categories/Edit/{id}`   | Edit   | Edit category form (placeholder)           |
| `/Categories/Delete/{id}` | Delete | Delete category confirmation (placeholder) |

---

### Stock

| Route                        | Action  | Description                          |
| ---------------------------- | ------- | ------------------------------------ |
| `/Stock`                     | Index   | Inventory overview (placeholder)     |
| `/Stock/In/{productId}`      | In      | Register stock entry (placeholder)   |
| `/Stock/Out/{productId}`     | Out     | Register stock output (placeholder)  |
| `/Stock/History/{productId}` | History | Stock movement history (placeholder) |

---

### Authentication

Authentication is represented at the routing level only. These routes exist as placeholders to complete the application structure.

| Route       | Action   | Description                 |
| ----------- | -------- | --------------------------- |
| `/Login`    | Login    | Login page placeholder      |
| `/Register` | Register | Register page placeholder   |
| `/Logout`   | Logout   | Logout endpoint placeholder |

---
## Project Structure

StockCore
├── Controllers/        → Application controllers
├── Dtos/				→ Dtos for service
├── Entities/			→ Tables and relations
├── Data/               → Entity Framework DbContext
├── Services/           → Business logic
├── Views/              → Razor views
│   └── Shared/         → Layout and shared components
├── Migrations/         → EF Core migrations
├── wwwroot/            → Static assets (CSS, JS)
├── Program.cs          → Application entry point
├── appsettings.json    → Configuration
└── StockCore.csproj

---

## Navigation

The main navigation menu provides access to:

* Home
* Products
* Categories
* Stock
* Login
* Profile

---