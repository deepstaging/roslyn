// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

// ReSharper disable MemberCanBePrivate.Global

namespace Deepstaging.Roslyn.Testing;

/// <summary>
/// Provides fluent API for testing code fixes.
/// </summary>
public class CodeFixTestContext
{
    private readonly string _source;
    private readonly CodeFixProvider _codeFix;
    private readonly List<AdditionalText> _additionalTexts = [];
    private DiagnosticAnalyzer? _analyzer;

    internal CodeFixTestContext(string source, CodeFixProvider codeFix, DiagnosticAnalyzer? analyzer = null)
    {
        _source = source;
        _codeFix = codeFix;
        _analyzer = analyzer;
    }

    /// <summary>
    /// Specify an analyzer to run to produce diagnostics for the code fix.
    /// Use this when testing code fixes for analyzer diagnostics (not compiler diagnostics).
    /// </summary>
    /// <typeparam name="TAnalyzer">The analyzer type that produces the diagnostics.</typeparam>
    /// <returns>This context for method chaining.</returns>
    public CodeFixTestContext WithAnalyzer<TAnalyzer>() where TAnalyzer : DiagnosticAnalyzer, new()
    {
        _analyzer = new TAnalyzer();
        return this;
    }

    /// <summary>
    /// Adds an additional text file available to the analyzer (e.g., user template files).
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="content">The file content.</param>
    public CodeFixTestContext WithAdditionalText(string path, string content)
    {
        _additionalTexts.Add(new InMemoryAdditionalText(path, content));
        return this;
    }

    /// <summary>
    /// Specify which diagnostic ID to fix.
    /// </summary>
    /// <param name="diagnosticId">The diagnostic ID that should be fixed.</param>
    /// <returns>A context for making assertions about the fixed code.</returns>
    public CodeFixAssertion ForDiagnostic(string diagnosticId) => new(this, diagnosticId);

    /// <summary>
    /// Get the code fix provider being tested.
    /// </summary>
    internal CodeFixProvider CodeFix => _codeFix;

    /// <summary>
    /// Get the source code being tested.
    /// </summary>
    internal string Source => _source;

    /// <summary>
    /// Get the optional analyzer for producing diagnostics.
    /// </summary>
    internal DiagnosticAnalyzer? Analyzer => _analyzer;

    /// <summary>
    /// Get the additional texts available to the analyzer.
    /// </summary>
    internal IReadOnlyList<AdditionalText> AdditionalTexts => _additionalTexts;
}

/// <summary>
/// Represents an assertion about a code fix result.
/// </summary>
public class CodeFixAssertion
{
    private readonly CodeFixTestContext _context;
    private readonly string _diagnosticId;

    internal CodeFixAssertion(CodeFixTestContext context, string diagnosticId)
    {
        _context = context;
        _diagnosticId = diagnosticId;
    }

    /// <summary>
    /// Assert that applying the code fix produces the expected source code.
    /// Use this for code fixes that modify existing documents.
    /// </summary>
    /// <param name="expectedSource">The expected source code after applying the fix.</param>
    public async Task ShouldProduce(string expectedSource)
    {
        var solution = await ApplyFirstFixAsync();

        var document = solution.Projects.First().Documents.First();
        var fixedSourceText = await document.GetTextAsync();
        var actualSource = fixedSourceText.ToString();

        var normalizedActual = NormalizeLineEndings(actualSource);
        var normalizedExpected = NormalizeLineEndings(expectedSource);

        if (normalizedActual != normalizedExpected)
            Assert.Fail(
                $"Code fix produced unexpected result.\n\n" +
                $"Expected:\n{normalizedExpected}\n\n" +
                $"Actual:\n{normalizedActual}");
    }

    /// <summary>
    /// Assert that the code fix adds an additional document to the project.
    /// Use this for code fixes that create new files (e.g., template scaffolds).
    /// </summary>
    public AdditionalDocumentAssertion ShouldAddAdditionalDocument() => new(this);

    /// <summary>
    /// Assert that the code fix adds a source document to the project.
    /// Use this for code fixes that generate compilable source files (.cs).
    /// </summary>
    public SourceDocumentAssertion ShouldAddSourceDocument() => new(this);

    /// <summary>
    /// Assert that the code fix offers at least one action for the diagnostic.
    /// Use this for project-level code fixes (e.g., modifying .csproj properties)
    /// that don't produce <see cref="ApplyChangesOperation"/> results.
    /// </summary>
    /// <param name="expectedTitle">Optional expected title of the code action.</param>
    public async Task ShouldOfferFix(string? expectedTitle = null)
    {
        var actions = await GetCodeActionsAsync();

        if (actions.Count == 0)
            Assert.Fail($"Code fix provider did not register any fixes for diagnostic '{_diagnosticId}'");

        if (expectedTitle is not null)
        {
            var titles = actions.Select(a => a.Title).ToList();

            if (!titles.Any(t => t.Contains(expectedTitle)))
                Assert.Fail(
                    $"Expected code fix with title containing '{expectedTitle}', " +
                    $"but found: {string.Join(", ", titles.Select(t => $"'{t}'"))}");
        }
    }

    /// <summary>
    /// Assert that the code fix does not offer any action for the diagnostic.
    /// Use this for code fixes that conditionally skip (e.g., returning null from CreateDocument).
    /// </summary>
    public async Task ShouldNotOfferFix()
    {
        var actions = await GetCodeActionsAsync();

        if (actions.Count > 0)
            Assert.Fail(
                $"Expected no code fix for diagnostic '{_diagnosticId}', " +
                $"but found: {string.Join(", ", actions.Select(a => $"'{a.Title}'"))}");
    }

    internal async Task<Solution> ApplyFirstFixAsync()
    {
        var actions = await GetCodeActionsAsync();

        if (actions.Count == 0)
            Assert.Fail($"Code fix provider did not register any fixes for diagnostic '{_diagnosticId}'");

        var operations = await actions[0].GetOperationsAsync(CancellationToken.None);

        var solution = operations
            .OfType<ApplyChangesOperation>()
            .FirstOrDefault()
            ?.ChangedSolution;

        if (solution == null)
            Assert.Fail("Code fix did not produce a solution with changes");

        return solution!;
    }

    private async Task<List<CodeAction>> GetCodeActionsAsync()
    {
        var (document, diagnostic) = await GetDocumentAndDiagnosticAsync();

        var codeActions = new List<CodeAction>();

        var context = new CodeFixContext(
            document,
            diagnostic,
            (action, _) => codeActions.Add(action),
            CancellationToken.None);

        await _context.CodeFix.RegisterCodeFixesAsync(context);

        return codeActions;
    }

    private async Task<(Document document, Diagnostic diagnostic)> GetDocumentAndDiagnosticAsync()
    {
        var compilation = CompilationHelper.CreateCompilation(_context.Source);
        var document = CreateDocument(_context.Source);

        var targetDiagnostic = await GetTargetDiagnosticAsync(compilation, document);

        if (targetDiagnostic == null)
        {
            var source = _context.Analyzer != null ? "analyzer" : "compiler";
            Assert.Fail($"No diagnostic '{_diagnosticId}' found from {source}. Cannot apply fix.");
        }

        return (document, targetDiagnostic!);
    }

    private async Task<Diagnostic?> GetTargetDiagnosticAsync(Compilation compilation, Document document)
    {
        if (_context.Analyzer != null)
        {
            var analyzers = ImmutableArray.Create(_context.Analyzer);

            var options = _context.AdditionalTexts.Count > 0
                ? new AnalyzerOptions([.. _context.AdditionalTexts])
                : null;

            var compilationWithAnalyzers = options is not null
                ? compilation.WithAnalyzers(analyzers, options)
                : compilation.WithAnalyzers(analyzers);

            var allDiagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync();

            var analyzerDiagnostic = allDiagnostics.FirstOrDefault(d => d.Id == _diagnosticId);

            if (analyzerDiagnostic == null)
                return null;

            var documentTree = await document.GetSyntaxTreeAsync();

            if (documentTree == null)
                return null;

            var originalSpan = analyzerDiagnostic.Location.SourceSpan;
            var newLocation = Location.Create(documentTree, originalSpan);

            return Diagnostic.Create(
                analyzerDiagnostic.Descriptor,
                newLocation,
                analyzerDiagnostic.Properties.ToImmutableDictionary(),
                analyzerDiagnostic.Descriptor.MessageFormat.ToString());
        }
        else
        {
            var semanticModel = await document.GetSemanticModelAsync();

            if (semanticModel == null)
            {
                Assert.Fail("Failed to get semantic model from document");
                return null;
            }

            var diagnostics = semanticModel.GetDiagnostics();
            return diagnostics.FirstOrDefault(d => d.Id == _diagnosticId);
        }
    }

    private static Document CreateDocument(string source)
    {
        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);

        var references = ReferenceConfiguration.GetAdditionalReferences()
            .Concat([MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);

        var solution = new AdhocWorkspace()
            .CurrentSolution
            .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
            .WithProjectParseOptions(projectId,
                CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp14));

        foreach (var reference in references) solution = solution.AddMetadataReference(projectId, reference);

        solution = solution.AddDocument(documentId, "TestDocument.cs", SourceText.From(source));

        var document = solution.GetDocument(documentId);
        if (document == null) throw new InvalidOperationException("Failed to create test document");

        return document;
    }

    private static string NormalizeLineEndings(string text) => text.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
}

/// <summary>
/// Fluent assertions for code fixes that add additional documents.
/// </summary>
public class AdditionalDocumentAssertion
{
    private readonly CodeFixAssertion _parent;
    private string? _expectedPathContains;
    private readonly List<string> _expectedContains = [];
    private readonly List<string> _expectedNotContains = [];

    internal AdditionalDocumentAssertion(CodeFixAssertion parent) => _parent = parent;

    /// <summary>
    /// Assert the added document's path contains the specified substring.
    /// </summary>
    public AdditionalDocumentAssertion WithPathContaining(string pathFragment)
    {
        _expectedPathContains = pathFragment;
        return this;
    }

    /// <summary>
    /// Assert the added document's content contains the specified text.
    /// </summary>
    /// <param name="expectedContent">Text that should appear in the added document.</param>
    public AdditionalDocumentAssertion WithContentContaining(string expectedContent)
    {
        _expectedContains.Add(expectedContent);
        return this;
    }

    /// <summary>
    /// Assert the added document's content does not contain the specified text.
    /// </summary>
    /// <param name="unexpectedContent">Text that should not appear in the added document.</param>
    public AdditionalDocumentAssertion WithoutContentContaining(string unexpectedContent)
    {
        _expectedNotContains.Add(unexpectedContent);
        return this;
    }

    /// <summary>
    /// Enables awaiting on the assertion to verify all conditions.
    /// </summary>
    public TaskAwaiter GetAwaiter() => VerifyAsync().GetAwaiter();

    private async Task VerifyAsync()
    {
        var solution = await _parent.ApplyFirstFixAsync();

        var project = solution.Projects.First();
        var additionalDocs = project.AdditionalDocuments.ToArray();

        if (additionalDocs.Length == 0)
            Assert.Fail("Expected code fix to add an additional document, but none were added.");

        var doc = additionalDocs[0];

        if (_expectedPathContains != null && !doc.Name.Contains(_expectedPathContains))
            Assert.Fail(
                $"Expected additional document path to contain '{_expectedPathContains}', " +
                $"but was '{doc.Name}'.");

        if (_expectedContains.Count > 0 || _expectedNotContains.Count > 0)
        {
            var text = await doc.GetTextAsync();
            var content = text.ToString();

            foreach (var expected in _expectedContains)
            {
                if (!content.Contains(expected))
                    Assert.Fail(
                        $"Expected additional document to contain '{expected}', " +
                        $"but content was:\n{content}");
            }

            foreach (var unexpected in _expectedNotContains)
            {
                if (content.Contains(unexpected))
                    Assert.Fail(
                        $"Expected additional document to NOT contain '{unexpected}', " +
                        $"but it was found in:\n{content}");
            }
        }
    }
}

/// <summary>
/// Fluent assertions for code fixes that add source documents.
/// </summary>
public class SourceDocumentAssertion
{
    private readonly CodeFixAssertion _parent;
    private string? _expectedPathContains;
    private readonly List<string> _expectedContains = [];
    private readonly List<string> _expectedNotContains = [];

    internal SourceDocumentAssertion(CodeFixAssertion parent) => _parent = parent;

    /// <summary>
    /// Assert the added document's path contains the specified substring.
    /// </summary>
    public SourceDocumentAssertion WithPathContaining(string pathFragment)
    {
        _expectedPathContains = pathFragment;
        return this;
    }

    /// <summary>
    /// Assert the added document's content contains the specified text.
    /// </summary>
    /// <param name="expectedContent">Text that should appear in the added document.</param>
    public SourceDocumentAssertion WithContentContaining(string expectedContent)
    {
        _expectedContains.Add(expectedContent);
        return this;
    }

    /// <summary>
    /// Assert the added document's content does not contain the specified text.
    /// </summary>
    /// <param name="unexpectedContent">Text that should not appear in the added document.</param>
    public SourceDocumentAssertion WithoutContentContaining(string unexpectedContent)
    {
        _expectedNotContains.Add(unexpectedContent);
        return this;
    }

    /// <summary>
    /// Enables awaiting on the assertion to verify all conditions.
    /// </summary>
    public TaskAwaiter GetAwaiter() => VerifyAsync().GetAwaiter();

    private async Task VerifyAsync()
    {
        var solution = await _parent.ApplyFirstFixAsync();

        var project = solution.Projects.First();

        // Filter out the original test document
        var addedDocs = project.Documents
            .Where(d => d.Name != "TestDocument.cs")
            .ToArray();

        if (addedDocs.Length == 0)
            Assert.Fail("Expected code fix to add a source document, but none were added.");

        var doc = addedDocs[0];

        if (_expectedPathContains != null && !doc.Name.Contains(_expectedPathContains))
            Assert.Fail(
                $"Expected source document path to contain '{_expectedPathContains}', " +
                $"but was '{doc.Name}'.");

        if (_expectedContains.Count > 0 || _expectedNotContains.Count > 0)
        {
            var text = await doc.GetTextAsync();
            var content = text.ToString();

            foreach (var expected in _expectedContains)
            {
                if (!content.Contains(expected))
                    Assert.Fail(
                        $"Expected source document to contain '{expected}', " +
                        $"but content was:\n{content}");
            }

            foreach (var unexpected in _expectedNotContains)
            {
                if (content.Contains(unexpected))
                    Assert.Fail(
                        $"Expected source document to NOT contain '{unexpected}', " +
                        $"but it was found in:\n{content}");
            }
        }
    }
}