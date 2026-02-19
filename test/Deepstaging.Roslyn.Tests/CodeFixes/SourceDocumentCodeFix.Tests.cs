// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.CodeFixes;

public class SourceDocumentCodeFixTests : RoslynTestBase
{
    private const string Source = """
                                  using Deepstaging.Roslyn.Tests.CodeFixes;

                                  namespace TestApp;

                                  [GenerateHelper]
                                  public class MyService { }
                                  """;

    [Test]
    public async Task AddsSourceDocument_WithExpectedPathAndContent() =>
        await AnalyzeAndFixWith<GenerateHelperAnalyzer, GenerateHelperCodeFix>(Source)
            .ForDiagnostic(GenerateHelperAnalyzer.DiagnosticId)
            .ShouldAddSourceDocument()
            .WithPathContaining("MyServiceHelper")
            .WithContentContaining("public static class MyServiceHelper")
            .WithContentContaining("""Describe() => "Helper for MyService""");

    [Test]
    public async Task GenericVariant_ResolvesSymbolAndGeneratesDocument() =>
        await AnalyzeAndFixWith<GenerateHelperAnalyzer, GenerateHelperWithSymbolCodeFix>(Source)
            .ForDiagnostic(GenerateHelperAnalyzer.DiagnosticId)
            .ShouldAddSourceDocument()
            .WithPathContaining("MyServiceHelper")
            .WithContentContaining("namespace TestApp;")
            .WithContentContaining("public static class MyServiceHelper");

    [Test]
    public async Task NullDocument_DoesNotOfferFix() =>
        await AnalyzeAndFixWith<GenerateHelperAnalyzer, NullSourceDocumentCodeFix>(Source)
            .ForDiagnostic(GenerateHelperAnalyzer.DiagnosticId)
            .ShouldNotOfferFix();

    [Test]
    public async Task ExcludesUnexpectedContent() =>
        await AnalyzeAndFixWith<GenerateHelperAnalyzer, GenerateHelperCodeFix>(Source)
            .ForDiagnostic(GenerateHelperAnalyzer.DiagnosticId)
            .ShouldAddSourceDocument()
            .WithContentContaining("MyServiceHelper")
            .WithoutContentContaining("private");
}