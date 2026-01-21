# ECommerce Platform

A modern, enterprise-grade e-commerce API built with **.NET 10**, designed with clean architecture principles and microservices-ready patterns. This Single-Vendor platform provides comprehensive product management, shopping cart functionality, order processing, and customer management capabilities.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Development Workflow](#development-workflow)
- [Docker Deployment](#docker-deployment)
- [Core Entities](#core-entities)
- [API Endpoints](#api-endpoints)
- [Testing Strategy](#testing-strategy)
- [Roadmap](#roadmap)
- [About the Developer](#about-the-developer)
- [Related Projects](#related-projects)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

---

## 🎯 Overview

The **ECommerce Platform** is a fully-featured e-commerce backend application built following **Clean Architecture** and **Domain-Driven Design (DDD)** principles. It implements industry-standard patterns including MediatR for command/query handling, Entity Framework Core for data persistence, and JWT for authentication.

The platform supports:
- **Product Catalog Management** - Organize products by categories with detailed information
- **Shopping Cart Management** - Redis-backed shopping cart with hybrid caching
- **Order Processing** - Complete order lifecycle from creation to fulfillment
- **Customer Management** - User profiles with authentication and authorization
- **Address Management** - Multiple shipping addresses per customer
- **Billing & Invoicing** - PDF invoice generation with QuestPDF
- **Audit Trail** - Automatic tracking of entity changes with timestamps and user information
- **Performance Monitoring** - Structured logging with Serilog and Seq
- **Real-time Notifications** - Domain event architecture for event-driven capabilities

---

## ✨ Key Features

### 1. **Clean Architecture**
- Separation of concerns across 4 distinct layers (API, Application, Domain, Infrastructure)
- Dependency inversion and dependency injection throughout
- Easy to test and maintain

### 2. **Advanced Caching Strategy**
- **Hybrid Caching**: Three-level caching (L1: Local, L2/L3: Distributed Redis)
- Automatic cache invalidation based on domain events
- Configurable TTL (Time To Live) for different cache regions

### 3. **Database Features**
- **Entity Framework Core 10** for ORM
- **SQL Server 2022** as primary database
- **Interceptors** for automatic audit trail and soft deletes
- **Migrations** support for schema versioning

### 4. **Authentication & Authorization**
- **JWT Bearer Token** authentication
- **Role-Based Access Control (RBAC)** with built-in Admin policy
- **Password Requirements**: Configurable security policies
- **ASP.NET Core Identity** integration

### 5. **API Versioning**
- **API v1 & v2** support with URL segment versioning
- Version-specific OpenAPI documentation
- Backward compatibility support

### 6. **API Documentation**
- **Scalar UI** - Modern, interactive API documentation viewer
- **OpenAPI/Swagger** - Auto-generated API specifications
- **Full endpoint documentation** with request/response examples

### 7. **Logging & Monitoring**
- **Serilog** - Structured, semantic logging
- **Seq** - Centralized log aggregation and visualization
- **Real-time log streaming** during development and production

### 8. **Business Logic**
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Declarative validation rules
- **Domain Events** - Event-driven architecture for complex scenarios
- **Result Pattern** - Functional error handling without exceptions

### 9. **Production-Ready**
- **Docker Containerization** - Complete Docker Compose setup
- **Error Handling** - Comprehensive exception and problem detail responses
- **CORS Support** - Configurable cross-origin resource sharing
- **Output Caching** - Response caching for GET endpoints

---

## 🏗️ Architecture

### Layered Architecture Diagram

```
┌─────────────────────────────────────────┐
│   ECommerce.API (Presentation Layer)    │
│   - Controllers, OpenAPI, Versioning    │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│  ECommerce.Application (Business Logic) │
│  - Commands, Queries, Validators,       │
│  - MediatR Pipeline Behaviors           │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│   ECommerce.Domain (Business Entities)  │
│   - Aggregates, Value Objects, Errors   │
│   - Domain Events                       │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│ ECommerce.Infrastructure (Data Access)  │
│ - DbContext, Identity, Services         │
│ - Caching, Logging, Authentication      │
└─────────────────────────────────────────┘
```

### Design Patterns Used

| Pattern | Implementation | Purpose |
|---------|---|---|
| **CQRS** | MediatR Commands & Queries | Separate read/write operations |
| **Repository** | DbContext with IAppDbContext | Data access abstraction |
| **Factory** | Static Create() methods | Entity creation with validation |
| **Mediator** | MediatR | Decoupled request handling |
| **Pipeline** | MediatR Behaviors | Cross-cutting concerns |
| **Domain Events** | INotification + INotificationHandler | Event-driven architecture |
| **Result Pattern** | Result<T> | Functional error handling |
| **DDD** | Aggregates & Value Objects | Rich domain modeling |

---

## 🛠️ Tech Stack

### Backend Framework
- **.NET 10** - Latest .NET framework
- **ASP.NET Core 10** - Web API framework
- **Entity Framework Core 10** - ORM

### Data & Caching
- **SQL Server 2022** - Relational database
- **Redis (Alpine)** - Distributed cache
- **Hybrid Caching** - Multi-level caching strategy

### Libraries & Packages
- **MediatR 14.0.0** - CQRS and Mediator pattern
- **FluentValidation 12.1.1** - Validation framework
- **Serilog 10.0.0** - Structured logging
- **Seq 2024** - Log aggregation & analysis
- **QuestPDF 2025.12.1** - PDF generation
- **Scalar.AspNetCore 2.12.4** - Interactive API docs
- **Asp.Versioning 8.1.1** - API versioning

### Authentication & Security
- **ASP.NET Core Identity** - User management
- **JWT Bearer** - Token-based authentication
- **CORS** - Cross-Origin Resource Sharing

### Development Tools
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration

---

## 📂 Project Structure

```
ECommerce/
├── src/
│   ├── ECommerce.API/                          # Presentation Layer
│   │   ├── Controllers/
│   │   │   ├── AddressesController.cs          # Address management endpoints
│   │   │   ├── BasketsController.cs            # Shopping cart endpoints
│   │   │   ├── CategoriesController.cs         # Product categories
│   │   │   ├── DashboardController.cs          # Analytics/Dashboard
│   │   │   ├── IdentityController.cs           # Authentication/Authorization
│   │   │   ├── OrdersController.cs             # Order management
│   │   │   ├── ProductsController.cs           # Product catalog
│   │   │   └── ApiController.cs                # Base controller class
│   │   ├── Contracts/                          # Request/Response DTOs
│   │   ├── OpenApi/Transformers/               # API documentation customization
│   │   ├── Services/
│   │   │   └── CurrentUser.cs                  # Current user context
│   │   ├── DependencyInjection.cs              # Service registration
│   │   ├── Program.cs                          # Application entry point
│   │   ├── appsettings.json                    # Configuration
│   │   └── appsettings.Development.json        # Development settings
│   │
│   ├── ECommerce.Application/                  # Business Logic Layer
│   │   ├── Features/
│   │   │   ├── Baskets/                        # Shopping cart logic
│   │   │   ├── Billing/                        # Invoice generation
│   │   │   ├── Categories/                     # Category management
│   │   │   ├── Customers/                      # Customer operations
│   │   │   ├── Dashboard/                      # Analytics queries
│   │   │   ├── Identity/                       # Auth operations
│   │   │   ├── Orders/                         # Order processing
│   │   │   ├── ProductItems/                   # Product variants
│   │   │   └── Reviews/                        # Customer reviews
│   │   ├── Common/
│   │   │   ├── Behaviors/                      # MediatR pipeline behaviors
│   │   │   │   ├── CachingBehavior.cs
│   │   │   │   ├── LoggingBehavior.cs
│   │   │   │   ├── PerformanceBehavior.cs
│   │   │   │   ├── TransactionBehavior.cs
│   │   │   │   ├── UnhandledExceptionBehavior.cs
│   │   │   │   └── ValidationBehavior.cs
│   │   │   ├── Errors/                         # Application error definitions
│   │   │   └── Interfaces/                     # Service contracts
│   │   ├── DependencyInjection.cs
│   │   └── ECommerce.Application.csproj
│   │
│   ├── ECommerce.Domain/                       # Domain Model Layer
│   │   ├── Baskets/
│   │   │   ├── BasketItem.cs                   # Shopping cart item
│   │   │   ├── CustomerBasket.cs               # Shopping cart aggregate
│   │   │   └── BasketErrors.cs                 # Basket error definitions
│   │   ├── Categories/
│   │   │   ├── Category.cs                     # Category aggregate
│   │   │   └── CategoryError.cs
│   │   ├── Customers/
│   │   │   ├── Customer.cs                     # Customer aggregate root
│   │   │   ├── Address.cs                      # Customer address value object
│   │   │   ├── CustomerError.cs
│   │   │   ├── Items/                          # Customer review items
│   │   │   └── Reviews/
│   │   ├── Orders/                             # Order aggregate
│   │   ├── Identity/                           # Auth domain models
│   │   ├── Common/
│   │   │   ├── AuditableEntity.cs              # Base with audit trail
│   │   │   ├── DomainEvent.cs                  # Domain event base class
│   │   │   ├── Entity.cs                       # Base entity class
│   │   │   ├── Constants/                      # Domain constants
│   │   │   └── Results/                        # Result<T> pattern
│   │   └── ECommerce.Domain.csproj
│   │
│   └── ECommerce.Infrastructure/               # Data Access Layer
│       ├── Data/
│       │   ├── AppDbContext.cs                 # Entity Framework DbContext
│       │   ├── ApplicationDbContextInitializer.cs
│       │   ├── Configurations/                 # EF entity mappings
│       │   └── Interceptors/                   # EF interceptors for auditing
│       ├── Identity/
│       │   ├── AppUser.cs                      # Identity user model
│       │   ├── IdentityService.cs              # User management service
│       │   └── TokenProvider.cs                # JWT token generation
│       ├── Services/
│       │   ├── BasketService.cs                # Basket operations
│       │   └── [Other domain services]
│       ├── Settings/                           # Configuration objects
│       ├── DependencyInjection.cs
│       └── ECommerce.Infrastructure.csproj
│
├── requests/
│   └── requests.http                           # HTTP test requests
├── docker-compose.yml                          # Docker services definition
├── Dockerfile                                  # Container image definition
├── ECommerce.sln                               # Solution file
└── README.md                                   # This file
```

---

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

### Required
- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download)
- **SQL Server 2022** - [Download](https://www.microsoft.com/sql-server/sql-server-downloads) or use Docker
- **Redis** - [Download](https://redis.io/download) or use Docker
- **Git** - [Download](https://git-scm.com/download)

### Optional (Recommended)
- **Docker & Docker Compose** - For containerized local development
- **Visual Studio 2024** or **VS Code** with C# extension
- **SQL Server Management Studio (SSMS)** - For database management
- **Postman or REST Client extension** - For API testing

### System Requirements
- **OS**: Windows 10/11, Linux, or macOS
- **RAM**: Minimum 8 GB (16 GB recommended)
- **Disk Space**: At least 5 GB free
- **CPU**: Intel/AMD processor with SSE2 support

---

## 🚀 Getting Started

### Option 1: Using Docker (Recommended)

The simplest way to get the entire application stack running:

```bash
# Clone the repository
git clone https://github.com/yourusername/ecommerce.git
cd ecommerce

# Start all services (API, SQL Server, Redis, Seq)
docker-compose up -d

# Verify services are running
docker-compose ps

# Check API health
curl http://localhost:5050/health

# Access the API documentation
# Open: http://localhost:5050/scalar/v1
```

**What starts with Docker Compose:**
- `ecommerce-api` - API on port 5050 (internal 8080)
- `ecommerce-sql` - SQL Server 2022 on port 1433
- `ecommerce-redis` - Redis on port 6379
- `ecommerce-seq` - Seq logging on port 8081 (UI) & 5341 (ingestion)

---

### Option 2: Local Development

#### Step 1: Clone the Repository

```bash
git clone https://github.com/MuTeach0/ECommerce.git
cd ECommerce
```

#### Step 2: Configure Connection Strings

Edit `src/ECommerce.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ECommerceDb;User Id=sa;Password=YourStrong@Password1;TrustServerCertificate=True;MultipleActiveResultSets=True;",
    "Redis": "localhost:6379"
  }
}
```

#### Step 3: Start Infrastructure Services

Using Docker (recommended):

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Password1" \
  -p 1433:1433 --name ecommerce-sql \
  mcr.microsoft.com/mssql/server:2022-latest

docker run -d -p 6379:6379 --name ecommerce-redis redis:alpine

docker run -d -e ACCEPT_EULA=Y -p 5341:5341 -p 8081:80 --name ecommerce-seq datalust/seq:latest
```

Or use Docker Compose for all infrastructure:

```bash
docker-compose up -d sqlserver redis seq
```

#### Step 4: Build the Solution

```bash
dotnet build ECommerce.sln
```

#### Step 5: Apply Database Migrations

```bash
cd src/ECommerce.API

# Apply all pending migrations
dotnet ef database update

# Or create migration from scratch (if no migrations exist)
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### Step 6: Run the Application

```bash
cd src/ECommerce.API
dotnet run

# or use watch mode for development
dotnet watch run
```

The application will start on `https://localhost:5001` or `http://localhost:5000`

#### Step 7: Access the Application

- **API Documentation**: https://localhost:5001/scalar/v1
- **OpenAPI JSON**: https://localhost:5001/openapi/v2.json
- **Seq Logging**: http://localhost:8081

---

## 🗄️ Database Setup

### Automatic Database Initialization

The application automatically initializes the database on first run (in Development environment):

```csharp
// In Program.cs
if (app.Environment.IsDevelopment())
{
    await app.InitializeDatabaseAsync();
}
```

This will:
1. Create the database if it doesn't exist
2. Apply all pending migrations
3. Seed initial data (if configured)

### Manual Database Operations

```bash
# Add a new migration
dotnet ef migrations add YourMigrationName -p src/ECommerce.Infrastructure

# Update database to latest migration
dotnet ef database update

# Revert to previous migration
dotnet ef database update PreviousMigrationName

# List all migrations
dotnet ef migrations list

# Remove the last migration
dotnet ef migrations remove
```

### Database Schema Overview

The application uses Entity Framework Core with the following main tables:

| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| **Customers** | Customer profiles | Id, Name, Email, PhoneNumber |
| **Products** | Product catalog | Id, Name, Description, Price, CategoryId |
| **Categories** | Product categories | Id, Name, Description |
| **Orders** | Customer orders | Id, CustomerId, OrderDate, TotalPrice |
| **OrderItems** | Order line items | Id, OrderId, ProductId, Quantity, UnitPrice |
| **Baskets** | Shopping carts | Id (Redis), Items |
| **Addresses** | Customer addresses | Id, CustomerId, Title, City, Street |
| **Reviews** | Product reviews | Id, CustomerId, ProductId, Rating, Comment |
| **AspNetUsers** | Identity users | Id, UserName, Email, PasswordHash |
| **AspNetRoles** | User roles | Id, Name |

---

## ⚙️ Configuration

### Environment Configuration Files

The application uses ASP.NET Core configuration hierarchy:

```
appsettings.json (base)
└── appsettings.{Environment}.json (override)
    ├── appsettings.Development.json
    ├── appsettings.Staging.json
    └── appsettings.Production.json
```

### Key Configuration Sections

#### ConnectionStrings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=ECommerceDb;...",
    "Redis": "localhost:6379"
  }
}
```

#### JwtSettings
```json
{
  "JwtSettings": {
    "Secret": "your-secret-key-min-32-chars",
    "TokenExpirationInMinutes": 60,
    "Issuer": "ECommerceApi",
    "Audience": "ECommerceUsers"
  }
}
```

#### Serilog
```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "Seq", "Args": { "serverUrl": "http://localhost:5341" } }
    ]
  }
}
```

#### AppSettings
```json
{
  "AppSettings": {
    "CorsPolicyName": "ECommercePolicy",
    "AllowedOrigins": [
      "https://localhost:7001",
      "http://localhost:5001"
    ]
  }
}
```

### Environment Variables

For production deployments, use environment variables:

```bash
# Database
export ConnectionStrings__DefaultConnection="Server=...;"

# JWT
export JwtSettings__Secret="your-secret-key"
export JwtSettings__TokenExpirationInMinutes="60"

# Logging
export Serilog__WriteTo__1__Args__serverUrl="http://seq:5341"

# Environment
export ASPNETCORE_ENVIRONMENT="Production"
```

---

## 📚 API Documentation

### Interactive Documentation (Scalar UI)

Once the application is running, visit:

```
https://localhost:5001/scalar/v1
```

This provides an interactive, user-friendly interface to:
- Browse all available endpoints
- View request/response schemas
- Test API calls directly from the browser
- Copy code samples in various languages

### OpenAPI Specification

Raw OpenAPI 3.0 specification available at:

```
GET https://localhost:5001/openapi/v2.json
GET https://localhost:5001/openapi/v1.json
```

### API Versioning

The API supports versioning through URL segments:

```
GET /api/v2/products          # Version 2 (Current)
GET /api/v1/products          # Version 1 (Legacy)
```

Headers automatically included:
```
api-supported-versions: 1.0, 2.0
api-deprecated-versions: 1.0
```

---

## 🔌 API Endpoints

### Authentication (Identity)

```http
POST /api/v2/identity/register
POST /api/v2/identity/login
POST /api/v2/identity/refresh
POST /api/v2/identity/logout
```

### Products

```http
GET    /api/v2/products              # List all products (paginated)
GET    /api/v2/products/{id}         # Get product details
POST   /api/v2/products              # Create new product (Admin)
PUT    /api/v2/products/{id}         # Update product (Admin)
DELETE /api/v2/products/{id}         # Delete product (Admin)
```

### Categories

```http
GET    /api/v2/categories             # List all categories
GET    /api/v2/categories/{id}        # Get category details
POST   /api/v2/categories             # Create category (Admin)
PUT    /api/v2/categories/{id}        # Update category (Admin)
DELETE /api/v2/categories/{id}        # Delete category (Admin)
```

### Shopping Cart (Baskets)

```http
GET    /api/v2/baskets                # Get current user's basket
POST   /api/v2/baskets/add            # Add item to basket
POST   /api/v2/baskets/remove         # Remove item from basket
DELETE /api/v2/baskets                # Clear basket
```

### Orders

```http
GET    /api/v2/orders                 # List user's orders
GET    /api/v2/orders/{id}            # Get order details
POST   /api/v2/orders                 # Create new order
PUT    /api/v2/orders/{id}            # Update order (Admin)
GET    /api/v2/orders/{id}/invoice    # Download invoice (PDF)
```

### Customers

```http
GET    /api/v2/customers              # List customers (Admin)
GET    /api/v2/customers/{id}         # Get customer profile
PUT    /api/v2/customers/{id}         # Update profile
DELETE /api/v2/customers/{id}         # Delete account
```

### Addresses

```http
GET    /api/v2/addresses              # List user's addresses
POST   /api/v2/addresses              # Add new address
PUT    /api/v2/addresses/{id}         # Update address
DELETE /api/v2/addresses/{id}         # Delete address
```

### Dashboard (Analytics)

```http
GET    /api/v2/dashboard/stats        # Get sales statistics (Admin)
GET    /api/v2/dashboard/revenue      # Get revenue report (Admin)
GET    /api/v2/dashboard/top-products # Get top-selling products
```

---

## 💻 Development Workflow

### Build Tasks

Available tasks in VS Code/Visual Studio:

```bash
# Build the solution
dotnet build

# Run tests (when test projects are added)
dotnet test

# Watch mode (rebuild on file change)
dotnet watch run

# Format code
dotnet format

# Publish for release
dotnet publish -c Release
```

Or use VS Code tasks:

```json
{
  "label": "build",
  "command": "dotnet",
  "args": ["build"],
  "problemMatcher": ["$msCompile"]
},
{
  "label": "watch",
  "command": "dotnet",
  "args": ["watch", "run"],
  "isBackground": true
}
```

### Code Style & Standards

The project uses:
- **C# 13** latest features
- **Nullable reference types** enabled
- **Implicit usings** enabled
- **StyleCop Analyzers** (via .editorconfig)

Configuration in `.editorconfig` or `EditorConfig` plugin

### Adding New Features

#### 1. Create Domain Entity
```csharp
// src/ECommerce.Domain/YourFeature/YourEntity.cs
public sealed class YourEntity : AuditableEntity
{
    // Domain logic
}
```

#### 2. Create Application Layer
```csharp
// src/ECommerce.Application/Features/YourFeature/Commands/CreateCommand.cs
public sealed class CreateYourEntityCommand : IRequest<Result<YourEntityDto>>
{
    // CQRS command
}

public sealed class CreateYourEntityCommandHandler : 
    IRequestHandler<CreateYourEntityCommand, Result<YourEntityDto>>
{
    // Business logic
}
```

#### 3. Add Validation
```csharp
// src/ECommerce.Application/Features/YourFeature/Validators/CreateValidator.cs
public sealed class CreateYourEntityValidator : 
    AbstractValidator<CreateYourEntityCommand>
{
    public CreateYourEntityValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
```

#### 4. Create Controller Endpoint
```csharp
// src/ECommerce.API/Controllers/YourFeatureController.cs
[ApiController]
[Route("api/v{version:apiVersion}/your-features")]
[ApiVersion("2.0")]
public class YourFeatureController : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Match(Ok, Problem);
    }
}
```

### Example Test Structure

Here's how to structure your tests following the project's testing strategy:

```csharp
// Domain Unit Test
public class CustomerCreationTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        // Act
        var result = Customer.Create(id, "Ahmed", "01234567890", "ahmed@example.com");
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}

// Application Command Handler Test
public class CreateOrderCommandTests
{
    [Fact]
    public async Task Handle_WithValidCommand_CreatesOrder()
    {
        // Arrange
        var command = new CreateOrderCommand { /* ... */ };
        var handler = new CreateOrderCommandHandler(/* ... */);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
    }
}
```

---

## 🐳 Docker Deployment

### Docker Compose Services

The `docker-compose.yml` defines 4 services:

#### 1. SQL Server
```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports:
      - "1433:1433"
```

#### 2. API Application
```yaml
  ecommerce-api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5050:8080"
```

#### 3. Redis Cache
```yaml
  redis:
    image: redis:alpine
    ports:
      - "6379:6379"
```

#### 4. Seq Logging
```yaml
  seq:
    image: datalust/seq:latest
    ports:
      - "8081:80"
      - "5341:5341"
```

### Building Docker Image

```bash
# Build the image
docker build -t ecommerce-api:latest .

# Build with specific .NET version
docker build -t ecommerce-api:net10 --build-arg DOTNET_VERSION=10.0 .

# Tag for registry
docker tag ecommerce-api:latest registry.example.com/ecommerce-api:latest

# Push to registry
docker push registry.example.com/ecommerce-api:latest
```

### Running Individual Services

```bash
# Start only database
docker-compose up -d sqlserver

# Start specific services
docker-compose up -d sqlserver redis

# Scale services (if applicable)
docker-compose up -d --scale api=3

# View logs
docker-compose logs -f ecommerce-api

# Stop services
docker-compose down
```

### Production Deployment

For production, create a `docker-compose.prod.yml`:

```yaml
version: '3.8'
services:
  ecommerce-api:
    image: registry.example.com/ecommerce-api:latest
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: ${DB_CONNECTION_STRING}
      JwtSettings__Secret: ${JWT_SECRET}
    ports:
      - "80:8080"
    restart: unless-stopped
```

Deploy with:
```bash
docker-compose -f docker-compose.prod.yml up -d
```

---

## 🔐 Core Entities

### Customer (Aggregate Root)

```csharp
public sealed class Customer : AuditableEntity
{
    public string Name { get; }
    public string PhoneNumber { get; }
    public string Email { get; }
    public IReadOnlyCollection<Order> Orders { get; }
    public IReadOnlyCollection<Address> Addresses { get; }
}
```

**Operations:**
- Create customer with validation
- Add multiple shipping addresses
- View order history
- Manage profile information

### Product (Aggregate)

```csharp
public sealed class Product : AuditableEntity
{
    public string Name { get; }
    public string Description { get; }
    public decimal Price { get; }
    public int Stock { get; }
    public Guid CategoryId { get; }
    public Category Category { get; }
}
```

### Order (Aggregate Root)

```csharp
public sealed class Order : AuditableEntity
{
    public Guid CustomerId { get; }
    public DateTime OrderDate { get; }
    public decimal TotalPrice { get; }
    public OrderStatus Status { get; }
    public IReadOnlyCollection<OrderItem> Items { get; }
}
```

### Shopping Basket (Non-Persistent)

```csharp
public sealed class CustomerBasket
{
    public string Id { get; }
    public List<BasketItem> Items { get; set; }
    
    public void AddOrUpdateItem(BasketItem item) { }
    public void RemoveItem(Guid productId) { }
    public void Clear() { }
}
```

Stored in Redis for high performance.

---

## 🧩 MediatR Pipeline Behaviors

The application uses MediatR behaviors for cross-cutting concerns:

### 1. Validation Behavior
Automatically validates all commands before execution using FluentValidation rules.

### 2. Logging Behavior
Logs all requests and responses for audit trail and debugging.

### 3. Performance Behavior
Monitors query execution time and warns if slow (> 3 seconds).

### 4. Caching Behavior
Automatically caches query results based on attributes.

### 5. Transaction Behavior
Wraps command handlers in database transactions.

### 6. Exception Handling Behavior
Catches and formats unhandled exceptions into consistent responses.

---

## 🔍 Troubleshooting

### Common Issues

#### 1. Database Connection Failed

**Error:** `Connection Timeout Exception`

**Solution:**
```bash
# Check if SQL Server is running
docker-compose ps

# Verify connection string
# Default: Server=localhost,1433;User Id=sa;Password=YourStrong@Password1;

# Test connection
sqlcmd -S localhost,1433 -U sa -P YourStrong@Password1
```

#### 2. Port Already in Use

**Error:** `Address already in use`

**Solution:**
```bash
# Find process using port 5050
netstat -ano | findstr :5050  # Windows
lsof -i :5050                 # Linux/Mac

# Kill process (Windows)
taskkill /PID <PID> /F

# Or change port in docker-compose.yml
ports:
  - "5051:8080"  # Changed from 5050
```

#### 3. Redis Connection Issues

**Error:** `Could not connect to redis`

**Solution:**
```bash
# Check Redis is running
redis-cli ping

# Restart Redis service
docker restart ecommerce-redis

# Verify connection string
# Should be: localhost:6379
```

#### 4. Migrations Not Applied

**Error:** `Table does not exist`

**Solution:**
```bash
# Check pending migrations
dotnet ef migrations list

# Apply migrations
dotnet ef database update

# Reset database (Development only!)
dotnet ef database drop
dotnet ef database update
```

#### 5. JWT Token Invalid

**Error:** `401 Unauthorized`

**Solution:**
```bash
# Verify JWT Secret in appsettings.json
# Must be at least 32 characters

# Check token expiration
# Default: 60 minutes

# Verify token format
Authorization: Bearer <your-jwt-token>
```

### Debug Mode

Enable detailed logging:

```bash
# Set log level
export Serilog__MinimumLevel=Debug

# Or in appsettings.Development.json
{
  "Serilog": {
    "MinimumLevel": "Debug"
  }
}
```

View logs in Seq:
```
http://localhost:8081
```

---

## 📊 Performance Optimization Tips

### 1. Caching Strategy

```csharp
// L1: Local cache (30 seconds)
// L2/L3: Redis (10 minutes)
[Cacheable(Duration = 600)]
public class GetProductQuery : IRequest<ProductDto> { }
```

### 2. Database Indexing

Ensure proper indexes on frequently queried fields:
- ProductId in OrderItems
- CustomerId in Orders
- CategoryId in Products

### 3. Asynchronous Operations

Always use async/await:
```csharp
var result = await _mediator.Send(query);
```

### 4. Pagination

Implement pagination for large result sets:
```http
GET /api/v2/products?pageNumber=1&pageSize=20
```

### 5. Output Caching

Cache HTTP responses:
```csharp
[OutputCache(Duration = 60)]
public async Task<IActionResult> GetProducts() { }
```

---

## 🤝 Contributing

1. **Fork** the repository: https://github.com/MuTeach0/ECommerce
2. **Create** a feature branch: `git checkout -b feature/amazing-feature`
3. **Commit** your changes: `git commit -m 'Add amazing feature'`
4. **Push** to the branch: `git push origin feature/amazing-feature`
5. **Open** a Pull Request

### Code Guidelines

- Follow C# naming conventions (PascalCase for public members)
- Add XML documentation comments for public APIs
- Write unit tests for new features
- Keep commits small and focused
- Update README if adding new features

---

## 📝 License

This project is licensed under the MIT License. See the LICENSE file for details.

---

## 📞 Support

For issues, questions, or feature requests:

- **Issues**: [GitHub Issues](https://github.com/MuTeach0/ECommerce/issues)
- **Discussions**: [GitHub Discussions](https://github.com/MuTeach0/ECommerce/discussions)
- **Email**: mahmoud.mutech@gmail.com

---

## 🙏 Acknowledgments

- Built with modern .NET 10 best practices
- Inspired by clean architecture and DDD principles
- Thanks to the open-source community for amazing libraries

---

## 👨‍💻 About the Developer

**Mahmoud Ahmed**
- 🔗 **LinkedIn**: [mahmoud-0ahmed](https://www.linkedin.com/in/mahmoud-0ahmed/)
- 📧 **Email**: mahmoud.mutech@gmail.com
- 🐙 **GitHub**: [MuTeach0](https://github.com/MuTeach0)

Passionate about building scalable, enterprise-grade applications using modern .NET technologies and clean architecture principles.

---

## 🗺️ Roadmap

Upcoming features and improvements for future releases:

### Phase 2 (Q2 2026)
- **Inventory Reservation System**
  - Temporary reservation mechanism for items in shopping cart
  - Automatic release after TTL (Time To Live)
  - Prevent overselling and stock depletion
  - Real-time inventory synchronization

- **Enhanced Basket Features**
  - Persistent basket across sessions
  - Basket sharing functionality
  - Wishlist feature
  - Cart abandonment recovery

### Phase 3 (Q3 2026)
- **Multi-Vendor Support** - Upgrade from Single-Vendor to Multi-Vendor marketplace
- **Payment Gateway Integration** - Stripe, PayPal support
- **Notification System** - Email, SMS, Push notifications
- **Advanced Analytics** - Custom reports and dashboards

### Phase 4 (Q4 2026)
- **Mobile App** - Native iOS/Android applications
- **GraphQL API** - Alongside REST API
- **Microservices Migration** - Separate services for Orders, Inventory, Payments
- **AI-Powered Recommendations** - Machine learning product recommendations

---

## 🧪 Testing Strategy

The project implements a comprehensive testing pyramid with multiple test projects:

### Test Projects Structure

```
tests/
├── ECommerce.Domain.UnitTests/
│   └── Domain entities and value objects
├── ECommerce.Application.UnitTests/
│   └── Commands, Queries, and business logic
├── ECommerce.Application.SubcutaneousTests/
│   └── Application layer with in-memory database
├── ECommerce.API.IntegrationTests/
│   └── Full API stack with real database
└── ECommerce.Tests.Common/
    ├── Fixtures and test builders
    ├── Common test utilities
    └── Mocking helpers
```

### Test Types

#### Unit Tests (Domain & Application)
```bash
cd tests/ECommerce.Domain.UnitTests
dotnet test
```
Focus on isolated business logic without external dependencies.

#### Subcutaneous Tests
```bash
cd tests/ECommerce.Application.SubcutaneousTests
dotnet test
```
Test Application layer with in-memory database, focusing on command/query handlers.

#### Integration Tests
```bash
cd tests/ECommerce.API.IntegrationTests
dotnet test
```
Test full API endpoints with real database and services.

#### Run All Tests
```bash
dotnet test ECommerce.sln
```

### Test Coverage Goals
- **Domain Layer**: 90%+ coverage
- **Application Layer**: 85%+ coverage
- **API Layer**: 70%+ coverage (critical endpoints)

---

## 📚 Related Projects

Other projects using similar architecture and technologies:

- Coming soon... 🔜

---

**Last Updated**: January 2026  
**Version**: 2.0.0 (Single-Vendor)  
**Status**: Active Development 🚀
