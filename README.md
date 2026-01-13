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

## Application Scope (Current)

### Included

* Base MVC project structure
* Controllers with defined routes
* Razor views for each route
* Shared layout and navigation

### Not Included Yet

* Database or Entity Framework
* Business logic
* Authentication / authorization
* CRUD operations
* Deployment configuration

---

## Application Purpose

The application is intended to manage inventory for a small business or internal team. At a high level, it will allow users to:

* Manage products
* Manage categories
* Track stock movements (in/out)
* View current inventory status

At this stage, these features are **defined conceptually only** and represented by routes and views.

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

## Navigation

The main navigation menu provides access to:

* Home
* Products
* Categories
* Stock
* Login

Navigation uses **ASP.NET Core Tag Helpers** (`asp-controller`, `asp-action`) to ensure routing safety and maintainability.

---

## Project Structure (Relevant Folders)

* `Controllers/` – MVC controllers defining routes (Home, Products, Categories, Stock, Account)
* `Views/` – Razor views organized by controller name
* `Views/Shared/` – Layout and shared UI components
* `wwwroot/` – Static assets (CSS, JS, libraries)

---
