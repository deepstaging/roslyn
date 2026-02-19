# TypeScript Testing

Test utilities for TypeScript code generation.

> **See also:** [Core Testing](../testing/index.md) | [TypeScript Emit Overview](index.md)

## Installation

```bash
dotnet add package Deepstaging.Roslyn.TypeScript.Testing --prerelease
```

Your test project also needs a local TypeScript installation for `tsc` validation:

```bash
cd test/YourProject.Tests
npm init -y && npm install typescript --save-dev
```

## Architecture

```
TsTestBase
  │
  ├── DefaultOptions              → TsEmitOptions (no validation, no formatting)
  ├── ValidatedOptions            → TsEmitOptions (tsc syntax validation)
  ├── FormattedOptions            → TsEmitOptions (dprint/prettier formatting)
  ├── FormattedAndValidatedOptions → TsEmitOptions (both)
  │
  ├── TscPath                     → auto-discovered path to tsc binary
  │
  └── VerifyEmit(result)          → snapshot comparison via Verify
```

## TsTestBase

Inherit from `TsTestBase` to get auto-discovered `tsc` path and pre-configured option presets.

```csharp
using Deepstaging.Roslyn.TypeScript.Emit;
using Deepstaging.Roslyn.TypeScript.Testing;

public class MyGeneratorTests : TsTestBase
{
    [Test]
    public async Task Emits_Valid_Interface()
    {
        var result = TsTypeBuilder.Interface("User")
            .Exported()
            .AddProperty("id", "number", p => p.AsReadonly())
            .Emit(ValidatedOptions);

        await Assert.That(result).IsSuccessful();
    }
}
```

### Option Presets

| Preset | Validation | Formatting | Use Case |
|--------|-----------|------------|----------|
| `DefaultOptions` | None | No | Fast unit tests — verify emitted string content only |
| `ValidatedOptions` | `tsc` syntax | No | Verify emitted code compiles — requires TypeScript installed |
| `FormattedOptions` | None | Yes | Verify formatted output — requires dprint or prettier |
| `FormattedAndValidatedOptions` | `tsc` syntax | Yes | Full pipeline — compile check + formatted output |

### TscPath Discovery

`TscPath` walks up from `AppContext.BaseDirectory` (typically `bin/Release/net10.0/`) looking for `node_modules/.bin/tsc` in each ancestor directory. If not found, `ValidatedOptions` and `FormattedAndValidatedOptions` throw `InvalidOperationException` at emit time.

## Assertions

The package provides fluent TUnit assertions for both emit result types. Import the assertions namespace:

```csharp
using Deepstaging.Roslyn.TypeScript.Testing.Assertions;
```

### TsOptionalEmit Assertions

```csharp
var result = TsTypeBuilder.Class("Foo").Emit();

await Assert.That(result).IsSuccessful();
await Assert.That(result).HasFailed();
await Assert.That(result).HasDiagnostics();
await Assert.That(result).HasNoDiagnostics();
await Assert.That(result).CodeContains("export class Foo");
await Assert.That(result).CodeDoesNotContain("import");
```

### TsValidEmit Assertions

After unwrapping with `TryValidate`:

```csharp
if (result.TryValidate(out var valid))
{
    await Assert.That(valid).CodeContains("export class Foo");
    await Assert.That(valid).CodeDoesNotContain("import");
}
```

## Snapshot Testing with VerifyEmit

`VerifyEmit` combines emit validation with [Verify](https://github.com/VerifyTests/Verify) snapshot testing. It asserts the emit succeeded, then compares the output against a `.verified.txt` file co-located with the test source.

```csharp
public class MySnapshotTests : TsTestBase
{
    [Test]
    public Task Emits_Expected_UserService() =>
        VerifyEmit(
            TsTypeBuilder.Class("UserService")
                .Exported()
                .AddField("baseUrl", "string", f => f
                    .WithAccessibility(TsAccessibility.Private)
                    .AsReadonly())
                .AddConstructor(c => c
                    .AddParameter("baseUrl", "string", p =>
                        p.AsParameterProperty(TsAccessibility.Private)))
                .AddMethod("getUsers", m => m
                    .Async()
                    .WithReturnType(new TsPromiseTypeRef(new TsArrayTypeRef("User")))
                    .WithBody(b => b
                        .AddReturn("await fetch(this.baseUrl + '/users')")))
                .Emit(DefaultOptions));
}
```

On first run, Verify creates a `.received.txt` file. Accept it as the `.verified.txt` baseline:

```bash
# Accept all pending snapshots
dotnet tool run verify -- accept
# Or manually rename .received.txt → .verified.txt
```

Future runs compare the emitted output against the baseline. If the output changes, the test fails with a diff.

### Tips

- **No `.ts` extension** — Verify stores snapshots as `.verified.txt` by default. The content is TypeScript but the file extension is `.txt`.
- **Co-location** — Snapshot files are stored next to the test source file, not in the build output directory.
- **`CallerFilePath`** — `VerifyEmit` uses `[CallerFilePath]` internally, so Verify can locate the correct snapshot file automatically.
- **Failure on emit error** — If the emit result contains diagnostics, `VerifyEmit` throws `InvalidOperationException` before reaching Verify.
