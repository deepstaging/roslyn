// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Scriban;

namespace Deepstaging.Roslyn.Tests.Scriban;

public class TemplateRenderFromTextTests
{
    [Test]
    public async Task RenderFromText_renders_with_model()
    {
        var result = Template.RenderFromText(
            "Hello {{ Name }}!", "test-template", new { Name = "World" });

        await Assert.That(result).IsTypeOf<RenderResult.Success>();
        await Assert.That(((RenderResult.Success)result).Text).IsEqualTo("Hello World!");
    }

    [Test]
    public async Task RenderFromText_returns_failure_for_parse_error()
    {
        var result = Template.RenderFromText(
            "{{ for }}", "test-template", new { });

        await Assert.That(result).IsTypeOf<RenderResult.Failure>();
    }

    [Test]
    public async Task RenderFromText_renders_with_null_context()
    {
        var result = Template.RenderFromText(
            "static content", "test-template", null);

        await Assert.That(result).IsTypeOf<RenderResult.Success>();
        await Assert.That(((RenderResult.Success)result).Text).IsEqualTo("static content");
    }

    [Test]
    public async Task RenderFromText_sets_template_name_on_result()
    {
        var result = Template.RenderFromText(
            "content", "my-template", null);

        await Assert.That(result.TemplateName).IsEqualTo("my-template");
    }
}
