# eShopLegacy .NET Framework 4.8 → .NET 10.0 Migration Plan

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
- [Package Migration Reference](#package-migration-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description

This plan guides the migration of **eShopLegacy** from **.NET Framework 4.8** to **.NET 10.0 (Long Term Support)**. This is not a traditional framework upgrade—it represents a **complete architectural transformation** from ASP.NET WebForms to ASP.NET Core.

### Scope

**Single Project**: `eShopLegacy\eShopLegacy.csproj`
- **Current State**: .NET Framework 4.8, ASP.NET WebForms, OWIN middleware, ASP.NET Identity, Entity Framework 6
- **Target State**: .NET 10.0, **ASP.NET Core Blazor** (Interactive Server Components), ASP.NET Core middleware, ASP.NET Core Identity, Entity Framework Core
- **Codebase Size**: 1,912 LOC across 41 code files
- **Estimated Impact**: 680+ LOC requiring modification (35.6% of codebase)

### Discovered Metrics

| Metric | Value | Classification |
|--------|-------|----------------|
| **Projects** | 1 | Single project |
| **Dependency Depth** | 0 | No project dependencies |
| **API Issues** | 680 total<br/>551 binary incompatible<br/>127 source incompatible<br/>2 behavioral changes | **CRITICAL** |
| **Incompatible Packages** | 8 of 9 packages | **CRITICAL** - architectural replacements required |
| **Security Vulnerabilities** | 0 | ✅ None detected |
| **Code Modification** | 35.6% of codebase | **HIGH** |

### Complexity Classification: 🔴 **CRITICAL**

**Justification**:
- **WebForms → Blazor**: No direct migration path exists. WebForms relies on stateful server-side controls, ViewState, and Page lifecycle—but **Blazor Interactive Server is the closest architectural equivalent** in .NET Core
- **8 incompatible packages**: OWIN and ASP.NET Identity stacks must be completely replaced with ASP.NET Core equivalents
- **551 binary incompatible APIs**: Nearly all `System.Web.*` APIs (95.4% of issues) are unavailable in .NET Core
- **Architectural paradigm shift**: Server-side event-driven WebForms controls → Blazor Interactive Server Components (stateful, component lifecycle, event callbacks)

This is **not** an incremental upgrade—it is a **platform migration** requiring:
1. UI layer rewrite (`.aspx` → `.razor` Blazor components)
2. Authentication/authorization rewrite (ASP.NET Identity → ASP.NET Core Identity)
3. Middleware pipeline rewrite (OWIN → ASP.NET Core middleware)
4. Data access layer conversion (EF6 → EF Core)
5. State management redesign (ViewState/Session → Blazor component state + cascading values)

### Selected Strategy

**All-At-Once Strategy** — Single coordinated transformation

**Rationale**:
- ✅ **Single project**: No inter-project dependencies to coordinate
- ✅ **Clean architectural boundary**: WebForms and ASP.NET Core cannot coexist; intermediate states provide no value
- ✅ **Atomic unit**: The application cannot be partially migrated and remain functional
- ✅ **Complete replacement**: All incompatible technologies (WebForms, OWIN, ASP.NET Identity) must be replaced simultaneously

**Alternative considered**: Incremental migration via System.Web.Adapters was evaluated but rejected:
- System.Web.Adapters provide limited WebForms compatibility (HttpContext, Session)
- WebForms controls (TextBox, GridView, Repeater) and Page lifecycle remain incompatible
- Adapters introduce technical debt and performance overhead
- Complete rewrite provides cleaner, more maintainable result

### Critical Issues

**🔴 Architectural Blockers**:
1. **No WebForms support in ASP.NET Core**: `System.Web.UI.*` namespace entirely absent
2. **OWIN incompatibility**: `Microsoft.Owin.*` packages have no .NET Core versions
3. **ASP.NET Identity incompatibility**: `Microsoft.AspNet.Identity.*` packages replaced by new ASP.NET Core Identity system
4. **ViewState/PostBack model**: Stateful server-side controls have no equivalent

**⚠️ High-Risk Areas**:
- Authentication flows (login, logout, registration)
- Session-based state management (shopping cart, user preferences)
- Data binding patterns (Repeater controls, manual DataBind calls)
- URL routing and navigation

### Recommended Approach

**Phase 0**: Prerequisites & Preparation
- Install .NET 10 SDK
- Convert project to SDK-style
- Add Blazor project structure (`App.razor`, `Routes.razor`, `MainLayout.razor`)

**Phase 1**: Atomic Transformation
1. Replace UI layer (WebForms → **Blazor Interactive Server Components** `.razor`)
2. Replace authentication (ASP.NET Identity → ASP.NET Core Identity with Blazor auth state)
3. Replace middleware (OWIN → ASP.NET Core)
4. Replace data access (EF6 → EF Core)
5. Update all packages
6. Resolve all compilation errors
7. Build and verify

**Phase 2**: Test Validation
- Execute functional tests
- Validate authentication flows
- Verify data access correctness

### Iteration Strategy Used

**Critical Solution Approach**:
- **Iteration 1.1-1.3**: Skeleton creation, discovery, strategy description (COMPLETE)
- **Iteration 2.1**: Dependency analysis
- **Iteration 2.2**: Migration strategy deep dive
- **Iteration 2.3**: Risk overview and complexity assessment
- **Iteration 3.1**: Detailed project transformation plan with architectural guidance
- **Iteration 3.2**: Success criteria and source control strategy

**Total Expected Iterations**: 6

---

## Migration Strategy

### Approach Selection: All-At-Once Strategy

**Selected Strategy**: All components of eShopLegacy.csproj upgraded simultaneously in a single coordinated operation.

**Justification**:

✅ **Single project solution** — No inter-project coordination needed

✅ **Incompatible architectural paradigms** — ASP.NET WebForms and ASP.NET Core cannot coexist meaningfully:
- WebForms relies on stateful server controls, ViewState, and Page lifecycle
- ASP.NET Core uses stateless request-response with MVC/Razor Pages
- No intermediate architectural state provides value

✅ **All-or-nothing dependencies** — Technology stack components are tightly coupled:
- OWIN middleware requires OWIN packages
- ASP.NET Identity requires System.Web infrastructure
- WebForms UI requires System.Web.UI controls
- Replacing one component forces replacement of dependent components

✅ **Atomic transformation** — The application cannot function in a partially migrated state:
- Cannot run WebForms pages without System.Web runtime
- Cannot run ASP.NET Core middleware without ASP.NET Core runtime
- Session state, authentication, and routing mechanisms are incompatible between platforms

✅ **Clean architectural outcome** — A complete migration avoids:
- Technical debt from compatibility shims (System.Web.Adapters)
- Performance overhead from abstraction layers
- Maintenance burden of dual technology stacks
- Confusion from mixed architectural patterns

**Alternative Considered: Incremental Migration with System.Web.Adapters**

Microsoft's `System.Web.Adapters` package provides partial compatibility for migrating ASP.NET Framework applications. This approach was **rejected** for the following reasons:

❌ **Limited WebForms support**: Adapters only provide HttpContext, Session, and a few infrastructure types. Critical WebForms components remain unsupported:
- Server controls (TextBox, DropDownList, Repeater, GridView, etc.)
- Page lifecycle events (Page_Load, IsPostBack, ViewState)
- Control events (Button_Click, etc.)
- Data binding (DataBind, DataSource)

❌ **Technical debt**: Adapters are designed as a transitional bridge, not a long-term solution. They introduce:
- Additional package dependencies
- Performance overhead
- Potential compatibility issues with future .NET versions

❌ **No functional benefit**: Since WebForms controls must be rewritten regardless, using adapters provides no reduction in migration effort

❌ **Less maintainable outcome**: A hybrid application with compatibility layers is harder to understand, debug, and extend than a clean ASP.NET Core application

**Conclusion**: All-At-Once strategy provides the fastest path to a modern, maintainable ASP.NET Core application.

### All-At-Once Strategy Rationale

The All-At-Once strategy is specifically suited for this migration because:

1. **Single Migration Unit**: With only one project, there are no coordination complexities across multiple teams or release cycles

2. **Binary Technology Choice**: The application must be either ASP.NET Framework OR ASP.NET Core—there is no hybrid state that can be deployed

3. **Faster Completion**: A single coordinated effort completes faster than attempting incremental steps with no intermediate value

4. **Clearer Success Criteria**: "Entire application migrated and functional" is easier to validate than tracking partial migration states

5. **Lower Risk of Abandonment**: Smaller projects have higher risk of incremental migrations stalling mid-process. A focused all-at-once effort ensures completion

### Execution Phases

The All-At-Once strategy organizes work into sequential phases, executed as a single continuous operation:

#### Phase 0: Prerequisites (if needed)
- **Validation**: Verify .NET 10 SDK installed
- **Project Conversion**: Convert eShopLegacy.csproj to SDK-style format with `Sdk="Microsoft.NET.Sdk.Web"`
- **Blazor Scaffolding**: Create Blazor host entry points (`App.razor`, `Routes.razor`, `MainLayout.razor`, `_Imports.razor`)
- **Outcome**: Project file compatible with .NET 10 and Blazor Interactive Server rendering

#### Phase 1: Atomic Transformation
**Operations** (performed as single coordinated batch):

1. **Update Project File**
   - Set `<TargetFramework>net10.0</TargetFramework>`
   - Remove all OWIN and ASP.NET Identity package references
   - Add ASP.NET Core framework package references (includes Blazor via `Sdk.Web`)
   - Add ASP.NET Core Identity package references
   - Update EntityFramework 6.4.4 → EntityFrameworkCore 10.0

2. **Replace UI Layer with Blazor Components**
   - Convert `.aspx` pages → `.razor` Blazor components with `@page "/route"` directive
   - Replace WebForms server controls → Blazor component markup with Razor syntax
   - Replace server-side event handlers (`OnClick`, `OnCommand`) → Blazor event callbacks (`@onclick`, handler methods)
   - Replace ViewState → Blazor component state (fields, properties in `@code` block)
   - Replace PostBack model → Blazor interactive event loop (SignalR connection)
   - Replace `Site.Master` → `MainLayout.razor` layout component
   - Replace `<asp:Repeater>` / `<asp:GridView>` → `@foreach` loops in Razor
   - Replace `<asp:Panel Visible="false">` → `@if` conditional rendering

3. **Replace Authentication Layer with Blazor Auth State**
   - Convert ASP.NET Identity User/Role models → ASP.NET Core Identity models
   - Replace SignInManager/UserManager → ASP.NET Core SignInManager/UserManager (DI)
   - Add `AuthenticationStateProvider` cascade for Blazor auth context
   - Use `<AuthorizeView>` and `[Authorize]` for Blazor-aware authorization
   - Convert OWIN authentication middleware → ASP.NET Core authentication middleware

4. **Replace Middleware Pipeline**
   - Remove `Global.asax.cs` and `Startup.cs` (OWIN)
   - Create `Program.cs` with ASP.NET Core + Blazor middleware pipeline
   - Register `AddRazorComponents()`, `AddInteractiveServerComponents()`
   - Map Blazor endpoints: `MapRazorComponents<App>().AddInteractiveServerRenderMode()`
   - Configure services and middleware

5. **Update Data Access Layer**
   - Convert `DbContext` from EF6 → EF Core (with DI constructor)
   - Update `DbSet` configurations
   - Update LINQ queries incompatible with EF Core
   - Register contexts in `Program.cs` via `AddDbContext<T>`

6. **Build and Fix Compilation Errors**
   - Address all breaking changes from framework and package upgrades
   - Replace `System.Web.*` APIs with ASP.NET Core equivalents
   - Resolve namespace changes
   - Fix Blazor component lifecycle issues

7. **Verify Build Success**
   - Solution builds with 0 errors
   - No unresolved package dependencies

**Deliverables**: Application compiles successfully with .NET 10 and Blazor Interactive Server

#### Phase 2: Test Validation
**Operations**:
1. Execute all test projects (if any)
2. Perform manual functional testing
3. Validate authentication flows (login, logout, registration) via Blazor auth state
4. Verify data access correctness
5. Test component state and shopping cart functionality
6. Address any runtime failures

**Deliverables**: All tests pass, application functions correctly

### Dependency-Based Ordering

**Not applicable** — Single project has no inter-project dependencies.


Technology stack ordering within the project follows the sequence defined in Phase 1 above. The key principle is that **project file and package updates must precede code changes**, but code changes must be performed **immediately after** package updates to resolve compilation errors.

### Parallel vs Sequential Execution

**Sequential execution required** — All work occurs within a single project. The operations in Phase 1 are interdependent:
- Cannot update code before updating packages (references won't resolve)
- Cannot build before resolving compilation errors
- Cannot test before build succeeds

**No parallelization opportunities** within this single-project migration.

### Risk Mitigation for All-At-Once

**Higher Initial Risk**: All-At-Once strategy concentrates risk into a single operation. Mitigations:

1. **Comprehensive Planning**: This detailed plan identifies all breaking changes and provides specific guidance (see Breaking Changes Catalog)

2. **Clear Validation Checkpoints**: Each phase has explicit success criteria and validation steps

3. **Version Control Safety**: Work performed on dedicated `upgrade-to-NET10` branch with ability to roll back if needed

4. **Incremental Code Changes**: While the overall strategy is All-At-Once, code changes within Phase 1 can be performed incrementally:
   - Convert one page at a time
   - Test each converted page before moving to next
   - Commit functional increments

5. **Fallback Plan**: If migration proves more complex than expected, project can return to `main` branch and reassess approach

---

## Detailed Dependency Analysis

### Project Structure

```
eShopLegacy Solution
└── eShopLegacy.csproj (net48 → net10.0)
    ├── No project dependencies
    └── No dependent projects
```

**Dependency Graph Summary**:
- **Total Projects**: 1
- **Leaf Projects**: 1 (eShopLegacy.csproj — no dependencies)
- **Root Projects**: 1 (eShopLegacy.csproj — no dependants)
- **Circular Dependencies**: None

### Migration Phases

Since this is a single-project solution with no inter-project dependencies, there is only one migration unit:

**Phase 1: Complete Application Transformation**
- **Project**: eShopLegacy.csproj
- **Migration Type**: Atomic replacement (all components upgraded simultaneously)
- **Parallelization**: Not applicable (single unit)

### Critical Path

The critical path is linear and encompasses the entire application:

```
Start → eShopLegacy.csproj transformation → Build validation → Test validation → Complete
```

**No dependency ordering constraints** — all work occurs within a single project boundary.

### Technology Stack Dependencies

The migration must respect **technology integration dependencies**:

1. **Project File Conversion** (FIRST)
   - Convert to SDK-style before any other changes
   - Enables .NET 10 target framework

2. **Framework Migration** (SECOND)
   - Update TargetFramework to net10.0
   - Establishes runtime environment

3. **Package Migration** (THIRD - Coordinated)
   - Remove OWIN packages
   - Remove ASP.NET Identity packages
   - Add ASP.NET Core framework packages
   - Add ASP.NET Core Identity packages
   - Migrate EntityFramework → EntityFrameworkCore

4. **Code Migration** (FOURTH - Coordinated)
   - UI Layer: WebForms → Razor Pages/MVC
   - Auth Layer: ASP.NET Identity → ASP.NET Core Identity
   - Middleware: OWIN → ASP.NET Core pipeline
   - Data Layer: EF6 contexts → EF Core contexts

**Key Principle**: Steps 3 and 4 are interdependent and must be performed together. Updating packages without code changes will result in compilation failures; updating code without packages will also fail. The All-At-Once strategy treats these as a single atomic operation.

### Grouping Rationale

**No grouping needed** — single project serves as the only migration unit. The All-At-Once strategy applies to the technology stack transformation within this project:

- All UI components migrated together
- All authentication components migrated together
- All middleware components migrated together
- All data access components migrated together

This ensures the application remains in a consistent architectural state (cannot have partial WebForms + partial ASP.NET Core).

---

## Project-by-Project Plans

### eShopLegacy.csproj

**Current State**:
- **Target Framework**: net48 (.NET Framework 4.8)
- **Project Type**: ASP.NET WebForms Application (WAP - Web Application Project)
- **SDK-Style**: No (classic .csproj format)
- **Lines of Code**: 1,912 LOC across 41 code files
- **Estimated LOC Impact**: 680+ LOC (35.6% of project)
- **Project Dependencies**: None
- **Dependent Projects**: None

**Technology Stack (Current)**:
- **UI**: ASP.NET WebForms (`.aspx` pages with code-behind)
- **Authentication**: ASP.NET Identity 2.2.3 with OWIN middleware
- **Middleware**: OWIN 4.2.2 pipeline
- **Data Access**: Entity Framework 6.4.4
- **Session Management**: ASP.NET Session State (`System.Web.SessionState`)

**Packages (Current)**:
| Package | Version |
|---------|---------|
| EntityFramework | 6.4.4 |
| Microsoft.AspNet.Identity.Core | 2.2.3 |
| Microsoft.AspNet.Identity.EntityFramework | 2.2.3 |
| Microsoft.AspNet.Identity.Owin | 2.2.3 |
| Microsoft.Owin | 4.2.2 |
| Microsoft.Owin.Host.SystemWeb | 4.2.2 |
| Microsoft.Owin.Security | 4.2.2 |
| Microsoft.Owin.Security.Cookies | 4.2.2 |
| Owin | 1.0.0 |

**Target State**:
- **Target Framework**: net10.0 (.NET 10.0 LTS)
- **Project Type**: ASP.NET Core Blazor Web App (Interactive Server)
- **SDK-Style**: Yes (will be converted)

**Technology Stack (Target)**:
- **UI**: **Blazor Interactive Server Components** (`.razor` files with `@rendermode InteractiveServer`)
- **Authentication**: ASP.NET Core Identity with Blazor `AuthenticationStateProvider` and `<AuthorizeView>` components
- **Middleware**: ASP.NET Core middleware pipeline with `AddRazorComponents()` and `AddInteractiveServerComponents()`
- **Data Access**: Entity Framework Core 10.0
- **State Management**: Blazor component state (`@code` fields), cascading values, and ASP.NET Core Session for server-side persistence

**Blazor Render Mode Choice: Interactive Server**

| Render Mode | Characteristics | Fit for WebForms Migration |
|-------------|-----------------|---------------------------|
| **Static SSR** | HTML rendered server-side, no interactivity without forms/JS | ❌ Too limited — no event handling |
| **Interactive Server** ✅ | Components hosted on server via SignalR; stateful, full C# event handling | ✅ **Best match** — closest to WebForms stateful model |
| **Interactive WebAssembly** | Components run in browser; no direct server DB access | ❌ Requires API layer, more rearchitecting |
| **Interactive Auto** | Starts as Server, downloads to WASM | ⚠️ Complex for first migration |

**Justification for Interactive Server**: WebForms was a stateful, server-side UI model. Blazor Interactive Server preserves this pattern—C# event handlers run on the server, state lives in server memory, database access is direct (no APIs required). This minimises conceptual distance from the existing code.

**Packages (Target)**:
| Package | Version | Notes |
|---------|---------|-------|
| Microsoft.AspNetCore.App | (Framework) | Includes Blazor, Authentication, MVC — via `Sdk="Microsoft.NET.Sdk.Web"` |
| Microsoft.EntityFrameworkCore | 10.0.0 | Replaces EntityFramework 6.4.4 |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.0 | SQL Server provider |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 10.0.0 | Identity with EF Core |
| Microsoft.EntityFrameworkCore.Tools | 10.0.0 | Design-time tools |

**Packages Removed** (incompatible with .NET Core):
- All Microsoft.Owin.* packages
- All Microsoft.AspNet.Identity.* packages
- Owin package

### eShopLegacy.csproj — Detailed Migration Plan

#### Prerequisites

Before beginning the atomic transformation, ensure:

1. **.NET 10 SDK Installed**
   - Verify: `dotnet --list-sdks` includes version 10.0.x
   - Install from: https://dotnet.microsoft.com/download/dotnet/10.0

2. **Database Backup**
   - Backup identity database (contains user accounts)
   - Backup application database (contains catalog, orders, etc.)
   - Document connection strings

3. **Project File Conversion**
   - Convert `eShopLegacy.csproj` from classic format to SDK-style
   - Tool: `dotnet upgrade-assistant` or manual conversion
   - Verify conversion successful (project loads in Visual Studio)

#### Migration Steps

This section provides detailed guidance for the atomic transformation. While steps are numbered for clarity, many operations are interdependent and must be performed together (especially steps 2-5).

---

### Step 1: Convert Project to SDK-Style

**File**: `eShopLegacy\eShopLegacy.csproj`

**Current** (classic .csproj):
```xml
<Project ToolsVersion="15.0" ...>
  <Import Project="$(MSBuildExtensionsPath)\..." />
  <PropertyGroup>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
    ...
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="System.Web" />
    ...
  </ItemGroup>
  <ItemGroup>
    <Compile Include="..." />
    ...
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\..." />
</Project>
```

**Target** (SDK-style):
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <!-- Package references moved to here in Step 2 -->
  </ItemGroup>
</Project>
```

**Changes**:
- Replace `<Project ...>` with `<Project Sdk="Microsoft.NET.Sdk.Web">`
- Remove all `<Import>` statements
- Simplify `<PropertyGroup>`: only keep essential properties
- Remove explicit `<Compile>` items (SDK-style auto-includes `.cs` files)
- Remove `<Reference>` items for framework assemblies (SDK provides them)
- Remove `<Content>` items for static files (SDK auto-includes)

**Tool**: Use `dotnet upgrade-assistant analyze` then `dotnet upgrade-assistant upgrade` for semi-automated conversion, or convert manually.

**Validation**:
- [ ] Project loads in Visual Studio without errors
- [ ] All source files visible in Solution Explorer
- [ ] No missing file warnings

---

### Step 2: Update Target Framework and Packages

**File**: `eShopLegacy\eShopLegacy.csproj`

**Update TargetFramework**:
```xml
<TargetFramework>net10.0</TargetFramework>
```

**Remove Incompatible Packages**:
```xml
<!-- REMOVE these packages -->
<PackageReference Include="Microsoft.AspNet.Identity.Core" Version="2.2.3" />
<PackageReference Include="Microsoft.AspNet.Identity.EntityFramework" Version="2.2.3" />
<PackageReference Include="Microsoft.AspNet.Identity.Owin" Version="2.2.3" />
<PackageReference Include="Microsoft.Owin" Version="4.2.2" />
<PackageReference Include="Microsoft.Owin.Host.SystemWeb" Version="4.2.2" />
<PackageReference Include="Microsoft.Owin.Security" Version="4.2.2" />
<PackageReference Include="Microsoft.Owin.Security.Cookies" Version="4.2.2" />
<PackageReference Include="Owin" Version="1.0.0" />
<PackageReference Include="EntityFramework" Version="6.4.4" />
```

**Add ASP.NET Core Packages**:
```xml
<ItemGroup>
  <!-- ASP.NET Core framework (includes MVC, Razor Pages, middleware) -->
  <!-- Implicitly referenced by Sdk="Microsoft.NET.Sdk.Web" -->

  <!-- Entity Framework Core -->
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0">
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>

  <!-- ASP.NET Core Identity with EF Core -->
  <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.0" />

  <!-- Session state (if needed) -->
  <PackageReference Include="Microsoft.AspNetCore.Session" Version="2.2.0" />
</ItemGroup>
```

**Rationale**:
- `Microsoft.NET.Sdk.Web` automatically references ASP.NET Core framework packages
- EF Core replaces EF6 for data access
- ASP.NET Core Identity replaces ASP.NET Identity 2.x
- Session package needed if maintaining session-based state

**Validation**:
- [ ] `dotnet restore` succeeds
- [ ] No package conflict warnings

---

### Step 3: Replace Authentication & Identity

This is one of the most complex transformations. ASP.NET Identity 2.x and ASP.NET Core Identity have similar concepts but different implementations.

#### 3.1: Update Identity Data Models

**Current** (`Identity\ApplicationUser.cs`):
```csharp
using Microsoft.AspNet.Identity.EntityFramework;

public class ApplicationUser : IdentityUser
{
    // Custom properties
}

public class ApplicationRole : IdentityRole
{
}
```

**Target**:
```csharp
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Custom properties (same as before)
}

public class ApplicationRole : IdentityRole
{
    // Roles configuration
}
```

**Changes**:
- Namespace: `Microsoft.AspNet.Identity.EntityFramework` → `Microsoft.AspNetCore.Identity`
- Base classes remain same names but different implementations

#### 3.2: Update Identity DbContext

**Current** (`DAL\IdentityContext.cs` or similar):
```csharp
using Microsoft.AspNet.Identity.EntityFramework;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext() : base("DefaultConnection")
    {
    }

    public static ApplicationDbContext Create()
    {
        return new ApplicationDbContext();
    }
}
```

**Target**:
```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Custom configurations
    }
}
```

**Changes**:
- Namespace: `Microsoft.AspNet.Identity.EntityFramework` → `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- Constructor: connection string parameter → `DbContextOptions` dependency injection
- Remove static `Create()` method (not used in ASP.NET Core)
- Configuration: EF6 Fluent API → EF Core Fluent API (mostly compatible)

#### 3.3: Remove OWIN Startup — Create Blazor Program.cs

**Current** (`Startup.cs`):
```csharp
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(eShopLegacy.Startup))]

namespace eShopLegacy
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
```

**Target** (`Program.cs` — new file, Blazor Interactive Server):
```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using eShopLegacy.Components;   // Blazor App root component
using eShopLegacy.DAL;
using eShopLegacy.Identity;

var builder = WebApplication.CreateBuilder(args);

// ─── Data Access ─────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<eShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("eShopContext")));

// ─── Identity ─────────────────────────────────────────────────────────────────
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<ApplicationRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// ─── Blazor ───────────────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ─── Application Services (DI) ───────────────────────────────────────────────
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<BasketService>();

var app = builder.Build();

// ─── Middleware Pipeline ──────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();           // Required by Blazor

// ─── Endpoint Mapping ─────────────────────────────────────────────────────────
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

**New Root Files to Create**:

**`Components/App.razor`**:
```razor
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <link rel="stylesheet" href="app.css" />
    <link rel="stylesheet" href="eShopLegacy.styles.css" />
    <HeadOutlet />
</head>
<body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

**`Components/Routes.razor`**:
```razor
<Router AppAssembly="typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)" />
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
</Router>
```

**`Components/_Imports.razor`**:
```razor
@using System.Net.Http
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using eShopLegacy.Components
@using eShopLegacy.Models
@using eShopLegacy.DAL
```

**Changes**:
- OWIN `Startup.cs` → ASP.NET Core `Program.cs`
- `app.CreatePerOwinContext` → `builder.Services.AddScoped<T>()` (dependency injection)
- `AddRazorPages()` → `AddRazorComponents().AddInteractiveServerComponents()`
- `MapRazorPages()` → `MapRazorComponents<App>().AddInteractiveServerRenderMode()`
- `app.UseAntiforgery()` required for Blazor form handling

**Delete Files**:
- `Startup.cs`
- `App_Start\IdentityConfig.cs`
- `App_Start\Startup.Auth.cs`

#### 3.4: Update User Manager and Sign-In Manager for Blazor

**Current** (OWIN context lookup):
```csharp
var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

var user = userManager.FindByName(username);
var result = await signInManager.PasswordSignInAsync(username, password, rememberMe, shouldLockout: false);
```

**Target** (Blazor component — dependency injection via `[Inject]`):
```csharp
// In Blazor component (.razor or partial class)
@inject UserManager<ApplicationUser> UserManager
@inject SignInManager<ApplicationUser> SignInManager
@inject NavigationManager NavigationManager

// In @code block:
var user = await UserManager.FindByNameAsync(username);
var result = await SignInManager.PasswordSignInAsync(username, password, rememberMe, lockoutOnFailure: false);

if (result.Succeeded)
    NavigationManager.NavigateTo("/");
```

**Changes**:
- OWIN context lookup → `[Inject]` attribute or `@inject` directive in Blazor components
- `FindByName` → `FindByNameAsync` (async required in Blazor)
- `shouldLockout` → `lockoutOnFailure`
- `Response.Redirect` → `NavigationManager.NavigateTo`

**Custom UserManager/SignInManager classes**: Register in `Program.cs` as scoped services.

---

### Step 4: Convert UI Layer (WebForms → Blazor Components)

This is the most labor-intensive part. Each `.aspx` page must be converted to a `.razor` Blazor component with `@page` directive. Blazor Interactive Server is the chosen render mode because it is the closest conceptual match to WebForms: both are **stateful, server-side, event-driven**.

#### General Conversion Pattern

**WebForms Structure**:
```
Catalog/
├── Default.aspx              (markup)
├── Default.aspx.cs           (code-behind)
└── Default.aspx.designer.cs  (generated controls)
```

**Blazor Structure**:
```
Components/
└── Pages/
    └── Catalog/
        └── Index.razor       (markup + @code block — replaces both files above)
```

#### WebForms → Blazor Concept Mapping

| WebForms Concept | Blazor Equivalent | Notes |
|------------------|-------------------|-------|
| `.aspx` page | `.razor` component with `@page` | Same URL routing concept |
| Code-behind `.aspx.cs` | `@code { }` block in same `.razor` file | Co-located markup and logic |
| `Page` base class | `ComponentBase` (implicit) | Blazor components inherit this |
| `Page_Load` event | `OnInitializedAsync()` lifecycle | Runs on component first render |
| `IsPostBack` check | Separate event handler methods | Blazor events are always explicit |
| `<asp:TextBox>` | `<input>` with `@bind` | Two-way data binding built-in |
| `<asp:Button OnClick>` | `<button @onclick="MethodName">` | Direct event binding |
| `<asp:DropDownList>` | `<select @bind="SelectedValue">` | Native HTML with Blazor binding |
| `<asp:Repeater>` | `@foreach` loop | Clean Razor template iteration |
| `<asp:Panel Visible="false">` | `@if (condition) { }` | Conditional rendering |
| `<%# Eval("Property") %>` | `@item.Property` | Strongly-typed, no reflection |
| ViewState | Component field (`private string _value`) | In-memory component state |
| PostBack | Blazor event callback (SignalR round-trip) | Stateful, no full page reload |
| `Session["key"]` | `[Inject] ISessionService` or cascading | Requires service abstraction |
| `Response.Redirect` | `NavigationManager.NavigateTo("/path")` | Client-side navigation |
| `Request.QueryString` | `[SupplyParameterFromQuery]` attribute | Declarative query binding |
| `Site.Master` | `MainLayout.razor` | Layout component |
| `ContentPlaceHolder` | `@Body` in layout | Body content slot |

#### 4.1: Convert Catalog Page (Example)

**Current** (`Catalog/Default.aspx` + `Default.aspx.cs`):
```aspx
<%-- Default.aspx --%>
<asp:DropDownList ID="ddlBrand" runat="server" />
<asp:TextBox ID="txtSearch" runat="server" />
<asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />

<asp:Repeater ID="rptProducts" runat="server">
    <ItemTemplate>
        <div>
            <h3><%# Eval("Name") %></h3>
            <p><%# Eval("Price", "{0:C}") %></p>
            <asp:Button CommandName="AddToCart"
                        CommandArgument='<%# Eval("Id") + "|" + Eval("Price") %>'
                        OnCommand="btnAddToCart_Command" Text="Add to Cart" />
        </div>
    </ItemTemplate>
</asp:Repeater>

<asp:Panel ID="pnlPager" runat="server">
    <asp:HyperLink ID="btnPrev" runat="server" Text="Previous" />
    <asp:Label ID="lblPage" runat="server" />
    <asp:HyperLink ID="btnNext" runat="server" Text="Next" />
</asp:Panel>
```

```csharp
// Default.aspx.cs
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack) { BindFilters(); BindProducts(); }
}
protected void btnSearch_Click(object sender, EventArgs e)   { BindProducts(); }
protected void btnAddToCart_Command(object sender, CommandEventArgs e)
{
    var args = e.CommandArgument.ToString().Split('|');
    int itemId = int.Parse(args[0]);
    decimal price = decimal.Parse(args[1]);
    // Add to cart logic
}
private void BindFilters() { /* populate dropdowns */ }
private void BindProducts() { /* set rptProducts.DataSource, .DataBind() */ }
```

**Target** (`Components/Pages/Catalog/Index.razor` — single file):
```razor
@page "/catalog"
@rendermode InteractiveServer
@inject CatalogService CatalogSvc
@inject BasketService BasketSvc
@inject AuthenticationStateProvider AuthState
@inject NavigationManager Nav

<PageTitle>Catalog</PageTitle>

<div class="filters">
    <select @bind="selectedBrand">
        <option value="0">All Brands</option>
        @foreach (var brand in brands)
        {
            <option value="@brand.Id">@brand.Brand</option>
        }
    </select>

    <select @bind="selectedType">
        <option value="0">All Types</option>
        @foreach (var type in types)
        {
            <option value="@type.Id">@type.Type</option>
        }
    </select>

    <input type="text" @bind="searchQuery" placeholder="Search..." />
    <button @onclick="ApplyFilters">Search</button>
    <button @onclick="ClearFilters">Clear</button>
</div>

@if (products.Count == 0)
{
    <p>No products found.</p>
}
else
{
    <div class="product-grid">
        @foreach (var product in products)
        {
            <div class="product-card">
                <img src="@product.PictureUri" alt="@product.Name" />
                <h3>@product.Name</h3>
                <p>@product.Price.ToString("C")</p>
                <button @onclick="() => AddToCart(product.Id, product.Price)">
                    Add to Cart
                </button>
            </div>
        }
    </div>
}

@if (totalPages > 1)
{
    <div class="pager">
        <button @onclick="PrevPage" disabled="@(currentPage == 0)">Previous</button>
        <span>Page @(currentPage + 1) of @totalPages</span>
        <button @onclick="NextPage" disabled="@((currentPage + 1) * PageSize >= total)">Next</button>
    </div>
}

@if (addedToCart)
{
    <div class="toast">Item added to cart!</div>
}

@code {
    private const int PageSize = 8;

    // Component state (replaces ViewState + code-behind fields)
    private int currentPage = 0;
    private int selectedBrand = 0;
    private int selectedType = 0;
    private string searchQuery = "";
    private int total = 0;
    private int totalPages = 1;
    private bool addedToCart = false;

    private List<CatalogBrand> brands = new();
    private List<CatalogType> types = new();
    private List<CatalogItem> products = new();

    // Equivalent to Page_Load (runs once on first render)
    protected override async Task OnInitializedAsync()
    {
        brands = await CatalogSvc.GetCatalogBrandsAsync();
        types  = await CatalogSvc.GetCatalogTypesAsync();
        await LoadProductsAsync();
    }

    // Equivalent to btnSearch_Click event handler
    private async Task ApplyFilters()
    {
        currentPage = 0;
        await LoadProductsAsync();
    }

    private async Task ClearFilters()
    {
        selectedBrand = 0;
        selectedType  = 0;
        searchQuery   = "";
        currentPage   = 0;
        await LoadProductsAsync();
    }

    private async Task PrevPage()
    {
        if (currentPage > 0) { currentPage--; await LoadProductsAsync(); }
    }

    private async Task NextPage()
    {
        if ((currentPage + 1) * PageSize < total) { currentPage++; await LoadProductsAsync(); }
    }

    // Equivalent to btnAddToCart_Command event handler
    private async Task AddToCart(int itemId, decimal price)
    {
        string buyerId = await GetBuyerIdAsync();
        await BasketSvc.AddItemToBasketAsync(buyerId, itemId, price);
        addedToCart = true;
        // Auto-dismiss toast after 2 seconds
        await Task.Delay(2000);
        addedToCart = false;
    }

    private async Task LoadProductsAsync()
    {
        int? brandId = selectedBrand == 0 ? null : selectedBrand;
        int? typeId  = selectedType  == 0 ? null : selectedType;
        (products, total) = await CatalogSvc.GetCatalogItemsAsync(
            currentPage, PageSize, brandId, typeId, searchQuery);
        totalPages = Math.Max(1, (int)Math.Ceiling((double)total / PageSize));
    }

    private async Task<string> GetBuyerIdAsync()
    {
        var authState = await AuthState.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity?.IsAuthenticated == true)
            return user.Identity.Name!;

        // Anonymous buyer — use a service that abstracts session/cookie storage
        // (Session is not directly available in Blazor Server components; inject a scoped service)
        return await BasketSvc.GetOrCreateAnonymousBuyerIdAsync();
    }
}
```

**Key Differences vs. WebForms**:

1. **Single file** — `.razor` replaces both `.aspx` and `.aspx.cs`
2. **`@rendermode InteractiveServer`** — activates the SignalR connection (equivalent to WebForms stateful lifecycle)
3. **`@bind`** — two-way data binding replaces `TextBox.Text` / `DropDownList.SelectedValue`
4. **`@onclick`** — replaces `OnClick` / `OnCommand` event wiring
5. **`@code { }` fields** — replace ViewState and code-behind instance fields
6. **`OnInitializedAsync()`** — replaces `Page_Load` with `if (!IsPostBack)`
7. **No `DataBind()`** — Blazor re-renders reactively when state changes; no manual binding calls
8. **`NavigationManager.NavigateTo`** — replaces `Response.Redirect`
9. **`[Inject]` / `@inject`** — replaces `new Service()` or OWIN context lookups

**Validation**:
- [ ] Component renders without errors
- [ ] Filters apply correctly
- [ ] Products list updates reactively (no page reload)
- [ ] Pagination works
- [ ] Add to cart works
- [ ] Toast notification shows and auto-dismisses

#### 4.2: Convert Master Page → Blazor Layout

**Current** (`Site.Master`):
```aspx
<%@ Master Language="C#" CodeBehind="Site.master.cs" Inherits="eShopLegacy.SiteMaster" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title><%: Page.Title %> - eShopLegacy</title>
    <asp:ContentPlaceHolder ID="head" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ContentPlaceHolder ID="MainContent" runat="server" />
    </form>
</body>
</html>
```

**Target** (`Components/Layout/MainLayout.razor`):
```razor
@inherits LayoutComponentBase

<div class="page">
    <header>
        <nav>
            <a href="/">eShopLegacy</a>
            <AuthorizeView>
                <Authorized>
                    <span>Hello, @context.User.Identity?.Name</span>
                    <a href="/Account/Logout">Logout</a>
                </Authorized>
                <NotAuthorized>
                    <a href="/Account/Login">Login</a>
                    <a href="/Account/Register">Register</a>
                </NotAuthorized>
            </AuthorizeView>
        </nav>
    </header>

    <main>
        @Body   @* Replaces <asp:ContentPlaceHolder ID="MainContent"> *@
    </main>

    <footer>
        &copy; eShopLegacy
    </footer>
</div>
```

**Key Differences**:
- `@inherits LayoutComponentBase` — declares this as a layout component
- `@Body` — replaces all `<asp:ContentPlaceHolder>` slots
- `<AuthorizeView>` — declarative auth UI (replaces `if (User.Identity.IsAuthenticated)` in code-behind)
- No `<form runat="server">` wrapper needed — Blazor handles forms at component level
- Master page code-behind (`Site.master.cs`) → inline `@code` block or `@inject` directives

**`Components/Layout/NavMenu.razor`** (extract navigation as reusable component):
```razor
<nav>
    <NavLink href="/catalog" Match="NavLinkMatch.Prefix">Catalog</NavLink>
    <NavLink href="/basket">Basket</NavLink>
    <AuthorizeView>
        <Authorized>
            <NavLink href="/Account/Manage">My Account</NavLink>
        </Authorized>
    </AuthorizeView>
</nav>
```

#### 4.3: Convert Other Pages → Blazor Components

Repeat the pattern above for each remaining page:
- **`Account/Login.aspx`** → **`Components/Pages/Account/Login.razor`**
- **`Account/Register.aspx`** → **`Components/Pages/Account/Register.razor`**
- **`Basket.aspx`** → **`Components/Pages/Basket/Index.razor`**
- **`Checkout.aspx`** → **`Components/Pages/Checkout/Index.razor`**

**Blazor Identity UI Scaffolding** (for Account pages):
```bash
dotnet aspnet-codegenerator identity --useDefaultUI
```
This generates standard login/register/logout Razor Pages (non-Blazor) which can be used alongside Blazor components. For a full Blazor experience, scaffold them and convert to `.razor` components, or use the Blazor Identity UI scaffolding directly:
```bash
dotnet new blazoridentity --project eShopLegacy
```

**Authentication State in Blazor Components**:
```razor
@page "/account/login"
@rendermode InteractiveServer
@inject SignInManager<ApplicationUser> SignInManager
@inject NavigationManager Nav

<EditForm Model="loginModel" OnValidSubmit="HandleLogin">
    <DataAnnotationsValidator />
    <InputText @bind-Value="loginModel.Email" />
    <InputText @bind-Value="loginModel.Password" type="password" />
    <InputCheckbox @bind-Value="loginModel.RememberMe" /> Remember me
    <button type="submit">Login</button>
    <ValidationSummary />
</EditForm>

@code {
    private LoginModel loginModel = new();

    private async Task HandleLogin()
    {
        var result = await SignInManager.PasswordSignInAsync(
            loginModel.Email, loginModel.Password,
            loginModel.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
            Nav.NavigateTo("/");
        else
            // Handle error
            errorMessage = "Invalid login attempt.";
    }
}
```

Note: Blazor Interactive Server components using `SignInManager` must handle the auth state cookie refresh. Use `NavigationManager.NavigateTo("/", forceLoad: true)` after sign-in/sign-out to ensure the browser reloads with updated auth cookies.

---

### Step 5: Update Data Access Layer (EF6 → EF Core)

#### 5.1: Update DbContext

**Current** (`DAL/eShopContext.cs`):
```csharp
using System.Data.Entity;

public class eShopContext : DbContext
{
    public eShopContext() : base("eShopContext")
    {
    }

    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<CatalogBrand> CatalogBrands { get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogItem>()
            .Property(c => c.Price)
            .HasPrecision(18, 2);
    }
}
```

**Target**:
```csharp
using Microsoft.EntityFrameworkCore;

public class eShopContext : DbContext
{
    public eShopContext(DbContextOptions<eShopContext> options)
        : base(options)
    {
    }

    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<CatalogBrand> CatalogBrands { get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogItem>()
            .Property(c => c.Price)
            .HasPrecision(18, 2);

        // Additional configurations
    }
}
```

**Changes**:
- Namespace: `System.Data.Entity` → `Microsoft.EntityFrameworkCore`
- Constructor: connection string → `DbContextOptions` dependency injection
- `DbModelBuilder` → `ModelBuilder` (mostly compatible API)

#### 5.2: Update Service Classes

**Current** (`DAL/CatalogService.cs`):
```csharp
public class CatalogService
{
    private readonly eShopContext _context;

    public CatalogService(eShopContext context)
    {
        _context = context;
    }

    public List<CatalogItem> GetCatalogItems(int page, int pageSize, int? brandId, int? typeId, string search, out int total)
    {
        IQueryable<CatalogItem> query = _context.CatalogItems;

        if (brandId.HasValue)
            query = query.Where(i => i.CatalogBrandId == brandId.Value);

        if (typeId.HasValue)
            query = query.Where(i => i.CatalogTypeId == typeId.Value);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(i => i.Name.Contains(search));

        total = query.Count();

        return query
            .OrderBy(i => i.Name)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToList();
    }
}
```

**Target**:
```csharp
public class CatalogService
{
    private readonly eShopContext _context;

    public CatalogService(eShopContext context)
    {
        _context = context;
    }

    public List<CatalogItem> GetCatalogItems(int page, int pageSize, int? brandId, int? typeId, string search, out int total)
    {
        IQueryable<CatalogItem> query = _context.CatalogItems;

        if (brandId.HasValue)
            query = query.Where(i => i.CatalogBrandId == brandId.Value);

        if (typeId.HasValue)
            query = query.Where(i => i.CatalogTypeId == typeId.Value);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(i => i.Name.Contains(search)); // EF Core: case-sensitive by default on some providers

        total = query.Count(); // Consider CountAsync for better performance

        return query
            .OrderBy(i => i.Name)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToList(); // Consider ToListAsync for better performance
    }
}
```

**Changes**:
- LINQ syntax mostly compatible
- Consider async methods (`ToListAsync`, `CountAsync`, `FirstOrDefaultAsync`) for better scalability
- String comparisons may be case-sensitive depending on database provider
- Lazy loading disabled by default (use `.Include()` for eager loading if needed)

**⚠️ Breaking Changes to Watch**:
- **Lazy Loading**: Disabled by default. Explicitly enable if needed or use eager loading (`.Include()`)
- **String.Contains()**: May be case-sensitive depending on provider
- **Synchronous methods**: Still available but async preferred for scalability

#### 5.3: Regenerate Migrations (if using Code-First)

If you're using EF migrations:

```bash
# Remove old EF6 migrations
Remove-Item .\Migrations\*.cs

# Create initial EF Core migration
dotnet ef migrations add InitialCreate --context eShopContext

# Apply migration
dotnet ef database update --context eShopContext
```

**⚠️ Important**: If you have an existing production database:
- Test migration on a copy first
- EF Core migration may recreate tables (data loss risk)
- Consider keeping EF6 database schema and using EF Core with existing database
- Or manually adjust migration to preserve existing data

---

### Step 6: Update Configuration

**Current** (`Web.config`):
```xml
<configuration>
  <connectionStrings>
    <add name="eShopContext" connectionString="Server=...;Database=eShop;..." providerName="System.Data.SqlClient" />
    <add name="DefaultConnection" connectionString="Server=...;Database=eShopIdentity;..." providerName="System.Data.SqlClient" />
  </connectionStrings>

  <appSettings>
    <add key="SomeSetting" value="SomeValue" />
  </appSettings>

  <system.web>
    <compilation debug="true" targetFramework="4.8" />
    <authentication mode="None" />
    <sessionState mode="InProc" timeout="20" />
  </system.web>
</configuration>
```

**Target** (`appsettings.json`):
```json
{
  "ConnectionStrings": {
    "eShopContext": "Server=...;Database=eShop;...",
    "DefaultConnection": "Server=...;Database=eShopIdentity;..."
  },
  "AppSettings": {
    "SomeSetting": "SomeValue"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Access Configuration**:
```csharp
// In Program.cs or services
var connectionString = builder.Configuration.GetConnectionString("eShopContext");
var someSetting = builder.Configuration["AppSettings:SomeSetting"];

// Or use Options pattern
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
```

**Environment-Specific Configuration**:
- `appsettings.json` — Base settings
- `appsettings.Development.json` — Development overrides
- `appsettings.Production.json` — Production overrides

---

### Step 7: Remove Global.asax

**Current** (`Global.asax.cs`):
```csharp
using System.Web;
using System.Web.Routing;

public class Global : HttpApplication
{
    protected void Application_Start()
    {
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        // Other initialization
    }
}
```

**Delete**:
- `Global.asax`
- `Global.asax.cs`

**Move Initialization** to `Program.cs` (already covered in Step 3.3).

If `Global.asax` contains custom logic (error handling, session events, etc.), migrate:
- `Application_Start` → `Program.cs` service/middleware configuration
- `Application_Error` → `app.UseExceptionHandler()` middleware
- `Session_Start` → Session middleware configuration
- `Application_BeginRequest` → Custom middleware

---

### Step 8: Update Routing

**Current** (`App_Start/RouteConfig.cs`):
```csharp
using System.Web.Routing;

public class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.MapPageRoute("catalog", "catalog/{page}", "~/Catalog/Default.aspx");
        // Other routes
    }
}
```

**Target**: Routes handled by Razor Pages automatically:
- `/Pages/Catalog/Index.cshtml` → `/Catalog` or `/Catalog/Index`
- Custom routes via `@page` directive:

```cshtml
@page "/catalog/{page:int?}"
```

Or use MVC routing in `Program.cs`:
```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

---

### Step 9: Update Static Files and Content

**Current Structure**:
```
eShopLegacy/
├── Content/
│   ├── Site.css
│   └── images/
├── Scripts/
│   └── site.js
```

**Target Structure**:
```
eShopLegacy/
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   ├── js/
│   │   └── site.js
│   └── images/
```

**Changes**:
- Move `Content/` → `wwwroot/css/`
- Move `Scripts/` → `wwwroot/js/`
- Update references in layouts:
  ```html
  <link rel="stylesheet" href="~/css/site.css" />
  <script src="~/js/site.js"></script>
  ```

---

### Step 10: Build and Fix Compilation Errors

After completing steps 1-9, build the project:

```bash
dotnet build
```

**Expected Compilation Errors**:

1. **Missing `System.Web.*` References**
   - **Error**: `The type or namespace name 'Web' does not exist in the namespace 'System'`
   - **Fix**: Remove all `using System.Web.*` statements (already replaced)

2. **HttpContext API Differences**
   - **Error**: `HttpContext does not contain a definition for 'GetOwinContext'`
   - **Fix**: Use dependency injection instead (covered in Step 3)

3. **Session API Differences**
   - **Error**: `HttpSessionState does not support implicit string casting`
   - **Fix**: Use `HttpContext.Session.GetString()` / `SetString()` (requires serialization)

4. **Async Method Warnings**
   - **Warning**: `This async method lacks 'await' operators`
   - **Fix**: Use async EF Core methods (`ToListAsync`, etc.) or remove `async` keyword

5. **Nullable Reference Warnings** (if `<Nullable>enable</Nullable>`)
   - **Warning**: `Possible null reference assignment`
   - **Fix**: Add null checks or use nullable types (`string?`)

**Iterative Approach**:
1. Fix errors in batches (e.g., all `System.Web` errors first)
2. Build after each batch to verify progress
3. Commit functional increments to version control

**Validation**:
- [ ] Solution builds with 0 errors
- [ ] Only acceptable warnings remain (nullable warnings if enabled)
- [ ] No missing dependencies

---

### Expected Breaking Changes

See **Breaking Changes Catalog** section for comprehensive list. Key areas:

1. **WebForms Controls** — 95% of API issues
2. **Authentication APIs** — ASP.NET Identity → ASP.NET Core Identity
3. **Session State** — API and serialization differences
4. **HttpContext** — Different properties and methods
5. **Entity Framework** — LINQ query behavior, lazy loading defaults

---

### Code Modifications Required

**Areas Needing Review** (beyond automated replacements):

1. **Custom HttpModules/HttpHandlers**
   - Replace with ASP.NET Core middleware
   - Example: Custom authentication → middleware or policy-based authorization

2. **ViewState Usage**
   - Replace with query strings, hidden fields, or client-side state (JavaScript)

3. **Page Lifecycle Events**
   - `Page_Init`, `Page_PreRender`, etc. → Razor Page lifecycle methods or filters

4. **Control Events**
   - `Button_Click`, `GridView_RowDataBound`, etc. → Form handlers or JavaScript

5. **Membership Provider** (if used)
   - Migrate to ASP.NET Core Identity or custom authentication

6. **Cache Usage** (`HttpContext.Cache`)
   - Replace with `IMemoryCache` or `IDistributedCache`

---

### Testing Strategy

After migration complete and application builds:

#### Unit Tests (if available)
- Run existing unit tests
- Update test projects to .NET 10 if needed
- Fix test API incompatibilities

#### Manual Functional Testing

**Critical Paths**:
1. **Authentication**
   - [ ] User registration
   - [ ] User login
   - [ ] User logout
   - [ ] Password reset (if applicable)
   - [ ] Remember me functionality

2. **Catalog Browsing**
   - [ ] View product list
   - [ ] Filter by brand
   - [ ] Filter by type
   - [ ] Search products
   - [ ] Pagination

3. **Shopping Cart**
   - [ ] Add item to cart
   - [ ] View cart
   - [ ] Update quantities
   - [ ] Remove items
   - [ ] Cart persists across sessions (if applicable)

4. **Checkout** (if applicable)
   - [ ] Complete order
   - [ ] Payment processing
   - [ ] Order confirmation

5. **Session State**
   - [ ] Anonymous session tracking
   - [ ] Session timeout behavior
   - [ ] Session persistence across requests

**Performance Testing**:
- Load pages and verify response times
- Check for N+1 query issues (use EF Core logging)
- Monitor memory usage

---

### Validation Checklist

- [ ] Project builds without errors
- [ ] Project builds without warnings (or only acceptable warnings)
- [ ] No NuGet package conflicts
- [ ] No security vulnerabilities in packages
- [ ] All pages load without errors
- [ ] Authentication flows work (login, logout, register)
- [ ] Data displays correctly
- [ ] Forms submit correctly
- [ ] Session state persists
- [ ] Navigation and routing work
- [ ] Static files serve correctly
- [ ] Database operations succeed
- [ ] Performance acceptable

---

---

## Package Migration Reference

### Package Removal (Incompatible)

The following packages must be **removed** — they have no .NET Core equivalents:

| Package | Current Version | Status | Replacement |
|---------|----------------|--------|-------------|
| `Microsoft.Owin` | 4.2.2 | ❌ Incompatible | ASP.NET Core middleware (built-in) |
| `Microsoft.Owin.Host.SystemWeb` | 4.2.2 | ❌ Incompatible | Not needed (Kestrel web server) |
| `Microsoft.Owin.Security` | 4.2.2 | ❌ Incompatible | ASP.NET Core authentication (built-in) |
| `Microsoft.Owin.Security.Cookies` | 4.2.2 | ❌ Incompatible | ASP.NET Core cookie authentication (built-in) |
| `Owin` | 1.0.0 | ❌ Incompatible | Not needed |
| `Microsoft.AspNet.Identity.Core` | 2.2.3 | ❌ Incompatible | `Microsoft.AspNetCore.Identity` (framework) |
| `Microsoft.AspNet.Identity.EntityFramework` | 2.2.3 | ❌ Incompatible | `Microsoft.AspNetCore.Identity.EntityFrameworkCore` |
| `Microsoft.AspNet.Identity.Owin` | 2.2.3 | ❌ Incompatible | `Microsoft.AspNetCore.Identity` (framework) |
| `EntityFramework` | 6.4.4 | ❌ Incompatible | `Microsoft.EntityFrameworkCore.*` packages |

**Rationale for Removal**:
- **OWIN packages**: OWIN was a middleware abstraction for .NET Framework. ASP.NET Core has its own native middleware pipeline with superior performance and integration
- **ASP.NET Identity packages**: Completely redesigned for ASP.NET Core with different namespace, APIs, and data models
- **EntityFramework**: EF6 is .NET Framework-only. EF Core is the modern, cross-platform successor

### Package Additions (ASP.NET Core)

Add the following packages to support .NET 10 and ASP.NET Core:

| Package | Version | Purpose | Notes |
|---------|---------|---------|-------|
| `Microsoft.AspNetCore.App` | (Framework) | ASP.NET Core framework | Implicitly referenced by `Sdk="Microsoft.NET.Sdk.Web"` |
| `Microsoft.EntityFrameworkCore` | 10.0.0 | EF Core runtime | Replaces `EntityFramework` 6.4.4 |
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.0 | SQL Server provider | Required for SQL Server database access |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.0 | Design-time tools | Enables `dotnet ef` commands for migrations |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 10.0.0 | Identity with EF Core | Replaces `Microsoft.AspNet.Identity.EntityFramework` |
| `Microsoft.AspNetCore.Session` | 2.2.0 | Session state | Required if using session state |

**Optional Packages** (depending on features used):

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.0 | JWT authentication (if API endpoints) |
| `Microsoft.EntityFrameworkCore.InMemory` | 10.0.0 | In-memory database (for testing) |
| `Swashbuckle.AspNetCore` | 6.5.0 | Swagger/OpenAPI (if API endpoints) |

### Package Version Summary

**Complete Before/After**:

#### Before (.NET Framework 4.8)
```xml
<PackageReference Include="EntityFramework" Version="6.4.4" />
<PackageReference Include="Microsoft.AspNet.Identity.Core" Version="2.2.3" />
<PackageReference Include="Microsoft.AspNet.Identity.EntityFramework" Version="2.2.3" />
<PackageReference Include="Microsoft.AspNet.Identity.Owin" Version="2.2.3" />
<PackageReference Include="Microsoft.Owin" Version="4.2.2" />
<PackageReference Include="Microsoft.Owin.Host.SystemWeb" Version="4.2.2" />
<PackageReference Include="Microsoft.Owin.Security" Version="4.2.2" />
<PackageReference Include="Microsoft.Owin.Security.Cookies" Version="4.2.2" />
<PackageReference Include="Owin" Version="1.0.0" />
```

#### After (.NET 10.0)
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0">
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Session" Version="2.2.0" />
```

### Package Compatibility Notes

**EntityFramework 6.4.4 → EntityFrameworkCore 10.0.0**:
- Major version jump (6 → 10)
- Breaking API changes:
  - `DbContext` constructor signature different
  - `DbModelBuilder` → `ModelBuilder`
  - Some LINQ query behavior differences
  - Lazy loading disabled by default
  - `async` methods preferred
- **Reason for update**: EF6 not compatible with .NET Core; EF Core 10.0 is latest LTS version for .NET 10

**ASP.NET Identity 2.2.3 → ASP.NET Core Identity (framework)**:
- Namespace changes: `Microsoft.AspNet.Identity` → `Microsoft.AspNetCore.Identity`
- API changes:
  - `UserManager`/`SignInManager` constructors use dependency injection
  - Password hashing algorithm may differ (verify compatibility)
  - Cookie authentication configuration different
- **Reason for update**: ASP.NET Identity 2.x not compatible with .NET Core

**OWIN 4.2.2 → ASP.NET Core Middleware**:
- Complete architecture change
- No direct package replacement
- Middleware registered in `Program.cs` using `WebApplicationBuilder`
- **Reason for update**: OWIN designed for .NET Framework; ASP.NET Core has native middleware

### Security Considerations

✅ **No security vulnerabilities** detected in current packages.

**Post-Migration Security Posture**:
- ✅ **Entity Framework**: EF Core 10.0 includes latest security patches
- ✅ **ASP.NET Core Identity**: Modern authentication with improved security defaults
- ✅ **ASP.NET Core**: Built-in protections (CSRF, XSS, etc.)
- ⚠️ **Session Package**: Version 2.2.0 shown as example; verify latest compatible version

**Recommended Post-Migration**:
1. Run `dotnet list package --vulnerable` to check for vulnerabilities
2. Run `dotnet list package --outdated` to check for newer versions
3. Review cookie settings (HttpOnly, Secure, SameSite)
4. Enable HTTPS redirection (`app.UseHttpsRedirection()`)

### Breaking Changes by Package

#### EntityFramework → EntityFrameworkCore

**API Breaking Changes**:
- `DbContext` constructor: connection string → `DbContextOptions`
- `DbSet.Add()` → `DbSet.Add()` (same) but consider `AddAsync()`
- `SaveChanges()` → `SaveChanges()` (same) but consider `SaveChangesAsync()`
- `Include()` syntax similar but improved
- `AsNoTracking()` syntax same
- Lazy loading requires explicit configuration

**Configuration Breaking Changes**:
- Connection string: Web.config → appsettings.json
- Database initialization: dropped in EF Core (use migrations)
- Model configuration: `DbModelBuilder` → `ModelBuilder` (mostly compatible)

**LINQ Breaking Changes**:
- Some queries may execute differently
- String comparisons may be case-sensitive (provider-dependent)
- Complex queries may require explicit `ToList()` before further operations

#### Microsoft.AspNet.Identity → Microsoft.AspNetCore.Identity

**API Breaking Changes**:
- Namespace: `Microsoft.AspNet.Identity` → `Microsoft.AspNetCore.Identity`
- User manager access: `HttpContext.GetOwinContext().GetUserManager()` → constructor injection
- Sign-in: `PasswordSignInAsync` parameter names changed
- User creation: similar but async preferred

**Data Model Breaking Changes**:
- Identity tables may have schema differences
- Custom properties on `ApplicationUser` should be compatible
- Password hash format may differ (verify with test logins)

**Configuration Breaking Changes**:
- Cookie configuration: OWIN options → ASP.NET Core options
- Password requirements: similar but configured differently
- Lockout settings: similar structure, different API

#### Microsoft.Owin → ASP.NET Core Middleware

**Architecture Breaking Changes**:
- `IAppBuilder` → `WebApplicationBuilder` / `WebApplication`
- Middleware registration: `app.Use()` → `app.UseMiddleware()` or `app.Use()`
- Startup class: `Startup.cs` → `Program.cs`
- Service registration: `app.CreatePerOwinContext()` → `builder.Services.AddScoped()`

**Configuration Breaking Changes**:
- Configuration source: Web.config → appsettings.json
- Configuration access: `ConfigurationManager` → `IConfiguration` injection

---

## Breaking Changes Catalog

This section documents the comprehensive breaking changes identified in the assessment, organized by category for efficient resolution during migration.

### Overview

| Category | Count | Impact Level |
|----------|-------|--------------|
| **Binary Incompatible** | 551 | 🔴 CRITICAL — Require code changes |
| **Source Incompatible** | 127 | 🟡 MEDIUM — Require re-compilation and potential API adjustments |
| **Behavioral Changes** | 2 | 🔵 LOW — Require runtime testing |
| **Total Breaking Changes** | 680 | |

### Category 1: WebForms UI Controls (Binary Incompatible)

**Impact**: 🔴 CRITICAL — 95.4% of all breaking changes

The entire `System.Web.UI.WebControls` namespace is unavailable in ASP.NET Core. All server-side controls must be replaced with Razor syntax or HTML helpers.

#### Most Frequent Control Issues

| Control | Occurrences | Blazor Replacement |
|---------|-------------|----------------------|
| `TextBox` | 62 | `<input @bind="fieldName" />` — two-way bound to `@code` field |
| `Label` | 39 | `<span>@fieldName</span>` or `<label>` — display-only bound property |
| `Panel` | 29 | `@if (condition) { <div>...</div> }` — conditional rendering |
| `DropDownList` | 28 | `<select @bind="selectedValue">` with `@foreach` options |
| `Repeater` | 15 | `@foreach (var item in items)` in Razor template |
| `Button` | 10 | `<button @onclick="HandlerMethod">` — direct async event binding |
| `Literal` | 8 | `@fieldName` or `@((MarkupString)htmlContent)` for raw HTML |
| `HyperLink` | 6 | `<a href="/path">` or `<NavLink href="/path">` for active tracking |
| `GridView` | 3 | `@foreach` loop rendering `<table>` rows |
| `CheckBox` | 4 | `<input type="checkbox" @bind="boolField" />` |

#### Conversion Patterns — WebForms → Blazor

**TextBox**:
```razor
@* Before: WebForms *@
<%-- <asp:TextBox ID="txtSearch" runat="server" /> --%>
<%-- string search = txtSearch.Text; --%>

@* After: Blazor *@
<input type="text" @bind="searchQuery" placeholder="Search..." />

@code {
    private string searchQuery = "";
}
```

**DropDownList**:
```razor
@* Before: WebForms *@
<%-- <asp:DropDownList ID="ddlBrand" runat="server" />
     ddlBrand.Items.Add(new ListItem("Text", "Value"));
     string selected = ddlBrand.SelectedValue; --%>

@* After: Blazor *@
<select @bind="selectedBrand">
    <option value="0">All Brands</option>
    @foreach (var brand in brands)
    {
        <option value="@brand.Id">@brand.Brand</option>
    }
</select>

@code {
    private int selectedBrand = 0;
    private List<CatalogBrand> brands = new();
}
```

**Repeater**:
```razor
@* Before: WebForms *@
<%-- <asp:Repeater ID="rptProducts" runat="server">
         <ItemTemplate><div><%# Eval("Name") %></div></ItemTemplate>
     </asp:Repeater>
     rptProducts.DataSource = products; rptProducts.DataBind(); --%>

@* After: Blazor — no DataBind() needed; re-renders automatically *@
@foreach (var product in products)
{
    <div>@product.Name</div>
}

@code {
    private List<CatalogItem> products = new();
}
```

**Button with Event Handler**:
```razor
@* Before: WebForms *@
<%-- <asp:Button OnClick="btnSearch_Click" Text="Search" />
     protected void btnSearch_Click(object sender, EventArgs e) { BindProducts(); } --%>

@* After: Blazor — direct async event binding *@
<button @onclick="SearchAsync">Search</button>

@code {
    private async Task SearchAsync()
    {
        await LoadProductsAsync();
    }
}
```

**Panel (Visibility)**:
```razor
@* Before: WebForms *@
<%-- <asp:Panel ID="pnlEmpty" runat="server" Visible="false">No items found.</asp:Panel>
     pnlEmpty.Visible = items.Count == 0; --%>

@* After: Blazor *@
@if (products.Count == 0)
{
    <div>No items found.</div>
}
```

### Category 2: Page Lifecycle & ViewState (Binary Incompatible)

**Impact**: 🔴 CRITICAL

WebForms page lifecycle events and ViewState have no equivalent in ASP.NET Core. Blazor provides the **closest equivalent** to the WebForms lifecycle through its component lifecycle.

| API | Occurrences | Blazor Replacement |
|-----|-------------|-------------|
| `Page.IsPostBack` | 8 | No concept needed — `OnInitializedAsync()` runs once; events explicit |
| `Page.Response.Redirect()` | 20 | `NavigationManager.NavigateTo("/path")` |
| `Page.Request.QueryString` | 7 | `[SupplyParameterFromQuery]` attribute on `@code` property |
| `Page.Session` | 11 | Inject a scoped service wrapping `ISessionService` (see note) |
| `Page.User` | 19 | `AuthenticationStateProvider` via `[CascadingParameter]` |
| `Control.ViewState` | 5 | `@code` private fields — component instance state |

**Blazor Component Lifecycle (replaces Page Lifecycle)**:

| WebForms Event | Blazor Lifecycle Method | Notes |
|----------------|------------------------|-------|
| `Page_Load` (first time) | `OnInitializedAsync()` | Load data, set up state |
| `Page_Load` (postback) | Event handler method | Explicit event, not automatic |
| `Page_PreRender` | No equivalent needed | Blazor re-renders after state changes |
| `Page_Unload` | `IDisposable.Dispose()` | Implement `IDisposable` on component |

**IsPostBack Pattern → Blazor Lifecycle**:
```razor
@* Before: WebForms — guards initial load vs. postback *@
@* protected void Page_Load(object sender, EventArgs e)
   {
       if (!IsPostBack) { LoadData(); }  // Only on first render
   }
   protected void btnSearch_Click(...) { LoadData(); } // On button click *@

@* After: Blazor — no IsPostBack needed; lifecycle is explicit *@
<button @onclick="SearchAsync">Search</button>

@code {
    // OnInitializedAsync = Page_Load with !IsPostBack check
    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();   // Runs once on first render
    }

    // Button click = explicit event, runs independently of lifecycle
    private async Task SearchAsync()
    {
        await LoadDataAsync();   // Triggered explicitly
    }
}
```

**ViewState → Blazor Component State**:
```razor
@* Before: WebForms *@
@* ViewState["CurrentPage"] = pageNumber;
   int page = (int)ViewState["CurrentPage"]; *@

@* After: Blazor — plain @code fields; state lives in component instance on server *@
@code {
    private int currentPage = 0;   // Lives in server memory (Interactive Server)
}
```

**⚠️ Session Note for Blazor Interactive Server**:
`HttpContext.Session` is **not available** inside Blazor Interactive Server components (only available during the initial static render). Abstract session behind a scoped service:
```csharp
// Program.cs
builder.Services.AddScoped<IAnonymousBasketService, AnonymousBasketService>();

// AnonymousBasketService.cs — access session via IHttpContextAccessor (set during SSR)
public class AnonymousBasketService
{
    private string _anonymousBuyerId;
    public string GetOrCreateBuyerId() =>
        _anonymousBuyerId ??= Guid.NewGuid().ToString();
    public void SetBuyerId(string id) => _anonymousBuyerId = id;
}
```

### Category 3: HttpContext & Request/Response (Source Incompatible)

**Impact**: 🟡 MEDIUM

`HttpContext` is not directly accessible in Blazor Interactive Server components. Use `NavigationManager` and injected services instead.

| API | Occurrences | Blazor Replacement |
|-----|-------------|-----------------|
| `HttpResponse.Redirect()` | 20 | `NavigationManager.NavigateTo("/path")` |
| `HttpRequest.QueryString` | 7 | `[SupplyParameterFromQuery]` attribute |
| `HttpSessionState.Item[]` | 13 | Scoped service (see Session Note above) |
| `HttpContext` (general) | 8 | `IHttpContextAccessor` during SSR only; inject services for components |

**Response.Redirect → NavigationManager**:
```csharp
// Before: WebForms
Response.Redirect("~/Catalog/Default.aspx?id=5");

// After: Blazor
[Inject] NavigationManager Nav { get; set; }
Nav.NavigateTo($"/catalog?id=5");

// Force full page reload (needed after sign-in/sign-out to refresh auth cookie):
Nav.NavigateTo("/", forceLoad: true);
```

**QueryString → SupplyParameterFromQuery**:
```razor
// Before: WebForms
string page = Request.QueryString["page"];

// After: Blazor
@page "/catalog"

@code {
    [SupplyParameterFromQuery(Name = "page")]
    public int Page { get; set; } = 0;

    [SupplyParameterFromQuery(Name = "brand")]
    public int Brand { get; set; } = 0;
}
```

**Session State**:
```csharp
// Before: WebForms
Session["Key"] = "value";
string value = (string)Session["Key"];

// After: Blazor — via injected scoped service
// (Direct Session access not available in Interactive Server components)
@inject IAnonymousBasketService BasketService
string buyerId = BasketService.GetOrCreateBuyerId();

HttpContext.Session.SetString("ComplexObject", json);
var obj = JsonSerializer.Deserialize<MyObject>(HttpContext.Session.GetString("ComplexObject"));
```

**QueryString Access**:
```csharp
// Before: WebForms
string page = Request.QueryString["page"];

// After: ASP.NET Core (preferred - model binding)
[BindProperty(SupportsGet = true)]
public int Page { get; set; }

// Or manual access:
string pageStr = HttpContext.Request.Query["page"];
```

**Response.Redirect**:
```csharp
// Before: WebForms
Response.Redirect("~/Catalog/Default.aspx?id=5");

// After: Razor Pages
return RedirectToPage("/Catalog/Index", new { id = 5 });

// Or MVC:
return RedirectToAction("Index", "Catalog", new { id = 5 });

// Or raw redirect (not recommended):
return Redirect("/Catalog?id=5");
```

### Category 4: Authentication & Identity (Binary Incompatible)

**Impact**: 🔴 CRITICAL

Entire authentication system replaced.

| API | Occurrences | Replacement |
|-----|-------------|-------------|
| `HttpContext.GetOwinContext().GetUserManager()` | N/A | Constructor injection of `UserManager<TUser>` |
| `DefaultAuthenticationTypes.ApplicationCookie` | 5 | Configuration in `Program.cs` with `AddDefaultIdentity()` |
| `UserManager` methods | 4+ | Similar but different namespace and some parameter changes |
| `SignInManager` methods | N/A | Similar but `PasswordSignInAsync` parameters differ |

**UserManager Access**:
```csharp
// Before: WebForms
var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
var user = userManager.FindByName(username);

// After: ASP.NET Core
// In PageModel constructor:
private readonly UserManager<ApplicationUser> _userManager;
public LoginModel(UserManager<ApplicationUser> userManager)
{
    _userManager = userManager;
}

// In handler:
var user = await _userManager.FindByNameAsync(username);
```

**SignIn**:
```csharp
// Before: WebForms
var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
var result = await signInManager.PasswordSignInAsync(username, password, rememberMe, shouldLockout: false);

// After: ASP.NET Core
private readonly SignInManager<ApplicationUser> _signInManager;
var result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, lockoutOnFailure: false);
```

### Category 5: Entity Framework 6 → EF Core (Source Incompatible)

**Impact**: 🟡 MEDIUM

Most EF6 code is compatible, but some patterns require changes.

**Breaking Changes**:

| Pattern | EF6 | EF Core |
|---------|-----|---------|
| DbContext constructor | `DbContext("connectionName")` | `DbContext(DbContextOptions)` |
| Model builder | `DbModelBuilder` | `ModelBuilder` |
| Lazy loading | Enabled by default | Disabled by default |
| Async preferred | Optional | Strongly recommended |
| Database initialization | `Database.SetInitializer` | Not supported (use migrations) |

**DbContext Constructor**:
```csharp
// Before: EF6
public class eShopContext : DbContext
{
    public eShopContext() : base("eShopContext")
    {
    }
}
// Usage:
using (var ctx = new eShopContext())
{
    // ...
}

// After: EF Core
public class eShopContext : DbContext
{
    public eShopContext(DbContextOptions<eShopContext> options)
        : base(options)
    {
    }
}
// Usage (dependency injection):
// In Program.cs:
builder.Services.AddDbContext<eShopContext>(options =>
    options.UseSqlServer(connectionString));
// In PageModel:
private readonly eShopContext _context;
public IndexModel(eShopContext context)
{
    _context = context;
}
```

**Lazy Loading**:
```csharp
// Before: EF6 (lazy loading automatic)
var item = ctx.CatalogItems.First();
var brandName = item.CatalogBrand.Name; // Automatically loads CatalogBrand

// After: EF Core (lazy loading disabled by default)
// Option 1: Eager loading
var item = ctx.CatalogItems
    .Include(i => i.CatalogBrand)
    .First();
var brandName = item.CatalogBrand.Name;

// Option 2: Enable lazy loading (requires proxies)
// In Program.cs:
builder.Services.AddDbContext<eShopContext>(options =>
    options.UseSqlServer(connectionString)
           .UseLazyLoadingProxies());
// And make navigation properties virtual:
public virtual CatalogBrand CatalogBrand { get; set; }
```

### Category 6: Configuration System (Behavioral Change)

**Impact**: 🔵 LOW (structural change, not a breaking API)

Configuration moves from Web.config to appsettings.json.

**Web.config → appsettings.json**:
```xml
<!-- Before: Web.config -->
<connectionStrings>
  <add name="eShopContext" connectionString="Server=...;" />
</connectionStrings>
<appSettings>
  <add key="Setting1" value="Value1" />
</appSettings>

<!-- After: appsettings.json -->
{
  "ConnectionStrings": {
    "eShopContext": "Server=...;"
  },
  "AppSettings": {
    "Setting1": "Value1"
  }
}
```

**Access Configuration**:
```csharp
// Before: WebForms
using System.Configuration;
string connStr = ConfigurationManager.ConnectionStrings["eShopContext"].ConnectionString;
string setting = ConfigurationManager.AppSettings["Setting1"];

// After: ASP.NET Core
// Inject IConfiguration:
private readonly IConfiguration _configuration;
string connStr = _configuration.GetConnectionString("eShopContext");
string setting = _configuration["AppSettings:Setting1"];

// Or use Options pattern (recommended):
// Define class:
public class AppSettings
{
    public string Setting1 { get; set; }
}
// Register in Program.cs:
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
// Inject in PageModel:
private readonly AppSettings _appSettings;
public IndexModel(IOptions<AppSettings> appSettings)
{
    _appSettings = appSettings.Value;
}
```

### Category 7: Routing & Navigation

**Impact**: 🟡 MEDIUM

Routing patterns change significantly.

**Before: WebForms Routing**:
```csharp
// In RouteConfig.cs:
routes.MapPageRoute("catalog", "catalog", "~/Catalog/Default.aspx");

// In markup:
<a href="<%= GetRouteUrl("catalog", new { id = 5 }) %>">Link</a>
```

**After: Razor Pages Routing**:
```csharp
// Convention-based routing (automatic):
// Pages/Catalog/Index.cshtml → /Catalog or /Catalog/Index

// Custom route via @page directive:
@page "/catalog/{id:int?}"

// In markup:
<a asp-page="/Catalog/Index" asp-route-id="5">Link</a>
```

### Category 8: Static File Handling

**Impact**: 🟡 MEDIUM

Static files must be in `wwwroot` folder.

**Before**:
```
eShopLegacy/
├── Content/
│   └── site.css
├── Scripts/
│   └── site.js
└── Images/
    └── logo.png
```

**After**:
```
eShopLegacy/
└── wwwroot/
    ├── css/
    │   └── site.css
    ├── js/
    │   └── site.js
    └── images/
        └── logo.png
```

**References**:
```html
<!-- Before -->
<link href="~/Content/site.css" rel="stylesheet" />

<!-- After -->
<link href="~/css/site.css" rel="stylesheet" />
```

### Known Workarounds

**For WebForms Controls**:
- ⚠️ No direct workaround exists
- System.Web.Adapters provides limited compatibility (HttpContext, Session) but NOT UI controls
- Complete rewrite to Razor syntax required

**For ViewState**:
- Use query strings for navigation state (pagination, filters)
- Use hidden fields for form state
- Use client-side state (sessionStorage, localStorage) for UI state

**For OWIN**:
- No workaround needed — ASP.NET Core middleware is superior replacement
- Custom OWIN middleware must be rewritten as ASP.NET Core middleware

**For EF6**:
- EF6 can technically run on .NET Core 3.1+ but NOT recommended
- Better to migrate to EF Core for long-term maintainability

### Testing Recommendations for Breaking Changes

After applying fixes for breaking changes:

1. **Compile-Time Validation**
   - [ ] Solution builds with 0 errors
   - [ ] No `CS0103` (type/namespace not found) errors
   - [ ] No `CS1061` (member not found) errors

2. **Runtime Validation**
   - [ ] All pages load without exceptions
   - [ ] Navigation works correctly
   - [ ] Form submissions work
   - [ ] Data displays correctly

3. **Behavioral Validation**
   - [ ] Session state persists correctly
   - [ ] Authentication flows work
   - [ ] Data access returns correct results
   - [ ] Configuration values load correctly

4. **Performance Validation**
   - [ ] Page load times acceptable
   - [ ] No N+1 query issues (check EF Core logging)
   - [ ] No excessive database calls

---

## Risk Management

### High-Risk Changes

| Risk Area | Risk Level | Description | Mitigation |
|-----------|------------|-------------|------------|
| **WebForms → ASP.NET Core UI** | 🔴 CRITICAL | Complete UI rewrite required. WebForms controls, ViewState, PostBack, and Page lifecycle have no ASP.NET Core equivalent. 551 binary incompatible APIs. | • Preserve business logic in separate service layer<br/>• Convert one page at a time<br/>• Use Razor Pages (similar structure to WebForms)<br/>• Manual functional testing per page<br/>• Reference original `.aspx` files during conversion |
| **Authentication System** | 🔴 CRITICAL | ASP.NET Identity 2.x → ASP.NET Core Identity requires data model changes, API replacements, and middleware reconfiguration. User authentication may break. | • Backup identity database before migration<br/>• Test login/logout flows thoroughly<br/>• Verify password hashing compatibility<br/>• Document cookie authentication differences<br/>• Plan for potential user re-authentication |
| **Session State** | 🔴 HIGH | ASP.NET Session State → ASP.NET Core Session requires configuration changes and API updates. Shopping cart and user preferences may be lost. | • Document current session usage<br/>• Configure distributed cache (Redis) for production<br/>• Test session timeout behavior<br/>• Verify serialization compatibility<br/>• Plan session migration or user re-login |
| **OWIN → ASP.NET Core Middleware** | 🟡 MEDIUM | Complete middleware pipeline replacement. Startup patterns and configuration significantly different. | • Map OWIN middleware → ASP.NET Core equivalents<br/>• Test authentication pipeline thoroughly<br/>• Verify middleware ordering<br/>• Check custom middleware compatibility |
| **Entity Framework 6 → EF Core** | 🟡 MEDIUM | API differences, LINQ query behavior changes, migration regeneration needed. Data access may fail or produce incorrect results. | • Test all LINQ queries<br/>• Regenerate migrations<br/>• Verify lazy loading behavior<br/>• Test database initialization<br/>• Backup database before testing |
| **Routing & Navigation** | 🟡 MEDIUM | RouteCollection → ASP.NET Core endpoint routing. URL patterns may break. | • Document current routes<br/>• Test all navigation paths<br/>• Verify query string handling<br/>• Check SEO/bookmarked URLs |
| **Configuration System** | 🟡 MEDIUM | Web.config → appsettings.json. Configuration keys and structure differ. | • Map all Web.config settings → appsettings.json<br/>• Test configuration loading<br/>• Verify connection strings<br/>• Document environment-specific settings |

### Security Vulnerabilities

✅ **No security vulnerabilities detected** in current NuGet packages.

**Post-Migration Security Considerations**:
- Verify authentication cookie security settings (HttpOnly, Secure, SameSite)
- Review CORS configuration if applicable
- Validate input validation and anti-XSRF tokens
- Test authorization policies
- Verify password hashing algorithm compatibility

### Contingency Plans

#### If WebForms Conversion Proves Too Complex

**Symptoms**:
- Business logic too tightly coupled to WebForms controls
- Conversion time exceeds estimates significantly
- Complex custom controls without clear migration path

**Options**:
1. **System.Web.Adapters** (limited support):
   - Add `Microsoft.AspNetCore.SystemWebAdapters` package
   - Provides HttpContext, Session, some infrastructure
   - Still requires rewriting UI layer
   - Consider only if data/business layer can be partially decoupled

2. **Gradual Rewrite**:
   - Run legacy application alongside new ASP.NET Core app
   - Proxy requests between applications
   - Migrate pages incrementally over longer timeline
   - Requires URL routing strategy

3. **Reassess Timeline**:
   - Continue with All-At-Once but extend effort estimate
   - Break UI conversion into smaller phases
   - Focus on critical pages first

#### If Authentication Migration Fails

**Symptoms**:
- Users cannot log in with existing credentials
- Password hashing incompatibility
- Identity database schema conflicts

**Options**:
1. **Reset All Passwords**:
   - Force password reset for all users
   - Simplifies migration
   - Requires user communication

2. **Custom Password Hasher**:
   - Implement `IPasswordHasher<TUser>` compatible with old format
   - Gradually migrate to new format on login
   - Maintains user credentials

3. **Separate Identity Database**:
   - Create new ASP.NET Core Identity schema
   - Migrate users programmatically
   - Requires data migration script

#### If EF6 → EF Core Migration Breaks Data Access

**Symptoms**:
- LINQ queries produce incorrect results
- Database initialization fails
- Migrations cannot be applied

**Options**:
1. **Isolate Breaking Queries**:
   - Rewrite specific queries causing issues
   - Use raw SQL for complex queries temporarily
   - Refactor incrementally

2. **Parallel EF6 and EF Core**:
   - Keep EF6 package for legacy contexts
   - Introduce EF Core for new code
   - Gradually migrate contexts
   - Requires dual dependency management

3. **Database-First Fallback**:
   - Use EF Core database-first approach
   - Scaffold from existing database
   - May lose custom configurations

### Rollback Strategy

**Immediate Rollback** (if critical issues discovered early):
```bash
git checkout main
git branch -D upgrade-to-NET10
```

**Partial Rollback** (if some progress should be preserved):
```bash
# Create a backup branch
git checkout upgrade-to-NET10
git branch upgrade-to-NET10-backup

# Reset to specific commit before issue
git reset --hard <commit-sha>
```

**Point of No Return**: Once identity database schema is modified, rollback requires database restore from backup.

---

## Testing & Validation Strategy

### Overview

Testing for this migration is **critical** due to the architectural transformation from WebForms to ASP.NET Core. Unlike a typical framework upgrade, this migration rewrites the UI layer and replaces multiple technology stacks, requiring comprehensive validation at multiple levels.

### Multi-Level Testing Approach

#### Level 1: Compilation Validation
**Timing**: After completing code migration (Step 10 in project plan)

**Objective**: Ensure solution compiles without errors

**Validation Steps**:
1. Clean solution: `dotnet clean`
2. Restore packages: `dotnet restore`
3. Build solution: `dotnet build --configuration Release`

**Success Criteria**:
- [ ] No compilation errors (CS0xxx)
- [ ] No package restore errors
- [ ] No missing reference errors
- [ ] Warnings minimized (nullable warnings acceptable if `<Nullable>enable</Nullable>`)

**Common Issues**:
- Missing `using` statements for ASP.NET Core namespaces
- Incorrect `HttpContext` API usage
- Session state API differences
- EF Core query differences

**Fix Process**:
- Address errors in batches by category
- Rebuild after each batch
- Commit functional increments

---

#### Level 2: Smoke Testing (Per-Page Validation)
**Timing**: After each page is converted from WebForms to Razor Pages

**Objective**: Quick validation that converted pages load and function

**For Each Converted Page**:
1. **Load Test**
   - [ ] Page loads without exception
   - [ ] No HTTP 500 errors
   - [ ] Page renders completely (no partial renders)

2. **Visual Inspection**
   - [ ] Layout matches expected design (may differ from WebForms)
   - [ ] All content sections display
   - [ ] Images and static assets load
   - [ ] No broken CSS

3. **Basic Interaction**
   - [ ] Links navigate correctly
   - [ ] Forms render correctly
   - [ ] Buttons respond to clicks

**Quick Fix Cycle**:
- If smoke test fails, fix immediately before converting next page
- Don't accumulate broken pages

---

#### Level 3: Functional Testing (Feature Validation)
**Timing**: After smoke testing passes for all pages

**Objective**: Verify application features work correctly end-to-end

### Critical Test Scenarios

#### Scenario 1: Authentication & User Management

**User Registration**:
1. Navigate to `/Account/Register`
2. Fill registration form:
   - Email: `test@example.com`
   - Password: `Test123!`
   - Confirm password: `Test123!`
3. Submit form
4. **Expected**: User created successfully, redirected to home page or confirmation page
5. **Verify**: User exists in identity database

**User Login**:
1. Navigate to `/Account/Login`
2. Fill login form:
   - Email: `test@example.com`
   - Password: `Test123!`
3. Submit form
4. **Expected**: User logged in, redirected to home page
5. **Verify**: `User.Identity.IsAuthenticated` returns `true`
6. **Verify**: User name displayed in navigation (if applicable)

**User Logout**:
1. Click logout link/button
2. **Expected**: User logged out, redirected to home page
3. **Verify**: `User.Identity.IsAuthenticated` returns `false`
4. **Verify**: Authenticated-only links hidden

**Remember Me**:
1. Login with "Remember Me" checked
2. Close browser
3. Reopen browser and navigate to site
4. **Expected**: User still logged in

**Password Reset** (if applicable):
1. Navigate to "Forgot Password"
2. Enter email address
3. Follow reset workflow
4. **Expected**: Password reset successfully

**Test Checklist**:
- [ ] User can register new account
- [ ] User can log in with correct credentials
- [ ] User cannot log in with incorrect credentials
- [ ] User can log out
- [ ] Remember Me functionality works
- [ ] Password reset works (if applicable)
- [ ] Authorization protects restricted pages

---

#### Scenario 2: Catalog Browsing

**View Product List**:
1. Navigate to `/Catalog` or `/`
2. **Expected**: Product list displays
3. **Verify**: Products show name, price, image

**Filter by Brand**:
1. Select brand from dropdown
2. Submit filter
3. **Expected**: Product list filtered to selected brand
4. **Verify**: Only products matching brand displayed
5. **Verify**: Filter dropdown shows selected brand

**Filter by Type**:
1. Select type from dropdown
2. Submit filter
3. **Expected**: Product list filtered to selected type
4. **Verify**: Only products matching type displayed

**Search Products**:
1. Enter search term (e.g., "shirt")
2. Submit search
3. **Expected**: Products matching search term displayed
4. **Verify**: Product names contain search term

**Pagination**:
1. Navigate to catalog with >8 products (default page size)
2. Click "Next" button
3. **Expected**: Page 2 of products displayed
4. **Verify**: Page indicator shows "Page 2 of X"
5. Click "Previous" button
6. **Expected**: Page 1 of products displayed

**Combined Filters**:
1. Select brand, type, and enter search term
2. Submit
3. **Expected**: Products matching ALL criteria displayed
4. Navigate to page 2
5. **Expected**: Filters preserved in pagination links

**Clear Filters**:
1. Apply filters
2. Click "Clear Filters"
3. **Expected**: All filters reset, full product list displayed

**Test Checklist**:
- [ ] Product list displays correctly
- [ ] Brand filter works
- [ ] Type filter works
- [ ] Search works
- [ ] Pagination works
- [ ] Filters persist across pagination
- [ ] Combined filters work correctly
- [ ] Clear filters works

---

#### Scenario 3: Shopping Cart

**Add Item to Cart (Anonymous User)**:
1. Navigate to catalog
2. Click "Add to Cart" on a product
3. **Expected**: Item added, confirmation message/toast displayed
4. Navigate to cart page
5. **Expected**: Item appears in cart with correct product, quantity, price

**Add Item to Cart (Authenticated User)**:
1. Log in
2. Navigate to catalog
3. Click "Add to Cart" on a product
4. **Expected**: Item added to user's cart
5. Navigate to cart page
6. **Expected**: Item appears in cart

**View Cart**:
1. Navigate to `/Basket` or cart page
2. **Expected**: All cart items displayed
3. **Verify**: Product names, images, prices correct
4. **Verify**: Quantities correct
5. **Verify**: Subtotal and total correct

**Update Quantity**:
1. In cart page, change quantity of an item
2. Submit or trigger update
3. **Expected**: Quantity updated, totals recalculated
4. **Verify**: Subtotal and total correct

**Remove Item**:
1. In cart page, click "Remove" on an item
2. **Expected**: Item removed from cart
3. **Verify**: Totals recalculated

**Empty Cart**:
1. Remove all items from cart
2. **Expected**: "Your cart is empty" message displayed

**Session Persistence** (Anonymous User):
1. Add items to cart
2. Navigate away from cart
3. Close browser tab (but not entire browser to preserve session)
4. Reopen tab and navigate back to site
5. Navigate to cart
6. **Expected**: Cart items still present

**Cart Merge** (if applicable):
1. Add items to cart as anonymous user
2. Log in
3. **Expected**: Anonymous cart items merge with user's cart OR user asked which cart to keep

**Test Checklist**:
- [ ] Add to cart works (anonymous)
- [ ] Add to cart works (authenticated)
- [ ] Cart displays correctly
- [ ] Update quantity works
- [ ] Remove item works
- [ ] Cart totals calculate correctly
- [ ] Session state persists
- [ ] Cart merge works (if applicable)

---

#### Scenario 4: Checkout (if applicable)

**Complete Order**:
1. Add items to cart
2. Navigate to checkout
3. Fill in shipping information
4. Fill in payment information (if applicable)
5. Submit order
6. **Expected**: Order created successfully
7. **Verify**: Order confirmation page displays
8. **Verify**: Order exists in database

**Test Checklist**:
- [ ] Checkout form loads
- [ ] Validation works
- [ ] Order submission works
- [ ] Order confirmation displays
- [ ] Order persists to database

---

#### Scenario 5: Session State

**Anonymous Session Tracking**:
1. Visit site without logging in
2. Add item to cart
3. Navigate to another page
4. Return to cart
5. **Expected**: Item still in cart (session persisted)

**Session Timeout**:
1. Add item to cart
2. Wait beyond session timeout (default 20 minutes)
3. Navigate to cart
4. **Expected**: Session expired, cart empty OR user prompted to re-login

**Authenticated Session**:
1. Log in
2. Add items to cart
3. Navigate away
4. **Expected**: Cart persists even after session timeout (tied to user account)

**Test Checklist**:
- [ ] Anonymous session creates unique buyer ID
- [ ] Anonymous session persists across requests
- [ ] Session timeout behavior correct
- [ ] Authenticated cart persists beyond session

---

### Level 4: Data Integrity Validation

**Objective**: Ensure database operations work correctly

**Database Read Operations**:
1. Verify all catalog items load from database
2. Verify brands and types load correctly
3. Verify user accounts load correctly

**Database Write Operations**:
1. Create new user → Verify user record in database
2. Add to cart → Verify basket items in database
3. Update cart quantity → Verify database reflects change
4. Complete order → Verify order record in database

**Database Queries** (Enable EF Core logging):
```csharp
// In Program.cs:
builder.Services.AddDbContext<eShopContext>(options =>
    options.UseSqlServer(connectionString)
           .LogTo(Console.WriteLine, LogLevel.Information));
```

**Check for**:
- [ ] No N+1 query problems (excessive queries)
- [ ] Queries execute efficiently
- [ ] No unexpected table scans
- [ ] Lazy loading behavior correct (or disabled)

**Test Checklist**:
- [ ] All reads work correctly
- [ ] All writes work correctly
- [ ] No data corruption
- [ ] Query performance acceptable

---

### Level 5: Performance Validation

**Objective**: Ensure acceptable performance

**Page Load Times**:
1. Measure page load times for key pages
2. Compare to baseline (if available)
3. **Acceptable**: <3 seconds for catalog, <1 second for static pages

**Database Query Performance**:
1. Enable EF Core logging
2. Review query execution times
3. **Acceptable**: Most queries <100ms

**Memory Usage**:
1. Monitor memory during testing
2. Check for memory leaks (increasing memory over time)
3. **Acceptable**: Stable memory profile

**Test Checklist**:
- [ ] Page load times acceptable
- [ ] Database queries efficient
- [ ] No memory leaks
- [ ] Concurrent user load acceptable (if applicable)

---

### Level 6: Security Validation

**Authentication Security**:
- [ ] Passwords hashed correctly (verify with test login)
- [ ] Authentication cookies HttpOnly
- [ ] Authentication cookies Secure (HTTPS only)
- [ ] Authentication cookies have SameSite attribute

**Authorization**:
- [ ] Unauthenticated users cannot access protected pages
- [ ] Authorization redirects to login page

**Session Security**:
- [ ] Session IDs not exposed in URLs
- [ ] Session cookies HttpOnly

**Input Validation**:
- [ ] Forms validate input
- [ ] SQL injection not possible (EF Core parameterizes queries)
- [ ] XSS protection active (Razor auto-encodes output)

**HTTPS**:
- [ ] HTTPS redirection enabled (`app.UseHttpsRedirection()`)
- [ ] HSTS enabled (`app.UseHsts()`)

**Test Checklist**:
- [ ] Authentication secure
- [ ] Authorization enforced
- [ ] Session secure
- [ ] Input validation works
- [ ] HTTPS enforced

---

### Level 7: Regression Testing

**Objective**: Ensure no functionality lost in migration

**Compare WebForms vs. ASP.NET Core**:
For each feature:
1. Document expected behavior from WebForms version
2. Perform same action in ASP.NET Core version
3. Compare results

**Focus Areas**:
- Business logic correctness
- Calculation accuracy (prices, totals, taxes)
- Workflow completeness (order processing, user registration)
- Error handling (validation messages, error pages)

**Test Checklist**:
- [ ] All WebForms features present in ASP.NET Core
- [ ] Business logic behaves identically
- [ ] No missing functionality

---

### Test Execution Strategy

**Phase 1: Development Testing**
- Continuous smoke testing as pages are converted
- Immediate fixes for broken pages

**Phase 2: Integrated Testing**
- Functional testing after all pages converted
- Critical path scenarios first (auth, catalog, cart)
- Extended scenarios second (edge cases, error handling)

**Phase 3: Performance & Security**
- Performance validation once functional tests pass
- Security validation before deployment

**Phase 4: User Acceptance Testing** (if applicable)
- End-users test real-world scenarios
- Feedback collected and issues fixed

---

### Test Environment Setup

**Database**:
- Use copy of production database (anonymized if needed)
- OR seed test data

**Configuration**:
- Configure `appsettings.Development.json` with test values
- Set `ASPNETCORE_ENVIRONMENT=Development`

**Logging**:
- Enable detailed logging for troubleshooting:
  ```json
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
  ```

**Browser DevTools**:
- Use browser console to check for JavaScript errors
- Use Network tab to verify API calls
- Use Application tab to inspect cookies and session storage

---

### Issue Tracking

**For each issue found**:
1. Document issue clearly (steps to reproduce, expected vs actual behavior)
2. Assign priority (Critical, High, Medium, Low)
3. Fix critical and high priority issues before proceeding
4. Track in issue tracker or checklist

**Example Issue Format**:
```
Issue: Login fails for existing users
Priority: CRITICAL
Steps to Reproduce:
1. Navigate to /Account/Login
2. Enter email: existing@user.com
3. Enter password: (correct password)
4. Submit
Expected: User logs in successfully
Actual: "Invalid login attempt" error
Root Cause: Password hash format incompatible between ASP.NET Identity 2.x and ASP.NET Core Identity
Resolution: Implement custom password hasher for migration period
```

---

### Success Criteria Summary

Migration testing is **complete** when:

- [x] Solution builds with 0 errors
- [ ] All pages load without exceptions
- [ ] Authentication flows work correctly
- [ ] Catalog browsing works correctly
- [ ] Shopping cart works correctly
- [ ] Checkout works correctly (if applicable)
- [ ] Session state persists correctly
- [ ] Database operations complete successfully
- [ ] No data integrity issues
- [ ] Performance acceptable
- [ ] Security measures active
- [ ] No critical or high priority issues remain
- [ ] User acceptance testing passed (if applicable)

---

## Complexity & Effort Assessment

### Project Complexity Matrix

| Project | Complexity | Dependencies | Risk | Rationale |
|---------|------------|--------------|------|-----------|
| eShopLegacy.csproj | 🔴 **CRITICAL** | 0 projects<br/>9 packages | 🔴 **CRITICAL** | WebForms → ASP.NET Core requires complete architectural rewrite. 680 API issues, 8 incompatible packages, 35.6% of codebase impacted. No direct migration path. |

### Detailed Complexity Breakdown

#### eShopLegacy.csproj — CRITICAL Complexity

**Factors Contributing to Complexity**:

1. **Architectural Paradigm Shift** (🔴 CRITICAL)
   - WebForms stateful server controls → MVC/Razor Pages stateless patterns
   - Server-side event model → request-response model
   - ViewState/PostBack → client-side state management
   - Page lifecycle (Page_Load, IsPostBack) → action methods/page handlers

2. **API Incompatibility** (🔴 CRITICAL)
   - 551 binary incompatible APIs (81% of issues)
   - 127 source incompatible APIs (19% of issues)
   - 95.4% of issues from `System.Web.*` namespace
   - No polyfills or compatibility layers available

3. **Package Ecosystem Replacement** (🔴 CRITICAL)
   - 8 of 9 packages incompatible (88.9%)
   - Requires replacing entire technology stacks:
     - OWIN → ASP.NET Core middleware
     - ASP.NET Identity → ASP.NET Core Identity
     - EF6 → EF Core
   - New package ecosystem with different APIs

4. **UI Layer Rewrite** (🔴 CRITICAL)
   - All `.aspx` pages must be converted to `.cshtml`
   - Server controls (TextBox, DropDownList, Repeater, GridView, etc.) must be replaced
   - Data binding patterns completely different
   - Event handlers → controller actions or page handlers

5. **Code Volume** (🟡 MEDIUM)
   - 1,912 LOC total
   - 680+ LOC requiring modification (35.6%)
   - 30 files with incidents (73% of code files)
   - Moderate size, but high percentage impacted

**Relative Complexity Rating**: **CRITICAL**

While the project is small in absolute size (single project, ~2K LOC), the **architectural transformation depth** elevates this to CRITICAL complexity:
- Not a framework upgrade—a platform migration
- No incremental path—full rewrite required
- Multiple technology stack replacements
- High risk of functionality regression

### Phase Complexity Assessment

Since this is a single-project All-At-Once migration, complexity assessment focuses on **technology layer complexity**:

#### Phase 0: Prerequisites — LOW Complexity
- SDK installation (automated)
- Project file conversion (semi-automated with manual review)
- Estimated relative effort: **LOW**

#### Phase 1: Atomic Transformation — CRITICAL Complexity

**By Technology Layer**:

| Layer | Complexity | LOC Impact | Key Challenges |
|-------|------------|------------|----------------|
| **UI Layer** | 🔴 CRITICAL | ~400 LOC | WebForms → Razor Pages: controls, events, ViewState, data binding all require replacement |
| **Authentication** | 🔴 HIGH | ~150 LOC | Identity models, managers, middleware, cookie configuration all different |
| **Middleware** | 🟡 MEDIUM | ~50 LOC | OWIN Startup → ASP.NET Core Program.cs, pipeline patterns differ |
| **Data Access** | 🟡 MEDIUM | ~80 LOC | EF6 → EF Core: DbContext patterns similar but LINQ query differences |
| **Configuration** | 🟡 MEDIUM | N/A | Web.config → appsettings.json: structure and loading differ |

**Overall Phase 1 Effort**: **CRITICAL**

#### Phase 2: Test Validation — MEDIUM Complexity
- Functional testing required (no automated tests assumed)
- Authentication flow validation
- Data integrity verification
- Estimated relative effort: **MEDIUM**

### Resource Requirements

**Skills Required**:
- ✅ **ASP.NET Core expertise** (REQUIRED): Must understand Razor Pages/MVC, middleware pipeline, dependency injection
- ✅ **ASP.NET Core Identity expertise** (REQUIRED): Authentication and authorization patterns
- ✅ **Entity Framework Core expertise** (REQUIRED): DbContext configuration, migrations, LINQ differences
- ✅ **WebForms knowledge** (HELPFUL): Understanding legacy code facilitates conversion
- ✅ **Front-end skills** (HELPFUL): HTML, JavaScript for client-side state management

**Team Capacity**:
- **Minimum**: 1 experienced full-stack .NET developer
- **Recommended**: 2 developers (one focused on UI, one on backend/auth)
- **Parallel Work**: Limited opportunities due to single project

### Effort Estimation Guidance

⚠️ **No Time Estimates Provided**: This plan does not include hour/day/week estimates because:
- Agent cannot reliably predict development speed
- Complexity varies based on developer experience
- Unforeseen issues common in architectural migrations
- Better to measure progress by milestones

**Relative Complexity Scale**:
- **LOW**: Routine framework upgrade, minimal code changes
- **MEDIUM**: Moderate refactoring, some API replacements
- **HIGH**: Significant refactoring, major architectural adjustments
- **CRITICAL**: Complete rewrite, platform migration, multiple technology stack replacements

**This Migration**: **CRITICAL** — Expect significant effort comparable to building a new application with similar functionality.

---

## Source Control Strategy

### Branch Strategy

**Current Setup**:
- **Source Branch**: `main`
- **Upgrade Branch**: `upgrade-to-NET10` (already created)

**Branch Workflow**:
```
main
 └── upgrade-to-NET10 (feature branch)
```

### Commit Strategy

Given the All-At-Once strategy and the architectural nature of this migration, a **structured incremental commit approach** is recommended:

#### Phase 0: Prerequisites
```bash
# After SDK-style conversion
git add eShopLegacy/eShopLegacy.csproj
git commit -m "Convert eShopLegacy.csproj to SDK-style"
```

#### Phase 1: Atomic Transformation

**Option 1: Granular Commits (Recommended)**

Commit functional increments to preserve working history and enable easier troubleshooting:

```bash
# 1. After project file and package updates
git add eShopLegacy/eShopLegacy.csproj
git commit -m "Update target framework to net10.0 and replace packages

- Set TargetFramework to net10.0
- Remove OWIN and ASP.NET Identity packages
- Add ASP.NET Core and EF Core packages"

# 2. After Program.cs created and Startup.cs removed
git add eShopLegacy/Program.cs
git add eShopLegacy/Startup.cs
git add eShopLegacy/App_Start/
git commit -m "Replace OWIN startup with ASP.NET Core Program.cs

- Create Program.cs with middleware pipeline
- Remove Startup.cs and App_Start folder
- Configure dependency injection"

# 3. After Identity models and DbContext updated
git add eShopLegacy/Identity/
git add eShopLegacy/DAL/ApplicationDbContext.cs
git commit -m "Update Identity models and DbContext to ASP.NET Core

- Update ApplicationUser and ApplicationRole namespaces
- Convert ApplicationDbContext to EF Core
- Update UserManager/SignInManager usage"

# 4. After data access layer updated
git add eShopLegacy/DAL/
git add eShopLegacy/Models/
git commit -m "Update data access layer to EF Core

- Convert eShopContext to EF Core
- Update service classes to use DbContextOptions
- Update LINQ queries for EF Core compatibility"

# 5. After master page and layout converted
git add eShopLegacy/Pages/Shared/
git add eShopLegacy/Site.Master
git commit -m "Convert master page to Razor layout

- Create _Layout.cshtml
- Create _ViewStart.cshtml and _ViewImports.cshtml
- Remove Site.Master"

# 6. After each major page conversion (example for catalog)
git add eShopLegacy/Pages/Catalog/
git add eShopLegacy/Catalog/Default.aspx
git add eShopLegacy/Catalog/Default.aspx.cs
git commit -m "Convert catalog page from WebForms to Razor Pages

- Create Pages/Catalog/Index.cshtml
- Create Pages/Catalog/Index.cshtml.cs
- Preserve all filtering, pagination, and add-to-cart functionality
- Remove WebForms files"

# 7-N. Repeat for remaining pages
git commit -m "Convert account pages from WebForms to Razor Pages"
git commit -m "Convert basket page from WebForms to Razor Pages"
git commit -m "Convert checkout page from WebForms to Razor Pages"

# N+1. After configuration updated
git add eShopLegacy/appsettings.json
git add eShopLegacy/appsettings.Development.json
git add eShopLegacy/Web.config
git commit -m "Replace Web.config with appsettings.json"

# N+2. After static files moved
git add eShopLegacy/wwwroot/
git add eShopLegacy/Content/
git add eShopLegacy/Scripts/
git commit -m "Move static files to wwwroot

- Move Content/ to wwwroot/css/
- Move Scripts/ to wwwroot/js/
- Update references in layouts"

# N+3. After Global.asax removed and routing updated
git add eShopLegacy/Global.asax
git add eShopLegacy/Global.asax.cs
git commit -m "Remove Global.asax and update routing

- Remove Global.asax files
- Routing now handled by Razor Pages conventions"

# N+4. After all compilation errors fixed
git add .
git commit -m "Fix compilation errors and complete migration

- Resolve all System.Web namespace errors
- Update HttpContext and Session usage
- Fix EF Core query issues
- Solution builds successfully"
```

**Option 2: Single Commit (Alternative, Less Recommended)**

If the team prefers a clean history with single merge commit:

```bash
# Perform ALL migration work on upgrade-to-NET10 branch
# Commit intermediate progress locally for safety, but squash before merge

# After all work complete and tested:
git add .
git commit -m "Migrate eShopLegacy from .NET Framework 4.8 to .NET 10.0

Complete architectural transformation:
- Convert WebForms to ASP.NET Core Razor Pages
- Replace ASP.NET Identity 2.x with ASP.NET Core Identity
- Replace OWIN with ASP.NET Core middleware
- Replace Entity Framework 6 with EF Core 10
- Update all dependencies to .NET 10 compatible versions

All functionality preserved and tested."
```

**Recommendation**: **Option 1 (Granular Commits)** is strongly recommended because:
- Easier to identify when issues were introduced
- Can cherry-pick or revert specific changes if needed
- Better documentation of migration process
- Easier for code review (can review in phases)
- Safer for troubleshooting

#### Phase 2: Testing

```bash
# After functional tests pass
git commit -m "Add test validation notes and update documentation"
```

### Commit Message Format

Use clear, descriptive commit messages following this template:

```
<Short summary (50 chars or less)>

<Detailed description of changes (wrap at 72 chars)>

<What was changed>
<Why it was changed>
<Any breaking changes or important notes>
```

**Example**:
```
Convert catalog page from WebForms to Razor Pages

Replace Catalog/Default.aspx with Pages/Catalog/Index.cshtml. All
functionality preserved:
- Product listing with pagination
- Brand and type filters
- Search functionality
- Add to cart

Breaking changes:
- URLs changed from /Catalog/Default.aspx to /Catalog
- ViewState replaced with query string parameters
```

### Code Review Process

**Review Checkpoints**:

1. **After Project File Conversion** (Phase 0)
   - Verify SDK-style conversion correct
   - Check TargetFramework and package references

2. **After Package Migration**
   - Verify all incompatible packages removed
   - Verify all required packages added
   - Check package versions consistent

3. **After Each Major Component** (Phase 1)
   - Verify functionality preserved
   - Check for code quality issues
   - Ensure no WebForms artifacts remain

4. **Before Final Merge**
   - Full code review of all changes
   - Verify all tests pass
   - Check performance acceptable
   - Confirm no regressions

**Review Criteria**:
- [ ] Code follows ASP.NET Core best practices
- [ ] No WebForms patterns remain
- [ ] Proper use of dependency injection
- [ ] Async/await used appropriately
- [ ] Error handling adequate
- [ ] Security measures in place
- [ ] No hardcoded values (use configuration)
- [ ] Comments explain complex logic
- [ ] No dead code or commented-out code

### Merge Strategy

**After All Tests Pass**:

```bash
# Ensure upgrade-to-NET10 is up to date
git checkout upgrade-to-NET10
git pull origin upgrade-to-NET10

# Merge main into upgrade-to-NET10 if needed (to include any parallel changes)
git merge main
# Resolve conflicts if any
# Test again after merge

# Switch to main and merge
git checkout main
git merge upgrade-to-NET10 --no-ff -m "Merge .NET 10 migration"

# Push to remote
git push origin main
```

**Merge Options**:

1. **Feature Branch Merge (Recommended)**
   ```bash
   git merge upgrade-to-NET10 --no-ff
   ```
   - Preserves feature branch history
   - Clear merge commit shows when migration completed
   - Easier to revert entire migration if needed

2. **Squash Merge** (if clean history preferred)
   ```bash
   git merge upgrade-to-NET10 --squash
   git commit -m "Migrate to .NET 10.0"
   ```
   - Single commit in main
   - Cleaner history
   - Loses intermediate commits (but preserved in feature branch)

3. **Rebase** (not recommended for large migrations)
   ```bash
   git rebase main
   ```
   - Linear history
   - Risky for large changes with many commits
   - Can complicate conflict resolution

**Recommendation**: **Feature Branch Merge (--no-ff)** preserves full history and makes the migration clearly visible.

### Rollback Plan

**If Critical Issue Discovered After Merge**:

1. **Immediate Revert** (if issue found quickly):
   ```bash
   git revert -m 1 <merge-commit-sha>
   git push origin main
   ```
   - Safe operation, creates new commit undoing merge
   - Preserves history

2. **Forward Fix** (preferred if issue is minor):
   ```bash
   git checkout main
   git checkout -b hotfix-migration-issue
   # Fix issue
   git commit -m "Fix migration issue: <description>"
   git checkout main
   git merge hotfix-migration-issue
   git push origin main
   ```
   - Fixes issue while preserving migration
   - Preferred approach if migration mostly successful

3. **Hard Reset** (emergency only, requires force push):
   ```bash
   git reset --hard <commit-before-merge>
   git push origin main --force
   ```
   - ⚠️ DANGEROUS: rewrites history, affects all developers
   - Only use if merge must be completely undone and no one else has pulled

### Branch Cleanup

**After Successful Merge and Deployment**:

```bash
# Delete local branch
git branch -d upgrade-to-NET10

# Delete remote branch
git push origin --delete upgrade-to-NET10
```

**Recommendation**: Keep upgrade branch for some period (e.g., 1-2 weeks) in case rollback needed. Delete only after confident migration is stable.

### All-At-Once Strategy Specific Guidance

The All-At-Once strategy means:
- All project file and package changes committed together (or in rapid sequence)
- All code changes across UI, auth, data access layers committed together (or in logical groupings)
- **Single feature branch** (`upgrade-to-NET10`) contains all migration work
- **Single merge** brings entire migration into `main`

However, **within the feature branch**, use granular commits to:
- Preserve working states
- Enable troubleshooting
- Document migration progress
- Facilitate code review

The "atomic" nature applies to the **deployment**, not the **development process**.

### Continuous Integration (if applicable)

**CI Pipeline Configuration**:

If CI/CD pipeline exists:

```yaml
# Example: .github/workflows/dotnet.yml or .gitlab-ci.yml
name: .NET 10 Build

on:
  push:
    branches: [ main, upgrade-to-NET10 ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET 10
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 10.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore --configuration Release
    - name: Test
      run: dotnet test --no-build --configuration Release --verbosity normal
```

**Benefits**:
- Automatic build verification on each commit
- Early detection of compilation errors
- Test execution automation

---

### Summary

**Recommended Approach**:
1. ✅ Work on dedicated `upgrade-to-NET10` branch
2. ✅ Make granular commits for each logical step (project file, identity, pages, etc.)
3. ✅ Write clear commit messages documenting what and why
4. ✅ Conduct code reviews at logical checkpoints
5. ✅ Merge to `main` using `--no-ff` to preserve history
6. ✅ Keep branch for 1-2 weeks after merge before deleting

This approach balances the All-At-Once strategy (single coordinated migration) with practical development workflow (incremental commits, code review, troubleshooting capability).

---

## Success Criteria

The migration from .NET Framework 4.8 to .NET 10.0 is considered **complete and successful** when all criteria in this section are met.

### Technical Criteria

#### 1. Compilation & Build

- [x] Solution builds with 0 errors
  - Command: `dotnet build --configuration Release`
  - No CS0xxx compilation errors
  - No package restore errors
  - No missing reference errors

- [ ] Warnings minimized to acceptable level
  - Nullable reference warnings acceptable if `<Nullable>enable</Nullable>`
  - No critical or high-severity warnings
  - All remaining warnings documented and justified

- [ ] All projects target net10.0
  - `<TargetFramework>net10.0</TargetFramework>` in eShopLegacy.csproj

- [ ] All incompatible packages removed
  - No Microsoft.Owin.* packages
  - No Microsoft.AspNet.Identity.* packages
  - No EntityFramework (EF6) package

- [ ] All required packages added at correct versions
  - Microsoft.EntityFrameworkCore 10.0.0
  - Microsoft.EntityFrameworkCore.SqlServer 10.0.0
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.0

- [ ] No package dependency conflicts
  - `dotnet list package` shows no conflicts
  - All packages compatible with net10.0

#### 2. Runtime & Functionality

- [ ] Application starts without errors
  - `dotnet run` succeeds
  - No startup exceptions
  - Application listens on expected port

- [ ] All pages load successfully
  - No HTTP 500 Internal Server Errors
  - No unhandled exceptions
  - All routes resolve correctly

- [ ] Authentication system functional
  - User registration works
  - User login works
  - User logout works
  - Password hashing compatible with legacy users (if applicable)
  - Authorization enforces access control

- [ ] Catalog browsing functional
  - Product list displays correctly
  - Brand filter works
  - Type filter works
  - Search works
  - Pagination works
  - Filters persist across navigation

- [ ] Shopping cart functional
  - Add to cart works
  - Cart displays correct items and totals
  - Update quantity works
  - Remove item works
  - Session state persists

- [ ] Checkout functional (if applicable)
  - Order submission works
  - Order persists to database
  - Confirmation displays correctly

- [ ] Session state works correctly
  - Anonymous sessions create unique buyer IDs
  - Session persists across requests
  - Session timeout behavior correct
  - Authenticated sessions persist cart data

- [ ] Database operations successful
  - All reads succeed
  - All writes succeed
  - No data integrity issues
  - EF Core migrations applied (if using migrations)

### Quality Criteria

#### 3. Code Quality

- [ ] No WebForms artifacts remain
  - No `.aspx` files
  - No `.aspx.cs` code-behind files
  - No `System.Web.UI.*` using statements
  - No ViewState usage
  - No PostBack patterns

- [ ] No OWIN artifacts remain
  - No `Startup.cs` with OWIN configuration
  - No `app.CreatePerOwinContext()` calls
  - No OWIN middleware registrations

- [ ] ASP.NET Core patterns followed
  - Dependency injection used throughout
  - Async/await used for I/O operations
  - Tag helpers used in views
  - Options pattern used for configuration
  - Middleware registered in correct order

- [ ] Data access patterns updated
  - EF Core DbContext uses dependency injection
  - No `using (var ctx = new Context())` patterns
  - Lazy loading explicitly configured or avoided
  - Eager loading with `.Include()` where needed

- [ ] Configuration migrated
  - Web.config removed or only contains deployment settings
  - appsettings.json contains application settings
  - appsettings.Development.json for dev overrides
  - Connection strings in configuration, not hardcoded

- [ ] Error handling adequate
  - Exception handling middleware configured
  - User-friendly error pages
  - Logging configured
  - No sensitive information exposed in errors

- [ ] Security measures in place
  - HTTPS redirection enabled
  - HSTS configured for production
  - Authentication cookies secure (HttpOnly, Secure, SameSite)
  - Input validation active
  - Output encoding automatic (Razor)
  - CSRF protection enabled (default)

#### 4. Testing & Validation

- [ ] All functional tests pass
  - Critical path scenarios validated
  - Edge cases tested
  - Error handling tested

- [ ] No regressions identified
  - All WebForms functionality preserved
  - Business logic behaves identically
  - Calculations correct (prices, totals, taxes)

- [ ] Performance acceptable
  - Page load times <3 seconds for catalog
  - Database queries efficient (no N+1 issues)
  - Memory usage stable
  - No memory leaks

- [ ] Security validation passed
  - Authentication secure
  - Authorization enforced
  - Session management secure
  - No vulnerabilities in packages
    - `dotnet list package --vulnerable` returns no vulnerabilities

- [ ] Cross-browser testing completed (if applicable)
  - Chrome
  - Firefox
  - Safari
  - Edge

### Process Criteria

#### 5. Documentation & Knowledge Transfer

- [ ] Migration documented
  - This plan.md completed and accurate
  - Key decisions documented
  - Workarounds and tradeoffs noted

- [ ] Configuration documented
  - appsettings.json structure explained
  - Environment-specific settings documented
  - Connection string format documented

- [ ] Breaking changes documented
  - All API changes noted
  - Behavioral differences documented
  - Known issues and limitations documented

- [ ] Deployment guide created
  - Prerequisites listed (.NET 10 SDK, database setup)
  - Deployment steps documented
  - Rollback procedure documented

- [ ] Team knowledge transfer completed (if applicable)
  - Team trained on ASP.NET Core patterns
  - Key differences from WebForms explained
  - Troubleshooting guide provided

#### 6. Source Control & Deployment

- [ ] All changes committed
  - No uncommitted files
  - Commit history logical and documented
  - Commit messages clear

- [ ] Code reviewed
  - All changes reviewed by peer(s)
  - Review feedback addressed
  - Approval obtained

- [ ] Merged to main branch
  - Feature branch merged successfully
  - No merge conflicts remain
  - Main branch builds successfully

- [ ] CI/CD pipeline updated (if applicable)
  - Pipeline builds .NET 10 project
  - All pipeline jobs pass
  - Deployment automation updated

- [ ] Deployment successful (when ready)
  - Application deployed to target environment
  - Smoke tests pass in deployed environment
  - No production errors

### All-At-Once Strategy Criteria

#### 7. Strategy-Specific Success

- [ ] Atomic transformation completed
  - All components migrated together (UI, auth, data, middleware)
  - No partial migration states
  - Application in consistent architectural state

- [ ] No compatibility shims or adapters
  - No System.Web.Adapters package
  - No OWIN compatibility layers
  - Clean ASP.NET Core implementation

- [ ] Single coordinated operation
  - All project file updates applied
  - All package updates applied
  - All code migrations applied
  - All compilation errors fixed
  - All in single feature branch

### Final Checklist

**Before declaring migration complete, verify**:

- [ ] ✅ All technical criteria met
- [ ] ✅ All quality criteria met
- [ ] ✅ All process criteria met
- [ ] ✅ All strategy-specific criteria met
- [ ] ✅ No critical or high-priority issues remain
- [ ] ✅ Stakeholder approval obtained (if applicable)
- [ ] ✅ Production deployment plan ready
- [ ] ✅ Rollback plan documented and tested

### Definition of Done

**The migration is COMPLETE when**:

1. ✅ **Application builds** without errors
2. ✅ **All features work** as expected
3. ✅ **No regressions** identified
4. ✅ **Performance acceptable**
5. ✅ **Security validated**
6. ✅ **Tests pass** (functional, integration, performance)
7. ✅ **Documentation complete**
8. ✅ **Code reviewed and approved**
9. ✅ **Merged to main branch**
10. ✅ **Ready for deployment**

**The migration is SUCCESSFUL when**:

- Application runs in production environment without issues
- Users can perform all workflows successfully
- No critical bugs reported
- Performance meets or exceeds expectations
- Security posture maintained or improved

---

### Post-Migration Activities

After migration declared complete:

1. **Monitor Production** (first 24-48 hours)
   - Watch error logs closely
   - Monitor performance metrics
   - Track user feedback
   - Be ready for hotfixes

2. **Address Minor Issues**
   - Fix any low-priority bugs found
   - Optimize performance if needed
   - Improve user experience based on feedback

3. **Update Dependencies** (ongoing)
   - Keep packages up to date
   - Apply security patches promptly
   - Monitor for .NET 10 updates

4. **Knowledge Base**
   - Document common issues and solutions
   - Create troubleshooting guide
   - Share lessons learned with team

5. **Celebrate Success** 🎉
   - Acknowledge team effort
   - Document migration success story
   - Share knowledge with broader organization
