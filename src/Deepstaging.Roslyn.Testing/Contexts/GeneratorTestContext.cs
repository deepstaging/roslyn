// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Assembly = System.Reflection.Assembly;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Deepstaging.Roslyn.Testing;

/// <summary>
/// Provides fluent API for making assertions about generator output.
/// </summary>
public class GeneratorTestContext
{
    private readonly string _source;
    private readonly object _generator;
    private readonly Type _generatorType;
    private GeneratorDriverRunResult _result;
    private bool _hasResult;

    internal GeneratorTestContext(string source, object generator)
    {
        _source = source;
        _generator = generator;
        _generatorType = generator.GetType();
        _result = null!; // Will be set before use
        _hasResult = false;
    }

    /// <summary>
    /// Assert that the generator should produce output.
    /// </summary>
    public GeneratorAssertions ShouldGenerate()
    {
        return new GeneratorAssertions(this, shouldGenerate: true);
    }

    /// <summary>
    /// Assert that the generator should NOT produce output.
    /// </summary>
    public async Task ShouldNotGenerate()
    {
        var result = await GetResultAsync();

        if (result.GeneratedTrees.Length > 0)
        {
            Assert.Fail(
                $"Expected no generated files, but found {result.GeneratedTrees.Length} file(s).");
        }
    }

    /// <summary>
    /// Get the generator run result.
    /// </summary>
    internal async Task<GeneratorDriverRunResult> GetResultAsync()
    {
        if (_hasResult)
        {
            return _result;
        }

        await Task.CompletedTask; // Make this async-compatible

        var syntaxTree = CSharpSyntaxTree.ParseText(_source, GetParseOptions());
        var compilation = CreateCompilation(syntaxTree);

        // All generators now implement IIncrementalGenerator directly
        GeneratorDriver driver;
        if (_generator is IIncrementalGenerator incrementalGenerator)
        {
            // Standard incremental generator
            driver = CSharpGeneratorDriver.Create(incrementalGenerator);
        }
        else
        {
            throw new InvalidOperationException(
                $"Generator type {_generatorType.Name} must implement IIncrementalGenerator.");
        }

        driver = driver.RunGenerators(compilation);
        _result = driver.GetRunResult();
        _hasResult = true;

        return _result;
    }

    private static CSharpCompilation CreateCompilation(params SyntaxTree[] syntaxTrees)
    {
        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        return CSharpCompilation.Create(
            "TestAssembly",
            syntaxTrees,
            GetDefaultReferences(),
            options);
    }

    private static CSharpParseOptions GetParseOptions()
    {
        return CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp13);
    }

    private static IEnumerable<MetadataReference> GetDefaultReferences()
    {
        var references = new List<MetadataReference>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var runtimeAssemblies = assemblies
            .Where(a => !string.IsNullOrEmpty(a.Location))
            .Distinct()
            .Select(a => MetadataReference.CreateFromFile(a.Location));

        references.AddRange(runtimeAssemblies);

        try
        {
            var testAssemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (testAssemblyDir != null)
            {
                var featureAssemblies = Directory.GetFiles(testAssemblyDir, "Deepstaging.*.dll")
                    .Where(f =>
                    {
                        var name = Path.GetFileNameWithoutExtension(f);
                        return !name.EndsWith(".Generators") &&
                               !name.EndsWith(".Analyzers") &&
                               !name.EndsWith(".CodeFixes") &&
                               !name.EndsWith(".Tests") &&
                               !name.EndsWith(".Testing") &&
                               !name.EndsWith(".Common") &&
                               !name.Contains(".Common.");
                    })
                    .ToList();

                foreach (var assemblyPath in featureAssemblies)
                    references.Add(MetadataReference.CreateFromFile(assemblyPath));
            }
        }
        catch
        {
            // Continue without feature assemblies
        }

        return references.OrderBy(x => x.Display).ToList();
    }
}

/// <summary>
/// Fluent assertions for generator output.
/// </summary>
public class GeneratorAssertions
{
    private readonly GeneratorTestContext _context;
    private readonly bool _shouldGenerate;
    private int? _expectedFileCount;
    private string? _expectedFileName;
    private string? _expectedContent;
    private string? _unexpectedContent;
    private bool _shouldHaveNoDiagnostics;
    private DiagnosticFilter? _diagnosticFilter;

    internal GeneratorAssertions(GeneratorTestContext context, bool shouldGenerate)
    {
        _context = context;
        _shouldGenerate = shouldGenerate;
    }

    /// <summary>
    /// Assert the expected number of generated files.
    /// </summary>
    public GeneratorAssertions WithFileCount(int count)
    {
        _expectedFileCount = count;
        return this;
    }

    /// <summary>
    /// Assert that a file with the given name was generated.
    /// </summary>
    public GeneratorAssertions WithFileNamed(string fileName)
    {
        _expectedFileName = fileName;
        return this;
    }

    /// <summary>
    /// Assert that the generated code contains specific content.
    /// </summary>
    public GeneratorAssertions WithFileContaining(string content)
    {
        _expectedContent = content;
        return this;
    }

    /// <summary>
    /// Assert that the generated code does NOT contain specific content.
    /// </summary>
    public GeneratorAssertions WithoutFileContaining(string content)
    {
        _unexpectedContent = content;
        return this;
    }

    /// <summary>
    /// Assert that no diagnostics were emitted.
    /// </summary>
    public GeneratorAssertions WithNoDiagnostics()
    {
        _shouldHaveNoDiagnostics = true;
        return this;
    }

    /// <summary>
    /// Assert that no diagnostics matching the filter were emitted.
    /// </summary>
    /// <param name="filterBuilder">Configure diagnostic filters.</param>
    /// <example>
    /// <code>
    /// await GenerateWith(source)
    ///     .ShouldGenerate()
    ///     .WithNoDiagnostics(filter => filter
    ///         .WithSeverity(DiagnosticSeverity.Error)
    ///         .WithId("DS0001"));
    /// </code>
    /// </example>
    public GeneratorAssertions WithNoDiagnostics(Action<DiagnosticFilter> filterBuilder)
    {
        var filter = new DiagnosticFilter();
        filterBuilder(filter);
        _shouldHaveNoDiagnostics = true;
        _diagnosticFilter = filter;
        return this;
    }

    /// <summary>
    /// Assert that no error diagnostics were emitted.
    /// </summary>
    public GeneratorAssertions WithNoErrors()
    {
        return WithNoDiagnostics(filter => filter.WithSeverity(DiagnosticSeverity.Error));
    }

    /// <summary>
    /// Assert that no warning diagnostics were emitted.
    /// </summary>
    public GeneratorAssertions WithNoWarnings()
    {
        return WithNoDiagnostics(filter => filter.WithSeverity(DiagnosticSeverity.Warning));
    }

    /// <summary>
    /// Verify using snapshot testing.
    /// </summary>
    public async Task VerifySnapshot([CallerFilePath] string sourceFile = "")
    {
        var result = await _context.GetResultAsync();

        // First verify all assertions
        await VerifyAssertions(result);

        // Then do snapshot verification
        var generatedSource = string.Join("\n\n",
            result.GeneratedTrees.Select(tree => tree.GetText().ToString()));

        // Use VerifyTUnit with caller file path so snapshots are stored next to test file
        await Verify(generatedSource, sourceFile: sourceFile);
    }

    /// <summary>
    /// Enables awaiting on the assertions to verify all conditions.
    /// </summary>
    public TaskAwaiter GetAwaiter()
    {
        return VerifyAsync().GetAwaiter();
    }

    private async Task VerifyAsync()
    {
        var result = await _context.GetResultAsync();
        await VerifyAssertions(result);
    }

    private async Task VerifyAssertions(GeneratorDriverRunResult result)
    {
        await Task.CompletedTask; // Make async

        if (_shouldHaveNoDiagnostics)
        {
            VerifyNoDiagnostics(result);
        }

        if (_shouldGenerate && result.GeneratedTrees.Length == 0)
        {
            Assert.Fail("Expected generated files, but none were produced.");
        }

        if (_expectedFileCount.HasValue)
        {
            var actualCount = result.GeneratedTrees.Length;
            if (actualCount != _expectedFileCount.Value)
            {
                Assert.Fail(
                    $"Expected {_expectedFileCount.Value} generated file(s), but found {actualCount}.");
            }
        }

        if (_expectedFileName != null)
        {
            var fileNames = result.GeneratedTrees
                .Select(t => Path.GetFileName(t.FilePath))
                .ToArray();

            if (!fileNames.Contains(_expectedFileName))
            {
                Assert.Fail(
                    $"Expected file '{_expectedFileName}' was not generated. " +
                    $"Generated files: {string.Join(", ", fileNames)}");
            }
        }

        var allContent = string.Join("\n",
            result.GeneratedTrees.Select(t => t.GetText().ToString()));

        if (_expectedContent != null)
        {
            if (!allContent.Contains(_expectedContent))
            {
                Assert.Fail(
                    $"Expected generated code to contain: {_expectedContent}\n\nReceived: {allContent}");
            }
        }

        if (_unexpectedContent != null)
        {
            if (allContent.Contains(_unexpectedContent))
            {
                Assert.Fail(
                    $"Expected generated code to NOT contain: {_unexpectedContent}\n\nReceived: {allContent}");
            }
        }
    }

    private void VerifyNoDiagnostics(GeneratorDriverRunResult result)
    {
        var diagnostics = result.Diagnostics;

        // Apply filter if provided
        if (_diagnosticFilter != null)
        {
            diagnostics = _diagnosticFilter.Apply(diagnostics);
        }

        if (diagnostics.Length == 0)
        {
            return;
        }

        // Build detailed error message
        var filterDescription = _diagnosticFilter?.GetDescription() ?? "any";
        var diagnosticDetails = string.Join("\n", diagnostics.Select(d =>
            $"  [{d.Severity}] {d.Id}: {d.GetMessage()} at {d.Location.GetLineSpan()}"));

        Assert.Fail(
            $"Expected no diagnostics matching '{filterDescription}', but found {diagnostics.Length}:\n{diagnosticDetails}");
    }
}

/// <summary>
/// Configures filters for diagnostic assertions.
/// </summary>
public class DiagnosticFilter
{
    private DiagnosticSeverity? _severity;
    private string? _id;
    private string? _messageContains;
    private readonly List<string> _ids = new();

    /// <summary>
    /// Filter by diagnostic severity.
    /// </summary>
    public DiagnosticFilter WithSeverity(DiagnosticSeverity severity)
    {
        _severity = severity;
        return this;
    }

    /// <summary>
    /// Filter by specific diagnostic ID.
    /// </summary>
    public DiagnosticFilter WithId(string id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Filter by multiple diagnostic IDs.
    /// </summary>
    public DiagnosticFilter WithIds(params string[] ids)
    {
        _ids.AddRange(ids);
        return this;
    }

    /// <summary>
    /// Filter by diagnostics whose message contains the specified text.
    /// </summary>
    public DiagnosticFilter WithMessageContaining(string text)
    {
        _messageContains = text;
        return this;
    }

    internal ImmutableArray<Diagnostic> Apply(ImmutableArray<Diagnostic> diagnostics)
    {
        var filtered = diagnostics.AsEnumerable();

        if (_severity.HasValue)
        {
            filtered = filtered.Where(d => d.Severity == _severity.Value);
        }

        if (_id != null)
        {
            filtered = filtered.Where(d => d.Id == _id);
        }

        if (_ids.Count > 0)
        {
            filtered = filtered.Where(d => _ids.Contains(d.Id));
        }

        if (_messageContains != null)
        {
            filtered = filtered.Where(d => d.GetMessage().Contains(_messageContains, StringComparison.Ordinal));
        }

        return [..filtered];
    }

    internal string GetDescription()
    {
        var parts = new List<string>();

        if (_severity.HasValue)
        {
            parts.Add($"severity={_severity.Value}");
        }

        if (_id != null)
        {
            parts.Add($"id={_id}");
        }

        if (_ids.Count > 0)
        {
            parts.Add($"ids=[{string.Join(", ", _ids)}]");
        }

        if (_messageContains != null)
        {
            parts.Add($"message contains '{_messageContains}'");
        }

        return parts.Count > 0 ? string.Join(", ", parts) : "any diagnostics";
    }
}