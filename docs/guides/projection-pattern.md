# The Projection Pattern

The Projection layer converts Roslyn symbols into strongly-typed models through three components.

## 1. AttributeQuery Types

Wrap attribute access with typed properties and defaults:

```csharp
// Attributes/AutoNotifyAttributeQuery.cs
public sealed record AutoNotifyAttributeQuery(AttributeData AttributeData)
    : AttributeQuery(AttributeData)
{
    public bool GenerateBaseImplementation => 
        NamedArg<bool>("GenerateBaseImplementation").OrDefault(true);
}
```

### More AttributeQuery Examples

```csharp
// Handle constructor arguments
public sealed record StrongIdAttributeQuery(AttributeData AttributeData)
    : AttributeQuery(AttributeData)
{
    public string BackingType => ConstructorArg<string>(0).OrDefault("Guid");
    public bool GenerateJsonConverter => NamedArg<bool>("Json").OrDefault(true);
    public string? Prefix => NamedArg<string>("Prefix").OrNull();
}

// Handle enum values (Roslyn stores enums as int)
public sealed record CacheAttributeQuery(AttributeData AttributeData)
    : AttributeQuery(AttributeData)
{
    public CacheStrategy Strategy => NamedArg<int>("Strategy").ToEnum<CacheStrategy>().OrDefault(CacheStrategy.Memory);
    public int ExpirationSeconds => NamedArg<int>("ExpirationSeconds").OrDefault(300);
}

// Handle array arguments
public sealed record ValidateAttributeQuery(AttributeData AttributeData)
    : AttributeQuery(AttributeData)
{
    public string[] Rules => NamedArg<string[]>("Rules").OrDefault([]);
}
```

## 2. Models

Simple records capturing data needed for generation:

```csharp
// Models/AutoNotifyModel.cs
public sealed record AutoNotifyModel
{
    public required string Namespace { get; init; }
    public required string TypeName { get; init; }
    public required Accessibility Accessibility { get; init; }
    public required ImmutableArray<NotifyPropertyModel> Properties { get; init; }
}
```

### More Model Examples

```csharp
// Nested models for complex generation
public sealed record NotifyPropertyModel
{
    public required string FieldName { get; init; }
    public required string PropertyName { get; init; }
    public required string TypeName { get; init; }
    public required ImmutableArray<string> AlsoNotify { get; init; }
}

// Model with generation options
public sealed record StrongIdModel
{
    public required string Namespace { get; init; }
    public required string TypeName { get; init; }
    public required string BackingType { get; init; }
    public required bool GenerateJsonConverter { get; init; }
    public required bool GenerateTypeConverter { get; init; }
    public required bool GenerateEfConverter { get; init; }
}

// Model capturing method signatures
public sealed record EffectMethodModel
{
    public required string MethodName { get; init; }
    public required string ReturnType { get; init; }
    public required ImmutableArray<ParameterModel> Parameters { get; init; }
    public required bool IsAsync { get; init; }
}
```

## 3. Query Extensions

Extension methods on `ValidSymbol<T>` that build models:

```csharp
// AutoNotify.cs
extension(ValidSymbol<INamedTypeSymbol> symbol)
{
    public AutoNotifyModel? QueryAutoNotify()
    {
        var properties = symbol.QueryNotifyProperties();
        if (properties.IsEmpty)
            return null;

        return new AutoNotifyModel
        {
            Namespace = symbol.Namespace ?? "",
            TypeName = symbol.Name,
            Accessibility = symbol.Accessibility,
            Properties = properties
        };
    }
}
```

### More Query Extension Examples

```csharp
// Query fields with a specific attribute
extension(ValidSymbol<INamedTypeSymbol> symbol)
{
    public ImmutableArray<NotifyPropertyModel> QueryNotifyProperties()
    {
        return [..symbol.QueryFields()
            .ThatArePrivate()
            .WithAttribute<NotifyAttribute>()
            .GetAll()
            .Select(field => new NotifyPropertyModel
            {
                FieldName = field.Name,
                PropertyName = field.Name.TrimStart('_').ToPascalCase(),
                TypeName = field.Type.ToDisplayString(),
                AlsoNotify = field.GetAttribute<AlsoNotifyAttribute>()
                    .NamedArgArray<string>("Properties")
                    .OrEmpty()
            })];
    }
}

// Query methods matching a pattern
extension(ValidSymbol<INamedTypeSymbol> symbol)
{
    public ImmutableArray<EffectMethodModel> QueryEffectMethods()
    {
        return [..symbol.QueryMethods()
            .ThatArePublic()
            .ThatAreNotStatic()
            .WithAttribute<EffectAttribute>()
            .GetAll()
            .Select(method => new EffectMethodModel
            {
                MethodName = method.Name,
                ReturnType = method.ReturnType.ToDisplayString(),
                Parameters = [..method.Parameters.Select(p => new ParameterModel
                {
                    Name = p.Name,
                    Type = p.Type.ToDisplayString()
                })],
                IsAsync = method.IsAsync
            })];
    }
}

// Query with validation
extension(ValidSymbol<INamedTypeSymbol> symbol)
{
    public StrongIdModel? QueryStrongId()
    {
        var attr = symbol.GetAttribute<StrongIdAttribute>();
        if (attr.IsNotValid(out var valid))
            return null;

        var query = new StrongIdAttributeQuery(valid.AttributeData);

        return new StrongIdModel
        {
            Namespace = symbol.Namespace ?? "",
            TypeName = symbol.Name,
            BackingType = query.BackingType,
            GenerateJsonConverter = query.GenerateJsonConverter,
            GenerateTypeConverter = query.GenerateTypeConverter,
            GenerateEfConverter = query.GenerateEfConverter
        };
    }
}
```

## Query Best Practices

### Use Fluent Chains

Compose filters naturally:

```csharp
// Good: fluent chain
var methods = type.QueryMethods()
    .ThatArePublic()
    .ThatAreAsync()
    .WithReturnType("Task")
    .GetAll();

// Avoid: manual filtering
var methods = type.GetMembers()
    .OfType<IMethodSymbol>()
    .Where(m => m.DeclaredAccessibility == Accessibility.Public)
    .Where(m => m.IsAsync)
    .Where(m => m.ReturnType.Name == "Task");
```

### More Fluent Chain Examples

```csharp
// Find all public properties with setters
var mutableProperties = type.QueryProperties()
    .ThatArePublic()
    .ThatHaveSetter()
    .GetAll();

// Find constructors with parameters
var parameterizedCtors = type.QueryConstructors()
    .WithMinParameters(1)
    .GetAll();

// Find fields of a specific type
var loggerFields = type.QueryFields()
    .OfType("ILogger")
    .GetAll();

// Combine multiple filters
var commandMethods = type.QueryMethods()
    .ThatArePublic()
    .ThatAreNotStatic()
    .WithReturnType("Task")
    .WithAttribute<CommandAttribute>()
    .GetAll();
```

### Early Exit with IsNotValid

Use the projection pattern for null-safety:

```csharp
var attr = symbol.GetAttribute("MyAttribute");

if (attr.IsNotValid(out var valid))
    return null;  // Early exit

// valid is guaranteed non-null
var name = valid.NamedArg("Name").OrDefault("Default");
```

### Use OrDefault for Optional Values

```csharp
var maxRetries = attr.NamedArg("MaxRetries").OrDefault(3);
var prefix = attr.NamedArg("Prefix").OrDefault("");
```

### Handle Missing Values Explicitly

```csharp
// OrNull for truly optional values
var customName = attr.NamedArg<string>("CustomName").OrNull();
if (customName is not null)
{
    // Use custom name
}

// OrThrow for required values
var requiredId = attr.NamedArg<string>("Id").OrThrow("Id is required");

// Check validity before accessing
var optional = attr.NamedArg<int>("Timeout");
if (optional.IsValid(out var timeout))
{
    // Use timeout value
}
```
