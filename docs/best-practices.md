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
| [RoslynKit Template](https://github.com/deepstaging/roslyn/tree/main/templates) | Starting point for your own Roslyn projects. Use `dotnet new roslynkit` to scaffold a new solution with the recommended structure. |

## Summary

1. **Separate concerns** into distinct projects (Attributes, Projection, Generators, Analyzers, CodeFixes)
2. **Use the Projection pattern** as the single source of truth for attribute interpretation
3. **Use [PipelineModel](api/projections/pipeline-model.md) records** with `EquatableArray<T>` and [snapshot types](api/projections/snapshots.md) for correct incremental caching
4. **Keep generators thin**—delegate to projection queries and writer extensions
5. **Leverage base classes** for analyzers and code fixes
6. **Use fluent APIs** for queries and emit—they're more readable and composable
7. **Test thoroughly** with RoslynTestBase and snapshot verification
