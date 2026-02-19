# Deepstaging.RoslynKit.Generators

Source generator that emits partial classes with property implementations for `[AutoNotify]` types.

## How It Works

1. `ForAttribute<AutoNotifyAttribute>()` discovers annotated types
2. `QueryAutoNotify()` projects each type into an `AutoNotifyModel`
3. `WriteAutoNotifyClass()` emits the partial class via the Emit API

<!--#if (includeRuntime) -->
The generated class inherits from `ObservableObject` and uses `SetField` for equality-checked property setters. No `INotifyPropertyChanged` boilerplate is emitted — it's all provided by the base class.
<!--#else -->
The generated class implements `INotifyPropertyChanged` directly, emitting the event, `OnPropertyChanged`, and a private `SetProperty<T>` helper for equality-checked setters.
<!--#endif -->

## Key Files

| File | Purpose |
|------|---------|
| `AutoNotifyGenerator.cs` | Entry point — wires pipeline stages together |
| `Writers/AutoNotifyWriter.cs` | Emit logic — builds the partial class with properties |

## Bundling

The csproj uses `GetDependencyTargetPaths` to bundle `Deepstaging.Roslyn`, the attribute assembly, and `Projection` DLLs alongside the generator. Source generators are loaded in isolation by the compiler, so all dependencies must be co-located.
