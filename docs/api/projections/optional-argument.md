# OptionalArgument<T>

Wraps an attribute argument value that may or may not be present.

> **See also:** [Projections Overview](index.md) | [OptionalAttribute](optional-attribute.md) | [OptionalValue](optional-value.md)

## Extracting Values

```csharp
string value = arg.OrDefault("fallback");
int count = arg.OrDefault(() => ComputeDefault());
string value = arg.OrThrow("Argument required");
string value = arg.OrThrow(() => new CustomException());
string? maybeNull = arg.OrNull();
```

## Transforming

```csharp
OptionalArgument<int> length = arg.Map(s => s.Length);
OptionalArgument<int> length = arg.Select(s => s.Length);  // alias
OptionalArgument<MyEnum> enumValue = arg.ToEnum<MyEnum>(); // Roslyn stores enums as ints
```

## Pattern Matching

```csharp
string result = arg.Match(
    whenPresent: v => $"Value: {v}",
    whenEmpty: () => "No value");

if (arg.TryGetValue(out var value)) { /* use value */ }
if (arg.IsMissing(out var value)) return;  // early exit pattern
```

## State & Actions

```csharp
arg.HasValue    // bool
arg.IsEmpty     // bool
arg.Value       // T (throws if empty)
arg.Do(v => Console.WriteLine(v));
arg.OrElse(() => fallback);
```
