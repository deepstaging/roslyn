# CodeFixTestContext

Test Roslyn code fix providers with before/after source assertions.

> **See also:** [RoslynTestBase](RoslynTestBase.md) | [Analyzer Testing](AnalyzerTestContext.md) | [Workspace](../../Deepstaging.Roslyn.Workspace/README.md)

## Overview

`CodeFixTestContext` tests code fix providers by applying a fix to source code and verifying the transformed result.

```csharp
await AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>(source)
    .ForDiagnostic("MY001")
    .ShouldProduce(expectedSource);
```

---

## Entry Points

From `RoslynTestBase`:

### For Analyzer Diagnostics

Use when your code fix targets diagnostics from a custom analyzer:

```csharp
CodeFixTestContext AnalyzeAndFixWith<TAnalyzer, TCodeFix>(string source)
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
```

### For Compiler Diagnostics

Use when your code fix targets compiler errors/warnings (CS* codes):

```csharp
CodeFixTestContext FixWith<TCodeFix>(string source)
    where TCodeFix : CodeFixProvider, new()
```

---

## Basic Usage

### Fixing Analyzer Diagnostics

```csharp
[Test]
public async Task AddsPartialModifier()
{
    var source = """
        [GenerateEquality]
        public class Customer { }
        """;

    var expected = """
        [GenerateEquality]
        public partial class Customer { }
        """;

    await AnalyzeAndFixWith<PartialAnalyzer, AddPartialCodeFix>(source)
        .ForDiagnostic("GEN001")
        .ShouldProduce(expected);
}
```

### Fixing Compiler Diagnostics

```csharp
[Test]
public async Task AddsUsingDirective()
{
    var source = """
        public class Foo 
        {
            List<int> items;
        }
        """;

    var expected = """
        using System.Collections.Generic;

        public class Foo 
        {
            List<int> items;
        }
        """;

    await FixWith<AddUsingCodeFix>(source)
        .ForDiagnostic("CS0246")
        .ShouldProduce(expected);
}
```

---

## Adding an Analyzer Dynamically

You can specify an analyzer after creating the context:

```csharp
await FixWith<MyCodeFix>(source)
    .WithAnalyzer<MyAnalyzer>()
    .ForDiagnostic("MY001")
    .ShouldProduce(expectedSource);
```

This is equivalent to `AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>`.

---

## How Diagnostics Are Found

### Analyzer Diagnostics

When you use `AnalyzeAndFixWith` or `WithAnalyzer`:

1. The analyzer runs against the source
2. First diagnostic matching the ID is selected
3. Code fix is invoked for that diagnostic

### Compiler Diagnostics

When you use `FixWith` without an analyzer:

1. Semantic model is queried for compiler diagnostics
2. First diagnostic matching the ID is selected
3. Code fix is invoked for that diagnostic

---

## Common Patterns

### Testing Multiple Code Fixes

```csharp
[Test]
public async Task FixesMultipleIssues()
{
    // Test first fix
    var source1 = "public class Foo { }";
    var expected1 = "public partial class Foo { }";
    
    await AnalyzeAndFixWith<MyAnalyzer, AddPartialFix>(source1)
        .ForDiagnostic("MY001")
        .ShouldProduce(expected1);

    // Test second fix
    var source2 = "public class Bar { }";
    var expected2 = "public sealed class Bar { }";
    
    await AnalyzeAndFixWith<MyAnalyzer, AddSealedFix>(source2)
        .ForDiagnostic("MY002")
        .ShouldProduce(expected2);
}
```

### Testing Fix Does Nothing for Wrong Diagnostic

```csharp
[Test]
public async Task DoesNotFixUnrelatedDiagnostic()
{
    var source = """
        public class Foo { }
        """;

    var context = FixWith<AddPartialFix>(source);
    
    // This should fail because CS0246 isn't what AddPartialFix handles
    // You'd typically test this by checking no code action is registered
}
```

### Preserving Formatting

The assertion normalizes line endings for comparison, so you don't need to worry about `\r\n` vs `\n`:

```csharp
var expected = """
    public partial class Foo
    {
        public string Name { get; set; }
    }
    """;

// Works regardless of platform line endings
await AnalyzeAndFixWith<MyAnalyzer, MyFix>(source)
    .ForDiagnostic("MY001")
    .ShouldProduce(expected);
```

---

## Limitations

### First Code Action Only

If your code fix registers multiple code actions, only the **first** one is applied and verified.

### First Diagnostic Only

If multiple diagnostics match the ID, only the **first** one triggers the fix.

### No Batch Testing

Currently tests one fix at a time. For "fix all" scenarios, you'd need to apply fixes iteratively.

---

## How It Works

1. Source is compiled into a `Document` in an `AdhocWorkspace`
2. Analyzer runs (if provided) to produce diagnostics
3. `CodeFixProvider.RegisterCodeFixesAsync` is called for the target diagnostic
4. First registered `CodeAction` is executed
5. `ApplyChangesOperation` produces the new solution
6. Fixed document text is compared to expected source

---

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../../../LICENSE) for the full legal text.
