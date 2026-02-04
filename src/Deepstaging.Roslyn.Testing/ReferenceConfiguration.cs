// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Assembly = System.Reflection.Assembly;

namespace Deepstaging.Roslyn.Testing;

/// <summary>
/// Provides a mechanism for consumers to configure additional metadata references
/// for test compilations. This allows test projects to inject their own assemblies
/// and dependencies into the compilation context.
/// </summary>
public static class ReferenceConfiguration
{
    private static readonly List<MetadataReference> AdditionalReferences = [];
    private static readonly List<Assembly> AdditionalAssemblies = [];
    private static bool _isConfigured;

    /// <summary>
    /// Add metadata references to be included in all test compilations.
    /// This should be called once during test initialization (e.g., in an AssemblyHook or ModuleInitializer).
    /// </summary>
    /// <param name="references">The metadata references to add.</param>
    public static void AddReferences(params MetadataReference[] references)
    {
        lock (AdditionalReferences)
        {
            AdditionalReferences.AddRange(references);
            _isConfigured = true;
        }
    }

    /// <summary>
    /// Add metadata references from assemblies to be included in all test compilations.
    /// This should be called once during test initialization (e.g., in an AssemblyHook or ModuleInitializer).
    /// </summary>
    /// <param name="assemblies">The assemblies to add as references.</param>
    public static void AddReferences(params Assembly[] assemblies)
    {
        lock (AdditionalAssemblies)
        {
            AdditionalAssemblies.AddRange(assemblies);
            _isConfigured = true;
        }
    }

    /// <summary>
    /// Add metadata references from assembly locations to be included in all test compilations.
    /// This should be called once during test initialization (e.g., in an AssemblyHook or ModuleInitializer).
    /// </summary>
    /// <param name="assemblyPaths">The paths to assembly files to add as references.</param>
    public static void AddReferencesFromPaths(params string[] assemblyPaths)
    {
        lock (AdditionalReferences)
        {
            AdditionalReferences.AddRange(
                assemblyPaths.Select(path => MetadataReference.CreateFromFile(path)));
            _isConfigured = true;
        }
    }

    /// <summary>
    /// Add metadata references from types' containing assemblies.
    /// This should be called once during test initialization (e.g., in an AssemblyHook or ModuleInitializer).
    /// </summary>
    /// <param name="types">The types whose assemblies should be added as references.</param>
    public static void AddReferencesFromTypes(params Type[] types)
    {
        lock (AdditionalAssemblies)
        {
            AdditionalAssemblies.AddRange(types.Select(t => t.Assembly).Distinct());
            _isConfigured = true;
        }
    }

    /// <summary>
    /// Gets all configured metadata references.
    /// </summary>
    internal static IEnumerable<MetadataReference> GetAdditionalReferences()
    {
        lock (AdditionalReferences)
        {
            var references = AdditionalReferences.ToList();

            lock (AdditionalAssemblies)
            {
                references.AddRange(AdditionalAssemblies
                    .Where(a => !string.IsNullOrEmpty(a.Location))
                    .Select(a => MetadataReference.CreateFromFile(a.Location)));
            }

            return references;
        }
    }

    /// <summary>
    /// Checks if any references have been configured.
    /// </summary>
    internal static bool IsConfigured => _isConfigured;

    /// <summary>
    /// Clears all configured references. Primarily for testing purposes.
    /// </summary>
    public static void Clear()
    {
        lock (AdditionalReferences)
        {
            AdditionalReferences.Clear();
        }

        lock (AdditionalAssemblies)
        {
            AdditionalAssemblies.Clear();
        }

        _isConfigured = false;
    }
}