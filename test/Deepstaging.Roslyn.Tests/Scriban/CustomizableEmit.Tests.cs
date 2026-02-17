// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;
using Deepstaging.Roslyn.Scriban;
using Microsoft.CodeAnalysis.Text;

namespace Deepstaging.Roslyn.Tests.Scriban;

public class CustomizableEmitTests : RoslynTestBase
{
    #region WithUserTemplate

    [Test]
    public async Task WithUserTemplate_preserves_default_emit()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();
        var customizable = emit.WithUserTemplate("Test/MyClass", new { });

        await Assert.That(customizable.DefaultEmit.Success).IsTrue();
        await Assert.That(customizable.TemplateName).IsEqualTo("Test/MyClass");
    }

    [Test]
    public async Task WithUserTemplate_with_map_records_bindings()
    {
        var map = new TemplateMap<SimpleModel>();

        TypeBuilder.Class(map.Bind("MyClass", m => m.TypeName))
            .InNamespace(map.Bind("TestApp", m => m.Namespace))
            .Emit()
            .WithUserTemplate("Test/MyClass", new SimpleModel(), map);

        await Assert.That(map.Bindings).Count().IsEqualTo(2);
        await Assert.That(map.Bindings[0].PropertyPath).IsEqualTo("TypeName");
        await Assert.That(map.Bindings[0].Value).IsEqualTo("MyClass");
        await Assert.That(map.Bindings[1].PropertyPath).IsEqualTo("Namespace");
        await Assert.That(map.Bindings[1].Value).IsEqualTo("TestApp");
    }

    private sealed class SimpleModel
    {
        public string TypeName { get; init; } = "";
        public string Namespace { get; init; } = "";
    }

    #endregion

    #region ResolveFrom - No User Template

    [Test]
    public async Task ResolveFrom_returns_default_when_no_user_template()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();
        var customizable = emit.WithUserTemplate("Test/MyClass", new { });

        var resolved = customizable.ResolveFrom(UserTemplates.Empty);

        await Assert.That(resolved.Success).IsTrue();
        await Assert.That(resolved.Code).Contains("class MyClass");
    }

    #endregion

    #region ResolveFrom - User Template Override

    [Test]
    public async Task ResolveFrom_uses_user_template_when_available()
    {
        var model = new { TypeName = "CustomClass" };
        var emit = TypeBuilder.Class("DefaultClass").Emit();
        var customizable = emit.WithUserTemplate("Test/MyType", model);

        var texts = CreateAdditionalTexts(
            ("Templates/Test/MyType.scriban-cs",
                "public class {{ TypeName }} { }"));

        var templates = UserTemplates.From(texts);
        var resolved = customizable.ResolveFrom(templates);

        await Assert.That(resolved.Success).IsTrue();
        await Assert.That(resolved.Code).Contains("class CustomClass");
        await Assert.That(resolved.Code).DoesNotContain("DefaultClass");
    }

    #endregion

    #region ResolveFrom - Validation

    [Test]
    public async Task ResolveFrom_validates_rendered_output()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();
        var customizable = emit.WithUserTemplate("Test/MyType", new { });

        var texts = CreateAdditionalTexts(
            ("Templates/Test/MyType.scriban-cs",
                "public class { invalid c# syntax ;;;; }"));

        var templates = UserTemplates.From(texts);
        var resolved = customizable.ResolveFrom(templates);

        await Assert.That(resolved.Success).IsFalse();
        await Assert.That(resolved.Diagnostics).IsNotEmpty();
    }

    [Test]
    public async Task ResolveFrom_invalid_csharp_reports_DSRK006_with_template_name()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();
        var customizable = emit.WithUserTemplate("Test/MyType", new { });

        var texts = CreateAdditionalTexts(
            ("Templates/Test/MyType.scriban-cs",
                "public class { }"));

        var templates = UserTemplates.From(texts);
        var resolved = customizable.ResolveFrom(templates);

        await Assert.That(resolved.Success).IsFalse();
        await Assert.That(resolved.Diagnostics.First().Id).IsEqualTo("DSRK006");
        await Assert.That(resolved.Diagnostics.First().GetMessage()).Contains("Test/MyType");
    }

    [Test]
    public async Task ResolveFrom_invalid_csharp_includes_cs_errors_in_message()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();
        var customizable = emit.WithUserTemplate("Test/MyType", new { });

        var texts = CreateAdditionalTexts(
            ("Templates/Test/MyType.scriban-cs",
                "public class { }"));

        var templates = UserTemplates.From(texts);
        var resolved = customizable.ResolveFrom(templates);

        await Assert.That(resolved.Diagnostics.First().GetMessage()).Contains("CS");
    }

    [Test]
    public async Task ResolveFrom_invalid_csharp_location_points_to_template_file()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();
        var customizable = emit.WithUserTemplate("Test/MyType", new { });

        var texts = CreateAdditionalTexts(
            ("Templates/Test/MyType.scriban-cs",
                "public class { }"));

        var templates = UserTemplates.From(texts);
        var resolved = customizable.ResolveFrom(templates);

        var location = resolved.Diagnostics.First().Location;
        await Assert.That(location.IsInSource).IsFalse();
        await Assert.That(location.GetLineSpan().Path).Contains("Templates/Test/MyType.scriban-cs");
    }

    [Test]
    public async Task ResolveFrom_returns_default_emit_when_default_is_invalid()
    {
        var method = MethodBuilder
            .For("Bad")
            .WithBody(b => b.AddStatement("not valid c#"));

        var emit = TypeBuilder.Class("MyClass").AddMethod(method).Emit();
        var customizable = emit.WithUserTemplate("Test/MyType", new { });

        // Even if user template exists, default emit failures are returned as-is
        var texts = CreateAdditionalTexts(
            ("Templates/Test/MyType.scriban-cs", "public class Good { }"));

        var templates = UserTemplates.From(texts);
        var resolved = customizable.ResolveFrom(templates);

        await Assert.That(resolved.Success).IsFalse();
    }

    #endregion

    #region ResolveFrom - Template Render Failure

    [Test]
    public async Task ResolveFrom_returns_failure_when_template_has_errors()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();
        var customizable = emit.WithUserTemplate("Test/MyType", new { });

        var texts = CreateAdditionalTexts(
            ("Templates/Test/MyType.scriban-cs", "{{ for }}"));

        var templates = UserTemplates.From(texts);
        var resolved = customizable.ResolveFrom(templates);

        await Assert.That(resolved.Success).IsFalse();
    }

    [Test]
    public async Task ResolveFrom_scriban_error_reports_DSRK007_with_template_name()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();
        var customizable = emit.WithUserTemplate("Test/MyType", new { });

        var texts = CreateAdditionalTexts(
            ("Templates/Test/MyType.scriban-cs", "{{ for }}"));

        var templates = UserTemplates.From(texts);
        var resolved = customizable.ResolveFrom(templates);

        await Assert.That(resolved.Diagnostics.First().Id).IsEqualTo("DSRK007");
        await Assert.That(resolved.Diagnostics.First().GetMessage()).Contains("Test/MyType");
    }

    [Test]
    public async Task ResolveFrom_scriban_error_location_points_to_template_file()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();
        var customizable = emit.WithUserTemplate("Test/MyType", new { });

        var texts = CreateAdditionalTexts(
            ("Templates/Test/MyType.scriban-cs", "{{ for }}"));

        var templates = UserTemplates.From(texts);
        var resolved = customizable.ResolveFrom(templates);

        var location = resolved.Diagnostics.First().Location;
        await Assert.That(location.GetLineSpan().Path).Contains("Templates/Test/MyType.scriban-cs");
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