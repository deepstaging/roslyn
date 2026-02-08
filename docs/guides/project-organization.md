# Project Organization

A well-structured Roslyn toolkit separates concerns across multiple projects:

```
MyProject/
├── src/
│   ├── MyProject.RoslynKit/           # Attributes only
│   ├── MyProject.RoslynKit.Projection/ # Queries and Models
│   ├── MyProject.RoslynKit.Generators/ # Source generators
│   ├── MyProject.RoslynKit.Analyzers/  # Diagnostic analyzers
│   ├── MyProject.RoslynKit.CodeFixes/  # Code fix providers
│   └── MyProject.RoslynKit.Tests/      # All tests
```

## Project Responsibilities

| Project | Purpose | Dependencies |
|---------|---------|--------------|
| **RoslynKit** | Attribute definitions only | None (users reference this) |
| **Projection** | Query + Model layer | RoslynKit, Deepstaging.Roslyn |
| **Generators** | Source generation | Projection |
| **Analyzers** | Diagnostics | Projection |
| **CodeFixes** | Quick fixes | Analyzers (for DiagnosticId) |

!!! tip "Why separate Projection?"
    The Projection layer is the **single source of truth** for interpreting your attributes. Both generators and analyzers consume the same queries and models, ensuring consistent behavior.

## File Organization

### Projection Layer

```
Projection/
├── Attributes/           # AttributeQuery wrapper types
│   └── AutoNotifyAttributeQuery.cs
├── Models/               # Data models
│   ├── AutoNotifyModel.cs
│   └── NotifyPropertyModel.cs
├── Queries.cs            # ValidAttribute → AttributeQuery conversions
├── AutoNotify.cs         # ValidSymbol extensions for AutoNotify
└── GlobalUsings.cs
```

### Generators Layer

```
Generators/
├── Writers/              # Code generation logic
│   └── AutoNotifyWriter.cs
├── AutoNotifyGenerator.cs
└── GlobalUsings.cs
```

### Analyzers Layer

```
Analyzers/
├── AutoNotifyMustBePartialAnalyzer.cs
├── AutoNotifyFieldMustBePrivateAnalyzer.cs
└── README.md             # Document all diagnostic IDs
```

## Learning Resources

| Resource | Purpose |
|----------|---------|
| [RoslynKit Template](https://github.com/deepstaging/templates) | Starting point for your own Roslyn projects. Use `dotnet new roslynkit` to scaffold a new solution with the recommended structure. |
| [Deepstaging.Ids](https://github.com/deepstaging/ids) | Reference implementation of a complete Roslyn toolkit. A strongly-typed ID generator demonstrating the Projection pattern, Emit API extensions, and testing practices. |
