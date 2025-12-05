# .NET 8.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 8.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 8.0 upgrade.
3. Upgrade EgsWebFTB\EgsWebFTB.vbproj
4. Upgrade EgsData\EgsData.vbproj
5. Upgrade EgsReport\EgsReport.vbproj
6. Upgrade CalcmenuAPI_CMC_CoopGastro.vbproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|
|                                                |                             |

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                        | Current Version                    | New Version | Description                                   |
|:------------------------------------|:----------------------------------:|:-----------:|:----------------------------------------------|
| AttributeRouting.Core               | 3.5.6                              |             | Not supported on .NET 8; replace with routing in ASP.NET Core |
| AttributeRouting.Core.Http          | 3.5.6                              |             | Not supported on .NET 8; replace with routing in ASP.NET Core |
| AttributeRouting.Core.Web           | 3.5.6                              |             | Not supported on .NET 8; replace with routing in ASP.NET Core |
| AttributeRouting.WebApi             | 3.5.6                              |             | Not supported on .NET 8; replace with routing in ASP.NET Core |
| Microsoft.AspNet.WebApi             | 4.0.20710.0                        |             | Functionality included with ASP.NET Core      |
| Microsoft.AspNet.WebApi.Client      | 4.0.30506.0;4.0.20710.0            | 6.0.0      | Update to latest compatible                   |
| Microsoft.AspNet.WebApi.Core        | 4.0.30506.0;4.0.20710.0            |             | Not supported on .NET 8; migrate to ASP.NET Core |
| Microsoft.AspNet.WebApi.WebHost     | 4.0.30506.0;4.0.20710.0            |             | Not supported on .NET 8; migrate to ASP.NET Core |
| Microsoft.Net.Http                  | 2.0.20710.0                        |             | Deprecated; replace with System.Net.Http 4.3.4 |
| Newtonsoft.Json                     | 4.5.11                             | 13.0.4     | Security vulnerability; update                |
| Microsoft.Web.Infrastructure        | 1.0.0.0                            |             | Functionality included with ASP.NET Core      |
| System.Net.Http                     |                                    | 4.3.4      | Replacement for Microsoft.Net.Http            |

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### EgsWebFTB\EgsWebFTB.vbproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net8.0`.
  - Convert project file to SDK-style.

Other changes:
  - Update code to remove `System.Web` dependencies and migrate to ASP.NET Core equivalents if it is a web project.

#### EgsData\EgsData.vbproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net8.0-windows` (uses Windows-only APIs).
  - Convert project file to SDK-style.

Other changes:
  - Update any legacy APIs to .NET 8 equivalents.

#### EgsReport\EgsReport.vbproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net8.0-windows` (uses Windows-only APIs and DevExpress WinForms reporting APIs).
  - Convert project file to SDK-style.

NuGet packages changes:
  - Update DevExpress reporting packages to versions compatible with .NET 8.

Other changes:
  - Update report export APIs (use `XtraReport.ExportToPdf`).

#### CalcmenuAPI_CMC_CoopGastro.vbproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net8.0`.
  - Convert project file to SDK-style.

NuGet packages changes:
  - Remove AttributeRouting.* packages (unsupported); migrate to ASP.NET Core routing attributes (`[HttpGet]`, `[HttpPost]`).
  - Remove `Microsoft.AspNet.WebApi.*` packages (unsupported); migrate to ASP.NET Core.
  - Replace `Microsoft.Net.Http` with `System.Net.Http` 4.3.4.
  - Update `Newtonsoft.Json` to 13.0.4.

Feature upgrades:
  - Migrate Web API controllers from `System.Web.Http.ApiController` to ASP.NET Core `Microsoft.AspNetCore.Mvc.ControllerBase`.
  - Replace Web.config with appsettings.json and minimal hosting model in `Program.vb`.

Other changes:
  - Update logging to `Microsoft.Extensions.Logging` or keep `log4net` via compatible adapter.
