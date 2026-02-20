# Architecture

## The Projection Pattern

Every Roslyn tool does three things: **query** symbols, **validate** them, and **emit** code or diagnostics. This project separates those concerns into distinct projects:

```
Roslyn Symbol → Projection (Query → Model) → Generator / Analyzer
```

The **Projection** layer is the key insight — it extracts strongly-typed models from Roslyn symbols once, then both the generator and analyzer consume the same models. No duplicated symbol-walking logic.

## Data Flow

```
┌──────────────────────────────────────────────────┐
│                RoslynKit (Attributes)              │
│  [AutoNotify], [AlsoNotify]                       │
└──────────────────────────────────────────────────┘
                        │
                        ▼
┌──────────────────────────────────────────────────┐
│             RoslynKit.Projection                   │
│                                                    │
│  ValidSymbol<INamedTypeSymbol>                     │
│       → .QueryAutoNotify()                         │
│       → AutoNotifyModel                            │
│           ├── Namespace, TypeName, Accessibility   │
│           └── Fields[]                             │
│               ├── FieldName, PropertyName, Type    │
│               └── AlsoNotify[]                     │
└──────────────────────────────────────────────────┘
           │                         │
           ▼                         ▼
   ┌──────────────┐          ┌──────────────┐
   │  Generators   │          │  Analyzers   │
   │               │          │              │
   │  Model →      │          │  Symbol →    │
   │  TypeBuilder → │          │  Diagnostic  │
   │  Source Code   │          │              │
   └──────────────┘          └──────────────┘
                                     │
                                     ▼
                             ┌──────────────┐
                             │  CodeFixes   │
                             │              │
                             │  Diagnostic →│
                             │  Syntax Fix  │
                             └──────────────┘
```

## Why Separate Projects?

| Project | Loaded by | Target |
|---------|-----------|--------|
| **RoslynKit** | Consumer code (runtime) | `netstandard2.0` |
| **Projection** | Generator & Analyzer (compile-time) | `netstandard2.0` |
| **Generators** | Compiler (compile-time) | `netstandard2.0` |
| **Analyzers** | Compiler + IDE (compile-time) | `netstandard2.0` |
| **CodeFixes** | IDE only (design-time) | `netstandard2.0` |

All target `netstandard2.0` because Roslyn loads analyzers and generators in a constrained environment. Everything ships as a single NuGet package — the root `.csproj` bundles all DLLs into the correct package paths.

### Satellite Extensibility

The **Projection** project is also bundled in a `satellite/` folder within the NuGet package. This allows other packages to reference your projection models and query extensions, building new generators on top of your attribute semantics without duplicating symbol-walking logic. See the [Packaging guide](packaging.md#satellite-projection) for details.

## Key Deepstaging.Roslyn APIs

| API | Purpose |
|-----|---------|
| `ValidSymbol<T>` | Safe wrapper over `ISymbol` — guarantees non-null, enables fluent queries |
| `AttributeQuery` | Typed accessor for attribute constructor arguments |
| `TypeBuilder` / `MethodBuilder` | Fluent syntax tree builders — call `.Emit()` to get `CompilationUnitSyntax` |
| `SymbolAnalyzer<T>` | One-method analyzer base class — implement `ShouldReport` |
| `SyntaxCodeFix<T>` | One-method code fix base class — implement `Fix` |
| `RoslynTestBase` | Test base with `GenerateWith<T>`, `AnalyzeWith<T>`, `AnalyzeAndFixWith<A, F>` |

See the [Deepstaging.Roslyn documentation](https://deepstaging.github.io/roslyn) for full API reference.
