# Warehouse Management System

This project is a lightweight Warehouse Management System (WMS) built with ASP.NET Core MVC, following modern architectural best practices. It was designed to demonstrate a maintainable, scalable, and robust application structure.

## Features

- **Product Inventory Management**: Full CRUD (Create, Read, Update, Delete) operations for products.
- **CSV Data Import**: Bulk import and update product data from a CSV file. The import logic is idempotent; it updates existing products (by SKU) and creates new ones.
- **Normalized Database Schema**: The `Product` entity is correctly normalized. `Category` and `Manufacturer` are stored in separate tables to ensure data integrity and efficiency (Single Source of Truth).
- **Price Margin Calculation**: The main product view automatically calculates and displays the price margin percentage `((Sell Price - Cost Price) / Sell Price) * 100`.
- **Efficient UI**: The frontend uses jQuery DataTables with server-side processing via AJAX to ensure the UI is fast and scalable, even with a large number of products.

## Architectural Approach: Clean Architecture & CQRS

The application is built using a combination of **Clean Architecture** and **Command Query Responsibility Segregation (CQRS)**.

- **`WarehouseManagementSystem.Domain` (Entities)**: The core of the application. Contains the `Product`, `Category`, and `Manufacturer` entities. It has no dependencies.
- **`WarehouseManagementSystem.Application` (Application Layer)**: Contains all business logic (Use Cases). It depends only on the Domain layer via abstractions (`IWarehouseDbContext`).
- **`WarehouseManagementSystem.Persistence` (Infrastructure Layer)**: Implements data access using Entity Framework Core. It references the Application layer to implement its interfaces.
- **`WarehouseManagementSystem.Web` (Presentation Layer)**: The ASP.NET Core MVC project. It depends on the Application layer (via MediatR) to execute commands and queries.

This structure ensures the system is **Maintainable**, **Testable**, and **Scalable**.

## Setup Instructions

1.  **Prerequisites**:

    - .NET 7 or later SDK.
    - Visual Studio 2022 with the ASP.NET and web development workload (which includes SQL Server LocalDB).

2.  **Database Configuration**:

    - The connection string in `WarehouseManagementSystem.Web/appsettings.json` points to SQL Server LocalDB.
    - `"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WMS_Normalized_Final;Trusted_Connection=True;"`

3.  **Run Migrations**:

    - Open the **Package Manager Console** in Visual Studio (`View -> Other Windows -> Package Manager Console`).
    - Set the `Default project` dropdown to `WarehouseManagementSystem.Persistence`.
    - Run the command: `Add-Migration InitialCreate` (or a name of your choice).
    - Run the command: `Update-Database`
    - This will create the database and the `Products`, `Categories`, and `Manufacturers` tables with the correct relationships.

4.  **Run the Application**:
    - Set `WarehouseManagementSystem.Web` as the startup project.
    - Press F5 or the Run button in Visual Studio.

## Future Enhancements

Given the 4-hour time constraint, the focus was on building a solid architectural foundation. If this project were to evolve, the following enhancements would be considered:

### Phase 1: Decoupling with a Dedicated Web API

- **Current State:** The application is a traditional monolith where the backend and frontend are tightly coupled. Data is served from MVC controller actions directly to Razor views.
- **Enhancement:** Refactor all data access endpoints (like `GetProducts`) into a separate, dedicated **Web API project**. This API would follow RESTful principles and become the single source of truth for all data.
- **Benefit:** This is the foundational step for all future growth. It decouples the system, allowing the API and any number of frontend clients to be developed, deployed, and scaled independently.

### Phase 2: Frontend Modernization with a Single Page Application (SPA)

- **Current State:** The UI is built using server-rendered Razor views enhanced with jQuery. While robust and quick to develop for this test, this approach leads to full-page reloads and limits the potential for a modern, interactive user experience.
- **Enhancement:** Once the Dedicated Web API is in place, the frontend would be re-architected as a **Single Page Application (SPA)** using a modern JavaScript framework like **React, Vue, or Angular**.
- **Benefits:**
  - **Rich, App-like User Experience:** Eliminates page flashes and enables fluid transitions, real-time updates, and complex interactive components that users expect from modern software.
  - **Platform Versatility:** The same Web API built in Phase 1 could simultaneously power the web SPA, native mobile apps (iOS/Android), and other third-party services, creating a unified and efficient ecosystem.
  - **Clear Separation of Concerns:** Creates a definitive line between frontend (UI/UX) and backend (business logic/data) developers, improving team efficiency and code maintainability.

### Other High-Priority Improvements

- **Comprehensive Testing:** Create dedicated test projects to write **unit tests** for the Application layer handlers (using a mocking framework like `Moq`) and **integration tests** for the full request pipeline to ensure reliability and prevent regressions.

- **High-Performance Data Paging**:
  - **Problem:** The current product table loads all inventory data to the client on page load. While functional for a small dataset, this approach will not scale and will cause significant UI lag with thousands of products.
  - **Solution:** Implement **Server-Side Processing (SSP)**. This would involve creating a new API endpoint that accepts specific parameters for paging, sorting, and searching. The business logic would be updated to perform these operations directly on the database using LINQ (`.Skip()`, `.Take()`, `OrderBy()`, `Where()`), ensuring that only the 10-25 records needed for the current view are ever sent to the client.
