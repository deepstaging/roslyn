# Deepstaging.Roslyn.Tests

Test suite for the Deepstaging.Roslyn library, covering symbol querying, code emission, and projections.

## Overview

Uses **TUnit** testing framework with fluent assertions. All tests extend `RoslynTestBase` which provides the
`SymbolsFor()` helper for compiling inline C# code and querying symbols.

## Test Categories

### Symbol Querying

Tests for fluent symbol querying APIs:

```csharp
var methods = SymbolsFor(code)
    .RequireNamedType("TestClass")
    .QueryMethods()
    .ThatArePublic()
    .ThatAreAsync()
    .GetAll();
```

| Test File                   | Coverage                                 |
|-----------------------------|------------------------------------------|
| `MethodQuery.Tests.cs`      | Method filtering (public, async, static) |
| `PropertyQuery.Tests.cs`    | Property queries and accessors           |
| `FieldQuery.Tests.cs`       | Field symbol queries                     |
| `ConstructorQuery.Tests.cs` | Constructor discovery                    |
| `EventQuery.Tests.cs`       | Event member queries                     |
| `TypeQuery.Tests.cs`        | Named type queries                       |
| `ParameterQuery.Tests.cs`   | Parameter inspection                     |

### Code Emission

Tests in `Emit/` directory cover code generation builders:

- Type builders
- Method builders
- Property builders
- Field builders
- Parameter options

### Projections

Tests in `Projections/` cover symbol projection utilities and extension methods for `Optional<T>` and `Valid<T>` types.

## Running Tests

```bash
dotnet test
```

## Writing Tests

```csharp
public class MyTests : RoslynTestBase
{
    [Test]
    public async Task Can_query_something()
    {
        var code = """
            public class Example { }
            """;

        var types = SymbolsFor(code)
            .QueryTypes()
            .GetAll();

        await Assert.That(types.Any()).IsTrue();
    }
}
```

## Related Documentation

- **[Documentation Site](https://deepstaging.github.io/roslyn/)** — Full API reference
- **[Core Toolkit](../Deepstaging.Roslyn/README.md)** — Library being tested
- **[Testing Infrastructure](../Deepstaging.Roslyn.Testing/README.md)** — Test base classes and utilities
