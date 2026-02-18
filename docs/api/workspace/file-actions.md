# ProjectFileActionsBuilder

Fluent builder for composing multiple file operations into a single `CodeAction`. Created via `project.FileActions("title")`.

```csharp
return project.FileActions("Generate configuration files")
    .Write("schema.json", schemaContent)
    .WriteIfNotExists("settings.json", "{}")
    .AppendLine(".gitignore", "secrets.json")
    .MergeJsonFile("appsettings.json", templateJson)
    .SyncJsonFile("config.schema.json", schemaJson)
    .ModifyProjectFile(doc => doc.SetPropertyGroup("Features", pg => pg
        .Property("EnableFeature", "true")))
    .ModifyXmlFile("deepstaging.props", doc => { ... })
    .If(useSecrets, b => b.Write("secrets.json", "{}"),
        otherwise: b => b.Write("config.json", defaultConfig))
    .WithEach(schemas, (b, s) => b.Write(s.Path, s.Content))
    .ToCodeAction();
```

## Methods

| Method | Description |
|--------|-------------|
| `Write(path, content)` | Write a file, overwriting any existing content |
| `WriteIfNotExists(path, content)` | Write only if the file does not already exist |
| `AppendLine(path, line)` | Append a line if not already present; creates file if needed |
| `MergeJsonFile(path, template)` | Deep merge template JSON, adding missing keys while preserving existing values |
| `SyncJsonFile(path, template)` | Sync JSON with template: add missing keys, remove extra keys, preserve `$`-prefixed keys |
| `ModifyProjectFile(action, createIfMissing)` | Modify the `.csproj` XML document. Optionally create with `<Project>` root if missing |
| `ModifyXmlFile(path, action)` | Modify (or create) an XML file relative to the project directory |
| `If(condition, configure)` | Conditionally apply operations — callback runs only when condition is `true` |
| `If(condition, configure, otherwise)` | Branch between two sets of operations based on a condition |
| `WithEach(items, configure)` | Iterate a collection, invoking the callback with the builder and each item. Null-safe |
| `ToCodeAction()` | Build all operations into a single `CodeAction` |

## Conditional Operations

### If

Apply operations only when a condition is true:

```csharp
.If(includeSecrets, b => b
    .Write("secrets.json", "{}")
    .AppendLine(".gitignore", "secrets.json"))
```

Branch between two paths:

```csharp
.If(useCustomConfig,
    b => b.Write("config.json", customConfig),
    otherwise: b => b.Write("config.json", defaultConfig))
```

### WithEach

Drive file operations from a runtime collection. Null-safe — a `null` collection is a no-op:

```csharp
.WithEach(schemaFiles, (b, schema) => b
    .Write(schema.Path, schema.Content)
    .AppendLine(".gitignore", schema.Path))
```

## Integration with ManagedPropsFile

The builder integrates with [ManagedPropsFile](managed-props.md) for managed `.props` files:

```csharp
project.FileActions("Setup generator")
    .ModifyPropsFile<MyGeneratorProps>(doc =>
        doc.SetPropertyGroup("MyLabel", pg => pg
            .Property("MyProp", "value")))
    .Write("template.json", content)
    .ToCodeAction();
```
