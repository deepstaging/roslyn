# ManagedPropsFile

Base class for managed MSBuild `.props` files that code fixes can read from and write to. Subclasses declare a file name and optional default contents; the framework ensures those defaults exist before any modification is applied.

There are two layers of API:

- **`PropsBuilder`** — declares structural defaults inside `ConfigureDefaults`. These are ensured to exist before any code fix modification runs.
- **`PropsXmlExtensions`** — fluent XML manipulation used inside code fix actions via `ModifyPropsFile`. These operate on the live `XDocument` with merge semantics.

## Defining a Managed Props File

```csharp
public sealed class MyGeneratorProps : ManagedPropsFile
{
    public override string FileName => "mygenerator.props";

    protected override void ConfigureDefaults(PropsBuilder builder) =>
        builder
            .Property("CompilerGeneratedFilesOutputPath", "!generated")
            .ItemGroup(items =>
            {
                items.Remove("Compile", "!generated/**");
                items.Include("None", "!generated/**");
            });
}
```

If the local props file is purely a container for code-fix-managed content (no structural defaults needed), leave `ConfigureDefaults` empty:

```csharp
public sealed class DeepstagingProps : ManagedPropsFile
{
    public override string FileName => "deepstaging.props";

    protected override void ConfigureDefaults(PropsBuilder builder)
    {
        // No structural defaults — NuGet package handles base config.
        // This file only holds per-project settings managed by code fixes.
    }
}
```

### Properties

| Member | Type | Description |
|--------|------|-------------|
| `FileName` | `string` (abstract) | File name relative to the project directory (e.g., `"deepstaging.props"`) |
| `ConfigureDefaults` | `void` (abstract) | Declares defaults via `PropsBuilder`. Called lazily on first use. |
| `EnsureDefaults` | `void` | Applies declared defaults to an `XDocument`. Called automatically by `ModifyPropsFile`. |

## Usage Pattern

1. Subclass `ManagedPropsFile` — override `FileName` and `ConfigureDefaults`
2. In your NuGet `build/*.props`, conditionally import the local file:
   ```xml
   <Import Project="$(MSBuildProjectDirectory)/$(DataDir)/myfile.props"
           Condition="Exists('$(MSBuildProjectDirectory)/$(DataDir)/myfile.props')"/>
   ```
3. In code fixes, use `ModifyPropsFileAction<T>` or `builder.ModifyPropsFile<T>` (see below)

---

## PropsBuilder (Defaults Layer)

Fluent builder used inside `ConfigureDefaults` to declare what should exist in the file before any code fix modifications run. All defaults use add-if-missing semantics.

```csharp
builder
    .Property("OutputPath", "bin")              // Unlabeled PropertyGroup
    .PropertyGroup("MyLabel", pg =>             // Labeled PropertyGroup
        pg.Property("Feature", "true"))
    .ItemGroup(items =>                         // ItemGroup with Include/Remove/Update
    {
        items.Include("None", "!generated/**");
        items.Remove("Compile", "!generated/**");
        items.Update("AdditionalFiles", "*.json", meta =>
            meta.Add("DependentUpon", "%(Filename).cs"));
    })
    .If(enableSecrets, b => b
        .Property("UserSecretsId", secretsId))
    .WithEach(extraItems, (b, item) => b
        .Property(item.Name, item.Value));
```

### PropsBuilder API

| Method | Description |
|--------|-------------|
| `Property(name, value)` | Add to the unlabeled PropertyGroup |
| `PropertyGroup(label, configure)` | Declare a labeled PropertyGroup via `PropsPropertyGroupBuilder` |
| `ItemGroup(configure, label?)` | Declare an ItemGroup via `PropsItemGroupBuilder` |
| `If(condition, configure)` | Conditional builder |
| `If(condition, configure, otherwise)` | Conditional with else branch |
| `WithEach(items, configure)` | Iterate a collection (null-safe) |

### PropsPropertyGroupBuilder

| Method | Description |
|--------|-------------|
| `Property(name, value)` | Add a property |
| `If(condition, configure)` | Conditional |
| `WithEach(items, configure)` | Iterate |

### PropsItemGroupBuilder

| Method | Description |
|--------|-------------|
| `Include(itemType, pattern)` | Add an `Include` item |
| `Remove(itemType, pattern)` | Add a `Remove` item |
| `Update(itemType, pattern, metadata?)` | Add an `Update` item with optional child elements |
| `If(condition, configure)` | Conditional |
| `WithEach(items, configure)` | Iterate |

### PropsItemMetadataBuilder

| Method | Description |
|--------|-------------|
| `Add(name, value)` | Add a metadata child element |

---

## ManagedPropsFileExtensions (Code Fix Integration)

Extension methods that connect `ManagedPropsFile` to the code fix action system. Each method ensures defaults before applying the modification callback.

### Standalone Code Action

```csharp
// Write to project root
project.ModifyPropsFileAction<MyGeneratorProps>("Add property", doc =>
    doc.SetPropertyGroup("MyLabel", pg => pg
        .Property("MyProp", "value")));

// Write to a subdirectory (e.g., .config/mygenerator.props)
project.ModifyPropsFileAction<MyGeneratorProps>("Add property", ".config", doc =>
    doc.SetPropertyGroup("MyLabel", pg => pg
        .Property("MyProp", "value")));
```

### Inside FileActions Builder

```csharp
project.FileActions("Generate configuration files")
    .ModifyPropsFile<DeepstagingProps>(".config", doc =>
    {
        doc.SetPropertyGroup("Settings", group => group
            .Comment("General settings")
            .If(hasSecrets, b => b
                .Property("UserSecretsId", Guid.NewGuid().ToString())));
    })
    .ModifyPropsFile<DeepstagingTargets>(".config", doc =>
    {
        doc.SetItemGroup("File Nesting", items => items
            .Comment("Nest configuration files for IDE organization")
            .Item("None", "Update", "deepstaging.settings.json",
                m => m.Set("DependentUpon", "deepstaging.props"))
            .WithEach(environments, (b, env) => b
                .Item("None", "Update", $"deepstaging.settings.{env}.json",
                    m => m.Set("DependentUpon", "deepstaging.settings.json"))));
    })
    .Write("schema.json", schemaContent)
    .SyncJsonFile("settings.json", template)
    .ToCodeAction();
```

### API Reference

| Method | On | Description |
|--------|----|-------------|
| `ModifyPropsFileAction<T>(title, modify)` | `Project` | Code action targeting project root |
| `ModifyPropsFileAction<T>(title, directory, modify)` | `Project` | Code action targeting a subdirectory |
| `ModifyPropsFile<T>(modify)` | `ProjectFileActionsBuilder` | Chained action targeting project root |
| `ModifyPropsFile<T>(directory, modify)` | `ProjectFileActionsBuilder` | Chained action targeting a subdirectory |

The `directory` parameter is combined with `FileName` to produce the relative path (e.g., `".config"` + `"deepstaging.props"` → `".config/deepstaging.props"`). Pass `null` to write at the project root.

---

## PropsXmlExtensions (XML Manipulation Layer)

Fluent methods for manipulating `.props` XML documents inside `ModifyPropsFile` callbacks. All methods return the same `XDocument` for chaining.

### MergeAction

Controls how individual properties and items are handled when the group already exists:

| Action | Behavior |
|--------|----------|
| `MergeAction.Add` | Add only if it does not already exist *(default)* |
| `MergeAction.Remove` | Remove the entry if it exists |
| `MergeAction.Set` | Add if missing, overwrite if it exists |

### SetPropertyGroup

Creates or updates a labeled `PropertyGroup`:

```csharp
doc.SetPropertyGroup("Generator", pg => pg
    .Comment("Generator configuration")
    .Property("EmitCompilerGeneratedFiles", "true")
    .Property("CompilerGeneratedFilesOutputPath", "!generated",
        comment: "Redirect generated files")
    .Property("OldFeature", MergeAction.Remove)
    .Property("OutputPath", "bin", MergeAction.Set));
```

### SetItemGroup

Creates or updates a labeled `ItemGroup`:

```csharp
doc.SetItemGroup("Generated", items => items
    .Comment("Generated file handling")
    .Item("None", "Update", "*.g.cs", meta =>
        meta.Set("DependentUpon", "%(Filename).cs"))
    .Item("Compile", "Remove", "!generated/**",
        comment: "Exclude generated files from compilation")
    .Item("Compile", "Remove", "old/**", MergeAction.Remove)
    .If(includeJson, ig => ig
        .Item("AdditionalFiles", "Include", "*.json"))
    .WithEach(patterns, (ig, p) => ig
        .Item(p.Type, p.Action, p.Pattern)));
```

### Conditional and Iteration

```csharp
doc.SetPropertyGroup("Settings", pg => pg.Property("Feature", "true"))
   .If(hasSecrets, d => d
       .SetPropertyGroup("Secrets", pg => pg
           .Property("UserSecretsId", secretsId)))
   .WithEach(modules, (d, m) => d
       .SetPropertyGroup(m.Label, pg => pg
           .Property(m.Key, m.Value)));
```

### PropertyGroupXmlBuilder

| Method | Description |
|--------|-------------|
| `Comment(text)` | XML comment before the group element (on creation only) |
| `Property(name, value, comment?)` | Add property (default: `MergeAction.Add`) |
| `Property(name, value, MergeAction, comment?)` | Add property with explicit merge action |
| `Property(name, MergeAction)` | Merge action with no value (typically `Remove`) |
| `If(condition, configure)` | Conditional |
| `If(condition, configure, otherwise)` | Conditional with else |
| `WithEach(items, configure)` | Iterate (null-safe) |

### ItemGroupXmlBuilder

| Method | Description |
|--------|-------------|
| `Comment(text)` | XML comment before the group element (on creation only) |
| `Item(itemType, action, pattern, metadata?, comment?)` | Add item (default: `MergeAction.Add`) |
| `Item(itemType, action, pattern, MergeAction, metadata?, comment?)` | Add item with explicit merge action |
| `If(condition, configure)` | Conditional |
| `If(condition, configure, otherwise)` | Conditional with else |
| `WithEach(items, configure)` | Iterate (null-safe) |

### ItemMetadataXmlBuilder

| Method | Description |
|--------|-------------|
| `Set(name, value)` | Add a metadata child element |
| `If(condition, configure)` | Conditional |
| `WithEach(items, configure)` | Iterate (null-safe) |

### Output Example

```csharp
doc.SetPropertyGroup("Generator", pg => pg
       .Comment("Generator configuration")
       .Property("EmitCompilerGeneratedFiles", "true"))
   .SetItemGroup("Generated", items => items
       .Comment("Generated file handling")
       .Item("None", "Update", "*.g.cs",
           meta => meta.Set("DependentUpon", "%(Filename).cs")));
```

Produces:

```xml
<!-- Generator configuration -->
<PropertyGroup Label="Generator">
    <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
</PropertyGroup>
<!-- Generated file handling -->
<ItemGroup Label="Generated">
    <None Update="*.g.cs">
        <DependentUpon>%(Filename).cs</DependentUpon>
    </None>
</ItemGroup>
```
