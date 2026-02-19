# Deepstaging.RoslynKit.CodeFixes

Code fix providers for RK001 and RK002 diagnostics.

## Fixes

| Fix | Diagnostic | Action |
|-----|-----------|--------|
| `MakePartialCodeFix` | RK001 | Adds `partial` modifier to the class |
| `MakePrivateCodeFix` | RK002 | Changes field accessibility to `private` |

Both extend `SyntaxCodeFix<T>` from Deepstaging.Roslyn — a one-method base class that handles all the boilerplate of `RegisterCodeFixesAsync`.
