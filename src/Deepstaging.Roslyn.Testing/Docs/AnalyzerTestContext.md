# AnalyzerTestContext

Test Roslyn analyzers with fluent diagnostic assertions.

> **See also:** [RoslynTestBase](RoslynTestBase.md) | [Code Fix Testing](CodeFixTestContext.md)

## Overview

`AnalyzerTestContext` runs your analyzer against source code and provides a fluent API for asserting on the diagnostics produced.

```csharp
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldReportDiagnostic("MY001")
    .WithSeverity(DiagnosticSeverity.Error)
    .WithMessage("*must be partial*");
```

---

## Entry Point

From `RoslynTestBase`:

```csharp
AnalyzerTestContext AnalyzeWith<TAnalyzer>(string source)
    where TAnalyzer : DiagnosticAnalyzer, new()
```

---

## Asserting Diagnostics Exist

### Specific Diagnostic ID

```csharp
// Assert MY001 is reported
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldReportDiagnostic("MY001");

// With severity check
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldReportDiagnostic("MY001")
    .WithSeverity(DiagnosticSeverity.Error);

// With message pattern (wildcards supported)
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldReportDiagnostic("MY001")
    .WithMessage("*must be partial*");

// Combined
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldReportDiagnostic("MY001")
    .WithSeverity(DiagnosticSeverity.Error)
    .WithMessage("Type '*' must be declared as partial");
```

### Any Diagnostics

```csharp
// Assert some diagnostics exist (without specific ID)
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldHaveDiagnostics();

// Assert diagnostics exist with specific ID
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldHaveDiagnostics()
    .WithErrorCode("MY001");

// With severity and message
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldHaveDiagnostics()
    .WithErrorCode("MY001")
    .WithSeverity(DiagnosticSeverity.Warning)
    .WithMessage("*deprecated*");
```

---

## Asserting No Diagnostics

### No Specific Diagnostic

```csharp
// Assert MY001 is NOT reported
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldNotReportDiagnostic("MY001");
```

### No Diagnostics At All

```csharp
// Assert no analyzer diagnostics
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldHaveNoDiagnostics();
```

---

## Message Patterns

The `WithMessage()` assertion supports wildcard patterns using `*`:

```csharp
// Exact match
.WithMessage("Type 'Customer' must be partial")

// Starts with
.WithMessage("Type '*' must be partial")

// Contains
.WithMessage("*must be partial*")

// Ends with
.WithMessage("*must be partial")
```

Wildcards are converted to regex `.*` patterns.

---

## Common Patterns

### Testing Valid Code Produces No Diagnostics

```csharp
[Test]
public async Task ValidCode_NoDiagnostics()
{
    var source = """
        public partial class Customer { }
        """;

    await AnalyzeWith<PartialClassAnalyzer>(source)
        .ShouldHaveNoDiagnostics();
}
```

### Testing Invalid Code Reports Expected Diagnostic

```csharp
[Test]
public async Task NonPartialClass_ReportsDiagnostic()
{
    var source = """
        [GenerateEquality]
        public class Customer { }
        """;

    await AnalyzeWith<PartialClassAnalyzer>(source)
        .ShouldReportDiagnostic("GEN001")
        .WithSeverity(DiagnosticSeverity.Error)
        .WithMessage("*must be declared as partial*");
}
```

### Testing Multiple Scenarios

```csharp
[Test]
[Arguments("public partial class Foo { }", false)]
[Arguments("public class Foo { }", true)]
public async Task PartialRequirement(string source, bool expectsDiagnostic)
{
    var context = AnalyzeWith<PartialClassAnalyzer>(source);
    
    if (expectsDiagnostic)
        await context.ShouldReportDiagnostic("GEN001");
    else
        await context.ShouldHaveNoDiagnostics();
}
```

### Testing Diagnostic Location

For precise location testing, get the raw diagnostics:

```csharp
[Test]
public async Task DiagnosticOnCorrectLine()
{
    var source = """
        public class Foo { }
        """;

    var context = AnalyzeWith<MyAnalyzer>(source);
    var diagnostics = await context.GetDiagnosticsAsync();
    
    await Assert.That(diagnostics).HasCount(1);
    
    var location = diagnostics[0].Location.GetLineSpan();
    await Assert.That(location.StartLinePosition.Line).IsEqualTo(0);
}
```

---

## How It Works

1. Source code is compiled with configured references
2. Your analyzer runs against the compilation
3. Compiler diagnostics (CS*) are filtered out
4. Only analyzer diagnostics are available for assertions

---

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../../../LICENSE) for the full legal text.
