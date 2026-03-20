# eShopLegacy .NET Framework 4.8 to .NET 10.0 & WebForms to Blazor Migration Tasks

## Overview

This document tracks the execution of eShopLegacy migration from .NET Framework 4.8 (WebForms) to .NET 10.0 (Blazor Server). The migration follows a phased architectural transformation approach with infrastructure migration, core UI migration, interactive features migration, and final validation.

**Progress**: 0/5 tasks complete (0%) ![0%](https://progress-bar.xyz/0)

---

## Tasks

### [▶] TASK-001: Verify prerequisites and environment setup
**References**: Plan §Phase 0

- [▶] (1) Verify .NET 10 SDK installed: `dotnet --version` per Plan §Step 0.1
- [ ] (2) .NET 10.x.x SDK confirmed (**Verify**)
- [ ] (3) Verify development tooling compatible with .NET 10 per Plan §Step 0.2
- [ ] (4) Visual Studio 17.12+ or latest C# Dev Kit available (**Verify**)
- [ ] (5) Create source control checkpoint per Plan §Step 0.3
- [ ] (6) Commit with message: "Pre-migration checkpoint - net48 baseline"
- [ ] (7) Tag baseline: `git tag baseline-net48-before-migration`
- [ ] (8) Clean working directory confirmed (**Verify**)

---

### [ ] TASK-002: Phase 1 - Foundation migration (infrastructure)
**References**: Plan §Phase 1, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [ ] (1) Create backup of eShopLegacy.csproj per Plan §Step 1.1
- [ ] (2) Convert to SDK-style project targeting net10.0 per Plan §Step 1.1
- [ ] (3) Project file is SDK-style with TargetFramework=net10.0 (**Verify**)
- [ ] (4) Remove incompatible packages and add .NET 10 equivalents per Plan §Step 1.2 and Plan §Package Update Reference
- [ ] (5) All package references updated to .NET 10 compatible versions (**Verify**)
- [ ] (6) Update DbContext class and regenerate EF Core migrations per Plan §Step 1.3
- [ ] (7) EF Core data access functional, database connection works (**Verify**)
- [ ] (8) Create Program.cs, appsettings.json, _Host.cshtml, App.razor, MainLayout.razor per Plan §Step 1.4
- [ ] (9) Blazor Server infrastructure configured and application starts (**Verify**)
- [ ] (10) Update ApplicationUser model and DbContext for ASP.NET Core Identity per Plan §Step 1.5
- [ ] (11) Identity migrations applied, authentication pipeline functional (**Verify**)
- [ ] (12) Update service classes for dependency injection per Plan §Step 1.6
- [ ] (13) All services registered and use constructor injection (**Verify**)
- [ ] (14) Build project: `dotnet build`
- [ ] (15) Resolve compilation errors per Plan §Breaking Changes Catalog
- [ ] (16) Project builds with 0 errors (**Verify**)
- [ ] (17) Run application and verify infrastructure: `dotnet run`
- [ ] (18) Application starts, Blazor page loads, database connection functional (**Verify**)
- [ ] (19) Commit changes with message: "Phase 1 complete: Foundation migration - SDK-style project, EF Core, ASP.NET Core Identity, Blazor infrastructure"

---

### [ ] TASK-003: Phase 2 - Core UI migration (product catalog)
**References**: Plan §Phase 2, Plan §Breaking Changes Catalog

- [ ] (1) Migrate Default.aspx to Index.razor per Plan §Phase 2
- [ ] (2) Migrate Catalog/Default.aspx to Pages/Catalog.razor per Plan §Phase 2
- [ ] (3) Migrate Catalog/Details.aspx to Pages/ProductDetails.razor per Plan §Phase 2
- [ ] (4) Create shared components (CategoryFilter, ProductCard, ProductList, Pagination) per Plan §Phase 2
- [ ] (5) Update component code-behind with data binding and navigation per Plan §Breaking Changes Catalog
- [ ] (6) Build and test catalog browsing: users can browse products by category, view details, navigate between pages
- [ ] (7) All catalog pages functional, data loads correctly, routing works (**Verify**)
- [ ] (8) Commit changes with message: "Phase 2 complete: Core UI migration - Catalog, ProductDetails, reusable components"

---

### [ ] TASK-004: Phase 3 - Interactive UI migration (cart, checkout, account)
**References**: Plan §Phase 3, Plan §Breaking Changes Catalog

- [ ] (1) Implement cart state management service per Plan §Phase 3
- [ ] (2) Migrate Cart/ShoppingCart.aspx to Pages/Cart.razor per Plan §Phase 3
- [ ] (3) Migrate Checkout/Checkout.aspx and OrderComplete.aspx to Pages/Checkout.razor and OrderConfirmation.razor per Plan §Phase 3
- [ ] (4) Migrate Account pages (Login.aspx, Register.aspx, Manage.aspx) to Pages/Account/ components per Plan §Phase 3
- [ ] (5) Update authentication/authorization integration per Plan §Breaking Changes Catalog
- [ ] (6) Build and test end-to-end flow: users can add products to cart, update quantities, complete checkout, register/login
- [ ] (7) Cart persists across pages, checkout creates orders, authentication works (**Verify**)
- [ ] (8) Commit changes with message: "Phase 3 complete: Interactive UI migration - Cart, Checkout, Account, full e-commerce flow"

---

### [ ] TASK-005: Phase 4 - Validation and final commit
**References**: Plan §Phase 4, Plan §Testing & Validation Strategy

- [ ] (1) Run automated test projects per Plan §Phase 4 (if test projects exist)
- [ ] (2) All tests pass with 0 failures (**Verify**)
- [ ] (3) Commit changes with message: "Phase 4 complete: Validation, optimization, documentation - eShopLegacy migrated to .NET 10.0 + Blazor"

---
