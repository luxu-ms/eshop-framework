# eShopLegacy WebForms to Blazor Web App Migration Tasks

## Overview

This document tracks the migration of eShopLegacy from ASP.NET WebForms on .NET Framework 4.8 to Blazor Web App on .NET 8 with Interactive Server render mode. All migration work is performed in a single coordinated atomic operation.

**Progress**: 1/2 tasks complete (50%) ![0%](https://progress-bar.xyz/50)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-03-17 07:43)*
**References**: Plan §4.3 Phase 0

- [✓] (1) Verify .NET 8 SDK installed per Plan §4.3 Step 1 (`dotnet --list-sdks` shows 8.0.x entry)
- [✓] (2) .NET 8 SDK available (**Verify**)
- [✓] (3) Verify dotnet-ef global tool installed per Plan §4.3 Step 2 (`dotnet ef --version`)
- [✓] (4) dotnet-ef tool version 8.x available (**Verify**)

---

### [ ] TASK-002: Atomic WebForms to Blazor migration
**References**: Plan §4.4-4.7 (Phases 1-4), §4.9 (Breaking Changes), §8 (Commit Strategy)

- [ ] (1) Execute Phase 1: Project conversion and infrastructure per Plan §4.4 (SDK-style .csproj targeting net8.0, delete all legacy files per §4.4 Step 2, create appsettings.json/Program.cs/Blazor root files, move assets to wwwroot/)
- [ ] (2) Execute Phase 2: Data access migration to EF Core 8 per Plan §4.5 (update eShopContext with EF Core 8 namespaces and Fluent API, create DatabaseSeeder.cs, create initial EF Core migration via `dotnet ef migrations add InitialCreate`, convert all DAL services to async/await)
- [ ] (3) Execute Phase 3: Authentication migration to ASP.NET Core Identity 8 per Plan §4.6 (update ApplicationUser namespace to Microsoft.AspNetCore.Identity, remove GenerateUserIdentityAsync method, update eShopContext base class namespace, make extended properties nullable)
- [ ] (4) Execute Phase 4: UI layer rewrite as Blazor components per Plan §4.7 (create MainLayout.razor per §4.7 Step 1, create all 10 Blazor page components per §4.7 Steps 2-11: Home, Catalog, ProductDetail, ShoppingCart, Checkout, OrderComplete, OrderHistory, Login, Register, Admin/Products)
- [ ] (5) Restore all NuGet dependencies
- [ ] (6) All dependencies restored successfully (**Verify**)
- [ ] (7) Build solution and fix all compilation errors per Plan §4.9 Breaking Changes Catalog (focus areas: EF6→EF Core API changes, sync→async conversions, OWIN/Identity replacements, WebForms→Blazor patterns)
- [ ] (8) Solution builds with 0 errors (**Verify**)
- [ ] (9) Apply EF Core migration to verify database schema: `dotnet ef database update --project eShopLegacy`
- [ ] (10) EF Core migration applied successfully (**Verify**)
- [ ] (11) Commit all changes with message per Plan §8 commit template: "feat: migrate eShopLegacy from WebForms (.NET Framework 4.8) to Blazor Web App (.NET 8)"

---
