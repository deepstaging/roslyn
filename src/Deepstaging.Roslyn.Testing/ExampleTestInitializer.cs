// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Testing;

/// <summary>
/// Example showing how to configure references for test compilations using a ModuleInitializer.
/// This runs once before any code in the assembly executes, ensuring all tests have access
/// to the configured references.
/// </summary>
internal static class ExampleTestInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Example 1: Add references from types
        // ReferenceConfiguration.AddReferencesFromTypes(
        //     typeof(MyFeature),
        //     typeof(MyOtherFeature)
        // );
        
        // Example 2: Add references from assemblies
        // ReferenceConfiguration.AddReferences(
        //     typeof(MyFeature).Assembly
        // );
        
        // Example 3: Add references from file paths
        // ReferenceConfiguration.AddReferencesFromPaths(
        //     "path/to/MyAssembly.dll"
        // );
    }
}
