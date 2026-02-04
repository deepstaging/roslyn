# GeneratorTestContext

Test Roslyn source generators with fluent output assertions.

> **See also:** [RoslynTestBase](RoslynTestBase.md) | [Scriban Templates](../../Deepstaging.Roslyn.Scriban/README.md)

## Overview

`GeneratorTestContext` runs your incremental source generator against source code and provides a fluent API for asserting on the generated output.

```csharp
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileCount(2)
    .WithFileNamed("Customer.g.cs")
    .WithFileContaining("public partial class")
    .VerifySnapshot();
```

---

## Entry Point

From `RoslynTestBase`:

```csharp
GeneratorTestContext GenerateWith<TGenerator>(string source)
    where TGenerator : IIncrementalGenerator, new()
```

---

## Asserting Generation Occurs

### Basic Generation Check

```csharp
// Assert something was generated
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate();

// Assert nothing was generated
await GenerateWith<MyGenerator>(source)
    .ShouldNotGenerate();
```

### File Count

```csharp
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileCount(3);
```

### File Names

```csharp
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileNamed("Customer.g.cs");
```

### File Content

```csharp
// Assert content exists
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileContaining("public partial class Customer");

// Assert content does NOT exist
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithoutFileContaining("// TODO");
```

---

## Chaining Assertions

All assertions can be chained:

```csharp
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileCount(2)
    .WithFileNamed("Customer.g.cs")
    .WithFileNamed("CustomerBuilder.g.cs")
    .WithFileContaining("public partial class Customer")
    .WithFileContaining("public class CustomerBuilder")
    .WithoutFileContaining("NotImplementedException");
```

---

## Diagnostic Assertions

### No Diagnostics

```csharp
// No diagnostics at all
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithNoDiagnostics();

// No errors
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithNoErrors();

// No warnings
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithNoWarnings();
```

### Filtered Diagnostic Checks

```csharp
// No diagnostics matching filter
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithNoDiagnostics(filter => filter
        .WithSeverity(DiagnosticSeverity.Error));

// No specific diagnostic ID
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithNoDiagnostics(filter => filter
        .WithId("GEN001"));

// Multiple IDs
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithNoDiagnostics(filter => filter
        .WithIds("GEN001", "GEN002"));

// By message content
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithNoDiagnostics(filter => filter
        .WithMessageContaining("deprecated"));
```

---

## Snapshot Testing

Verify generates snapshot files for approval testing:

```csharp
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .VerifySnapshot();
```

Snapshots are stored next to your test file:
- Test: `MyGeneratorTests.cs`
- Snapshot: `MyGeneratorTests.GeneratesExpectedOutput.verified.txt`

### Combined with Other Assertions

```csharp
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileCount(2)
    .WithNoDiagnostics()
    .VerifySnapshot();
```

---

## Common Patterns

### Testing Generator Produces Expected Output

```csharp
[Test]
public async Task GeneratesPartialClass()
{
    var source = """
        [AutoNotify]
        public partial class Customer 
        {
            private string _name;
        }
        """;

    await GenerateWith<AutoNotifyGenerator>(source)
        .ShouldGenerate()
        .WithFileNamed("Customer.AutoNotify.g.cs")
        .WithFileContaining("public string Name")
        .WithFileContaining("PropertyChanged?.Invoke")
        .VerifySnapshot();
}
```

### Testing Generator Skips Invalid Input

```csharp
[Test]
public async Task SkipsNonPartialClass()
{
    var source = """
        [AutoNotify]
        public class Customer  // Not partial!
        {
            private string _name;
        }
        """;

    await GenerateWith<AutoNotifyGenerator>(source)
        .ShouldNotGenerate();
}
```

### Testing Generator Reports Diagnostics

```csharp
[Test]
public async Task ReportsErrorForInvalidUsage()
{
    var source = """
        [AutoNotify]
        public partial struct Point  // Struct not supported
        {
            private int _x;
        }
        """;

    // Generator might still produce output but with diagnostics
    var context = GenerateWith<AutoNotifyGenerator>(source);
    var result = await context.GetResultAsync();
    
    await Assert.That(result.Diagnostics)
        .Contains(d => d.Id == "AN001");
}
```

### Parameterized Tests

```csharp
[Test]
[Arguments("public partial class Foo { }", true)]
[Arguments("public class Foo { }", false)]
[Arguments("public partial struct Foo { }", false)]
public async Task GeneratesOnlyForPartialClasses(string source, bool shouldGenerate)
{
    var context = GenerateWith<MyGenerator>($"[MyAttribute] {source}");
    
    if (shouldGenerate)
        await context.ShouldGenerate();
    else
        await context.ShouldNotGenerate();
}
```

---

## How It Works

1. Source code is parsed with C# 13 language features
2. Compilation is created with configured references
3. Generator driver runs your `IIncrementalGenerator`
4. Results include generated syntax trees and diagnostics
5. Assertions validate against the run result

---

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../../../LICENSE) for the full legal text.
