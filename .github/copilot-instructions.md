# Copilot Instructions for Deepstaging.Roslyn

## Build, Test, Lint

```bash
# Build
dotnet build Deepstaging.Roslyn.slnx

# Test (all) — prefer `dotnet run` for TUnit (easier flag passing)
dotnet run --project src/Deepstaging.Roslyn.Tests -c Release

# Test (by class name)
dotnet run --project src/Deepstaging.Roslyn.Tests -c Release --treenode-filter /*/*/TypeQueryTests/*

# Test (by test name)
dotnet run --project src/Deepstaging.Roslyn.Tests -c Release --treenode-filter /*/*/*/MyTestName

# Test (by namespace wildcard)
dotnet run --project src/Deepstaging.Roslyn.Tests -c Release --treenode-filter /*/Deepstaging.Roslyn.Tests.Emit*/*/*

# Test via dotnet test (flags go after --)
dotnet test --project src/Deepstaging.Roslyn.Tests -c Release -- --treenode-filter /*/*/TypeQueryTests/*

# Pack (local dev)
./build/pack.sh

# Docs (local preview)
./docs.sh serve
```

There is no separate lint command. Warnings are treated as errors via `TreatWarningsAsErrors`, so `dotnet build` is the lint step.

The `--treenode-filter` syntax is `/<Assembly>/<Namespace>/<Class>/<Test>` with `*` wildcards. Combine conditions with `&` (AND) or `|` (OR within parentheses).

## Architecture

This is a fluent toolkit wrapping Roslyn APIs, organized into four layers:

- **Queries** (`TypeQuery`, `MethodQuery`, etc.) — Find Roslyn symbols with chainable filters. Returns actual `ISymbol` instances, not wrappers. Each query is a `readonly struct` with immutable `ImmutableArray<Func<...>>` filters.
- **Projections** (`OptionalSymbol<T>`, `ValidSymbol<T>`, `OptionalAttribute`, `ValidAttribute`) — Safe nullable wrappers over Roslyn symbols. Use the `IsNotValid(out var valid)` early-exit pattern to unwrap.
- **Emit** (`TypeBuilder`, `MethodBuilder`, etc.) — Fluent builders that produce `CompilationUnitSyntax`. Call `.Emit()` to get valid Roslyn syntax trees.
- **Scriban** — Template infrastructure for source generators using Scriban. Templates use `.scriban-cs` extension.

Reading and writing are symmetric: `TypeQuery` finds types → `TypeBuilder` creates types.

### Package/Project Map

| Project | Target | Purpose |
|---------|--------|---------|
| `Deepstaging.Roslyn` | netstandard2.0 | Core: queries, projections, emit (packable) |
| `Deepstaging.Roslyn.Analyzers` | netstandard2.0 | Analyzers for pipeline model (packable) |
| `Deepstaging.Roslyn.Scriban` | netstandard2.0 | Scriban template integration (bundled into core package) |
| `Deepstaging.Roslyn.Scriban.Analyzers` | netstandard2.0 | Template scaffolding analyzers (bundled into core package) |
| `Deepstaging.Roslyn.Scriban.CodeFixes` | netstandard2.0 | Template scaffolding code fixes (bundled into core package) |
| `Deepstaging.Roslyn.Workspace` | netstandard2.0 | Code fix provider infrastructure (bundled into core package) |
| `Deepstaging.Roslyn.Testing` | net10.0 | Test base classes (`RoslynTestBase`) (packable) |
| `Deepstaging.Roslyn.Tests` | net10.0 | Test suite (not packable) |

Scriban and Workspace are not published as separate NuGet packages — they are packaged within `Deepstaging.Roslyn`. All library projects target `netstandard2.0` for Roslyn analyzer/generator compatibility. Only test projects target `net10.0`.

## Conventions

### License Headers (Required)

Every source file must start with SPDX license headers. A pre-commit hook enforces this.

```csharp
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
```

```xml
<!-- SPDX-FileCopyrightText: 2024-present Deepstaging -->
<!-- SPDX-License-Identifier: RPL-1.5 -->
```

```bash
# SPDX-FileCopyrightText: 2024-present Deepstaging
# SPDX-License-Identifier: RPL-1.5
```

### Testing

- **Framework**: TUnit with `Verify` for snapshot testing
- Tests inherit from `RoslynTestBase` (from `Deepstaging.Roslyn.Testing`)
- Test file naming mirrors source: `TypeQuery.cs` → `TypeQuery.Tests.cs`
- Nested test directories match source structure (e.g., `Emit/Extensions/`)
- Suppress `CS1591` (missing XML docs) in test project only

### Code Style

- Nullable reference types enabled everywhere
- XML documentation (`<summary>`) on all public members (enforced by `GenerateDocumentationFile` + warnings-as-errors)
- File name matches primary type name
- Fluent APIs return `this` or a new instance of the same struct type
- Central package versioning via `Directory.Packages.props` — never specify versions in `.csproj` files

### Build System

- `Directory.Build.props` imports modular props from `build/` — edit there, not in the root file
- Projects are not packable by default; opt in with `<IsPackable>true</IsPackable>` per `.csproj`
- `Deepstaging.Roslyn.Versions.props` is auto-updated by CI after publish — do not edit manually
- Local dev overrides go in `Directory.Build.Dev.props` (gitignored, template provided)
