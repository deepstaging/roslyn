// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Testing;

/// <summary>
/// Provides fluent API for testing template rendering with symbol queries.
/// </summary>
public class TemplateTestContext
{
    private readonly SymbolTestContext _symbolContext;
    private readonly Func<string, TemplateName> _templateNamer;

    internal TemplateTestContext(Compilation compilation, Func<string, TemplateName> templateNamer)
    {
        _symbolContext = new SymbolTestContext(compilation);
        _templateNamer = templateNamer;
    }

    /// <summary>
    /// The underlying symbol test context for querying symbols.
    /// </summary>
    public SymbolTestContext Symbols => _symbolContext;

    /// <summary>
    /// Render a template with a context object built from symbol queries.
    /// </summary>
    /// <param name="templateFileName">The template file name (e.g., "RuntimeCore.scriban-cs")</param>
    /// <param name="contextBuilder">A function that builds the template context from symbol queries.</param>
    /// <returns>A context for making assertions about the rendered output.</returns>
    public TemplateRenderContext Render(string templateFileName, Func<SymbolTestContext, object> contextBuilder)
    {
        var templateName = _templateNamer(templateFileName);
        var context = contextBuilder(_symbolContext);
        return new TemplateRenderContext(templateName, context);
    }

    /// <summary>
    /// Render a template with a direct context object.
    /// </summary>
    /// <param name="templateFileName">The template file name (e.g., "RuntimeCore.scriban-cs")</param>
    /// <param name="context">The template context object.</param>
    /// <returns>A context for making assertions about the rendered output.</returns>
    public TemplateRenderContext Render(string templateFileName, object context)
    {
        var templateName = _templateNamer(templateFileName);
        return new TemplateRenderContext(templateName, context);
    }
}

/// <summary>
/// Provides fluent API for making assertions about template rendering results.
/// </summary>
public class TemplateRenderContext
{
    private readonly TemplateName _templateName;
    private readonly object _context;
    private RenderResult? _result;

    internal TemplateRenderContext(TemplateName templateName, object context)
    {
        _templateName = templateName;
        _context = context;
    }

    /// <summary>
    /// Get the render result, executing the template if not already done.
    /// </summary>
    private async Task<RenderResult> GetResultAsync()
    {
        if (_result != null)
        {
            return _result;
        }

        await Task.CompletedTask; // Make this async-compatible
        _result = Template.RenderTemplate(_templateName, _context);
        return _result;
    }

    /// <summary>
    /// Assert that the template should render successfully.
    /// </summary>
    public TemplateRenderAssertions ShouldRender()
    {
        return new TemplateRenderAssertions(this, shouldSucceed: true);
    }

    /// <summary>
    /// Assert that the template should fail to render.
    /// </summary>
    public async Task ShouldFail()
    {
        var result = await GetResultAsync();

        if (result is RenderResult.Success)
        {
            Assert.Fail($"Expected template '{_templateName}' to fail, but it rendered successfully.");
        }
    }

    internal Task<RenderResult> GetRenderResultAsync() => GetResultAsync();
}

/// <summary>
/// Fluent assertions for template rendering.
/// </summary>
public class TemplateRenderAssertions
{
    private readonly TemplateRenderContext _context;
    private readonly bool _shouldSucceed;
    private string? _expectedContent;

    internal TemplateRenderAssertions(TemplateRenderContext context, bool shouldSucceed)
    {
        _context = context;
        _shouldSucceed = shouldSucceed;
    }

    /// <summary>
    /// Assert that the rendered output contains specific content.
    /// </summary>
    public TemplateRenderAssertions WithContent(string content)
    {
        _expectedContent = content;
        return this;
    }

    /// <summary>
    /// Verify using snapshot testing.
    /// </summary>
    public async Task VerifySnapshot([CallerFilePath] string sourceFile = "")
    {
        var result = await _context.GetRenderResultAsync();

        // First verify all assertions
        await VerifyAssertions(result);

        // Then do snapshot verification
        var renderedText = result switch
        {
            RenderResult.Success success => success.Text,
            RenderResult.Failure failure => failure.Diagnostic.GetMessage(),
            _ => throw new InvalidOperationException("Unexpected render result type")
        };

        await Verify(renderedText, sourceFile: sourceFile);
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
        var result = await _context.GetRenderResultAsync();
        await VerifyAssertions(result);
    }

    private async Task VerifyAssertions(RenderResult result)
    {
        await Task.CompletedTask; // Make async

        if (_shouldSucceed)
        {
            if (result is not RenderResult.Success success)
            {
                var failure = (RenderResult.Failure)result;
                Assert.Fail($"Expected template to render successfully, but got error: {failure.Diagnostic.GetMessage()}");
                return; // Unreachable but helps compiler
            }

            if (_expectedContent != null && !success.Text.Contains(_expectedContent))
            {
                Assert.Fail($"Expected rendered template to contain: {_expectedContent}");
            }
        }
        else
        {
            if (result is RenderResult.Success)
            {
                Assert.Fail("Expected template to fail, but it rendered successfully.");
            }
        }
    }
}
