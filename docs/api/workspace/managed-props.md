# ManagedPropsFile

Base class for managed MSBuild `.props` files that generators and code fixes can read from and write to. Subclasses declare a file name and default contents; the framework ensures those defaults exist whenever the file is modified.

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

## Usage Pattern

1. Subclass `ManagedPropsFile` and override `FileName` and `ConfigureDefaults`
2. In NuGet `build/*.props`, auto-import the local file with a `Condition="Exists(...)"`
3. In code fixes, use `project.ModifyPropsFileAction<T>(...)` or `builder.ModifyPropsFile<T>(...)`

## PropsBuilder

Fluent builder used inside `ConfigureDefaults`:

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
    .If(enableSecrets, b => b                   // Conditional property groups/item groups
        .Property("UserSecretsId", secretsId))
    .WithEach(extraItems, (b, item) => b        // Iterate a collection
        .Property(item.Name, item.Value));
```

All nested builders (`PropsPropertyGroupBuilder`, `PropsItemGroupBuilder`) support the same `If`/`WithEach` helpers:

## ManagedPropsFile Extensions

```csharp
// Standalone code action — ensures defaults, then applies modification
project.ModifyPropsFileAction<MyGeneratorProps>("Add property", doc =>
    doc.SetPropertyGroup("MyLabel", pg => pg
        .Property("MyProp", "value")))

// Inside a FileActions builder
project.FileActions("Setup generator")
    .ModifyPropsFile<MyGeneratorProps>(doc =>
        doc.SetPropertyGroup("MyLabel", pg => pg
            .Property("MyProp", "value")))
    .Write("template.json", content)
    .ToCodeAction();
```

## PropsXmlExtensions

Fluent helper methods for manipulating `.props` XML documents. All methods return the same `XDocument` for chaining.

`SetPropertyGroup` and `SetItemGroup` preserve existing user content in the group. By default, entries are added only if they don't already exist. Use `MergeAction` per entry to control behavior:

| Action | Behavior |
|--------|----------|
| `MergeAction.Add` | Add the entry only if it does not already exist (default) |
| `MergeAction.Remove` | Remove the entry if it exists |
| `MergeAction.Set` | Add the entry if missing, or overwrite its value if it already exists |

```csharp
doc.SetPropertyGroup("Generator", pg => pg
       .Property("EmitCompilerGeneratedFiles", "true")
       .Property("CompilerGeneratedFilesOutputPath", "!generated",
           comment: "Redirect generated files")
       .Property("OldFeature", MergeAction.Remove)
       .Property("OutputPath", "bin", MergeAction.Set))
   .SetItemGroup("Generated", items =>
       items.Item("None", "Update", "*.g.cs", meta =>
           meta.Set("DependentUpon", "%(Filename).cs"))
       .Item("Compile", "Remove", "!generated/**",
           comment: "Exclude generated files from compilation")
       .Item("Compile", "Remove", "old/**", MergeAction.Remove)
       .If(includeJson, ig => ig
           .Item("AdditionalFiles", "Include", "*.json"))
       .WithEach(patterns, (ig, p) => ig
           .Item(p.Type, p.Action, p.Pattern)));
```

Both builders support a `Comment()` method that sets an XML comment before the group element when it is first created:

```csharp
doc.SetPropertyGroup("Generator", pg => pg
       .Comment("Generator configuration")
       .Property("EmitCompilerGeneratedFiles", "true"))
   .SetItemGroup("Generated", items => items
       .Comment("Generated file handling")
       .Item("None", "Update", "*.g.cs"));
```

Produces:

```xml
<!-- Generator configuration -->
<PropertyGroup Label="Generator">
    <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
</PropertyGroup>
<!-- Generated file handling -->
<ItemGroup Label="Generated">
    <None Update="*.g.cs" />
</ItemGroup>
```
