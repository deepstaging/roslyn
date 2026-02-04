// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Basic.Reference.Assemblies;

namespace Deepstaging.Roslyn.Testing;

/// <summary>
/// Internal infrastructure for creating test compilations with proper references.
/// For tests, use SymbolTestBase, AnalyzerTestBase&lt;T&gt;, or GeneratorTestBase&lt;T&gt; instead.
/// </summary>
internal static class CompilationHelper
{
    /// <summary>
    /// Creates a compilation from source code.
    /// </summary>
    internal static CSharpCompilation CreateCompilation(
        string source,
        IEnumerable<MetadataReference>? additionalReferences = null)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            source,
            CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp14));

        var references = GetDefaultReferences();

        if (additionalReferences != null)
            references = references.AddRange(additionalReferences);

        return CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    /// <summary>
    /// Gets the default metadata references needed for compilation.
    /// Includes references to common types, generator-related assemblies, and any
    /// configured references from ReferenceConfiguration.
    /// </summary>
    private static ImmutableArray<MetadataReference> GetDefaultReferences() =>
    [
        ..Net100.ReferenceInfos.All
            .Select(x => x.Reference)
            .Concat(ReferenceConfiguration.GetAdditionalReferences())
    ];
}