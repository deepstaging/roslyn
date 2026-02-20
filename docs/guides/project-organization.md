# Project Organization

A well-structured Roslyn toolkit separates concerns across five projects. This guide uses the [Deepstaging](https://github.com/deepstaging/deepstaging) source generator suite as a reference architecture.

## The Five-Layer Pattern

```
src/
├── Deepstaging/                    # Attributes & enums only
├── Deepstaging.Projection/        # Queries and Models
├── Deepstaging.Generators/        # Source generators
├── Deepstaging.Analyzers/         # Diagnostic analyzers
├── Deepstaging.CodeFixes/         # Code fix providers
└── Deepstaging.Tests/             # All tests
```

Each layer has a single responsibility and a strict dependency direction:

```
Attributes ← Projection ← Generators
                        ← Analyzers ← CodeFixes
```

| Project | Purpose | References |
|---------|---------|------------|
| **Attributes** | Attribute definitions and enums only. This is what users reference. | None |
| **Projection** | Queries and models — the single source of truth for interpreting attributes. | Attributes, Deepstaging.Roslyn |
| **Generators** | Thin wiring + Writer classes for code generation. | Projection |
| **Analyzers** | Diagnostic rules. | Projection |
| **CodeFixes** | Quick-fix providers. | Analyzers (for diagnostic IDs) |

!!! tip "Why separate Projection?"
    Both generators and analyzers need to interpret the same attributes. The Projection layer ensures they share the same logic — one set of queries, one set of models, consistent behavior everywhere.

## File Organization

### Attributes

Keep this project minimal. Users depend on it directly, so it should have no Roslyn or Deepstaging.Roslyn dependency.

```
Deepstaging/
├── Effects/
│   ├── RuntimeAttribute.cs
│   ├── EffectsModuleAttribute.cs
│   ├── UsesAttribute.cs
│   └── CapabilityAttribute.cs
├── Ids/
│   ├── StrongIdAttribute.cs
│   ├── BackingType.cs
│   └── IdConverters.cs
├── Config/
│   └── ConfigProviderAttribute.cs
└── HttpClient/
    ├── HttpClientAttribute.cs
    ├── GetAttribute.cs
    └── PostAttribute.cs
```

Group by feature domain. Each domain gets its own namespace.

### Projection

Organize by domain, with a consistent three-part structure: `Attributes/` (queries), `Models/` (records), and `Queries.cs` (extensions).

```
Deepstaging.Projection/
├── Ids/
│   ├── Attributes/
│   │   └── StrongIdAttributeQuery.cs
│   ├── Models/
│   │   └── StrongIdModel.cs
│   └── Queries.cs
├── Effects/
│   ├── Attributes/
│   │   ├── EffectsModuleAttributeQuery.cs
│   │   ├── UsesAttributeQuery.cs
│   │   └── CapabilityAttributeQuery.cs
│   ├── Models/
│   │   ├── RuntimeModel.cs
│   │   ├── EffectsModuleModel.cs
│   │   ├── CapabilityModel.cs
│   │   ├── EffectMethodModel.cs
│   │   └── EffectParameterModel.cs
│   └── Queries.cs
├── Config/
│   ├── Attributes/
│   ├── Models/
│   └── Queries.cs
├── HttpClient/
│   ├── Attributes/
│   ├── Models/
│   └── Queries.cs
└── GlobalUsings.cs
```

Each domain follows the same pattern:

- **Attributes/** — `AttributeQuery` records that wrap `AttributeData` with typed properties
- **Models/** — `[PipelineModel]` records capturing everything needed for generation
- **Queries.cs** — Extension methods on `ValidSymbol<T>` that chain queries into models

### Generators

Generators are thin — they wire the Projection layer to Writer classes. Writers live in a `Writers/` directory, organized by domain.

```
Deepstaging.Generators/
├── StrongIdGenerator.cs
├── EffectsGenerator.cs
├── ConfigGenerator.cs
├── HttpClientGenerator.cs
├── PreludeGenerator.cs
├── Writers/
│   ├── Ids/
│   │   └── StrongIdWriter.cs
│   ├── Effects/
│   │   ├── RuntimeWriter.cs
│   │   ├── EffectsModuleWriter.cs
│   │   └── CapabilityWriter.cs
│   ├── Config/
│   │   └── ConfigWriter.cs
│   └── HttpClient/
│       ├── ClientWriter.cs
│       └── RequestWriter.cs
└── GlobalUsings.cs
```

### Analyzers

One file per diagnostic. Group by domain.

```
Deepstaging.Analyzers/
├── Effects/
│   ├── RuntimeMustBePartialAnalyzer.cs
│   ├── EffectsModuleMustBePartialAnalyzer.cs
│   ├── EffectsModuleShouldBeSealedAnalyzer.cs
│   └── EffectsModuleTargetMustBeInterfaceAnalyzer.cs
├── Ids/
│   ├── StrongIdMustBePartialAnalyzer.cs
│   └── StrongIdShouldBeReadonlyAnalyzer.cs
├── Config/
│   └── ConfigProviderMustBePartialAnalyzer.cs
└── HttpClient/
    └── HttpClientMustBePartialAnalyzer.cs
```

### CodeFixes

Code fixes reference the Analyzers project to access diagnostic IDs. Group by fix type.

```
Deepstaging.CodeFixes/
├── ClassMustBePartialCodeFix.cs
├── StructMustBePartialCodeFix.cs
├── ClassShouldBeSealedCodeFix.cs
└── StructShouldBeReadonlyCodeFix.cs
```

### Tests

A single test project covers all layers. Mirror the source project structure.

```
Deepstaging.Tests/
├── Ids/
│   ├── StrongIdGeneratorTests.cs
│   ├── StrongIdAnalyzerTests.cs
│   └── StrongIdModelTests.cs
├── Effects/
│   ├── RuntimeGeneratorTests.cs
│   ├── EffectsModuleGeneratorTests.cs
│   └── EffectsModuleAnalyzerTests.cs
└── ModuleInitializer.cs
```

## Dependency Constraints

These constraints prevent circular dependencies and keep the architecture clean:

1. **Attributes → nothing.** This is the user-facing package. No Roslyn dependencies.
2. **Projection → Attributes.** The Projection layer reads attributes but never generates code.
3. **Generators → Projection.** Generators never reference Analyzers. They get all data through the Projection layer.
4. **Analyzers → Projection.** Analyzers use the same queries as generators but for validation, not generation.
5. **CodeFixes → Analyzers.** Code fixes need analyzer diagnostic IDs for `[CodeFix("...")]` attributes.

!!! warning "Never reference Generators from Analyzers"
    If you find an analyzer needing generator logic, move that logic to the Projection layer.

## Packaging

Everything ships as a **single NuGet package**. The root project bundles all DLLs into the correct NuGet folders:

```
MyPackage.nupkg
├── lib/netstandard2.0/
│   └── MyPackage.dll                          (attributes — all consumers)
├── lib/net10.0/
│   ├── MyPackage.dll
│   └── MyPackage.Runtime.dll                  (optional runtime classes)
├── analyzers/dotnet/cs/
│   ├── MyPackage.Generators.dll
│   ├── MyPackage.Analyzers.dll
│   ├── MyPackage.CodeFixes.dll
│   ├── MyPackage.Projection.dll               (generator/analyzer dependency)
│   └── Deepstaging.Roslyn.dll                 (generator dependency)
├── satellite/netstandard2.0/
│   └── MyPackage.Projection.dll               (for downstream generators)
└── build/
    └── MyPackage.props                        (auto-imported by consuming projects)
```

Consumers only need:

```xml
<PackageReference Include="MyPackage" Version="1.0.0" />
```

The [roslynkit template](../templates.md) generates this structure automatically via `./build/pack.sh`.

### Satellite Projection

The `satellite/` folder enables **downstream packages** to build upon your Projection layer — your models, query extensions, and attribute wrappers — without you publishing a separate package.

#### The Problem

When package A defines attributes and a generator, and package B wants to generate code that builds upon A's semantics, B needs access to A's `Projection.dll` as a compile-time reference. But the copy in `analyzers/dotnet/cs/` is only loaded by the Roslyn compiler — it's not available for B's code to reference.

Putting Projection in `lib/` would expose Roslyn-specific types (symbol wrappers, query extensions) to all consumers via IntelliSense — types that only make sense for generator authors.

#### The Solution

A second copy of `Projection.dll` lives in `satellite/netstandard2.0/`. The package's build props conditionally add it as a reference when an opt-in property is set.

In the package's `build/MyPackage.props`:

```xml
<ItemGroup Condition="'$(MyPackageSatellite)' == 'true'">
    <Reference Include="MyPackage.Projection"
               HintPath="$(MSBuildThisFileDirectory)../satellite/netstandard2.0/MyPackage.Projection.dll"/>
</ItemGroup>
```

Downstream projects opt in with one property:

```xml
<PropertyGroup>
    <MyPackageSatellite>true</MyPackageSatellite>
</PropertyGroup>
```

#### Naming Convention

Every package uses `{PackageNameNoDots}Satellite` as its property name:

| Package | Property | Exposes |
|---------|----------|---------|
| `Deepstaging` | `DeepstagingSatellite` | `Deepstaging.Projection.dll` |
| `Deepstaging.Web` | `DeepstagingWebSatellite` | `Deepstaging.Web.Projection.dll` |
| `MyCompany.Foo` | `MyCompanyFooSatellite` | `MyCompany.Foo.Projection.dll` |

This lets a downstream project compose multiple satellite references:

```xml
<PropertyGroup>
    <DeepstagingSatellite>true</DeepstagingSatellite>
    <DeepstagingWebSatellite>true</DeepstagingWebSatellite>
</PropertyGroup>
```

#### Implementing Satellite Support

1. **Create build props** at `build/{PackageId}.props`:

    ```xml
    <Project>
        <ItemGroup Condition="'$({PackageNameNoDots}Satellite)' == 'true'">
            <Reference Include="{PackageId}.Projection"
                       HintPath="$(MSBuildThisFileDirectory)../satellite/netstandard2.0/{PackageId}.Projection.dll"/>
        </ItemGroup>
    </Project>
    ```

2. **Pack the build props** in your root `.csproj`:

    ```xml
    <None Include="build\{PackageId}.props" Pack="true" PackagePath="build"/>
    ```

3. **Pack Projection to satellite**:

    ```xml
    <None Include="path/to/{PackageId}.Projection.dll"
          Pack="true" PackagePath="satellite/netstandard2.0" Visible="false"/>
    ```

4. **Suppress NU5100** (NuGet warns about DLLs outside `lib/`):

    ```xml
    <NoWarn>$(NoWarn);NU5100</NoWarn>
    ```

The [roslynkit template](../templates.md) includes all of this. The `satellitePrefix` template symbol automatically derives the property name by stripping dots from the package name.

## Getting Started

| Resource | Purpose |
|----------|---------|
| [RoslynKit Template](https://github.com/deepstaging/roslyn/tree/main/templates) | Use `dotnet new roslynkit` to scaffold a new solution with this structure |
| [Deepstaging source](https://github.com/deepstaging/deepstaging) | Production reference with 6 generators, 25+ analyzers, and 6 code fixes |
| [Samples](https://github.com/deepstaging/samples) | See how end-users consume the generated code |
