# WebForms to Blazor Web App Migration Plan

*eShopLegacy — .NET Framework 4.8 → .NET 8 Blazor Web App (Interactive Server)*

---

## Table of Contents

- [1. Executive Summary](#1-executive-summary)
- [2. Migration Strategy](#2-migration-strategy)
- [3. Detailed Dependency Analysis](#3-detailed-dependency-analysis)
- [4. Project Migration Plan: eShopLegacy](#4-project-migration-plan-eshoplegacy)
  - [4.1 Current State](#41-current-state)
  - [4.2 Target State](#42-target-state)
  - [4.3 Phase 0: Prerequisites](#43-phase-0-prerequisites)
  - [4.4 Phase 1: Project Conversion & Infrastructure](#44-phase-1-project-conversion--infrastructure)
  - [4.5 Phase 2: Data Access Migration](#45-phase-2-data-access-migration)
  - [4.6 Phase 3: Authentication Migration](#46-phase-3-authentication-migration)
  - [4.7 Phase 4: UI Layer Migration](#47-phase-4-ui-layer-migration)
  - [4.8 Package Update Reference](#48-package-update-reference)
  - [4.9 Breaking Changes Catalog](#49-breaking-changes-catalog)
- [5. Risk Management](#5-risk-management)
- [6. Testing & Validation Strategy](#6-testing--validation-strategy)
- [7. Complexity & Effort Assessment](#7-complexity--effort-assessment)
- [8. Source Control Strategy](#8-source-control-strategy)
- [9. Success Criteria](#9-success-criteria)

---

## 1. Executive Summary

### Scenario Description

Migrate the **eShopLegacy** e-commerce application from ASP.NET WebForms on .NET Framework 4.8 to a **Blazor Web App on .NET 8** with Interactive Server render mode, preserving all existing functionality.

### Scope

| Dimension | Value |
|---|---|
| Solution | `eShopLegacy.sln` |
| Projects affected | 1 (`eShopLegacy`) |
| Current framework | .NET Framework 4.8, C# 7.3 |
| Target framework | .NET 8, C# 12 |
| Current UI | ASP.NET WebForms (10 pages + 1 master layout) |
| Target UI | Blazor Web App — Interactive Server render mode |
| Current data access | Entity Framework 6.4.4 (synchronous, Code-First) |
| Target data access | EF Core 8.0.25 (async/await, Code-First + Migrations) |
| Current auth | OWIN 4.2.2 + ASP.NET Identity 2.2.3 |
| Target auth | ASP.NET Core Identity 8.0.25 |
| Current config | `Web.config` (XML) |
| Target config | `appsettings.json` (JSON) |

### Selected Strategy

**All-at-Once Strategy** — All migration work performed in a single coordinated operation.

**Rationale**:
- Single project — no cross-project coordination needed
- Well-structured codebase with no circular dependencies
- Domain models are largely portable with minimal changes
- All 5 critical issues have clear, well-understood .NET 8 replacement paths
- No intermediate-state constraints (no consuming projects)

### Complexity Assessment

**Classification: Medium**

| Metric | Value |
|---|---|
| Projects | 1 |
| WebForms pages | 10 |
| Master layouts | 1 |
| DAL service classes | 3 |
| Domain model classes | 8 |
| NuGet packages (current) | 9 |
| Critical issues | 5 |
| High-priority issues | 5 |
| Circular dependencies | 0 |
| Existing test projects | 0 |

**Justification**: Single project eliminates cross-project coordination concerns, but the migration requires 5 critical infrastructure replacements (WebForms runtime, EF6, OWIN, ASP.NET Identity 2.x, non-SDK project format) plus a full UI rewrite of 10 pages — exceeds "Simple" but stays within "Medium" bounds given clean architecture and direct replacement paths.

### Critical Issues Summary

| # | Issue | Severity | Resolution |
|---|---|---|---|
| 1 | `System.Web` / WebForms not on .NET Core | Critical | Rewrite all UI as Blazor components |
| 2 | Entity Framework 6 not on .NET Core | Critical | Migrate to EF Core 8 |
| 3 | OWIN/Katana not on .NET Core | Critical | Replace with ASP.NET Core middleware |
| 4 | ASP.NET Identity 2.x not on .NET Core | Critical | Migrate to ASP.NET Core Identity 8 |
| 5 | Non-SDK-style project format | Critical | Convert to SDK-style `.csproj` |

### Recommended Approach

Organize work into logical phases (bottom-up: infrastructure → data → auth → UI) executed as a **single coordinated operation**. The solution compiles only after all phases are complete. No intermediate compilable project state exists.

---

## 2. Migration Strategy

### Approach Selection

**All-at-Once** migration with **phase-based organization** for human clarity.

Since `eShopLegacy` is a single project, there is no project-ordering concern. Work is organized into logical phases that respect the internal layer dependency order:

```
Domain Models (portable) → EF Core Context → ASP.NET Core Identity → DAL Services → Blazor UI
```

Each layer depends on the layers below it. Migration proceeds bottom-up within the single project.

### Dependency-Based Ordering Rationale

1. **Domain Models first** — Models use only `System.ComponentModel.DataAnnotations` (available on .NET 8). All other layers depend on models.
2. **EF Core Context second** — `eShopContext` depends on models and Identity; must be migrated before services can compile.
3. **ASP.NET Core Identity third** — `ApplicationUser` and Identity DI configuration depend on models and the EF Core context.
4. **DAL Services fourth** — `CatalogService`, `BasketService`, `OrderService` depend on `eShopContext` and models.
5. **UI Layer last** — All 10 Blazor components depend on all layers below.

### Execution Philosophy

- **Bottom-up migration**: Lower layers migrated before upper layers
- **Single build target**: No multi-targeting; project transitions directly to `net8.0`
- **Remove then replace**: Framework-only packages removed; .NET 8 equivalents added atomically
- **All compilation errors fixed in one pass** before final build verification
- **No intermediate compilable state** — the project is not expected to compile until all phases are complete

### Phase Definitions

| Phase | Scope | Depends On |
|---|---|---|
| Phase 0 | Prerequisites (SDK verification, upgrade branch) | None |
| Phase 1 | SDK-style `.csproj`, NuGet packages, `Program.cs`, `appsettings.json`, Blazor root files, static assets | None |
| Phase 2 | EF Core 8 context, async DAL services, EF Core migrations, database seeder | Phase 1 |
| Phase 3 | ASP.NET Core Identity 8, `ApplicationUser` update, DI configuration | Phase 2 |
| Phase 4 | `MainLayout.razor`, all 10 Blazor page components, `_Imports.razor` | Phase 3 |

---

## 3. Detailed Dependency Analysis

### Dependency Graph

```
eShopLegacy (single project — no cross-project dependencies)
│
├── Domain Layer (portable — no System.Web dependencies)
│   ├── Models/CatalogItem.cs
│   ├── Models/CatalogBrand.cs
│   ├── Models/CatalogType.cs
│   ├── Models/Basket.cs
│   ├── Models/BasketItem.cs
│   ├── Models/Order.cs
│   ├── Models/OrderItem.cs
│   └── Models/Address.cs
│
├── Identity Layer (replace namespace + remove OWIN method)
│   └── Models/ApplicationUser.cs  →  IdentityUser (ASP.NET Core Identity 8)
│
├── Data Access Layer (migrate EF6 → EF Core 8, sync → async)
│   ├── DAL/eShopContext.cs         →  EF Core 8 DbContext
│   ├── DAL/CatalogService.cs       →  async/await + EF Core 8 APIs
│   ├── DAL/BasketService.cs        →  async/await + EF Core 8 APIs
│   ├── DAL/OrderService.cs         →  async/await + EF Core 8 APIs
│   └── DAL/DatabaseInitializer.cs  →  DELETE → replaced by EF Core Migrations + DatabaseSeeder.cs
│
├── Application Startup (replace entirely)
│   ├── Global.asax.cs              →  Program.cs
│   ├── Startup.cs                  →  Program.cs (delete OwinStartup approach)
│   └── App_Start/Startup.Auth.cs   →  Program.cs (AddAuthentication/AddIdentity)
│
└── UI Layer (full rewrite as Blazor components)
    ├── Site.Master + Site.Master.cs         →  Components/Layout/MainLayout.razor
    ├── Default.aspx + .cs                  →  Components/Pages/Home.razor
    ├── Catalog/Default.aspx + .cs          →  Components/Pages/Catalog/Catalog.razor
    ├── Catalog/ProductDetail.aspx + .cs    →  Components/Pages/Catalog/ProductDetail.razor
    ├── Cart/ShoppingCart.aspx + .cs        →  Components/Pages/Cart/ShoppingCart.razor
    ├── Checkout/Checkout.aspx + .cs        →  Components/Pages/Checkout/Checkout.razor
    ├── Checkout/OrderComplete.aspx + .cs   →  Components/Pages/Checkout/OrderComplete.razor
    ├── Checkout/OrderHistory.aspx + .cs    →  Components/Pages/Checkout/OrderHistory.razor
    ├── Account/Login.aspx + .cs            →  Components/Pages/Account/Login.razor
    ├── Account/Register.aspx + .cs         →  Components/Pages/Account/Register.razor
    └── Admin/Products.aspx + .cs           →  Components/Pages/Admin/Products.razor
```

### Critical Path

```
SDK Conversion (.csproj + Program.cs)
    → EF Core Context (eShopContext.cs)
        → ASP.NET Core Identity (ApplicationUser.cs + DI)
            → DAL Services (async CatalogService, BasketService, OrderService)
                → Blazor UI (MainLayout + 10 components)
                    → Build Verification (dotnet build → 0 errors)
                        → Smoke Testing (18 manual checks)
```

### Circular Dependencies

**None.** The codebase has a clean layered architecture with no circular references.

### Cross-Project Dependencies

**None.** Single-project solution.

---

## 4. Project Migration Plan: eShopLegacy

### 4.1 Current State

| Dimension | Value |
|---|---|
| Framework | .NET Framework 4.8 (C# 7.3) |
| Project type | Non-SDK-style WebForms project (`packages.config`) |
| UI | ASP.NET WebForms — 10 `.aspx` pages + 1 `.master` layout |
| Data access | Entity Framework 6.4.4 — Code-First, synchronous, `System.Data.Entity` |
| Authentication | OWIN 4.2.2 + ASP.NET Identity 2.2.3 (`IAppBuilder`, `OwinStartup`) |
| Configuration | `Web.config` (XML: connection strings, appSettings, system.web) |
| DI container | None — services instantiated manually with `new` |
| Static assets | `/Content/` (CSS), `/Scripts/` (JS + jQuery 3.6.0) |
| Database | SQL Server LocalDB (`eShopLegacy.mdf`) via `CreateDatabaseIfNotExists` |
| Routing | No custom routes — pages accessed via `.aspx` file paths |

### 4.2 Target State

| Dimension | Value |
|---|---|
| Framework | .NET 8 (C# 12) |
| Project type | SDK-style Blazor Web App (`<Project Sdk="Microsoft.NET.Sdk.Web">`) |
| UI | Blazor Web App — Interactive Server render mode, 10 `.razor` components + `MainLayout.razor` |
| Data access | EF Core 8.0.25 — Code-First, async/await, `Microsoft.EntityFrameworkCore` |
| Authentication | ASP.NET Core Identity 8.0.25 (cookie auth via `AddIdentity` + `ConfigureApplicationCookie`) |
| Configuration | `appsettings.json` (JSON) + `IConfiguration` |
| DI container | ASP.NET Core built-in DI — scoped services injected via `@inject` |
| Static assets | `wwwroot/` (css/, js/, images/) — jQuery removed |
| Database | SQL Server LocalDB — migrated schema via EF Core Migrations |
| Routing | Blazor `@page` directives on each component |

### 4.3 Phase 0: Prerequisites

**Objective**: Ensure the development environment is ready before any migration work begins.

#### Step 1: Verify .NET 8 SDK

Confirm .NET 8 SDK is installed:

```
dotnet --list-sdks
```

Expected: An `8.0.x` entry is present. If absent, install from: https://dotnet.microsoft.com/download/dotnet/8.0

#### Step 2: Verify EF Core CLI Tools

Confirm `dotnet-ef` global tool is available:

```
dotnet ef --version
```

If not installed, run:

```
dotnet tool install --global dotnet-ef --version 8.*
```

#### Step 3: Create Upgrade Branch

Create and switch to the dedicated migration branch from `main`:

```
git checkout main
git checkout -b upgrade/webforms-to-blazor
```

> This branch has already been created as part of plan preparation.

### 4.4 Phase 1: Project Conversion & Infrastructure

**Objective**: Convert the non-SDK project to SDK-style targeting `net8.0`, replace all NuGet packages, create `Program.cs` (replacing `Global.asax.cs` + both `Startup` files), create `appsettings.json` (replacing `Web.config`), scaffold Blazor root files, and move static assets to `wwwroot/`.

#### Step 1: Convert `.csproj` to SDK-Style

Replace the entire content of `eShopLegacy\eShopLegacy.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>eShopLegacy</RootNamespace>
    <AssemblyName>eShopLegacy</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.25" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.25" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.25" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.25">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

</Project>
```

> ⚠️ **Verify at execution time**: Confirm `8.0.25` is the latest `8.0.x` patch available for all packages. All four packages must use matching `8.0.x` versions to avoid EF Core / Identity compatibility issues.

#### Step 2: Delete Legacy Files

Delete the following files — they have no equivalent in .NET 8:

**Project & package management**:
- `eShopLegacy\packages.config`

**Configuration** (replaced by `appsettings.json`):
- `eShopLegacy\Web.config`

**Startup & middleware** (replaced by `Program.cs`):
- `eShopLegacy\Global.asax`
- `eShopLegacy\Global.asax.cs`
- `eShopLegacy\Startup.cs`
- `eShopLegacy\App_Start\Startup.Auth.cs`
- `eShopLegacy\App_Start\IdentityConfig.cs`
- `eShopLegacy\App_Start\RouteConfig.cs`

**WebForms UI** (replaced by Blazor components):
- All `*.aspx`, `*.aspx.cs`, `*.aspx.designer.cs` files
- `eShopLegacy\Site.Master`
- `eShopLegacy\Site.Master.cs`
- `eShopLegacy\Site.Master.designer.cs`

**EF6 initializer** (replaced by EF Core Migrations + `DatabaseSeeder.cs`):
- `eShopLegacy\DAL\DatabaseInitializer.cs`

#### Step 3: Create `appsettings.json`

Create `eShopLegacy\appsettings.json`:

```json
{
  "ConnectionStrings": {
    "eShopContext": "Server=(localdb)\\mssqllocaldb;Database=eShopLegacy;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "CatalogItemsPerPage": 8,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

> Note: The original `Web.config` used `AttachDbFilename=|DataDirectory|\eShopLegacy.mdf` which is not supported in .NET Core. The new connection string uses a named database (`eShopLegacy`) on LocalDB instead — EF Core Migrations will create it automatically.

#### Step 4: Create `Program.cs`

Create `eShopLegacy\Program.cs` (consolidates `Global.asax.cs`, `Startup.cs`, `Startup.Auth.cs`, `IdentityConfig.cs`):

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using eShopLegacy.DAL;
using eShopLegacy.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("eShopContext")
    ?? throw new InvalidOperationException("Connection string 'eShopContext' not found.");
builder.Services.AddDbContext<eShopContext>(options =>
    options.UseSqlServer(connectionString));

// ── Identity ─────────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = null; // matches AllowOnlyAlphanumericUserNames = false
})
.AddEntityFrameworkStores<eShopContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(2880);
    options.SlidingExpiration = true;
});

// ── DAL Services ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<BasketService>();
builder.Services.AddScoped<OrderService>();

// ── Session (anonymous basket) ───────────────────────────────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// ── Blazor ───────────────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// ── Database migration & seeding ─────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<eShopContext>();
    context.Database.Migrate();
    await DatabaseSeeder.SeedAsync(context);
}

app.MapRazorComponents<eShopLegacy.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

#### Step 5: Create Blazor Root Files

Create `eShopLegacy\Components\App.razor`:

```razor
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <link rel="stylesheet" href="css/bootstrap.min.css" />
    <link rel="stylesheet" href="css/site.css" />
    <HeadOutlet />
</head>
<body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
    <script src="js/bootstrap.bundle.min.js"></script>
</body>
</html>
```

Create `eShopLegacy\Components\Routes.razor`:

```razor
<Router AppAssembly="typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(MainLayout)" />
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
    <NotFound>
        <LayoutView Layout="typeof(MainLayout)">
            <p role="alert">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
```

Create `eShopLegacy\Components\_Imports.razor`:

```razor
@using System.Net.Http
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Identity
@using eShopLegacy.Components
@using eShopLegacy.Components.Layout
@using eShopLegacy.DAL
@using eShopLegacy.Models
```

#### Step 6: Move Static Assets to `wwwroot/`

Move all static files from legacy virtual directories to `eShopLegacy\wwwroot\`:

| Source Path | Destination Path |
|---|---|
| `Content\bootstrap.min.css` | `wwwroot\css\bootstrap.min.css` |
| `Content\site.css` | `wwwroot\css\site.css` |
| `Content\eshop-logo.svg` | `wwwroot\images\eshop-logo.svg` |
| `Content\placeholder.png` | `wwwroot\images\placeholder.png` |
| `Scripts\bootstrap.bundle.min.js` | `wwwroot\js\bootstrap.bundle.min.js` |
| `images\products\*.png` | `wwwroot\images\products\*.png` |

> Delete source directories `/Content/` and `/Scripts/` after copying.
> **Remove jQuery** (`Scripts\jquery-3.6.0.min.js`) — not needed in Blazor Server.

### 4.5 Phase 2: Data Access Migration

**Objective**: Migrate `eShopContext` from EF6 to EF Core 8, fix Fluent API breaking changes, convert all DAL services to async/await, replace `DatabaseInitializer` with EF Core Migrations and a `DatabaseSeeder`, and create the initial migration.

#### Step 1: Update `eShopContext.cs`

**Namespace replacements**:

| Remove | Add |
|---|---|
| `using System.Data.Entity;` | `using Microsoft.EntityFrameworkCore;` |
| `using Microsoft.AspNet.Identity.EntityFramework;` | `using Microsoft.AspNetCore.Identity.EntityFrameworkCore;` |

**Constructor change**: Remove the `base("eShopContext")` call and `Configuration.LazyLoadingEnabled = false`. EF Core reads the connection string from DI-injected `DbContextOptions` and has lazy loading disabled by default.

```csharp
public eShopContext(DbContextOptions<eShopContext> options) : base(options) { }
```

**Remove** the static `Create()` factory — context is provided via DI.

**`OnModelCreating` — Fluent API changes** (replace EF6 with EF Core 8 equivalents):

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<CatalogItem>()
        .HasOne(c => c.CatalogBrand)
        .WithMany()
        .HasForeignKey(c => c.CatalogBrandId)
        .OnDelete(DeleteBehavior.NoAction);  // was: WillCascadeOnDelete(false)

    modelBuilder.Entity<CatalogItem>()
        .HasOne(c => c.CatalogType)          // was: HasRequired(...)
        .WithMany()
        .HasForeignKey(c => c.CatalogTypeId)
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<BasketItem>()
        .HasOne(b => b.Basket)               // was: HasRequired(...)
        .WithMany(b => b.Items)
        .HasForeignKey(b => b.BasketId);

    modelBuilder.Entity<OrderItem>()
        .HasOne(o => o.Order)                // was: HasRequired(...)
        .WithMany(o => o.OrderItems)
        .HasForeignKey(o => o.OrderId);
}
```

**EF6 → EF Core Fluent API mapping**:

| EF6 | EF Core 8 |
|---|---|
| `HasRequired(x => x.Nav)` | `HasOne(x => x.Nav)` |
| `.WillCascadeOnDelete(false)` | `.OnDelete(DeleteBehavior.NoAction)` |
| `DbModelBuilder` parameter type | `ModelBuilder` parameter type |
| `Configuration.LazyLoadingEnabled = false` | Remove (disabled by default) |

#### Step 2: Create `DatabaseSeeder.cs`

Create `eShopLegacy\DAL\DatabaseSeeder.cs` to replace `DatabaseInitializer.cs`:

```csharp
namespace eShopLegacy.DAL
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(eShopContext context)
        {
            if (await context.CatalogBrands.AnyAsync()) return; // Already seeded

            // Port exact seed data from DatabaseInitializer.Seed():
            // - 5 CatalogBrands (Azure, .NET, Visual Studio, SQL Server, Other)
            // - 4 CatalogTypes (Mug, T-Shirt, Sheet, USB Memory Stick)
            // - 12 CatalogItems (with Name, Description, Price, PictureUri, etc.)
        }
    }
}
```

> Port the complete seed data from `DatabaseInitializer.cs` (5 brands, 4 types, 12 catalog items with all property values).

#### Step 3: Create EF Core Initial Migration

After `eShopContext.cs` is updated, create the initial migration:

```
dotnet ef migrations add InitialCreate --project eShopLegacy
dotnet ef database update --project eShopLegacy
```

This creates `eShopLegacy\Migrations\` with the initial schema. Commit the generated migration files.

#### Step 4: Convert DAL Services to Async

Update all three service classes (`CatalogService.cs`, `BasketService.cs`, `OrderService.cs`):

**Namespace replacement** (all files):
- Remove: `using System.Data.Entity;`
- Add: `using Microsoft.EntityFrameworkCore;`

**Constructor change**: Services receive `eShopContext` via DI constructor injection (same pattern as before — no change needed structurally).

**Synchronous → Async method mapping** (apply to all EF operations):

| EF6 Sync | EF Core 8 Async |
|---|---|
| `.ToList()` | `await .ToListAsync()` |
| `.FirstOrDefault(x => ...)` | `await .FirstOrDefaultAsync(x => ...)` |
| `.Find(id)` | `await .FindAsync(id)` |
| `.Count()` | `await .CountAsync()` |
| `.Any()` | `await .AnyAsync()` |
| `.SaveChanges()` | `await context.SaveChangesAsync()` |

**`CatalogService` special case — `out` parameter removal**:

The `GetCatalogItems(...)` method uses `out int totalItems`, which is incompatible with async. Replace with a return tuple:

```csharp
// Before (EF6, sync):
public List<CatalogItem> GetCatalogItems(int pageIndex, int pageSize, int? brandId, int? typeId, string searchText, out int totalItems)

// After (EF Core 8, async):
public async Task<(List<CatalogItem> Items, int TotalItems)> GetCatalogItemsAsync(int pageIndex, int pageSize, int? brandId, int? typeId, string? searchText)
```

Update all call sites in Blazor components accordingly.

**All service method signatures after conversion**:

`CatalogService`:
- `GetCatalogItemsAsync(...)` → `Task<(List<CatalogItem> Items, int TotalItems)>`
- `GetCatalogItemAsync(int id)` → `Task<CatalogItem?>`
- `GetCatalogBrandsAsync()` → `Task<List<CatalogBrand>>`
- `GetCatalogTypesAsync()` → `Task<List<CatalogType>>`

`BasketService`:
- `GetOrCreateBasketAsync(string buyerId)` → `Task<Basket>`
- `GetBasketAsync(string buyerId)` → `Task<Basket?>`
- `AddItemToBasketAsync(...)` → `Task`
- `UpdateBasketItemAsync(int basketItemId, int quantity)` → `Task`
- `RemoveItemFromBasketAsync(int basketItemId)` → `Task`
- `TransferBasketAsync(string anonymousBuyerId, string userId)` → `Task`

`OrderService`:
- `CreateOrderFromBasketAsync(...)` → `Task<Order>`
- `GetOrdersForBuyerAsync(string buyerId)` → `Task<List<Order>>`
- `GetOrderAsync(int orderId)` → `Task<Order?>`

### 4.6 Phase 3: Authentication Migration

**Objective**: Replace OWIN/ASP.NET Identity 2.x with ASP.NET Core Identity 8. Update `ApplicationUser`, update `eShopContext` base class namespace, and confirm Identity DI configuration in `Program.cs` (already established in Phase 1).

#### Step 1: Update `ApplicationUser.cs`

**Namespace replacement**:
- Remove: `using Microsoft.AspNet.Identity;`, `using Microsoft.AspNet.Identity.EntityFramework;`, `using System.Security.Claims;`, `using System.Threading.Tasks;`
- Add: `using Microsoft.AspNetCore.Identity;`

**Remove** `GenerateUserIdentityAsync()` method — OWIN-specific, not used in ASP.NET Core Identity. `SignInManager<ApplicationUser>` handles identity generation internally.

**Make extended properties nullable** (aligns with C# 12 nullable reference types):

```csharp
using Microsoft.AspNetCore.Identity;

namespace eShopLegacy.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public int CardTypeId { get; set; }
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public string? CardExpiration { get; set; }
    }
}
```

> ⚠️ `CardNumber`, `CardHolderName`, `CardExpiration` are carried forward for feature parity. These fields should be reviewed for security compliance in a follow-up task.

#### Step 2: Update `eShopContext.cs` Base Class Namespace

The `eShopContext` inherits from `IdentityDbContext<ApplicationUser>`. The class name is the same in both Identity 2.x and ASP.NET Core Identity, but the namespace differs:

- Remove: `using Microsoft.AspNet.Identity.EntityFramework;`
- Add: `using Microsoft.AspNetCore.Identity.EntityFrameworkCore;`

No other changes needed to the base class declaration.

#### Step 3: Confirm Identity Configuration in `Program.cs`

Identity is already configured in `Program.cs` (Phase 1, Step 4). Key differences from OWIN/Identity 2.x:

| Old (OWIN + Identity 2.x) | New (ASP.NET Core Identity 8) |
|---|---|
| `[assembly: OwinStartup(typeof(Startup))]` | Deleted — `Program.cs` is the entry point |
| `IAppBuilder app` | `WebApplicationBuilder builder` |
| `IdentityConfig.CreateUserManager()` | `UserManager<ApplicationUser>` injected via DI |
| `app.CreatePerOwinContext(...)` | `builder.Services.AddScoped<...>()` |
| `app.UseCookieAuthentication(new CookieAuthenticationOptions { LoginPath = ... })` | `builder.Services.ConfigureApplicationCookie(options => { options.LoginPath = "/Account/Login"; ... })` |
| `Context.GetOwinContext().Authentication.SignIn(props, identity)` | `await signInManager.SignInAsync(user, isPersistent)` |
| `Context.GetOwinContext().Authentication.SignOut()` | `await signInManager.SignOutAsync()` |
| `DefaultAuthenticationTypes.ApplicationCookie` | Handled internally by `AddIdentity` |
| `SecurityStampValidator.OnValidateIdentity(...)` | Built-in via `AddIdentity` + `AddDefaultTokenProviders()` |
| `PasswordValidator` / `UserValidator` options | `options.Password.*` and `options.User.*` lambda in `AddIdentity(options => { ... })` |

### 4.7 Phase 4: UI Layer Migration

**Objective**: Rewrite `Site.Master` as `MainLayout.razor` and all 10 WebForms pages as Blazor Interactive Server components. Register services in DI (done in Phase 1) and use `@inject` in components.

#### Common WebForms → Blazor Pattern Mappings

These patterns apply to all components:

| WebForms | Blazor |
|---|---|
| `Page_Load(object sender, EventArgs e)` | `OnInitializedAsync()` |
| `IsPostBack` check | Remove — no postback model |
| `Response.Redirect("~/path/Page.aspx")` | `NavigationManager.NavigateTo("/path/page")` |
| `Request.QueryString["key"]` | `[SupplyParameterFromQuery(Name = "key")]` property |
| `ViewState["Key"] = value` | `private T _field;` (component field) |
| `lblText.Text = "value"` | `@_textValue` (bound field) |
| `pnlSection.Visible = bool` | `@if (condition) { ... }` |
| `rptItems DataBind()` | `@foreach (var item in _items)` |
| `btnAction_Click(sender, e)` | `async Task ActionAsync()` called via `@onclick` |
| `Page.IsValid` | `EditForm` manages validity via `DataAnnotationsValidator` |
| `Page.Title` | `<PageTitle>` component |
| `User.Identity.IsAuthenticated` | `[Authorize]` attribute or `<AuthorizeView>` |
| `User.Identity.Name` | `AuthState.User.Identity!.Name` from `AuthenticationStateProvider` |

---

#### Step 1: Create `MainLayout.razor` (replaces `Site.Master`)

Create `eShopLegacy\Components\Layout\MainLayout.razor`:

```razor
@inherits LayoutComponentBase
@inject NavigationManager Navigation
@inject SignInManager<ApplicationUser> SignInManager

<nav class="navbar navbar-expand-lg navbar-dark bg-dark eshop-navbar">
    <div class="container">
        <a class="navbar-brand" href="/">
            <img src="/images/eshop-logo.svg" height="32" alt="" class="me-2"
                 onerror="this.style.display='none'" />
            eShop
        </a>
        <!-- navbar collapse button -->
        <div class="collapse navbar-collapse" id="mainNav">
            <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                <li class="nav-item">
                    <a class="nav-link" href="/Catalog">Catalog</a>
                </li>
            </ul>
            <ul class="navbar-nav ms-auto mb-2 mb-lg-0 align-items-center">
                <li class="nav-item me-3">
                    <a class="nav-link" href="/Cart/ShoppingCart">
                        <!-- cart icon SVG (carry forward from Site.Master) -->
                    </a>
                </li>
                <AuthorizeView>
                    <Authorized>
                        <li class="nav-item dropdown">
                            <a class="nav-link dropdown-toggle" href="#">
                                Welcome, @context.User.Identity!.Name
                            </a>
                            <ul class="dropdown-menu dropdown-menu-end">
                                <li><a class="dropdown-item" href="/Checkout/OrderHistory">My Orders</a></li>
                                <li><hr class="dropdown-divider" /></li>
                                <li><button class="dropdown-item" @onclick="SignOutAsync">Sign Out</button></li>
                            </ul>
                        </li>
                    </Authorized>
                    <NotAuthorized>
                        <li class="nav-item"><a class="nav-link" href="/Account/Login">Sign In</a></li>
                        <li class="nav-item"><a class="nav-link" href="/Account/Register">Register</a></li>
                    </NotAuthorized>
                </AuthorizeView>
            </ul>
        </div>
    </div>
</nav>

<main class="container my-4">
    @Body
</main>

<footer class="footer bg-dark text-white py-3 mt-auto">
    <div class="container text-center">
        <small>eShop on .NET 8 &mdash; Blazor Web App</small>
    </div>
</footer>

@code {
    private async Task SignOutAsync()
    {
        await SignInManager.SignOutAsync();
        Navigation.NavigateTo("/", forceLoad: true);
    }
}
```

**Key mappings from `Site.Master`**:

| Site.Master | MainLayout.razor |
|---|---|
| `<asp:ContentPlaceHolder ID="MainContent">` | `@Body` |
| `<asp:LoginView>` with templates | `<AuthorizeView>` with `<Authorized>` / `<NotAuthorized>` |
| `<asp:LoginName>` | `context.User.Identity!.Name` inside `<AuthorizeView>` |
| `<asp:LinkButton OnClick="btnSignOut_Click">` | `<button @onclick="SignOutAsync">` |
| `<asp:ScriptManager>` | Remove — not needed in Blazor |
| `<asp:Label ID="lblCartCount">` | Cart count logic moved to `ShoppingCart.razor` or a cart state service |
| `btnSignOut_Click` calling OWIN SignOut | `await SignInManager.SignOutAsync()` + `Navigation.NavigateTo("/", forceLoad: true)` |

---

#### Step 2: `Home.razor` (replaces `Default.aspx`)

- **File**: `eShopLegacy\Components\Pages\Home.razor`
- **Route**: `@page "/"`
- **Render mode**: `@rendermode InteractiveServer`
- **Inject**: None required
- **Functionality**: Static welcome page with a link to the Catalog. No code-behind logic needed.

---

#### Step 3: `Catalog.razor` (replaces `Catalog/Default.aspx`)

- **File**: `eShopLegacy\Components\Pages\Catalog\Catalog.razor`
- **Route**: `@page "/Catalog"`
- **Inject**: `CatalogService`, `NavigationManager`
- **Query string parameters** (replace `Request.QueryString`):

```razor
[SupplyParameterFromQuery(Name = "page")]  public int Page  { get; set; }
[SupplyParameterFromQuery(Name = "brand")] public int Brand { get; set; }
[SupplyParameterFromQuery(Name = "type")]  public int Type  { get; set; }
[SupplyParameterFromQuery(Name = "q")]     public string? Q { get; set; }
```

**Key mappings**:

| WebForms | Blazor |
|---|---|
| `ddlBrand` DropDownList | `<select @bind="_selectedBrand" @bind:after="ApplyFilter">` |
| `ddlType` DropDownList | `<select @bind="_selectedType" @bind:after="ApplyFilter">` |
| `txtSearch` TextBox | `<input @bind="_search" />` |
| `rptProducts` Repeater | `@foreach (var item in _items)` |
| `pnlEmpty` Panel | `@if (!_items.Any())` |
| `pnlPager` Panel | `@if (_totalPages > 1)` |
| `btnPrev` / `btnNext` HyperLink | `<a href="@BuildUrl(Page - 1)">` |
| `BuildUrl(page)` method | `NavigationManager.GetUriWithQueryParameters(...)` |
| `BindFilters()` + `BindProducts()` in `Page_Load` | Single `OnInitializedAsync()` + `OnParametersSetAsync()` |
| `svc.GetCatalogItems(..., out total)` | `var (items, total) = await CatalogService.GetCatalogItemsAsync(...)` |

> `OnParametersSetAsync` should re-fetch data when query parameters change (page, brand, type, q).

---

#### Step 4: `ProductDetail.razor` (replaces `Catalog/ProductDetail.aspx`)

- **File**: `eShopLegacy\Components\Pages\Catalog\ProductDetail.razor`
- **Route**: `@page "/Catalog/ProductDetail"`
- **Inject**: `CatalogService`, `BasketService`, `NavigationManager`, `AuthenticationStateProvider`
- **Query parameter**: `[SupplyParameterFromQuery(Name = "id")] public int Id { get; set; }`

**Key mappings**:

| WebForms | Blazor |
|---|---|
| `Request.QueryString["id"]` | `[SupplyParameterFromQuery(Name = "id")]` |
| `ViewState["ProductId"] = item.Id` | `private int _productId;` |
| `ViewState["Price"] = item.Price` | `private decimal _price;` |
| `btnAddToCart_Click` | `async Task AddToCartAsync()` |
| `ResolveUrl("~/Content/placeholder.png")` | `/images/placeholder.png` |
| `lblBreadcrumb`, `lblName`, etc. | `@_item.Name`, `@_item.Price.ToString("0.00")`, etc. |
| `ShowNotFound()` | `NavigationManager.NavigateTo("/")` |

---

#### Step 5: `ShoppingCart.razor` (replaces `Cart/ShoppingCart.aspx`)

- **File**: `eShopLegacy\Components\Pages\Cart\ShoppingCart.razor`
- **Route**: `@page "/Cart/ShoppingCart"`
- **Inject**: `BasketService`, `NavigationManager`, `IHttpContextAccessor`, `AuthenticationStateProvider`

**Key mappings**:

| WebForms | Blazor |
|---|---|
| `rptCart` Repeater | `@foreach (var item in _items)` |
| `rptCart_ItemCommand` with `CommandName` | Separate `@onclick` handlers: `RemoveAsync(itemId)`, `IncrementAsync(itemId)`, `DecrementAsync(itemId)` |
| `GetBuyerId()` via `Session["BuyerId"]` | `GetBuyerId()` via `IHttpContextAccessor.HttpContext.Session` + cookie |
| `btnCheckout_Click` | `async Task ProceedToCheckoutAsync()` |
| `pnlEmpty` / `pnlCart` visibility | `@if (_items.Count == 0)` / `@else` |

> **Anonymous basket**: Access `HttpContext.Session` via `IHttpContextAccessor` in `OnInitializedAsync`. Session is available during the initial Blazor Server request. Store the anonymous buyer ID as `Session.SetString("BuyerId", Guid.NewGuid().ToString())`.

---

#### Step 6: `Checkout.razor` (replaces `Checkout/Checkout.aspx`)

- **File**: `eShopLegacy\Components\Pages\Checkout\Checkout.razor`
- **Route**: `@page "/Checkout/Checkout"`
- **Attribute**: `[Authorize]`
- **Inject**: `BasketService`, `OrderService`, `NavigationManager`, `AuthenticationStateProvider`, `UserManager<ApplicationUser>`

**Key mappings**:

| WebForms | Blazor |
|---|---|
| `if (!User.Identity.IsAuthenticated) Response.Redirect(...)` | `[Authorize]` attribute (redirects automatically) |
| `PreFillFromProfile()` in `Page_Load` | Load user address in `OnInitializedAsync` via `UserManager.FindByNameAsync(username)` |
| `rptSummary` Repeater | `@foreach (var item in _summaryItems)` |
| `Page.IsValid` | `<EditForm>` with `<DataAnnotationsValidator>` |
| `btnPlaceOrder_Click` | `async Task PlaceOrderAsync()` |
| `DateTime.TryParseExact("01/" + txtExpiry.Text, "dd/MM/yy", ...)` | Same logic in `PlaceOrderAsync` |
| `Response.Redirect("~/Cart/ShoppingCart.aspx")` | `NavigationManager.NavigateTo("/Cart/ShoppingCart")` |
| Post-order redirect | `NavigationManager.NavigateTo($"/Checkout/OrderComplete?orderId={order.Id}")` |

---

#### Step 7: `OrderComplete.razor` (replaces `Checkout/OrderComplete.aspx`)

- **File**: `eShopLegacy\Components\Pages\Checkout\OrderComplete.razor`
- **Route**: `@page "/Checkout/OrderComplete"`
- **Attribute**: `[Authorize]`
- **Inject**: `OrderService`, `NavigationManager`
- **Query parameter**: `[SupplyParameterFromQuery(Name = "orderId")] public int OrderId { get; set; }`
- **Functionality**: Display confirmation page with order ID and summary loaded via `OrderService.GetOrderAsync(OrderId)`.

---

#### Step 8: `OrderHistory.razor` (replaces `Checkout/OrderHistory.aspx`)

- **File**: `eShopLegacy\Components\Pages\Checkout\OrderHistory.razor`
- **Route**: `@page "/Checkout/OrderHistory"`
- **Attribute**: `[Authorize]`
- **Inject**: `OrderService`, `AuthenticationStateProvider`
- **Key mapping**: `rptOrders` Repeater → `@foreach` over `await OrderService.GetOrdersForBuyerAsync(userId)` in `OnInitializedAsync`.

---

#### Step 9: `Login.razor` (replaces `Account/Login.aspx`)

- **File**: `eShopLegacy\Components\Pages\Account\Login.razor`
- **Route**: `@page "/Account/Login"`
- **Inject**: `SignInManager<ApplicationUser>`, `UserManager<ApplicationUser>`, `NavigationManager`, `BasketService`, `IHttpContextAccessor`
- **Query parameter**: `[SupplyParameterFromQuery(Name = "ReturnUrl")] public string? ReturnUrl { get; set; }`

**Key mappings**:

| WebForms | Blazor |
|---|---|
| `Page.IsValid` | `<EditForm>` + `<DataAnnotationsValidator>` |
| `manager.Find(email, password)` | `await UserManager.FindByEmailAsync(email)` + `await SignInManager.CheckPasswordSignInAsync(user, password, false)` |
| `Context.GetOwinContext().Authentication.SignIn(props, identity)` | `await SignInManager.SignInAsync(user, isPersistent)` |
| `Request.Cookies["remember_email"]` | `IHttpContextAccessor.HttpContext.Request.Cookies["remember_email"]` |
| `Response.Cookies.Set(cookie)` | `IHttpContextAccessor.HttpContext.Response.Cookies.Append(...)` |
| `TransferAnonymousBasket(userId)` | `await BasketService.TransferBasketAsync(anonymousId, userId)` |
| `Response.Redirect(returnUrl ?? "~/")` | `NavigationManager.NavigateTo(ReturnUrl ?? "/", forceLoad: true)` |

> `forceLoad: true` is needed after `SignInAsync` so the browser reloads and picks up the new authentication cookie.

---

#### Step 10: `Register.razor` (replaces `Account/Register.aspx`)

- **File**: `eShopLegacy\Components\Pages\Account\Register.razor`
- **Route**: `@page "/Account/Register"`
- **Inject**: `UserManager<ApplicationUser>`, `SignInManager<ApplicationUser>`, `NavigationManager`

**Key mappings**:

| WebForms | Blazor |
|---|---|
| `manager.Create(user, password)` | `await UserManager.CreateAsync(user, password)` |
| `manager.CreateIdentity(...)` + `authManager.SignIn(...)` | `await SignInManager.SignInAsync(user, isPersistent: false)` |
| `IdentityResult.Errors` | `result.Errors` (same concept, `IdentityError` type in Core Identity) |
| Error display | Iterate `result.Errors` and display `error.Description` |
| `Page.IsValid` | `<EditForm>` + `<DataAnnotationsValidator>` |

---

#### Step 11: `Admin/Products.razor` (replaces `Admin/Products.aspx`)

- **File**: `eShopLegacy\Components\Pages\Admin\Products.razor`
- **Route**: `@page "/Admin/Products"`
- **Attribute**: `[Authorize(Roles = "Admin")]` *(improved over original — was only checking `IsAuthenticated`)*
- **Inject**: `CatalogService`

**Key mappings**:

| WebForms | Blazor |
|---|---|
| `GridView` (`gvProducts`) | `@foreach` + HTML `<table>` |
| `btnShowAdd_Click` | `@onclick="ShowAddForm"` |
| `btnCancel_Click` | `@onclick="() => _showForm = false"` |
| `ddlBrand`, `ddlType` | `<select @bind="_selectedBrandId">` |
| `hfEditId` hidden field | `private int _editId;` component field |
| `pnlForm` Panel | `@if (_showForm)` |
| `btnSave_Click` | `async Task SaveProductAsync()` calling `CatalogService` create/update method |

### 4.8 Package Update Reference

#### Packages Being Removed (all from `packages.config`)

| Package | Version | Reason |
|---|---|---|
| `EntityFramework` | 6.4.4 | Not supported on .NET Core; replaced by EF Core 8 |
| `Microsoft.AspNet.Identity.Core` | 2.2.3 | .NET Framework only; replaced by ASP.NET Core Identity 8 |
| `Microsoft.AspNet.Identity.EntityFramework` | 2.2.3 | .NET Framework only; included in Core Identity EF package |
| `Microsoft.AspNet.Identity.Owin` | 2.2.3 | OWIN-specific; not needed in ASP.NET Core |
| `Microsoft.Owin` | 4.2.2 | .NET Framework only; ASP.NET Core has built-in middleware |
| `Microsoft.Owin.Host.SystemWeb` | 4.2.2 | `System.Web` hosting; not applicable on .NET Core |
| `Microsoft.Owin.Security` | 4.2.2 | Replaced by ASP.NET Core authentication middleware |
| `Microsoft.Owin.Security.Cookies` | 4.2.2 | Replaced by `AddIdentity` + `ConfigureApplicationCookie` |
| `Owin` | 1.0 | OWIN abstraction; not needed in ASP.NET Core |

#### Packages Being Added (`<PackageReference>` in SDK-style `.csproj`)

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.25 | EF Core 8 SQL Server provider |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.25 | EF Core CLI tools (migrations) |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.25 | Design-time EF Core services (private assets) |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 8.0.25 | ASP.NET Core Identity 8 with EF Core store |

> ⚠️ All four packages must use matching `8.0.x` versions. `Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.25` is confirmed available. Verify `8.0.25` exists for EF Core packages at execution time; use the latest `8.0.x` patch if needed.

### 4.9 Breaking Changes Catalog

#### EF6 → EF Core 8 API Changes

| EF6 | EF Core 8 | Files Affected |
|---|---|---|
| `using System.Data.Entity;` | `using Microsoft.EntityFrameworkCore;` | All DAL files |
| `using Microsoft.AspNet.Identity.EntityFramework;` | `using Microsoft.AspNetCore.Identity.EntityFrameworkCore;` | `eShopContext.cs` |
| `DbModelBuilder` parameter | `ModelBuilder` parameter | `eShopContext.cs` |
| `.HasRequired(x => x.Nav)` | `.HasOne(x => x.Nav)` | `eShopContext.cs` |
| `.WillCascadeOnDelete(false)` | `.OnDelete(DeleteBehavior.NoAction)` | `eShopContext.cs` |
| `Configuration.LazyLoadingEnabled = false` | Remove (default in EF Core) | `eShopContext.cs` |
| `base("eShopContext")` constructor | `base(DbContextOptions<eShopContext> options)` | `eShopContext.cs` |
| `Database.SetInitializer(...)` | `context.Database.Migrate()` in `Program.cs` | `Program.cs` |
| `CreateDatabaseIfNotExists<T>` | EF Core Migrations + `DatabaseSeeder` | New files |
| Static `Create()` factory | Remove — use DI | `eShopContext.cs` |

#### Synchronous → Async DAL Changes

| Sync (EF6) | Async (EF Core 8) | Notes |
|---|---|---|
| `.ToList()` | `await .ToListAsync()` | All service methods |
| `.FirstOrDefault(x => ...)` | `await .FirstOrDefaultAsync(x => ...)` | All service methods |
| `.Find(id)` | `await .FindAsync(id)` | All service methods |
| `.Count()` | `await .CountAsync()` | `CatalogService` |
| `.Any()` | `await .AnyAsync()` | `OrderService`, `BasketService` |
| `.SaveChanges()` | `await context.SaveChangesAsync()` | All service methods |
| `out int totalItems` param | Return tuple `(List<T> Items, int TotalItems)` | `CatalogService.GetCatalogItemsAsync` |

#### OWIN / ASP.NET Identity 2.x → ASP.NET Core Identity 8

| Old | New | Notes |
|---|---|---|
| `[assembly: OwinStartup]` | Deleted | `Startup.cs` removed |
| `IAppBuilder app` | `WebApplicationBuilder builder` | `Program.cs` |
| `app.CreatePerOwinContext(...)` | `builder.Services.AddScoped<...>()` | DI registration |
| `app.UseCookieAuthentication(...)` | `builder.Services.ConfigureApplicationCookie(...)` | `Program.cs` |
| `Context.GetOwinContext().Authentication.SignIn(...)` | `await signInManager.SignInAsync(user, isPersistent)` | Login component |
| `Context.GetOwinContext().Authentication.SignOut()` | `await signInManager.SignOutAsync()` | Layout, Logout |
| `manager.Find(email, password)` | `await userManager.FindByEmailAsync(email)` + `CheckPasswordSignInAsync` | Login component |
| `manager.Create(user, password)` | `await userManager.CreateAsync(user, password)` | Register component |
| `DefaultAuthenticationTypes.ApplicationCookie` | Handled internally by `AddIdentity` | Remove all references |
| `user.GenerateUserIdentityAsync(manager)` | Remove — handled by `SignInManager` | `ApplicationUser.cs` |
| `PasswordValidator` / `UserValidator` classes | `options.Password.*` / `options.User.*` in `AddIdentity(...)` lambda | `Program.cs` |

#### WebForms → Blazor Pattern Changes

| WebForms | Blazor | Notes |
|---|---|---|
| `Page_Load(object sender, EventArgs e)` | `OnInitializedAsync()` | All components |
| `IsPostBack` | Remove entirely | No postback model |
| `Response.Redirect("~/path")` | `NavigationManager.NavigateTo("/path")` | All components |
| `Request.QueryString["key"]` | `[SupplyParameterFromQuery(Name = "key")]` | Catalog, ProductDetail, Login |
| `ViewState["Key"] = value` | `private T _field;` component field | ProductDetail |
| `lblText.Text = "value"` | `@_value` interpolation | All components |
| `pnlSection.Visible = condition` | `@if (condition) { ... }` | All components |
| `rptItems.DataSource = list; DataBind()` | `@foreach (var item in _items)` | Cart, Catalog, Checkout |
| `gvProducts` GridView | `@foreach` + HTML `<table>` | Admin/Products |
| `ddlX.Items.FindByValue(...)` | `<select @bind="_selectedValue">` | Catalog, Admin |
| `btnAction_Click(sender, e)` | `async Task ActionAsync()` + `@onclick` | All components |
| `Page.IsValid` | `<EditForm>` + `<DataAnnotationsValidator>` | Login, Register, Checkout |
| `Page.Title` | `<PageTitle>Title</PageTitle>` component | All components |
| `Session["BuyerId"]` | `IHttpContextAccessor.HttpContext.Session.GetString("BuyerId")` | ShoppingCart |
| `Request.Cookies["key"]` | `IHttpContextAccessor.HttpContext.Request.Cookies["key"]` | Login |
| `Response.Cookies.Set(cookie)` | `IHttpContextAccessor.HttpContext.Response.Cookies.Append(...)` | Login |
| `<asp:ContentPlaceHolder>` | `@Body` in layout | `MainLayout.razor` |
| `<asp:LoginView>` templates | `<AuthorizeView>` with `<Authorized>` / `<NotAuthorized>` | `MainLayout.razor` |
| `<asp:LoginName>` | `context.User.Identity!.Name` | `MainLayout.razor` |
| `<asp:ScriptManager>` | Remove | Not needed in Blazor |
| `ResolveUrl("~/Content/x.png")` | `"/images/x.png"` | ProductDetail |

---

## 5. Risk Management

### High-Risk Items

| Risk | Level | Description | Mitigation |
|---|---|---|---|
| Full UI rewrite scope (10 pages) | High | All pages must be rewritten from scratch as Blazor components | Pages are small and focused; direct WebForms → Blazor mappings are well-defined above |
| EF Core schema vs EF6 schema | High | EF Core Migrations generate a different schema than EF6's `CreateDatabaseIfNotExists` | Dev-only LocalDB — drop and recreate; re-seed via `DatabaseSeeder` |
| ASP.NET Core Identity schema changes | High | Identity table names/columns differ between Identity 2.x and Core Identity 8 | Acceptable for dev database; production would require a schema migration script |
| Async context in Blazor Server | Medium | `.Result` / `.Wait()` on async calls can deadlock the SignalR circuit | All DAL methods must be async; all components must `await` — enforce during code review |
| `IHttpContextAccessor` in Blazor Server | Medium | `HttpContext` is not reliably available after the initial render cycle | Access session and cookies only in `OnInitializedAsync`; do not access in later event handlers |

### Security Items

| Issue | Description | Action |
|---|---|---|
| Card data on `ApplicationUser` | `CardNumber`, `CardHolderName`, `CardExpiration` stored as plain text | Carry forward for feature parity; mark `⚠️ Requires security review` post-migration |
| Admin page lacks role guard (original) | Original `Admin/Products.aspx` only checks `IsAuthenticated`, not role | **Improvement during migration**: Apply `[Authorize(Roles = "Admin")]` to `Admin/Products.razor` |

### Contingency Plans

| Scenario | Response |
|---|---|
| EF Core migration fails to generate | Inspect `OnModelCreating` for nullable navigation property mismatches; run `dotnet ef migrations add --verbose`; check for missing `?` on optional navigations |
| Identity schema breaks login after migration | Drop dev database (`DROP DATABASE eShopLegacy`); re-run `dotnet ef database update`; restart app to re-seed |
| Anonymous basket session not persisting in Blazor | Verify `UseSession()` is called **before** `MapRazorComponents()` in `Program.cs`; confirm `AddSession()` + `AddDistributedMemoryCache()` are registered |
| Blazor component state lost on SignalR reconnect | Move critical per-user state (anonymous buyer ID) to a scoped service rather than component-level fields; persist to session on write |

---

## 6. Testing & Validation Strategy

> The `eShopLegacy` solution contains **no existing test projects**. Validation relies on build verification and a structured manual smoke test checklist.

### Build Verification (Primary Gate)

After completing all migration phases, run:

```
dotnet build eShopLegacy\eShopLegacy.csproj
```

Expected: **0 errors, 0 warnings**. Resolve all compilation errors before proceeding to smoke testing.

### EF Core Migration Verification

```
dotnet ef database update --project eShopLegacy
```

Expected: Migration applied successfully; `Migrations/` folder committed with `InitialCreate` migration.

### Smoke Test Checklist

Manually verify all flows after a successful `dotnet run`:

| # | Test Case | Expected Result |
|---|---|---|
| 1 | Navigate to `https://localhost:xxxx/` | Home page loads without error |
| 2 | Navigate to `/Catalog` | 8 products per page displayed |
| 3 | Apply brand filter in Catalog | Filtered product list updates |
| 4 | Apply type filter in Catalog | Filtered product list updates |
| 5 | Use search box in Catalog | Matching products appear |
| 6 | Navigate Catalog pages (Prev/Next) | Pagination works |
| 7 | Navigate to `/Catalog/ProductDetail?id=1` | Product detail loads with name, price, description |
| 8 | Click "Add to Cart" (anonymous user) | Item added; cart accessible |
| 9 | Navigate to `/Cart/ShoppingCart` | Cart shows added items with quantities and totals |
| 10 | Increment/Decrement cart item quantity | Quantities update correctly |
| 11 | Remove cart item | Item removed from cart |
| 12 | Navigate to `/Account/Register` | Registration form displays |
| 13 | Register new user | User created; redirected to home; nav shows username |
| 14 | Sign out | Redirected; nav shows Sign In / Register |
| 15 | Navigate to `/Account/Login` | Login form displays |
| 16 | Login with registered credentials | Authenticated; redirected; nav shows username |
| 17 | Navigate to `/Checkout/Checkout` (authenticated) | Checkout form loads with pre-filled address fields |
| 18 | Complete checkout form and place order | Order created; redirected to `/Checkout/OrderComplete` |
| 19 | Navigate to `/Checkout/OrderHistory` | Placed order appears in history |
| 20 | Navigate to `/Admin/Products` (authenticated) | Products admin page loads with product grid |
| 21 | Add new product via admin | Product appears in grid and Catalog |
| 22 | Unauthenticated access to `/Checkout/Checkout` | Redirected to `/Account/Login` |
| 23 | Bootstrap styles and site.css load | Page renders with correct styling |

---

## 7. Complexity & Effort Assessment

### Overall Classification: **Medium**

Single project with clean architecture, but requires 5 critical infrastructure replacements and a full UI rewrite.

### Per-Phase Complexity

| Phase | Complexity | Key Driver |
|---|---|---|
| Phase 0: Prerequisites | Low | SDK/tool verification, branch creation |
| Phase 1: Project Conversion | Medium | SDK-style `.csproj`, `Program.cs`, `appsettings.json`, Blazor root files, asset migration |
| Phase 2: EF Core Migration | Medium | Fluent API changes, sync→async conversion, migrations setup |
| Phase 3: Authentication | Medium | Identity 2.x → Core Identity, DI wiring, OWIN removal |
| Phase 4: UI Layer | High | 10 full component rewrites + layout; largest effort block |

### Component-Level Complexity

| Component | Complexity | Reason |
|---|---|---|
| `Home.razor` | Low | Static page; no data or auth |
| `OrderComplete.razor` | Low | Display-only; query-string param + order lookup |
| `OrderHistory.razor` | Low | Simple authenticated list with `@foreach` |
| `Catalog.razor` | Medium | Query-string params, filtering, pagination, async data binding |
| `ProductDetail.razor` | Medium | ViewState removal, add-to-cart with auth check |
| `ShoppingCart.razor` | Medium | Anonymous session, item CRUD operations |
| `Login.razor` | Medium | EditForm, SignInManager, remember-me cookies, basket transfer |
| `Register.razor` | Medium | EditForm, UserManager, IdentityResult error handling |
| `Admin/Products.razor` | Medium | CRUD form, GridView→table, dropdown binding, auth guard |
| `Checkout.razor` | High | EditForm + validation, address pre-fill, order creation, basket summary |
| `MainLayout.razor` | Medium | AuthorizeView, sign-out handler, nav cart badge |

### Resource Requirements

| Skill Area | Required Level |
|---|---|
| Blazor Web App / Razor Components | Intermediate |
| ASP.NET Core Identity & auth flows | Intermediate |
| EF Core 8 (Fluent API, Migrations) | Intermediate |
| C# async/await patterns | Intermediate |
| Bootstrap 5 (carry-forward) | Basic |

---

## 8. Source Control Strategy

### Branch Strategy

| Branch | Purpose |
|---|---|
| `main` | Starting branch (current) |
| `upgrade/webforms-to-blazor` | Migration branch — all changes committed here |

The `upgrade/webforms-to-blazor` branch has been created from `main` as part of plan preparation.

### Commit Strategy

**All-at-Once: Single Commit** (preferred)

All migration changes (infrastructure, data access, auth, UI) are committed as a single atomic commit once the solution builds with 0 errors and all smoke tests pass:

```
git add -A
git commit -m "feat: migrate eShopLegacy from WebForms (.NET Framework 4.8) to Blazor Web App (.NET 8)

- Convert project to SDK-style targeting net8.0
- Replace EF6 with EF Core 8 (async services, updated Fluent API, Migrations)
- Replace OWIN + ASP.NET Identity 2.x with ASP.NET Core Identity 8
- Replace Global.asax + Web.config with Program.cs + appsettings.json
- Rewrite all 10 WebForms pages as Blazor Interactive Server components
- Replace Site.Master with MainLayout.razor using AuthorizeView
- Move static assets from /Content/ and /Scripts/ to wwwroot/
- Add EF Core initial migration and DatabaseSeeder
- Register DAL services in built-in DI container (scoped)
- Remove jQuery dependency
- Add role-based [Authorize(Roles = \"Admin\")] to Admin page

Resolves: System.Web/WebForms not on .NET Core (Critical)
Resolves: EF6 not on .NET Core (Critical)
Resolves: OWIN/Katana not on .NET Core (Critical)
Resolves: ASP.NET Identity 2.x not on .NET Core (Critical)
Resolves: Non-SDK project format (Critical)"
```

**Alternative**: If migration is performed incrementally, commit at the end of each phase:
- `feat(infra): convert eShopLegacy to SDK-style Blazor Web App project (Phase 1)`
- `feat(data): migrate eShopContext and services to EF Core 8 async (Phase 2)`
- `feat(auth): replace OWIN/Identity 2.x with ASP.NET Core Identity 8 (Phase 3)`
- `feat(ui): rewrite all WebForms pages as Blazor Interactive Server components (Phase 4)`

### Pull Request Process

1. Push `upgrade/webforms-to-blazor` to origin
2. Open PR: `upgrade/webforms-to-blazor` → `main`
3. **PR checklist before merge**:
   - [ ] Solution builds with `0` errors (`dotnet build`)
   - [ ] EF Core `Migrations/` folder committed
   - [ ] No `.aspx`, `.aspx.cs`, `.aspx.designer.cs` files remaining
   - [ ] No `packages.config` or `Web.config` remaining
   - [ ] No `System.Web`, `System.Data.Entity`, or `Microsoft.Owin` references in any `.cs` file
   - [ ] `wwwroot/` contains all static assets
   - [ ] All 23 smoke tests verified
   - [ ] Single atomic commit (preferred)

---

## 9. Success Criteria

### Technical Criteria

| Criterion | Verification Method |
|---|---|
| Project targets `net8.0` | `<TargetFramework>net8.0</TargetFramework>` present in `.csproj` |
| SDK-style project format | `.csproj` opens with `<Project Sdk="Microsoft.NET.Sdk.Web">` |
| No `System.Web` references | `grep -r "System.Web" **/*.cs` returns no results |
| No EF6 references | `grep -r "System.Data.Entity" **/*.cs` returns no results |
| No OWIN references | `grep -r "Microsoft.Owin\|using Owin" **/*.cs` returns no results |
| No `.aspx` files | `Get-ChildItem -Recurse -Filter *.aspx` returns no results |
| Solution builds with 0 errors | `dotnet build` exits with code `0` |
| EF Core migration applied | `dotnet ef database update` succeeds; `Migrations/InitialCreate*.cs` committed |
| Seed data present | Catalog shows 12 products on first run |
| Authentication functional | Login, register, logout all function as expected |
| All 10 pages render | No 404 or unhandled exceptions on any route |
| Static assets served correctly | Bootstrap CSS, site styles, and images load |
| Anonymous basket persists | Add to cart without login; items visible in cart |
| Authenticated checkout works | Full checkout flow creates an order and shows confirmation |

### Quality Criteria

| Criterion | Requirement |
|---|---|
| All DAL methods async | No `.Result` or `.Wait()` calls anywhere in service code |
| DI used throughout | No `new CatalogService(...)`, `new BasketService(...)`, `new OrderService(...)` in component code |
| No `ViewState` usage | No `ViewState[...]` references in any `.razor` or `.cs` file |
| No `Response.Redirect` | No `Response.Redirect(...)` calls in any file |
| No `Page_Load` patterns | No `Page_Load` event handlers remain |
| Blazor routing used | All navigation via `@page` directives and `NavigationManager.NavigateTo` |
| Nullable enabled | All new files respect `#nullable enable`; no nullable warnings |

### Process Criteria

| Criterion | Requirement |
|---|---|
| All-at-Once strategy followed | Entire migration in single coordinated operation |
| Upgrade branch used | All changes committed to `upgrade/webforms-to-blazor` |
| Single atomic commit | Entire migration in one commit (preferred per strategy) |
| PR opened for review | `upgrade/webforms-to-blazor` → `main` PR created before merge |
| PR checklist passed | All checklist items verified before merge |

---

*This plan was generated by GitHub Copilot App Modernization Planning Agent.*
*Assessment: `eShopLegacy\.github\upgrades\scenarios\migratetoazure_a8aa75\assessment.md`*
