# Agent instructions

This repository is public for demonstration and reference, not to solicit
external contributions. These instructions guide agent-assisted maintenance;
do not frame the project as welcoming contributors or add contribution
workflows unless explicitly requested.

## Repository and compatibility

Collections Online publishes collection records, search, media, and a public API.
Start with `README.md`, but use the current project files as the source of truth
for dependencies: the README's RavenDB version is historical.

`src\CollectionsOnline.sln` contains Core, WebSite, Import, Tests, and Tasks.
Core, WebSite, Import, and Tests target .NET Framework 4.7.2 and use
`packages.config`; Tasks is SDK-style, targets .NET 6, and uses PackageReference.
The website uses Nancy 1.x, Ninject, Razor, and RavenDB 3.5 clients. These are
compatibility constraints, not instructions to upgrade or migrate frameworks.

## Responsibility boundaries

- `src\CollectionsOnline.WebSite`: keep Nancy `Modules` focused on HTTP binding,
  response selection, and coordination. Follow `SearchModule`:
  `Queries\SearchViewModelQuery.cs` handles Raven queries,
  `Factories\SearchViewModelFactory.cs` builds presentation models, and `Views`
  renders them. Keep API models, `Transformers`, and AutoMapper configuration
  consistent. Dependency wiring belongs in `Infrastructure\WebSiteBootstrapper.cs`.
- `src\CollectionsOnline.Core`: shared domain `Models`, `Factories`, `Utilities`,
  Raven `Indexes`, and infrastructure. Search changes may require coordinated
  updates to `Indexes\CombinedIndex.cs`, `CombinedFacets.cs`, and website
  `Queries\QueryBuilder.cs`; do not move web-specific concerns into Core.
- `src\CollectionsOnline.Import`: `Program.cs` wires dependencies;
  `Infrastructure\ImportRunner.cs` coordinates ordered imports and completion
  state; `Imports` handles ingestion; `Factories` maps IMu fields and updates
  documents and relationships. Follow a comparable factory before changing a map.
- `src\CollectionsOnline.Tasks`: `Program.cs` configures the host,
  `Extensions\ServiceCollectionExtensions.cs` registers services and discovers
  `ITask` implementations, and `Infrastructure\TaskRunner.cs` runs enabled tasks
  in order. Keep task work in `Tasks` and preserve cancellation handling.
- `src\CollectionsOnline.Tests`: mirror the relevant Core, Import, or Website
  area. Existing tests use xUnit, Shouldly, Given/When/Then, NSubstitute, and
  reusable `Fakes`. API tests use Nancy.Testing and RavenTestBase stores rather
  than production data.

## Change conventions and contracts

Read nearby code and a comparable implementation first. Preserve local naming,
formatting, constructor injection, and interface conventions; Ninject convention
binding and task discovery make names and registration significant. Keep changes
focused, reuse existing helpers, and retain Serilog error reporting rather than
turning unexpected failures into empty successful results.

Unless explicitly changing a contract, preserve routes, status codes, camel-case
API JSON, CSV fields, search parameters, facets, sorting, and pagination.
Preserve Raven document IDs and serialization, IMu column mappings, publication
visibility rules, relationship updates, media licences/attribution, and import
cancellation/completion semantics. Cover changed behaviour with focused
regressions, including relevant invalid-input and hidden-record cases.

Edit frontend sources in WebSite's `content\scss`, `content\js\app`, and `Views`,
not generated bundles. `gulpfile.js` builds Sass and Browserify assets and updates
cache-busted layout references; do not hand-edit generated `dist` output.

## Configuration and operational safety

The application is not self-contained: it depends on configured RavenDB and,
for imports, Museums Victoria's EMu/IMu service, `lib\IMu\imu-1.0.03.dll`, media
storage/shared folders, and other configured integrations.

Treat WebSite's `Web.config`, Import's `App.config`, and Tasks' `appsettings*.json`
as potentially sensitive. Never commit credentials, private endpoints, local
paths, or copied production configuration. Use approved local configuration or
environment overrides where supported; do not expose secrets in logs or diffs.

Do not start the website, imports, or tasks against live services without
explicit approval. The shared document-store provider creates databases,
indexes, facets, and application state during initialization; startup is not a
read-only check. Imports and tasks can modify stored data and generated files.

## Validation

Use the smallest relevant checks and report what actually ran, what did not,
and any missing prerequisites. Documentation-only changes need a saved-file and
diff review, not application startup.

For code changes, use Windows Visual Studio/MSBuild tooling with the .NET
Framework 4.7.2 targeting pack, ASP.NET Web Application build targets, and a
compatible SDK for Tasks. Build `src\CollectionsOnline.sln` in that configured
environment and run relevant Tests through the existing xUnit Visual Studio
adapter/Test Explorer. Do not assume a plain `dotnet build` or `dotnet test`
supports this mixed solution; distinguish toolchain failures from regressions.

For frontend changes, with existing dependencies available, run
`npx --no-install gulp build` from `src\CollectionsOnline.WebSite`. There are no
npm test/lint scripts in its `package.json`. Do not install or upgrade packages
merely to inspect the repository, and do not claim unrun commands passed.
