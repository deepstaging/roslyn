# Patterns

TypeBuilder extensions for common design patterns.

> **See also:** [TypeBuilder](type-builder.md) | [Emit Overview](index.md)

## Builder Pattern

Adds a nested record-based Builder using C#'s native `with` expression syntax.

!!! note
    Only applies to **classes**. Records already have native `with` syntax, interfaces cannot be instantiated, and structs are value types where builders aren't typical.

```csharp
var result = TypeBuilder
    .Class("Person")
    .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors())
    .AddProperty("Age", "int", p => p.WithAutoPropertyAccessors())
    .WithBuilder()
    .Emit();
```

Generates:

```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public static Builder CreateBuilder() => new Builder();

    public record Builder
    {
        public string Name { get; init; }
        public int Age { get; init; }

        public Builder WithName(string value) => this with { Name = value };
        public Builder WithAge(int value) => this with { Age = value };

        public Person Build() => new Person(Name, Age);
    }
}
```

### Custom Builder Name

```csharp
builder.WithBuilder("PersonBuilder")
```

---

## Singleton Pattern

Adds a private constructor and thread-safe static `Instance` property using `Lazy<T>`.

```csharp
var result = TypeBuilder
    .Class("Logger")
    .AsSingleton()
    .Emit();
```

Generates:

```csharp
public class Logger
{
    private static readonly Lazy<Logger> _instance = new(() => new Logger());
    
    private Logger() { }
    
    public static Logger Instance => _instance.Value;
}
```

### Custom Instance Property Name

```csharp
builder.AsSingleton("Current")  // Generates: public static Logger Current => ...
```

---

## BackgroundService Pattern

Scaffolds a `BackgroundService` subclass with the `ExecuteAsync` override already wired up.

### Body Builder Overload

```csharp
var result = TypeBuilder
    .Class("OrderProcessor")
    .AsSealed()
    .AsBackgroundService(body => body
        .AddStatement("await foreach (var item in _channel.Reader.ReadAllAsync(stoppingToken))")
        .AddStatement("{")
        .AddStatement("    await ProcessItemAsync(item);")
        .AddStatement("}"))
    .Emit();
```

Generates:

```csharp
public sealed class OrderProcessor : global::Microsoft.Extensions.Hosting.BackgroundService
{
    protected async override global::System.Threading.Tasks.Task ExecuteAsync(
        global::System.Threading.CancellationToken stoppingToken)
    {
        await foreach (var item in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            await ProcessItemAsync(item);
        }
    }
}
```

### Expression Body Overload

```csharp
TypeBuilder
    .Class("PingWorker")
    .AsSealed()
    .AsBackgroundService("RunLoopAsync(stoppingToken)")
    .Emit();
```

### Dispose Override

Add a `Dispose(bool)` override that runs cleanup before calling `base.Dispose(disposing)`:

```csharp
TypeBuilder
    .Class("QueueService")
    .AsSealed()
    .AsBackgroundService(body => body
        .AddStatement("await ProcessAsync(stoppingToken);"))
    .WithDisposeOverride("_channel?.Writer.TryComplete();")
    .Emit();
```

Generates a `Dispose` method that executes the provided statements then calls `base.Dispose(disposing);`.

### Combining with Constructor Injection

```csharp
TypeBuilder
    .Class("EventWorker")
    .AsSealed()
    .AddField("_channel", ChannelTypes.Channel(TypeRef.From("DomainEvent")),
        f => f.WithAccessibility(Accessibility.Private).AsReadOnly())
    .AddConstructor(c => c
        .AddParameter("channel", ChannelTypes.Channel(TypeRef.From("DomainEvent")))
        .WithBody(body => body.AddStatement("_channel = channel;")))
    .AsBackgroundService(body => body
        .AddStatement("await foreach (var evt in _channel.Reader.ReadAllAsync(stoppingToken))")
        .AddStatement("{")
        .AddStatement("    await HandleAsync(evt);")
        .AddStatement("}"))
    .Emit();
```

---

## ToString Override

Override `ToString()` to delegate to a backing value.

### Basic

```csharp
builder.OverridesToString("Value")
// Generates: public override string ToString() => Value.ToString();
```

### Null-Safe

```csharp
builder.OverridesToStringNullSafe("Value")
// Generates: public override string ToString() => Value?.ToString() ?? "";

builder.OverridesToStringNullSafe("Value", "\"<null>\"")
// Generates: public override string ToString() => Value?.ToString() ?? "<null>";
```

### Custom Expression

```csharp
builder.OverridesToString("$\"Person: {Name}, Age: {Age}\"", isCustomExpression: true)
// Generates: public override string ToString() => $"Person: {Name}, Age: {Age}";
```

### From Property or Field

```csharp
var valueProperty = PropertyBuilder.For("Value", "string");
builder.OverridesToString(valueProperty)

var backingField = FieldBuilder.Parse("private readonly Guid _value;");
builder.OverridesToString(backingField)
```
