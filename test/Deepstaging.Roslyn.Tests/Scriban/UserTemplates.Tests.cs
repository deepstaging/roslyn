// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;
using Deepstaging.Roslyn.Scriban;
using Microsoft.CodeAnalysis.Text;

namespace Deepstaging.Roslyn.Tests.Scriban;

public class UserTemplatesTests
{
    #region From AdditionalTexts

    [Test]
    public async Task Empty_additional_texts_produces_empty_provider()
    {
        var templates = UserTemplates.From([]);

        await Assert.That(templates.HasTemplate("Anything")).IsFalse();
    }

    [Test]
    public async Task Discovers_template_from_additional_text()
    {
        var texts = CreateAdditionalTexts(
            ("Templates/Deepstaging.Ids/StrongId.scriban-cs", "namespace {{ namespace }} {}"));

        var templates = UserTemplates.From(texts);

        await Assert.That(templates.HasTemplate("Deepstaging.Ids/StrongId")).IsTrue();
    }

    [Test]
    public async Task Ignores_non_scriban_files()
    {
        var texts = CreateAdditionalTexts(
            ("Templates/readme.md", "# Hello"));

        var templates = UserTemplates.From(texts);

        await Assert.That(templates.HasTemplate("readme")).IsFalse();
    }

    [Test]
    public async Task Handles_nested_directory_structure()
    {
        var texts = CreateAdditionalTexts(
            ("Templates/Deepstaging.Effects/Runtime.scriban-cs", "// runtime"),
            ("Templates/Deepstaging.Config/ConfigRoot.scriban-cs", "// config"));

        var templates = UserTemplates.From(texts);

        await Assert.That(templates.HasTemplate("Deepstaging.Effects/Runtime")).IsTrue();
        await Assert.That(templates.HasTemplate("Deepstaging.Config/ConfigRoot")).IsTrue();
    }

    #endregion

    #region TryRender

    [Test]
    public async Task TryRender_returns_null_when_no_template()
    {
        var templates = UserTemplates.From([]);

        var result = templates.TryRender("NonExistent", new { });

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task TryRender_renders_template_with_model()
    {
        var texts = CreateAdditionalTexts(
            ("Templates/Test/Greeting.scriban-cs", "Hello {{ Name }}!"));

        var templates = UserTemplates.From(texts);
        var result = templates.TryRender("Test/Greeting", new { Name = "World" });

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsTypeOf<RenderResult.Success>();
        await Assert.That(((RenderResult.Success)result!).Text).IsEqualTo("Hello World!");
    }

    [Test]
    public async Task TryRender_returns_failure_for_invalid_template()
    {
        var texts = CreateAdditionalTexts(
            ("Templates/Test/Bad.scriban-cs", "{{ for }}"));

        var templates = UserTemplates.From(texts);
        var result = templates.TryRender("Test/Bad", new { });

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsTypeOf<RenderResult.Failure>();
    }

    #endregion

    #region Path Normalization

    [Test]
    public async Task Handles_backslash_paths()
    {
        var texts = CreateAdditionalTexts(
            ("Templates\\Deepstaging.Ids\\StrongId.scriban-cs", "// content"));

        var templates = UserTemplates.From(texts);

        await Assert.That(templates.HasTemplate("Deepstaging.Ids/StrongId")).IsTrue();
    }

    #endregion

    #region GetFilePath

    [Test]
    public async Task GetFilePath_returns_path_for_existing_template()
    {
        var texts = CreateAdditionalTexts(
            ("Templates/Test/MyType.scriban-cs", "content"));

        var templates = UserTemplates.From(texts);

        await Assert.That(templates.GetFilePath("Test/MyType")).IsEqualTo("Templates/Test/MyType.scriban-cs");
    }

    [Test]
    public async Task GetFilePath_returns_null_for_missing_template()
    {
        var templates = UserTemplates.From([]);

        await Assert.That(templates.GetFilePath("NonExistent")).IsNull();
    }

    #endregion

    #region Helpers

    private static ImmutableArray<AdditionalText> CreateAdditionalTexts(
        params (string Path, string Content)[] files) =>
        [..files.Select(f => (AdditionalText)new InMemoryAdditionalText(f.Path, f.Content))];

    private sealed class InMemoryAdditionalText(string path, string content) : AdditionalText
    {
        public override string Path => path;

        public override SourceText? GetText(CancellationToken cancellationToken = default)
            => SourceText.From(content);
    }

    #endregion
}