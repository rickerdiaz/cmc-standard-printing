# Legacy solution analysis and .NET 9 migration plan

## Current structure and dependencies
- **Projects:** The solution `CalcmenuAPI_CMC_CoopGastro.sln` hosts the `CalcmenuAPI_CMC_CoopGastro` web API plus a shared `CalcmenuAPI.Models` library that still uses VB modules and types.
- **Startup and configuration:** `Global.asax.vb` boots the app, logs startup, and initializes configuration via `ConfigManager.Initialize()`, which reads multiple connection strings and debugging flags from `appSettings` and aborts on missing primary connection strings.
- **Shared modules:** `Core/Common.vb` defines dozens of stored-procedure names and enum values that shape API semantics (group levels, list display modes, unit types, error codes, etc.), indicating a heavy reliance on database procedures for business logic.
- **API surface:** Controllers (e.g., `AliasController`) are ASP.NET Core controllers that execute stored procedures with `SqlCommand`, map `SqlDataReader` results into model objects, and return `ActionResult` responses. The data access pattern is manually coded per endpoint and depends on connection strings resolved from configuration.
- **DevExpress reporting:** The `EgsReport` project provides reporting classes such as `clsReport` that construct DevExpress `XtraReport` objects with extensive runtime settings (footer/logo paths, recipe detail flags, coloring, thumbnail toggles, etc.). This dependency must remain in the migration.

## Migration objectives
- Upgrade to a **.NET 9 / C#** solution while preserving existing business logic and stored-procedure contracts.
- Keep **DevExpress reporting** capabilities intact and isolated so they can be maintained independently of the API.
- Improve maintainability through consistent C# code, dependency injection, configuration options, and testability without altering functional behavior.

## Proposed .NET 9 solution layout
- `Calcmenu.Api` (ASP.NET Core 9): hosts controllers/endpoints, request/response DTOs, and middleware.
- `Calcmenu.Domain` (class library): contains business contracts, enums, and constants migrated from `Common.vb`, plus cross-cutting types such as error codes and list semantics.
- `Calcmenu.Data` (class library): encapsulates SQL access to stored procedures using Dapper or `SqlClient` wrappers; exposes repository-like services injected into controllers.
- `Calcmenu.Reporting.DevExpress` (class library): ports `EgsReport` logic into C#, centralizes DevExpress references, and exposes report builders for the API layer.
- `Calcmenu.Shared` or `Calcmenu.Models` (class library): provides POCO models formerly in VB, with nullability annotations and serialization attributes where needed.

## Migration strategy
1. **Lift infrastructure into options/DI:** Replace `Global.asax` startup with ASP.NET Core `Program.cs`/`Startup` hosting; configure logging via `ILogger` and register `IOptions` for database connections instead of static modules.
2. **Model and enum conversion:** Incrementally port VB modules (`Common.vb`, models) into C# classes/records under `Calcmenu.Shared` or `Calcmenu.Domain`, keeping names and values to preserve stored-procedure compatibility.
3. **Data access modernization:** Centralize stored-procedure execution into reusable services (e.g., `IAliasRepository`) that use async ADO.NET or Dapper; ensure parameter names and result mappings mirror existing controller behavior.
4. **Controller migration:** Move existing controllers into the new API project, replacing direct `SqlCommand` usage with injected repositories while keeping routes, stored-procedure calls, and result shapes intact.
5. **DevExpress isolation:** Move `EgsReport` types into `Calcmenu.Reporting.DevExpress`, wrap them behind interfaces (e.g., `IReportBuilder`) so API endpoints can request reports without leaking DevExpress types; configure necessary licensing packages for .NET 9.
6. **Configuration and secrets:** Use `appsettings.json` for connection strings (`Default`, `MainDb`, debugging DSN) and consume them via the options pattern; include environment overrides for deployment.
7. **Testing and validation:** Add unit tests around repository mappings and integration smoke tests for key stored procedures to confirm behavior parity before retiring the legacy branch.

## Immediate next steps
- Generate a fresh `.NET 9` solution scaffold with the project layout above and wire up dependency injection, logging, and configuration.
- Port a representative vertical slice (e.g., the Alias flow) to validate the migration approach end-to-end: model conversion, repository wrapper for `API_GET_AliasInfo`/`API_UPDATE_Alias`, and controller rewritten against the new abstractions.
- Bring over DevExpress reporting code into its dedicated project and ensure a simple report generation path still works under .NET 9 runtime.
