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
    /// Specify which diagnostic ID to fix.
    /// </summary>
    /// <param name="diagnosticId">The diagnostic ID that should be fixed.</param>
    /// <returns>A context for making assertions about the fixed code.</returns>
    public CodeFixAssertion ForDiagnostic(string diagnosticId)
    {
        return new CodeFixAssertion(this, diagnosticId);
    }
    
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
    /// </summary>
    /// <param name="expectedSource">The expected source code after applying the fix.</param>
    public async Task ShouldProduce(string expectedSource)
    {
        var compilation = CompilationHelper.CreateCompilation(_context.Source);
        var document = CreateDocument(_context.Source);
        
        // Get diagnostics - either from analyzer or semantic model
        var targetDiagnostic = await GetTargetDiagnosticAsync(compilation, document);
        
        if (targetDiagnostic == null)
        {
            var source = _context.Analyzer != null ? "analyzer" : "compiler";
            Assert.Fail($"No diagnostic '{_diagnosticId}' found from {source}. Cannot apply fix.");
            return;
        }
        
        // Get code fixes for the diagnostic
        var codeActions = new List<CodeAction>();
        var context = new CodeFixContext(
            document,
            targetDiagnostic,
            (action, _) => codeActions.Add(action),
            CancellationToken.None);
        
        await _context.CodeFix.RegisterCodeFixesAsync(context);
        
        if (codeActions.Count == 0)
        {
            Assert.Fail($"Code fix provider did not register any fixes for diagnostic '{_diagnosticId}'");
            return;
        }
        
        // Apply the first code fix
        var operations = await codeActions[0].GetOperationsAsync(CancellationToken.None);
        var solution = operations
            .OfType<ApplyChangesOperation>()
            .FirstOrDefault()
            ?.ChangedSolution;
        
        if (solution == null)
        {
            Assert.Fail("Code fix did not produce a solution with changes");
            return;
        }
        
        // Get the fixed document
        var fixedDocument = solution.GetDocument(document.Id);
        if (fixedDocument == null)
        {
            Assert.Fail("Could not retrieve fixed document from solution");
            return;
        }
        
        var fixedSourceText = await fixedDocument.GetTextAsync();
        var actualSource = fixedSourceText.ToString();
        
        // Normalize line endings for comparison
        var normalizedActual = NormalizeLineEndings(actualSource);
        var normalizedExpected = NormalizeLineEndings(expectedSource);
        
        if (normalizedActual != normalizedExpected)
        {
            Assert.Fail(
                $"Code fix produced unexpected result.\n\n" +
                $"Expected:\n{normalizedExpected}\n\n" +
                $"Actual:\n{normalizedActual}");
        }
    }
    
    private async Task<Diagnostic?> GetTargetDiagnosticAsync(Compilation compilation, Document document)
    {
        if (_context.Analyzer != null)
        {
            // Run the analyzer to get diagnostics
            var analyzers = ImmutableArray.Create(_context.Analyzer);
            var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
            var allDiagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync();
            
            // Find matching diagnostic and remap to document location
            var analyzerDiagnostic = allDiagnostics.FirstOrDefault(d => d.Id == _diagnosticId);
            if (analyzerDiagnostic == null)
                return null;
            
            // The diagnostic location references the compilation's syntax tree.
            // We need to create a new diagnostic with a location in the document's syntax tree.
            var documentTree = await document.GetSyntaxTreeAsync();
            if (documentTree == null)
                return null;
            
            var originalSpan = analyzerDiagnostic.Location.SourceSpan;
            var newLocation = Location.Create(documentTree, originalSpan);
            
            // Create new diagnostic with remapped location
            return Diagnostic.Create(
                analyzerDiagnostic.Descriptor,
                newLocation,
                analyzerDiagnostic.Properties.ToImmutableDictionary(),
                analyzerDiagnostic.Descriptor.MessageFormat.ToString());
        }
        else
        {
            // Fall back to semantic model diagnostics (compiler diagnostics)
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
        
        // Get all configured references
        var references = ReferenceConfiguration.GetAdditionalReferences()
            .Concat([MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
        
        var solution = new AdhocWorkspace()
            .CurrentSolution
            .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
            .WithProjectParseOptions(projectId, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp14));
        
        // Add all references
        foreach (var reference in references)
        {
            solution = solution.AddMetadataReference(projectId, reference);
        }
        
        solution = solution.AddDocument(documentId, "TestDocument.cs", SourceText.From(source));
        
        var document = solution.GetDocument(documentId);
        if (document == null)
        {
            throw new InvalidOperationException("Failed to create test document");
        }
        
        return document;
    }
    
    private static string NormalizeLineEndings(string text)
    {
        return text.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
    }
}
