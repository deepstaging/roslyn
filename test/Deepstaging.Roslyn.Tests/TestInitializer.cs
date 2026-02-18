// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Runtime.CompilerServices;

namespace Deepstaging.Roslyn.Tests;

internal static class TestInitializer
{
    [ModuleInitializer]
    public static void Initialize() =>
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(PipelineModelAttribute),
            typeof(ValidSymbol<>),
            typeof(EquatableArray<>),
            typeof(ISymbol),
            typeof(INamedTypeSymbol),
            typeof(CodeFixes.GenerateHelperAttribute),
            typeof(CodeFixes.NeedsTypeFixAttribute),
            typeof(CodeFixes.NeedsMethodFixAttribute),
            typeof(CodeFixes.NeedsPropertyFixAttribute),
            typeof(CodeFixes.NeedsFieldFixAttribute)
        );
}