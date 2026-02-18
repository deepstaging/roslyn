// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

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
    public async Task NullDocument_DoesNotOfferFix()
    {
        var (document, diagnostic) = await CreateDocumentAndDiagnostic(Source);

        var codeActions = new List<CodeAction>();
        var fixContext = new CodeFixContext(
            document,
            diagnostic,
            (action, _) => codeActions.Add(action),
            CancellationToken.None);

        await new NullSourceDocumentCodeFix().RegisterCodeFixesAsync(fixContext);

        await Assert.That(codeActions).Count().IsEqualTo(0);
    }

    [Test]
    public async Task ExcludesUnexpectedContent() =>
        await AnalyzeAndFixWith<GenerateHelperAnalyzer, GenerateHelperCodeFix>(Source)
            .ForDiagnostic(GenerateHelperAnalyzer.DiagnosticId)
            .ShouldAddSourceDocument()
            .WithContentContaining("MyServiceHelper")
            .WithoutContentContaining("private");

    private async Task<(Document document, Diagnostic diagnostic)>
        CreateDocumentAndDiagnostic(string source)
    {
        var compilation = CompilationFor(source);
        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new GenerateHelperAnalyzer());
        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
        var diagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync();
        var diagnostic = diagnostics.First(d => d.Id == GenerateHelperAnalyzer.DiagnosticId);

        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);
        var references = compilation.References;

        var solution = new AdhocWorkspace()
            .CurrentSolution
            .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
            .WithProjectParseOptions(projectId,
                CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp14));

        foreach (var reference in references) solution = solution.AddMetadataReference(projectId, reference);
        solution = solution.AddDocument(documentId, "TestDocument.cs", SourceText.From(source));

        var document = solution.GetDocument(documentId)!;
        var documentTree = (await document.GetSyntaxTreeAsync())!;
        var newLocation = Location.Create(documentTree, diagnostic.Location.SourceSpan);
        var remappedDiagnostic = Diagnostic.Create(
            diagnostic.Descriptor,
            newLocation,
            diagnostic.Properties.ToImmutableDictionary(),
            diagnostic.Descriptor.MessageFormat.ToString());

        return (document, remappedDiagnostic);
    }
}
