// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.CodeAnalysis.Diagnostics;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Deepstaging.Roslyn.Testing;

/// <summary>
/// Provides fluent API for making assertions about analyzer diagnostics.
/// </summary>
public class AnalyzerTestContext
{
    private readonly string _source;
    private readonly DiagnosticAnalyzer _analyzer;
    // ReSharper disable once CollectionNeverQueried.Local
    private readonly List<DiagnosticAssertion> _assertions = [];
    
    internal AnalyzerTestContext(string source, DiagnosticAnalyzer analyzer)
    {
        _source = source;
        _analyzer = analyzer;
    }
    
    /// <summary>
    /// Assert that a specific diagnostic should be reported.
    /// </summary>
    public DiagnosticAssertion ShouldReportDiagnostic(string diagnosticId)
    {
        var assertion = new DiagnosticAssertion(this, diagnosticId, shouldExist: true);
        _assertions.Add(assertion);
        return assertion;
    }
    
    /// <summary>
    /// Assert that a specific diagnostic should NOT be reported.
    /// </summary>
    public async Task ShouldNotReportDiagnostic(string diagnosticId)
    {
        var diagnostics = await GetDiagnosticsAsync();
        var matching = diagnostics.Where(d => d.Id == diagnosticId).ToArray();
        
        if (matching.Length > 0)
        {
            Assert.Fail(
                $"Expected no diagnostic '{diagnosticId}', but found {matching.Length} occurrence(s).");
        }
    }
    
    /// <summary>
    /// Assert that diagnostics should be reported.
    /// </summary>
    public DiagnosticsAssertion ShouldHaveDiagnostics()
    {
        return new DiagnosticsAssertion(this, shouldHaveDiagnostics: true);
    }
    
    /// <summary>
    /// Assert that no diagnostics should be reported.
    /// </summary>
    public Task ShouldHaveNoDiagnostics()
    {
        return new DiagnosticsAssertion(this, shouldHaveDiagnostics: false).VerifyAsync();
    }
    
    /// <summary>
    /// Get all diagnostics produced by the analyzer for the source code.
    /// </summary>
    internal async Task<Diagnostic[]> GetDiagnosticsAsync()
    {
        var compilation = CompilationHelper.CreateCompilation(_source);
        var analyzers = ImmutableArray.Create(_analyzer);
        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
        var allDiagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync();
        
        // Filter to only analyzer diagnostics (not compiler errors/warnings)
        return allDiagnostics
            .Where(d => !d.Id.StartsWith("CS"))
            .ToArray();
    }
}

/// <summary>
/// Represents an assertion about diagnostics (with or without specific IDs).
/// </summary>
public class DiagnosticsAssertion
{
    private readonly AnalyzerTestContext _context;
    private readonly bool _shouldHaveDiagnostics;
    private string? _diagnosticId;
    private DiagnosticSeverity? _expectedSeverity;
    private string? _messagePattern;
    
    internal DiagnosticsAssertion(AnalyzerTestContext context, bool shouldHaveDiagnostics)
    {
        _context = context;
        _shouldHaveDiagnostics = shouldHaveDiagnostics;
    }
    
    /// <summary>
    /// Specify the expected error code/diagnostic ID.
    /// </summary>
    public DiagnosticsAssertion WithErrorCode(string diagnosticId)
    {
        _diagnosticId = diagnosticId;
        return this;
    }
    
    /// <summary>
    /// Assert the expected severity of the diagnostic.
    /// </summary>
    public DiagnosticsAssertion WithSeverity(DiagnosticSeverity severity)
    {
        _expectedSeverity = severity;
        return this;
    }
    
    /// <summary>
    /// Assert the diagnostic message matches a pattern (supports wildcards with *).
    /// </summary>
    public DiagnosticsAssertion WithMessage(string messagePattern)
    {
        _messagePattern = messagePattern;
        return this;
    }
    
    /// <summary>
    /// Enables awaiting on the assertion to verify all conditions.
    /// </summary>
    public TaskAwaiter GetAwaiter()
    {
        return VerifyAsync().GetAwaiter();
    }
    
    internal async Task VerifyAsync()
    {
        var diagnostics = await _context.GetDiagnosticsAsync();
        
        if (!_shouldHaveDiagnostics)
        {
            if (diagnostics.Length > 0)
            {
                var diagnosticDetails = string.Join(Environment.NewLine, 
                    diagnostics.Select(d => $"  - {d.Id}: {d.GetMessage()} at {d.Location.GetLineSpan()}"));
                Assert.Fail(
                    $"Expected no diagnostics, but found {diagnostics.Length} diagnostic(s):{Environment.NewLine}" +
                    diagnosticDetails);
            }
            return;
        }
        
        // If we expect diagnostics but no specific ID was provided
        if (_diagnosticId == null)
        {
            if (diagnostics.Length == 0)
            {
                Assert.Fail("Expected diagnostics to be reported, but none were found.");
            }
            return;
        }
        
        // Check for specific diagnostic ID
        var matching = diagnostics.Where(d => d.Id == _diagnosticId).ToArray();
        
        if (matching.Length == 0)
        {
            Assert.Fail(
                $"Expected diagnostic '{_diagnosticId}' to be reported, but it was not found. " +
                $"Found: {string.Join(", ", diagnostics.Select(d => d.Id))}");
        }
        
        var diagnostic = matching[0];
        
        if (_expectedSeverity.HasValue && diagnostic.Severity != _expectedSeverity.Value)
        {
            Assert.Fail(
                $"Expected diagnostic '{_diagnosticId}' to have severity '{_expectedSeverity.Value}', " +
                $"but actual severity was '{diagnostic.Severity}'.");
        }
        
        if (_messagePattern != null)
        {
            var message = diagnostic.GetMessage();
            var pattern = _messagePattern.Replace("*", ".*");
            if (!System.Text.RegularExpressions.Regex.IsMatch(message, pattern))
            {
                Assert.Fail(
                    $"Expected diagnostic '{_diagnosticId}' message to match pattern '{_messagePattern}', " +
                    $"but actual message was: {message}");
            }
        }
    }
}

/// <summary>
/// Represents an assertion about a diagnostic that should be reported.
/// </summary>
public class DiagnosticAssertion
{
    private readonly AnalyzerTestContext _context;
    private readonly string _diagnosticId;
    private readonly bool _shouldExist;
    private DiagnosticSeverity? _expectedSeverity;
    private string? _messagePattern;
    
    internal DiagnosticAssertion(AnalyzerTestContext context, string diagnosticId, bool shouldExist)
    {
        _context = context;
        _diagnosticId = diagnosticId;
        _shouldExist = shouldExist;
    }
    
    /// <summary>
    /// Assert the expected severity of the diagnostic.
    /// </summary>
    public DiagnosticAssertion WithSeverity(DiagnosticSeverity severity)
    {
        _expectedSeverity = severity;
        return this;
    }
    
    /// <summary>
    /// Assert the diagnostic message matches a pattern (supports wildcards with *).
    /// </summary>
    public DiagnosticAssertion WithMessage(string messagePattern)
    {
        _messagePattern = messagePattern;
        return this;
    }
    
    /// <summary>
    /// Enables awaiting on the assertion to verify all conditions.
    /// </summary>
    public TaskAwaiter GetAwaiter()
    {
        return VerifyAsync().GetAwaiter();
    }
    
    private async Task VerifyAsync()
    {
        var diagnostics = await _context.GetDiagnosticsAsync();
        var matching = diagnostics.Where(d => d.Id == _diagnosticId).ToArray();
        
        if (_shouldExist && matching.Length == 0)
        {
            Assert.Fail(
                $"Expected diagnostic '{_diagnosticId}' to be reported, but it was not found.");
        }
        
        if (!_shouldExist && matching.Length > 0)
        {
            Assert.Fail(
                $"Expected no diagnostic '{_diagnosticId}', but found {matching.Length} occurrence(s).");
        }
        
        if (_shouldExist && matching.Length > 0)
        {
            var diagnostic = matching[0];
            
            if (_expectedSeverity.HasValue && diagnostic.Severity != _expectedSeverity.Value)
            {
                Assert.Fail(
                    $"Expected diagnostic '{_diagnosticId}' to have severity '{_expectedSeverity.Value}', " +
                    $"but actual severity was '{diagnostic.Severity}'.");
            }
            
            if (_messagePattern != null)
            {
                var message = diagnostic.GetMessage();
                var pattern = _messagePattern.Replace("*", ".*");
                if (!System.Text.RegularExpressions.Regex.IsMatch(message, pattern))
                {
                    Assert.Fail(
                        $"Expected diagnostic '{_diagnosticId}' message to match pattern '{_messagePattern}', " +
                        $"but actual message was: {message}");
                }
            }
        }
    }
}
