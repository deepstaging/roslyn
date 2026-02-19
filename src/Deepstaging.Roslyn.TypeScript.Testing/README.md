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
- **`TsTestBase.VerifyEmit()`** — Snapshot-verify emitted TypeScript via [Verify](https://github.com/VerifyTests/Verify). Asserts success then compares against `.verified.txt`
- **`TsOptionalEmitAssertions`** — Fluent TUnit assertions for `TsOptionalEmit` results
- **`TsValidEmitAssertions`** — Fluent TUnit assertions for `TsValidEmit` results

## Snapshot Testing

Use `VerifyEmit` to snapshot-verify the emitted TypeScript output:

```csharp
public class MySnapshotTests : TsTestBase
{
    [Test]
    public Task Emits_Expected_Interface() =>
        VerifyEmit(
            TsTypeBuilder.Interface("User")
                .Exported()
                .AddProperty("name", "string", p => p)
                .AddProperty("age", "number", p => p)
                .Emit(DefaultOptions));
}
```

On first run, a `.received.txt` file is created. Accept it as the `.verified.txt` baseline. Future runs compare against it.

## Prerequisites

TypeScript must be installed in the test project directory:

```bash
cd test/YourProject.Tests
npm install typescript
```
