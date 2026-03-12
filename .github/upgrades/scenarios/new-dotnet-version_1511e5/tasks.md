# eShopLegacy .NET Framework 4.8 → .NET 10.0 Upgrade Tasks

## Overview

This document tracks the migration of eShopLegacy from .NET Framework 4.8 WebForms to .NET 10.0 Blazor Interactive Server. This is a complete architectural transformation requiring UI layer rewrite, authentication system replacement, and middleware pipeline overhaul.

**Progress**: 0/4 tasks complete (0%) ![0%](https://progress-bar.xyz/0)

---

## Tasks

### [▶] TASK-001: Verify prerequisites
**References**: Plan §Prerequisites

- [▶] (1) Verify .NET 10 SDK installed per Plan §Prerequisites (command: `dotnet --list-sdks`)
- [ ] (2) .NET 10 SDK version present (**Verify**)
- [ ] (3) Verify runtime version meets minimum requirements
- [ ] (4) Runtime version compatible with .NET 10 (**Verify**)

---

### [ ] TASK-002: Atomic framework and dependency upgrade with UI transformation
**References**: Plan §Step 1-10, Plan §Breaking Changes Catalog, Plan §Package Migration Reference

- [ ] (1) Convert eShopLegacy.csproj to SDK-style format per Plan §Step 1 (replace classic .csproj with `Sdk="Microsoft.NET.Sdk.Web"`)
- [ ] (2) Project file converted to SDK-style (**Verify**)
- [ ] (3) Update TargetFramework to net10.0 in eShopLegacy.csproj per Plan §Step 2
- [ ] (4) Remove all incompatible packages per Plan §Package Migration Reference (Microsoft.Owin.*, Microsoft.AspNet.Identity.*, EntityFramework 6.4.4)
- [ ] (5) Add ASP.NET Core packages per Plan §Package Migration Reference (Microsoft.EntityFrameworkCore 10.0.0, Microsoft.EntityFrameworkCore.SqlServer 10.0.0, Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.0)
- [ ] (6) All packages updated (**Verify**)
- [ ] (7) Create Blazor host entry points per Plan §Step 3.3 (Components/App.razor, Components/Routes.razor, Components/Layout/MainLayout.razor, Components/_Imports.razor)
- [ ] (8) Create Program.cs with ASP.NET Core + Blazor middleware pipeline per Plan §Step 3.3 (AddRazorComponents, AddInteractiveServerComponents, MapRazorComponents)
- [ ] (9) Remove Startup.cs, App_Start/IdentityConfig.cs, and App_Start/Startup.Auth.cs per Plan §Step 3.3
- [ ] (10) Update Identity models and DbContext per Plan §Step 3.1-3.2 (ApplicationUser, ApplicationRole, ApplicationDbContext to ASP.NET Core Identity namespaces and EF Core)
- [ ] (11) Identity models updated to ASP.NET Core (**Verify**)
- [ ] (12) Update data access layer per Plan §Step 5 (convert eShopContext to EF Core with DbContextOptions constructor, update service classes)
- [ ] (13) Data access layer updated to EF Core (**Verify**)
- [ ] (14) Convert all WebForms pages to Blazor components per Plan §Step 4 and Plan §Category 1: WebForms UI Controls (focus: Catalog/Default.aspx → Components/Pages/Catalog/Index.razor, Account pages, Basket.aspx, Site.Master → MainLayout.razor; replace server controls with Blazor markup, ViewState with @code fields, Page_Load with OnInitializedAsync)
- [ ] (15) All WebForms pages converted to Blazor components (**Verify**)
- [ ] (16) Update configuration per Plan §Step 6 (migrate Web.config connection strings and appSettings to appsettings.json)
- [ ] (17) Remove Global.asax and Global.asax.cs per Plan §Step 7
- [ ] (18) Move static files per Plan §Step 9 (Content/ → wwwroot/css/, Scripts/ → wwwroot/js/, update references in layouts)
- [ ] (19) Static files moved to wwwroot (**Verify**)
- [ ] (20) Restore all dependencies (command: `dotnet restore`)
- [ ] (21) Dependencies restored successfully (**Verify**)
- [ ] (22) Build solution and fix all compilation errors per Plan §Step 10 and Plan §Breaking Changes Catalog (focus: System.Web.* namespace replacements, HttpContext API differences, Session API changes, async method updates)
- [ ] (23) Solution builds with 0 errors (**Verify**)

---

### [ ] TASK-003: Test validation and functional verification
**References**: Plan §Testing & Validation Strategy, Plan §Level 3 Functional Testing

- [ ] (1) Execute authentication tests per Plan §Scenario 1 (user registration, login, logout, remember me functionality)
- [ ] (2) All authentication flows validated (**Verify**)
- [ ] (3) Execute catalog browsing tests per Plan §Scenario 2 (product list display, brand filter, type filter, search, pagination)
- [ ] (4) All catalog features validated (**Verify**)
- [ ] (5) Execute shopping cart tests per Plan §Scenario 3 (add to cart, view cart, update quantity, remove item, session persistence)
- [ ] (6) All cart features validated (**Verify**)
- [ ] (7) Verify database operations per Plan §Level 4 Data Integrity Validation (reads, writes, query performance)
- [ ] (8) Database operations complete successfully (**Verify**)

---

### [ ] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [ ] (1) Commit all changes with message: "Complete migration from .NET Framework 4.8 to .NET 10.0 - WebForms to Blazor transformation"

---
