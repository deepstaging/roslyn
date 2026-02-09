// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Testing;
using Deepstaging.RoslynKit.Analyzers;
using Deepstaging.RoslynKit.CodeFixes;

namespace Deepstaging.RoslynKit.Tests;

/// <summary>
/// Tests for the MakePartialClassCodeFixProvider.
/// </summary>
public class MakePartialClassCodeFixProviderTests : RoslynTestBase
{
    [Test]
    public async Task AddsPartialModifier_ToClass()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [GenerateWith]
                              public class Person
                              {
                                  public string Name { get; init; }
                              }
                              """;

        const string expected = """
                                using Deepstaging.RoslynKit;

                                namespace TestApp;

                                [GenerateWith]
                                public partial class Person
                                {
                                    public string Name { get; init; }
                                }
                                """;

        await AnalyzeAndFixWith<GenerateWithAnalyzer, MakePartialClassCodeFixProvider>(source)
            .ForDiagnostic(Diagnostics.GenerateWithMustBePartial)
            .ShouldProduce(expected);
    }

    [Test]
    public async Task AddsPartialModifier_ToStruct()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [GenerateWith]
                              public struct Point
                              {
                                  public int X { get; init; }
                              }
                              """;

        const string expected = """
                                using Deepstaging.RoslynKit;

                                namespace TestApp;

                                [GenerateWith]
                                public partial struct Point
                                {
                                    public int X { get; init; }
                                }
                                """;

        await AnalyzeAndFixWith<GenerateWithAnalyzer, MakePartialStructCodeFixProvider>(source)
            .ForDiagnostic(Diagnostics.GenerateWithMustBePartial)
            .ShouldProduce(expected);
    }

    [Test]
    public async Task PreservesOtherModifiers()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [GenerateWith]
                              public sealed class Config
                              {
                                  public string Value { get; init; }
                              }
                              """;

        const string expected = """
                                using Deepstaging.RoslynKit;

                                namespace TestApp;

                                [GenerateWith]
                                public sealed partial class Config
                                {
                                    public string Value { get; init; }
                                }
                                """;

        await AnalyzeAndFixWith<GenerateWithAnalyzer, MakePartialClassCodeFixProvider>(source)
            .ForDiagnostic(Diagnostics.GenerateWithMustBePartial)
            .ShouldProduce(expected);
    }
}