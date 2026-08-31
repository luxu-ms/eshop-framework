# eShop on .NET Framework 4.8 – WebForms

## ASP.NET Core application

The migrated application is in `eShop.Web` and targets .NET 10 on Windows. The original Web Forms project remains buildable for rollback but is not required at runtime.

```powershell
# Build the retained legacy baseline and the Core application
.\scripts\build.ps1 -Configuration Debug
dotnet build .\eShop.Web\eShop.Web.csproj

# Run the Core application
dotnet run --project .\eShop.Web\eShop.Web.csproj

# With the application running, execute HTTP smoke tests
.\scripts\test.ps1 -SkipBuild
```

For published output, set `DataDirectory` to the absolute directory containing `eShopLegacy.mdf`, or override `ConnectionStrings__eShopContext` with a production SQL Server connection string.

A classic reference e-commerce application built with **ASP.NET WebForms** on **.NET Framework 4.8**, Entity Framework 6, and ASP.NET Identity.  
This project serves as the "legacy baseline" for migration to ASP.NET Core.

---

## Project Structure

```
eShopLegacy/
├── Account/            # Login & Register pages
├── Admin/              # Admin product management
├── App_Start/          # Route, Identity, OWIN startup config
├── Cart/               # Shopping cart page
├── Catalog/            # Product listing and detail pages
├── Checkout/           # Checkout and order-confirmation pages
├── Content/            # CSS (Bootstrap + custom)
├── DAL/                # Entity Framework DbContext and services
│   ├── eShopContext.cs
│   ├── CatalogService.cs
│   ├── BasketService.cs
│   ├── OrderService.cs
│   └── DatabaseInitializer.cs   ← seeds sample data on first run
├── Models/             # Domain model classes
├── Scripts/            # JavaScript (jQuery + Bootstrap Bundle)
├── Site.Master         # Shared layout / navbar / footer
├── Default.aspx        # Root redirect → Catalog
├── Global.asax
├── Startup.cs          # OWIN/Identity startup
└── Web.config
```

---

## Prerequisites

| Tool | Version |
|------|---------|
| Visual Studio | 2019 or 2022 |
| .NET Framework | 4.8 |
| SQL Server | LocalDB (installed with VS) |
| NuGet | bundled with VS |

---

## Getting Started

### 1 – Restore NuGet packages

Open the solution in Visual Studio and let NuGet restore packages automatically, **or** run:

```powershell
cd eShopLegacy
nuget restore ..\eShopLegacy.sln
```

### 2 – Add front-end libraries

The project references Bootstrap 5 and jQuery. Copy the minified files into their expected locations:

```
eShopLegacy/
├── Content/
│   ├── bootstrap.min.css   ← Bootstrap 5.3
│   └── site.css            ← already present
└── Scripts/
    ├── bootstrap.bundle.min.js   ← Bootstrap 5.3 bundle (includes Popper)
    └── jquery-3.6.0.min.js       ← jQuery 3.6.0
```

You can download them from a CDN or install via **LibMan**:

```json
// libman.json
{
  "version": "1.0",
  "defaultProvider": "cdnjs",
  "libraries": [
    { "library": "bootstrap@5.3.3",   "destination": "Content/",  "files": ["css/bootstrap.min.css"] },
    { "library": "bootstrap@5.3.3",   "destination": "Scripts/",  "files": ["js/bootstrap.bundle.min.js"] },
    { "library": "jquery@3.6.0",      "destination": "Scripts/",  "files": ["jquery.min.js"] }
  ]
}
```

### 3 – Database

The database is created automatically on first run via `DatabaseInitializer` (uses **LocalDB**).  
12 sample catalog items with brands and types are seeded automatically.

To use a full SQL Server instance change the connection string in `Web.config`:

```xml
<add name="eShopContext"
     connectionString="Server=.;Database=eShopLegacy;Integrated Security=True;..."
     providerName="System.Data.SqlClient" />
```

### 4 – Run

Press **F5** in Visual Studio. The app opens in IIS Express and redirects to `/Catalog/Default.aspx`.

---

## Features

| Area | Pages |
|------|-------|
| **Catalog** | Browse products, filter by brand/type, paginate |
| **Product detail** | View description, price, stock; add to cart |
| **Shopping cart** | Adjust quantities, remove items, see totals |
| **Checkout** | Enter shipping address + payment info, place order |
| **Order confirmation** | View order summary after checkout |
| **Account** | Register, sign in (ASP.NET Identity + OWIN cookies) |
| **Admin** | CRUD product management (GridView + inline form) |

---

## Technology Stack

- **ASP.NET WebForms** (System.Web)
- **Entity Framework 6** – Code-First, LocalDB
- **ASP.NET Identity 2** – user registration & cookie auth
- **OWIN / Katana** – authentication middleware
- **Bootstrap 5** – responsive UI
- **SQL Server LocalDB** – development database

---

## Migration Notes

This project is intentionally structured to demonstrate patterns common in legacy WebForms apps:

- `Page_Load` + `IsPostBack` lifecycle
- Code-behind tightly coupled to HTML
- `GridView`, `Repeater`, `UpdatePanel` server controls
- Session-based anonymous basket
- `Response.Redirect` for navigation
- `FormsAuthentication` / OWIN cookie hybrid

These patterns are typical migration targets when moving to **ASP.NET Core Razor Pages** or **MVC**.
