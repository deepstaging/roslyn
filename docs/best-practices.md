# Best Practices

This guide covers recommended patterns for organizing Roslyn projects using Deepstaging.Roslyn.

## Quick Links

| Guide | Description |
|-------|-------------|
| [Project Organization](guides/project-organization.md) | Folder structure, project responsibilities, file organization |
| [Projection Pattern](guides/projection-pattern.md) | AttributeQuery types, Models, Query extensions |
| [Generators](guides/generators.md) | Generator pattern, Writer classes, Emit best practices |
| [Analyzers](guides/analyzers.md) | Analyzer base classes, examples |
| [CodeFixes](guides/codefixes.md) | CodeFix base classes, helpers |
| [Testing](guides/testing.md) | RoslynTestBase, snapshot testing, ModuleInitializer |

## Learning Resources

| Resource | Purpose |
|----------|---------|
| [RoslynKit Template](https://github.com/deepstaging/templates) | Starting point for your own Roslyn projects. Use `dotnet new roslynkit` to scaffold a new solution with the recommended structure. |
| [Deepstaging.Ids](https://github.com/deepstaging/ids) | Reference implementation of a complete Roslyn toolkit. A strongly-typed ID generator demonstrating the Projection pattern, Emit API extensions, and testing practices. |

## Summary

1. **Separate concerns** into distinct projects (Attributes, Projection, Generators, Analyzers, CodeFixes)
2. **Use the Projection pattern** as the single source of truth for attribute interpretation
3. **Keep generators thin**—delegate to projection queries and writer extensions
4. **Leverage base classes** for analyzers and code fixes
5. **Use fluent APIs** for queries and emit—they're more readable and composable
6. **Test thoroughly** with RoslynTestBase and snapshot verification
