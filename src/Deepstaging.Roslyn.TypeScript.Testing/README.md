# Deepstaging.Roslyn.TypeScript.Testing

Testing infrastructure for TypeScript code generation with
[Deepstaging.Roslyn.TypeScript](https://github.com/deepstaging/roslyn).

📚 **[Full Documentation](https://deepstaging.github.io/roslyn/api/typescript/)**

## Usage

Inherit from `TsTestBase` to get automatic `tsc` discovery and pre-configured emit options:

```csharp
using Deepstaging.Roslyn.TypeScript.Testing;

public class MyTests : TsTestBase
{
    [Test]
    public async Task Emits_Valid_Interface()
    {
        var result = TsTypeBuilder.Interface("User")
            .Exported()
            .AddProperty("name", "string", p => p)
            .Emit(ValidatedOptions);

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Diagnostics).IsEmpty();
    }
}
```

## Features

- **`TsTestBase`** — Base class with auto-discovered `TscPath` and option presets (`DefaultOptions`, `ValidatedOptions`, `FormattedOptions`, `FormattedAndValidatedOptions`)
- **`TsOptionalEmitAssertions`** — Fluent TUnit assertions for `TsOptionalEmit` results
- **`TsValidEmitAssertions`** — Fluent TUnit assertions for `TsValidEmit` results

## Prerequisites

TypeScript must be installed in the test project directory:

```bash
cd test/YourProject.Tests
npm install typescript
```
