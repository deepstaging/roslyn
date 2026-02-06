# OptionalValue<T>

Generic optional wrapper for any value (not specific to Roslyn symbols).

> **See also:** [Projections Overview](index.md) | [OptionalArgument](optional-argument.md)

## Creating

```csharp
OptionalValue<string>.WithValue("hello")
OptionalValue<string>.Empty()
```

## Usage

Same API as `OptionalArgument`:

```csharp
value.Map(s => s.Length)
value.OrDefault("fallback")
value.OrThrow()
value.Match(whenPresent: ..., whenEmpty: ...)
```
