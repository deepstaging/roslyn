# Configuring References for Test Compilations

> **See also:** [Testing README](../README.md) | [RoslynTestBase](RoslynTestBase.md) | [Roslyn Toolkit](../../Deepstaging.Roslyn/README.md)

When testing your Roslyn analyzers, generators, or code transformations, you often need to include references to your own assemblies and their dependencies in the test compilations. Deepstaging.Testing provides a `ReferenceConfiguration` API for this purpose.

## Quick Start

### Option 1: ModuleInitializer (Recommended)

Use a `ModuleInitializer` to configure references once before any code runs:

```csharp
using System.Runtime.CompilerServices;
using Deepstaging.Testing;

internal static class TestInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Add references from types in your feature assemblies
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(MyFeature),
            typeof(MyOtherFeature)
        );
    }
}
```

The `ModuleInitializer` runs automatically before any other code in your test assembly, ensuring references are configured for all tests.

### Option 2: TUnit BeforeHook

If you prefer TUnit's lifecycle hooks, create a hook class:

```csharp
using Deepstaging.Testing;
using TUnit.Core;

public class TestAssemblySetup
{
    [Before(HookType.Assembly)]
    public static void ConfigureReferences()
    {
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(MyFeature),
            typeof(MyOtherFeature)
        );
    }
}
```

### Option 3: Base Class Constructor

For smaller test suites, you can configure in a test base class:

```csharp
using Deepstaging.Testing;

public abstract class MyTestBase : RoslynTestBase
{
    static MyTestBase()
    {
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(MyFeature)
        );
    }
}
```

## API Reference

### `ReferenceConfiguration`

Static class that manages metadata references for test compilations.

#### Methods

**`AddReferencesFromTypes(params Type[] types)`**

Adds references from the assemblies containing the specified types.

```csharp
ReferenceConfiguration.AddReferencesFromTypes(
    typeof(MyFeature),
    typeof(AnotherFeature)
);
```

**`AddReferences(params Assembly[] assemblies)`**

Adds references from the specified assemblies.

```csharp
ReferenceConfiguration.AddReferences(
    typeof(MyFeature).Assembly,
    Assembly.Load("MyOtherAssembly")
);
```

**`AddReferencesFromPaths(params string[] assemblyPaths)`**

Adds references from assembly file paths.

```csharp
ReferenceConfiguration.AddReferencesFromPaths(
    "path/to/MyAssembly.dll",
    "path/to/AnotherAssembly.dll"
);
```

**`AddReferences(params MetadataReference[] references)`**

Adds pre-created metadata references.

```csharp
ReferenceConfiguration.AddReferences(
    MetadataReference.CreateFromFile("path/to/assembly.dll")
);
```

**`Clear()`**

Clears all configured references. Primarily useful for testing.

```csharp
ReferenceConfiguration.Clear();
```

## Complete Example

Here's a complete example using a `ModuleInitializer`:

```csharp
// TestInitializer.cs
using System.Runtime.CompilerServices;
using Deepstaging.Testing;
using MyApp.Features;
using MyApp.Core;

internal static class TestInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Add all your feature assemblies
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(UserFeature),
            typeof(OrderFeature),
            typeof(PaymentFeature),
            typeof(CoreServices)
        );
    }
}

// MyGeneratorTests.cs
using Deepstaging.Testing;
using TUnit.Core;

public class MyGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task ShouldGenerateCodeForMyFeature()
    {
        // The compilation will automatically include references
        // configured in TestInitializer
        var result = await Generate<MyGenerator>("""
            using MyApp.Features;
            
            [GenerateStuff]
            public class MyClass : UserFeature
            {
                public int Id { get; set; }
            }
            """)
            .ShouldGenerate()
            .Execute();
            
        await Verify(result);
    }
}
```

## How It Works

When you call methods like `Symbols()`, `Analyze<T>()`, or `Generate<T>()` in your tests, Deepstaging.Testing creates a `CSharpCompilation` with:

1. **Default references**: .NET runtime assemblies and Deepstaging assemblies
2. **Configured references**: Any references you added via `ReferenceConfiguration`
3. **Additional references**: Any passed directly to the method (e.g., `Symbols(source, additionalReferences)`)

This ensures your test compilations have access to all the types your generators and analyzers need to work correctly.

## Best Practices

1. **Use ModuleInitializer**: It's the simplest approach and runs before any other code in your assembly.

2. **Configure once**: Call `ReferenceConfiguration` methods only during initialization, not in individual tests.

3. **Add only what you need**: Don't add every assembly in your project—only those that your test compilations actually reference.

4. **Use type-based configuration**: Prefer `AddReferencesFromTypes()` over path-based configuration for better maintainability.

## Troubleshooting

**"Type or namespace not found" errors in tests**

Your test compilations don't have a reference to the assembly containing that type. Add it via `ReferenceConfiguration`:

```csharp
ReferenceConfiguration.AddReferencesFromTypes(typeof(MissingType));
```

**References not being applied**

Make sure your `ModuleInitializer` or hook is in the same assembly as your tests and marked as `internal` or `public`.

**Multiple initializations**

`ReferenceConfiguration` is thread-safe and can be called multiple times, but it's best to configure everything once in a single initializer.

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../../../LICENSE) for the full legal text.
