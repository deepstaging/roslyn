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
    });
```

## ManagedPropsFile Extensions

```csharp
// Standalone code action — ensures defaults, then applies modification
project.ModifyPropsFileAction<MyGeneratorProps>("Add property", doc =>
    doc.SetProperty("MyProp", "value"))

// Inside a FileActions builder
project.FileActions("Setup generator")
    .ModifyPropsFile<MyGeneratorProps>(doc =>
        doc.SetProperty("MyProp", "value"))
    .Write("template.json", content)
    .ToCodeAction();
```

## PropsXmlExtensions

Helper methods for manipulating `.props` XML documents:

```csharp
// Set a property (no-op if already exists)
doc.SetProperty("UserSecretsId", Guid.NewGuid().ToString())
doc.SetProperty("Feature", "true", label: "MyLabel", comment: "Enable feature")

// Replace a labeled ItemGroup with new items
doc.SetItemGroup("Generated", items =>
    items.Item("None", "Update", "*.g.cs", meta =>
        meta.Set("DependentUpon", "%(Filename).cs")));
```
