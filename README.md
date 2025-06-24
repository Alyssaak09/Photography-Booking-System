# Photographer Booking System API

## Overview
This API manages bookings by connecting Clients, Photographers, and Services using clean, efficient data structures and RESTful principles.

## Key Features
- BookingDto represents a full booking with:
    * Client’s name
    * Photographer’s name
    * List of all services attached

- Uses Entity Framework's Include and ThenInclude to eager-load related data in a single query, improving performance and reducing database calls.
- Manual mapping of EF entities to DTOs:
   * Converts complex models into simple, secure BookingDto objects
   * Avoids circular references and simplifies API responses
      
## API Endpoints
- List: Get all bookings with detailed info
- Find/{id}: Get a specific booking with full details
- Add: Create a new booking and link services
- Update/{id}: Update booking details and services
- Delete/{id}: Remove a booking and its service links

All follow RESTful naming conventions for clarity and ease of testing/documentation.

## Controller Responsibilities
- ClientsController
  * CRUD operations for clients
  * Optional: api/Clients/{id}/Bookings for client-specific bookings
  * 1-to-many relationship: 1 Client → Many Bookings
 
- BookingsController
  * Core logic for booking management
  * Manages booking-to-service relationships (many-to-many)
  * Returns BookingDto with all related details

- ServicesController
  * CRUD operations for photography services
  * Optional: api/Services/{id}/Bookings to list related bookings

## Relationships Summary

| Relationship             | Implementation                      | Controller to Handle |
| ------------------------ | ----------------------------------- | -------------------- |
| 1 Client → Many Bookings | `Client` has `ICollection<Booking>` | ClientsController    |
| Booking ↔ Services (M-M) | `Booking_Service` join table        | BookingsController   |

## Best Practices Followed
- Clean separation of concerns using DTOs
- Controllers handle only relevant entity logic
- Efficient data retrieval via eager loading
- RESTful naming conventions for all endpoints


## Common Errors
- Make sure "MySQL.Data" is installed in your project
  * If not installed, go to "Tools" > "Nuget Package Manager" > "Manage Nuget Packages for Solution" > "Browse" > type "MySQL.Data" > "Install"

- Make sure "Swashbuckle.AspNetCore.Swagger" is installed in your project
  * If not installed, go to "Tools" > "Nuget Package Manager" > "Manage Nuget Packages for Solution" > "Browse" > type "Swashbuckle.AspNetCore.Swagger" > "Install"

- Make sure "Swashbuckle.AspNetCore.SwaggerGen" is installed in your project
  * If not installed, go to "Tools" > "Nuget Package Manager" > "Manage Nuget Packages for Solution" > "Browse" > type "Swashbuckle.AspNetCore.SwaggerGen" > "Install"

- Make sure the view folder name 'BookingPage' needs to match the the first two name of the Controller 'BookingPageController' name
    
- Update Layout.cshtml in the shared folder and add a li class and make sure asp-controller matchs the view folder name
  
- Youtude any issues may have regarding same system issues with project (SDK error)

## Tools 
Tools used in this project is:
- ASP.NET Core Web API – Backend framework for building RESTful services
- Entity Framework Core – ORM for database access and relationship handling
- SQL Server – Relational database for storing bookings, clients, services, etc.
- DTO Pattern – Cleanly separates API models from EF entities
- LINQ with Include / ThenInclude – Eager-loads related data efficiently
- Swagger / Swashbuckle – API documentation and testing interface
- Visual Studio / Visual Studio Code – Development environment
