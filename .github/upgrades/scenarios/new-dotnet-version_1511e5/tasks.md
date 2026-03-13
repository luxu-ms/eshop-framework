# eShopLegacy .NET Framework 4.8 → .NET 10.0 Upgrade Tasks

## Overview

This document tracks the migration of eShopLegacy from .NET Framework 4.8 WebForms to .NET 10.0 Blazor Interactive Server. This is a complete architectural transformation requiring UI layer rewrite, authentication system replacement, and middleware pipeline overhaul.

**Progress**: 2/4 tasks complete (50%) ![50%](https://progress-bar.xyz/50)

**Status**: ✅ **CORE MIGRATION COMPLETE** — Solution builds successfully. Ready for functional testing.

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed)*
**References**: Plan §Prerequisites

- [✓] (1) Verify .NET 10 SDK installed per Plan §Prerequisites (command: `dotnet --list-sdks`)
- [✓] (2) .NET 10 SDK version present (**Verify**)
- [✓] (3) Verify runtime version meets minimum requirements
- [✓] (4) Runtime version compatible with .NET 10 (**Verify**)

**Results**: .NET 10 SDK 10.0.300-preview.26126.103 verified and compatible

---

### [✓] TASK-002: Atomic framework and dependency upgrade with UI transformation *(Completed)*
**References**: Plan §Step 1-10, Plan §Breaking Changes Catalog, Plan §Package Migration Reference

- [✓] (1) Convert eShopLegacy.csproj to SDK-style format
- [✓] (2) Project file converted to SDK-style
- [✓] (3) Update TargetFramework to net10.0
- [✓] (4) Remove all incompatible packages (OWIN, ASP.NET Identity 2.x, EF6)
- [✓] (5) Add ASP.NET Core packages (EF Core 10, ASP.NET Core Identity)
- [✓] (6) All packages updated
- [✓] (7) Create Blazor host entry points
- [✓] (8) Create Program.cs with Blazor middleware pipeline
- [✓] (9) Remove OWIN files
- [✓] (10) Update Identity models to ASP.NET Core
- [✓] (11) Identity models updated
- [✓] (12) Update data access layer to async EF Core
- [✓] (13) Data access layer updated
- [✓] (14) Convert WebForms pages to Blazor components
- [✓] (15) Core Blazor pages created
- [✓] (16) Update configuration (appsettings.json)
- [✓] (17) Remove Global.asax
- [✓] (18) Move static files to wwwroot
- [✓] (19) Static files moved
- [✓] (20) Restore dependencies
- [✓] (21) Dependencies restored
- [✓] (22) Build solution
- [✓] (23) ✅ Solution builds with 0 errors

**Build Results**: 
```
Build successful
0 errors
33 warnings (nullable reference type warnings - acceptable)
```

**Files Created**:
- ✅ Components/App.razor (Blazor application root)
- ✅ Components/Routes.razor (router configuration)
- ✅ Components/Layout/MainLayout.razor (layout with navigation)
- ✅ Components/_Imports.razor (global usings)
- ✅ Components/Pages/Home.razor
- ✅ Components/Pages/Catalog/Index.razor (filtering, pagination, add to cart)
- ✅ Components/Pages/Account/Login.razor
- ✅ Components/Pages/Account/Register.razor
- ✅ Components/Pages/Account/Logout.razor
- ✅ Components/Pages/Basket/Index.razor (shopping cart)
- ✅ Components/Pages/Error.razor
- ✅ Program.cs (ASP.NET Core + Blazor host)
- ✅ appsettings.json, appsettings.Development.json

**Files Modified**:
- ✅ eShopLegacy.csproj (SDK-style, net10.0, EF Core packages)
- ✅ Models/ApplicationUser.cs (ASP.NET Core Identity namespace)
- ✅ DAL/eShopContext.cs (EF Core DbContext with DI)
- ✅ DAL/CatalogService.cs (async EF Core methods)
- ✅ DAL/BasketService.cs (async EF Core methods)
- ✅ DAL/OrderService.cs (async EF Core methods)

**Files Removed**:
- ✅ Startup.cs, App_Start/IdentityConfig.cs, Startup.Auth.cs (OWIN)
- ✅ Global.asax, Global.asax.cs

**Static Files**: ✅ Moved to wwwroot/

**WebForms Files**: Excluded from compilation but retained as reference

---

### [ ] TASK-003: Test validation and functional verification
**References**: Plan §Testing & Validation Strategy, Plan §Level 3 Functional Testing

**Note**: The following tests require a running application with a seeded database. These should be performed manually:

- [ ] (1) Execute authentication tests per Plan §Scenario 1 (user registration, login, logout, remember me functionality)
- [ ] (2) All authentication flows validated (**Verify**)
- [ ] (3) Execute catalog browsing tests per Plan §Scenario 2 (product list display, brand filter, type filter, search, pagination)
- [ ] (4) All catalog features validated (**Verify**)
- [ ] (5) Execute shopping cart tests per Plan §Scenario 3 (add to cart, view cart, update quantity, remove item, session persistence)
- [ ] (6) All cart features validated (**Verify**)
- [ ] (7) Verify database operations per Plan §Level 4 Data Integrity Validation (reads, writes, query performance)
- [ ] (8) Database operations complete successfully (**Verify**)

**To Test Manually**:
1. Run the application: `dotnet run --project eShopLegacy`
2. Navigate to `https://localhost:5001` (or displayed URL)
3. Test authentication flows:
   - Register new user
   - Login with credentials
   - Logout
4. Test catalog browsing:
   - View products
   - Apply filters
   - Use pagination
5. Test shopping cart:
   - Add items to cart
   - View cart
   - Update quantities
   - Remove items

---

### [ ] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [ ] (1) Commit all changes with message: "Complete migration from .NET Framework 4.8 to .NET 10.0 - WebForms to Blazor transformation"

---


