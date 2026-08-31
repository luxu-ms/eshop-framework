# eShopLegacy Incremental Migration Plan

## Goal and constraints

Migrate the application from ASP.NET Web Forms on .NET Framework 4.8 to ASP.NET Core Razor Pages on .NET 10 while preserving behavior and keeping a buildable, testable, runnable application after every step.

`.NET Core` is called `.NET` starting with .NET 5. This plan targets `net10.0-windows`, the current LTS runtime, because the application uses SQL Server LocalDB and the 2025 modernization map has a direct .NET Framework 4.x -> .NET 10 minimal-change path.

The migration rules are:

- Preserve the existing `eShopLegacy` project as the runnable fallback until final cutover.
- Add a side-by-side `eShop.Web` ASP.NET Core project; do not retarget Web Forms because Web Forms is unsupported on modern .NET.
- Prefer copied code and small adapters over speculative redesign.
- Keep Entity Framework 6 and the existing schema. EF6 -> EF Core is explicitly deferred because it is not required to move runtimes.
- Keep synchronous service signatures, namespaces, CSS, JavaScript, URLs, and HTML behavior where practical.
- Set `Nullable` and `ImplicitUsings` to `disable` in migrated projects to avoid unrelated code churn.
- Do not introduce React, Blazor, microservices, containers, Azure, or a repository layer as part of this migration.
- A step is complete only when both applications compile, automated tests pass, and its live test passes.

## Current-state findings

| Area | Current implementation | Migration consequence |
|---|---|---|
| Runtime/UI | .NET Framework 4.8, Web Forms, ten `.aspx` pages, one master page | UI must move to Razor Pages or MVC; use Razor Pages for the closest page-oriented mapping. |
| Request model | `Page_Load`, `IsPostBack`, Web Forms event handlers, `Repeater`, and `GridView` | GET/POST page handlers, model binding, tag helpers, and POST-Redirect-GET replace page lifecycle behavior. |
| Data | EF 6.4.4 Code First and LocalDB | Keep EF6 initially and share the existing schema. Do not let two hosts initialize or mutate the schema concurrently. |
| Authentication | ASP.NET Identity 2 with Katana/OWIN cookies | Preserve OWIN/Identity first with System.Web OWIN adapters; migrate identity only if adapter verification fails. |
| State | In-process session stores `AnonymousBuyerId`; basket contents are in SQL | Introduce one buyer-ID adapter backed by a cookie so both hosts address the same basket. Do not try to share InProc session. |
| Configuration | `Web.config`, transforms, connection string, one functional setting | Keep legacy configuration untouched; bind equivalent values in the Core host through configuration. |
| Dependencies | Legacy project with `packages.config` and explicit `packages/` hint paths | Restore with NuGet before baseline build; do not use `dotnet restore` as proof for this project. |
| Tests | No test project | Add characterization and browser smoke tests before routing production behavior to Core. |

The baseline build was checked on 2026-08-31. .NET 10 SDKs are installed, but `dotnet build eShopLegacy.sln` fails because the `packages/` directory is absent and `packages.config` dependencies are not restored. Step 0 fixes this before migration work begins.

## Target checkpoint architecture

```text
Browser
  |
  v
eShop.Web (ASP.NET Core 10, public front door)
  |-- migrated Razor Page -----------------> EF6 commerce context -> LocalDB
  `-- unmigrated path via reverse proxy ---> eShopLegacy/IIS Express -> same LocalDB
```

Only the Core host is exposed during side-by-side operation. Routing one path back to legacy is the rollback mechanism for each page slice. The final checkpoint removes all fallback routes and the runtime dependency on IIS/System.Web.

## Sequence overview

| Step | Targeted X -> Y transformation | Runnable checkpoint |
|---|---|---|
| 0 | Missing `packages/` + unverified behavior -> restored, characterized .NET Framework baseline | Entire app on IIS Express |
| 1 | Ad hoc build/run -> repeatable build, test, and smoke-test commands | Entire app on IIS Express |
| 2 | IIS/Web Forms as public host -> ASP.NET Core 10 front door + legacy fallback | Core URL serves health/static content and proxies every feature |
| 3 | `Web.config`-only settings + Identity-coupled EF context -> Core configuration + EF6 commerce context adapter | Both hosts read the same database |
| 4 | `Site.Master` + root redirect -> Razor layout + Core routing/static files | Core shell; all feature paths still work |
| 5 | Session buyer ID + catalog/cart Web Forms pages -> shared buyer cookie + Razor catalog/cart pages | Anonymous browse-to-cart runs fully on Core |
| 6 | Identity 2/OWIN Web Forms account pages -> OWIN adapter + Razor account pages | Register, login, logout, and basket transfer run on Core |
| 7 | Checkout/order Web Forms pages -> Razor checkout/order pages | Authenticated purchase flow runs on Core |
| 8 | `GridView` admin page -> Razor admin page | All user-facing routes run on Core |
| 9 | Legacy fallback + dual configuration -> Core-only runtime + deployment package | Core host runs independently |

## Step 0 - Restore and freeze the baseline

**X -> Y:** unrestored `packages.config` application with no verified build -> reproducibly restored and behavior-baselined .NET Framework 4.8 application.

### Minimal changes

1. Restore `eShopLegacy/packages.config` with `nuget restore eShopLegacy.sln -PackagesDirectory packages`; do not edit source to hide missing references.
2. Add a small `tests/eShop.Legacy.SmokeTests` browser test project and test data reset script. Use Playwright against IIS Express and a disposable LocalDB database copied from a known seed.
3. Capture route, status, visible text, redirect, cookie, and database effects for:
   - `/` redirect and catalog filtering/paging/search.
   - Product detail and anonymous add-to-cart/update/remove.
   - Register/login/logout and anonymous-basket transfer.
   - Checkout, order complete, and order history.
   - Authenticated access to admin product create/edit/delete. The current application checks authentication, not an admin role; preserve that behavior during migration.
4. Never use a real payment card in test data. Assert that persisted card numbers are masked. Record the existing persistence of `CardSecurityNumber` as security debt, but do not combine its remediation with runtime migration.

### Exit criteria

**Acceptance criteria**

- `packages/` is restored from the checked-in manifest without manual DLL copying.
- Debug and Release builds of `eShopLegacy.sln` succeed with no missing EF/Identity/OWIN references.
- A clean seeded database produces the 12 documented catalog items.
- Baseline screenshots and observable results are stored as test artifacts, not committed binaries.

**Test plan**

- Run `nuget restore eShopLegacy.sln -PackagesDirectory packages`.
- Run `msbuild eShopLegacy.sln /t:Build /p:Configuration=Debug` and repeat for Release.
- Run the complete legacy Playwright characterization suite twice from a reset database to prove repeatability.

**Live test**

- Start IIS Express, open `/`, browse a product, add it to the cart, register a user, complete one order, and view it in order history. Confirm the app remains usable after a process restart.

## Step 1 - Establish the migration safety net

**X -> Y:** Visual Studio-only workflow -> one documented build/test/run workflow for both legacy and future Core hosts.

### Minimal changes

1. Add scripts or solution targets for `restore`, `build`, `test`, `run-legacy`, and later `run-all`; scripts must fail on the first failed command.
2. Add `eShop.Tests` for behavior tests that do not require a browser. Initially test `CatalogService`, `BasketService`, `OrderService`, card masking through order creation, and database initialization against disposable LocalDB.
3. Add a `/health` equivalent to the legacy host only if it can be done without changing application behavior; otherwise use the catalog GET as the legacy readiness probe.
4. Document ports and database override variables. Never point automated tests at the developer database.

### Exit criteria

**Acceptance criteria**

- A new checkout can restore, build, create/reset its test database, launch the legacy host, and run tests from documented commands.
- Unit/integration and browser tests are independent and can run repeatedly.
- Test failures retain server logs and screenshots.

**Test plan**

- Run the scripted Debug build and all non-browser tests.
- Run the legacy host readiness probe and the Playwright smoke suite.
- Deliberately use an invalid test connection string and confirm the workflow fails before touching any non-test database.

**Live test**

- Run the documented single-host command, use the displayed URL in a browser, and repeat the baseline purchase journey.

## Step 2 - Add the ASP.NET Core front door

**X -> Y:** public IIS/Web Forms host -> `eShop.Web` ASP.NET Core 10 host with reverse-proxy fallback to unchanged Web Forms.

### Minimal changes

1. Add `eShop.Web` with `Microsoft.NET.Sdk.Web`, `TargetFramework=net10.0-windows`, `Nullable=disable`, and `ImplicitUsings=disable`.
2. Add only the reverse-proxy package required for fallback routing. Configure a catch-all route to the private IIS Express legacy URL and preserve path, query string, method, headers, response status, and redirects.
3. Add `/health` to the Core host. It reports unhealthy when its legacy fallback is unavailable while fallback routes remain.
4. Configure forwarded headers and a fixed public origin so redirects and secure cookies use the Core URL.
5. Add both projects to the solution. The legacy project remains unmodified and directly runnable.

### Exit criteria

**Acceptance criteria**

- The solution builds both hosts.
- The Core URL serves `/health`; every application path still reaches the legacy application through one public origin.
- Query strings, POST bodies, cookies, redirects, CSS, JavaScript, and product images survive proxying.
- Stopping Core does not prevent direct legacy rollback; stopping legacy makes `/health` fail clearly.

**Test plan**

- Run `dotnet build eShopLegacy.sln --configuration Debug` after NuGet restore.
- Run proxy contract tests for GET, POST, redirect, cookie, static file, 404, and query-string behavior.
- Run the unchanged Step 0 Playwright suite against the Core URL.

**Live test**

- Start both hosts, use only the Core URL, and complete the baseline purchase journey. Verify browser navigation never exposes the private legacy origin.

## Step 3 - Add Core configuration and an EF6 commerce adapter

**X -> Y:** `Web.config` plus `IdentityDbContext<ApplicationUser>` as the only data entry point -> ASP.NET Core configuration plus a modern-.NET-compatible EF6 commerce context.

### Minimal changes

1. Copy commerce entities and the three DAL services into a modern class library or `eShop.Web`; preserve namespaces and method signatures where possible.
2. Add an EF6 `CommerceContext : DbContext` adapter containing only catalog, basket, and order sets/mappings. It maps the existing tables but does not derive from Identity 2 or create a second schema.
3. Keep EF6 and its LINQ queries. Change only the constructor/context type needed by the copied services. Do not migrate to EF Core, repositories, async, or new domain models.
4. Add the existing connection string and `CatalogItemsPerPage=10` to Core configuration. Permit environment-variable overrides. Keep the legacy `Web.config` unchanged.
5. Ensure database initialization has a single owner. During side-by-side execution, initialize/seed through legacy before Core starts; Core validates schema and fails fast instead of creating it.

### Exit criteria

**Acceptance criteria**

- Both hosts connect to the same disposable database and see identical catalog, basket, and order rows.
- Starting both hosts concurrently does not run competing initializers or alter schema.
- Existing service inputs, outputs, ordering, filtering, masking, and transaction effects are unchanged.
- No ASP.NET Identity 2 reference is required by the Core commerce context.

**Test plan**

- Run the same data-service contract tests against legacy `eShopContext` and Core `CommerceContext` and compare results.
- Test missing/invalid configuration and missing schema fail at startup with a useful error.
- Test one host writes a basket/order and the other reads the same values.

**Live test**

- With both hosts running, add an item through the proxied legacy UI and use a temporary diagnostic test endpoint or debugger to verify the Core service reads the same basket; remove the diagnostic endpoint before completing the step.

## Step 4 - Move the application shell

**X -> Y:** `Site.Master`, `Default.aspx`, and Web Forms static paths -> `_Layout.cshtml`, Core root routing, and `wwwroot`.

### Minimal changes

1. Copy the existing Bootstrap, jQuery, site CSS, favicon, placeholder, and product images into `wwwroot` without upgrading or restyling them.
2. Convert `Site.Master` markup to `_Layout.cshtml`; use normal links and `RenderBody`. Keep the same navigation labels and responsive behavior.
3. Map `/` to the catalog destination while preserving the observed redirect semantics from Step 0.
4. Keep the basket-count and authentication portions of the header proxied/neutral until their adapters are available; do not display fabricated state.
5. Route all feature paths to legacy explicitly. The catch-all remains the safety net.

### Exit criteria

**Acceptance criteria**

- Root, shared layout, and static assets are served by Core; feature links still work through fallback.
- No broken asset requests, mixed origins, duplicated forms, or visible layout regressions occur.
- Old bookmarked asset URLs either remain valid or redirect permanently to equivalent Core URLs.

**Test plan**

- Add `WebApplicationFactory` tests for `/`, headers, static files, and redirects.
- Run screenshot comparison at desktop and mobile widths with an agreed small rendering tolerance.
- Run the full browser suite through the Core front door.

**Live test**

- Open the Core root on desktop and a mobile viewport, navigate every header link, and verify CSS, JavaScript, favicon, and product images load without browser-console errors.

## Step 5 - Move catalog, product detail, and cart

**X -> Y:** `Page_Load`/postback catalog and cart pages plus InProc `AnonymousBuyerId` -> Razor GET/POST handlers plus a shared buyer-ID cookie adapter.

### Minimal changes

1. Introduce `IBuyerIdAccessor` at the presentation edge. For authenticated users it returns `User.Identity.Name`; otherwise it reads or creates an opaque `eshop-anon-id` cookie.
2. Change the four legacy buyer-ID call sites to use the same cookie as a fallback during coexistence. Retain recognition of an existing session ID and copy it into the cookie so active anonymous baskets are not lost.
3. Convert these pages as one state-consistent slice:
   - `Catalog/Default.aspx` -> `/Catalog/Default.aspx` Razor Page, preserving `page`, `brand`, `type`, and `q` query names.
   - `Catalog/ProductDetail.aspx` -> same-path Razor Page, preserving `id`.
   - `Cart/ShoppingCart.aspx` -> same-path Razor Page.
4. Map `Repeater` markup to Razor loops. Map `IsPostBack`/event handlers to explicit GET and antiforgery-protected POST handlers followed by redirects.
5. Keep service calls and synchronous behavior unchanged. Preserve old friendly aliases only if the baseline has them.
6. Switch these exact paths from proxy to Core together. Keep a route-level configuration switch that can return the slice to legacy without redeployment.

### Exit criteria

**Acceptance criteria**

- Search, filters, pagination, product detail, add, quantity update, remove, totals, and basket count match baseline.
- An anonymous basket created on either host is visible on the other host during rollback.
- Invalid IDs/quantities and empty-cart behavior match baseline status/redirect behavior.
- Migrated POSTs reject missing/invalid antiforgery tokens without weakening legacy behavior.

**Test plan**

- Add handler tests for every query/filter and cart command.
- Run service contract tests and database assertions for quantity and totals.
- Run the catalog/cart Playwright tests once with Core routing enabled and once after flipping the slice back to legacy, using the same browser cookie jar.

**Live test**

- Browse/filter/search, open a detail page, add two products, change quantity, remove one, restart Core, and confirm the remaining anonymous basket is intact. Flip the slice to legacy and confirm the same cart appears.

## Step 6 - Move authentication and account pages

**X -> Y:** ASP.NET Identity 2/OWIN account Web Forms -> OWIN hosted in ASP.NET Core through `Microsoft.AspNetCore.SystemWebAdapters.Owin`, with Razor account pages.

### Minimal changes

1. Add `Microsoft.AspNetCore.SystemWebAdapters` and `.Owin` only for the required authentication compatibility surface.
2. Copy the existing Identity manager/validation and OWIN cookie configuration into the Core host with the adapter. Preserve password hashing, security-stamp validation interval, username/email rules, and existing user tables.
3. Convert login and registration markup to Razor Pages with model binding, validation tag helpers, antiforgery, and the same messages/redirects.
4. Preserve `ReturnUrl` only when it is local. Preserve the remember-email behavior, cookie lifetime, logout, and anonymous-basket transfer.
5. If the installed adapter/package combination cannot compile and validate an existing Identity 2 password hash, stop this step and use the narrow fallback: ASP.NET Core Identity stores mapped to the existing `AspNetUsers` schema and password hasher compatibility. Do not change tables or bulk-reset passwords merely to pass migration.
6. Switch account routes to Core only after an existing legacy-created user and a newly Core-created user can authenticate across the side-by-side boundary.

### Exit criteria

**Acceptance criteria**

- Existing users log in without password reset; new users can register and log in.
- Security-stamp validation, logout, protected-route challenge, local return URL, remembered email, and duplicate-email/password rules match baseline.
- Anonymous basket transfer occurs once with no lost or duplicated items.
- External return URLs are rejected.

**Test plan**

- Unit test validators, local-return-URL checks, and basket-transfer idempotency.
- Integration test cookies, challenge redirects, security-stamp invalidation, and reads/writes to the existing Identity tables.
- Run browser tests for register, failed/successful login, logout, remembered email, protected-route return, and basket transfer.

**Live test**

- Add an item anonymously, register, verify the basket moves to the username, log out/in, and verify it remains. Repeat with a user created by the legacy host before this step.

## Step 7 - Move checkout and order history

**X -> Y:** authenticated checkout/order Web Forms event handlers -> authorized Razor Page handlers and model validation.

### Minimal changes

1. Convert checkout, order complete, and order history to Razor Pages while preserving their legacy paths and query keys.
2. Map controls to view-model properties with equivalent data annotations. Use GET for display and antiforgery-protected POST for order creation.
3. Reuse `OrderService` unchanged. Keep the existing single `SaveChanges` unit for order creation and basket clearing.
4. Keep user ownership checks for order detail. Prevent duplicate submission with POST-Redirect-GET and a one-time submission token if the baseline test demonstrates duplicate creation on refresh/retry.
5. Preserve card masking behavior exactly for migration. Track removal of security-code persistence as an immediate post-migration security task requiring product/data-retention approval.

### Exit criteria

**Acceptance criteria**

- Unauthenticated users are challenged and returned to checkout after login.
- Address prefill, validation messages, order totals/items, basket clearing, confirmation, history ordering, and ownership checks match baseline.
- Refreshing confirmation does not create a second order.
- A user cannot view another user's order by changing `orderId`.

**Test plan**

- Unit test checkout validation and ownership decisions.
- Integration test atomic order creation/basket clearing, masked card persistence, empty basket, and duplicate submission.
- Run authenticated checkout/history browser tests against Core and compare database effects with baseline fixtures.

**Live test**

- Log in, complete an order with test data, refresh the confirmation page, inspect order history, and verify exactly one order exists and the cart is empty. Attempt another user's `orderId` and verify access is denied or redirected as in baseline.

## Step 8 - Move product administration

**X -> Y:** Web Forms `GridView` row commands -> authorized Razor Page GET/POST handlers.

### Minimal changes

1. Convert the product list and inline create/edit/delete behavior to one Razor Page or a small Razor Pages folder.
2. Reuse `CatalogService` and existing models. Replace GridView commands with explicit antiforgery-protected POST handlers.
3. Preserve the current authorization rule exactly: any authenticated user can access the page. Record role-based admin authorization as post-migration hardening because changing it now would change behavior.
4. Preserve field values, validation, ordering, and redirect behavior; add delete confirmation only if the baseline already has it.

### Exit criteria

**Acceptance criteria**

- Anonymous access is challenged; authenticated access behaves as before.
- List, create, edit, and delete operations produce the same database changes and validation outcomes as baseline.
- Invalid and missing product IDs are handled without unhandled exceptions.
- No admin request depends on ViewState or Web Forms event fields.

**Test plan**

- Add handler/integration tests for authorization and every CRUD path.
- Verify antiforgery on all writes and assert database state after each operation.
- Run the admin Playwright suite against Core, including validation and stale/missing IDs.

**Live test**

- Log in, create a uniquely named product, find it in catalog search, edit it, then delete it and confirm it disappears from both admin and catalog views.

## Step 9 - Cut over to Core-only runtime

**X -> Y:** ASP.NET Core front door with IIS/Web Forms fallback and dual configuration -> independently deployable ASP.NET Core 10 application.

### Minimal changes

1. Prove no request uses fallback by logging proxy route usage during a full regression run; fail tests on any unexpected fallback.
2. Remove reverse-proxy routes/package, fallback health dependency, and side-by-side launch requirement. Keep legacy source buildable in the repository for a time-boxed rollback window; do not delete it in the migration commit.
3. Remove System.Web adapters that have no remaining consumers. Keep the OWIN adapter only if it remains the selected Identity compatibility implementation.
4. Make Core the solution startup/deployment project. Produce a self-contained or framework-dependent Windows deployment according to the existing hosting environment.
5. Run schema compatibility checks and back up the database before production cutover. No schema migration is expected.

### Exit criteria

**Acceptance criteria**

- `eShop.Web` builds, tests, publishes, starts, and serves every feature with the legacy process stopped and IIS Express absent.
- No request path, assembly, runtime configuration, or deployment artifact requires Web Forms, `System.Web`, or the legacy `bin` directory, except an explicitly retained OWIN compatibility package.
- All baseline features remain present; HTTP behavior, authentication, session/basket behavior, and database effects meet the approved equivalence tests.
- Rollback is documented as redeploying the last legacy build against the unchanged compatible database.

**Test plan**

- Run clean restore, Debug/Release builds, all unit/integration tests, full Playwright regression, and `dotnet publish` from an empty artifacts directory.
- Run the published output on a clean Windows test machine/VM with only the declared .NET runtime and SQL dependency.
- Compare key page latency and error rate with Step 0; investigate material regressions before release.
- Scan published files and runtime logs for fallback/System.Web dependencies.

**Live test**

- Stop and disable the legacy host. Start only the published Core application and complete browse, anonymous cart, register/login/logout, checkout/history, and admin CRUD from its production-like URL. Restart the process and repeat login and basket/order reads.

## Route migration ledger

Update this table in each implementation pull request. A route moves to `Core` only after that step's automated and live tests pass.

| Route | Initial owner | Target owner | Step |
|---|---|---|---|
| `/` | Legacy via proxy | Core | 4 |
| `/Catalog/Default.aspx` | Legacy via proxy | Core | 5 |
| `/Catalog/ProductDetail.aspx` | Legacy via proxy | Core | 5 |
| `/Cart/ShoppingCart.aspx` | Legacy via proxy | Core | 5 |
| `/Account/Login.aspx` | Legacy via proxy | Core | 6 |
| `/Account/Register.aspx` | Legacy via proxy | Core | 6 |
| Logout command | Legacy via proxy | Core | 6 |
| `/Checkout/Checkout.aspx` | Legacy via proxy | Core | 7 |
| `/Checkout/OrderComplete.aspx` | Legacy via proxy | Core | 7 |
| `/Checkout/OrderHistory.aspx` | Legacy via proxy | Core | 7 |
| `/Admin/Products.aspx` | Legacy via proxy | Core | 8 |
| Catch-all fallback | Legacy | Removed | 9 |

## Required evidence for every step

Each implementation pull request must include:

- The X -> Y transformation and exact routes/files affected.
- Restore/build/test commands with passing results.
- Automated test additions and retained baseline assertions.
- The live-test date, environment, tester, URL, and result.
- Database/schema impact, expected to be `none` unless approved.
- The route switch used for rollback and proof that rollback was exercised.
- Any behavior difference explicitly approved by the product owner; silence is not approval.

## Explicitly deferred modernization

These changes are valuable but increase risk without helping the runtime migration. Schedule them only after Step 9:

- EF6 -> EF Core and `System.Data.SqlClient` -> `Microsoft.Data.SqlClient`.
- ASP.NET Identity 2/OWIN adapter -> native ASP.NET Core Identity, if the adapter remains after cutover.
- LocalDB -> production SQL Server/Azure SQL and Windows -> cross-platform hosting.
- Synchronous -> asynchronous DAL APIs.
- Enabling nullable reference types or implicit usings.
- UI redesign, JavaScript framework replacement, clean architecture, repositories, microservices, or containerization.
- Authorization hardening from "authenticated user" to a true administrator policy.
- Payment-data redesign, including eliminating persisted card security codes. This is high-priority security work, but it requires a separate data migration and acceptance plan.

## Modernization map references

This sequence applies the following paths from the [2025 .NET modernization map](https://github.com/keschlob_microsoft/2025-dotnet-modernization-map):

- [.NET Framework 4.x -> .NET 10](https://github.com/keschlob_microsoft/2025-dotnet-modernization-map/blob/main/core-dotnet/netfx-to-net10-knowledge.md): side-by-side projects, behavior preservation, adapters first, EF6 retention, and Core-only final validation.
- [.NET Framework dependencies -> compatible NuGet packages](https://github.com/keschlob_microsoft/2025-dotnet-modernization-map/blob/main/core-dotnet/netfx-deps-to-compat-nugets-knowledge.md): explicit package restoration/references.
- [Web.config -> appsettings.json](https://github.com/keschlob_microsoft/2025-dotnet-modernization-map/blob/main/core-dotnet/config-to-appsettings-knowledge.md): hybrid configuration during coexistence.
- [ASP.NET Web Forms -> ASP.NET Core MVC](https://github.com/keschlob_microsoft/2025-dotnet-modernization-map/blob/main/web-frameworks/webforms-to-mvc-knowledge.md) and [WebForms ASPX pages -> Razor Pages](https://github.com/keschlob_microsoft/2025-dotnet-modernization-map/blob/main/web-frameworks/aspx-to-razor-pages-knowledge.md): page-by-page UI migration.
- [ASP.NET Master Pages -> ASP.NET Core Layout Pages](https://github.com/keschlob_microsoft/2025-dotnet-modernization-map/blob/main/web-frameworks/master-to-layout-knowledge.md).
- [Web Forms validation controls -> tag helpers and validation](https://github.com/keschlob_microsoft/2025-dotnet-modernization-map/blob/main/web-frameworks/validation-controls-to-taghelpers-knowledge.md).
- [ViewState -> modern state management](https://github.com/keschlob_microsoft/2025-dotnet-modernization-map/blob/main/web-frameworks/viewstate-to-modern-state-knowledge.md): query parameters, model binding, TempData, and sparing session use.
- [ASP.NET Identity -> ASP.NET Core Identity](https://github.com/keschlob_microsoft/2025-dotnet-modernization-map/blob/main/security/aspnet-identity-to-core-identity-knowledge.md): retained as the fallback when OWIN compatibility cannot meet the exit criteria.

Where map paths conflict, the direct .NET Framework -> .NET 10 guidance and this repository's minimal-change constraint take precedence: EF6 and OWIN compatibility are retained until they prevent a stated acceptance criterion.