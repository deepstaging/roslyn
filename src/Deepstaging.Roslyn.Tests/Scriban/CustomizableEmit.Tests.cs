// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;
using Deepstaging.Roslyn.Scriban;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Deepstaging.Roslyn.Tests.Scriban;

public class CustomizableEmitTests : RoslynTestBase
{
    #region DefineUserTemplate

    [Test]
    public async Task DefineUserTemplate_preserves_default_emit()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();
        var customizable = emit.DefineUserTemplate("Test/MyClass", new { });

        await Assert.That(customizable.DefaultEmit.Success).IsTrue();
        await Assert.That(customizable.TemplateName).IsEqualTo("Test/MyClass");
    }

    #endregion

    #region ResolveFrom - No User Template

    [Test]
    public async Task ResolveFrom_returns_default_when_no_user_template()
    {
        var emit = TypeBuilder.Class("MyClass").Emit();
        var customizable = emit.DefineUserTemplate("Test/MyClass", new { });

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
        var customizable = emit.DefineUserTemplate("Test/MyType", model);

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
        var customizable = emit.DefineUserTemplate("Test/MyType", new { });

        var texts = CreateAdditionalTexts(
            ("Templates/Test/MyType.scriban-cs",
                "public class { invalid c# syntax ;;;; }"));

        var templates = UserTemplates.From(texts);
        var resolved = customizable.ResolveFrom(templates);

        await Assert.That(resolved.Success).IsFalse();
        await Assert.That(resolved.Diagnostics).IsNotEmpty();
    }

    [Test]
    public async Task ResolveFrom_returns_default_emit_when_default_is_invalid()
    {
        var method = MethodBuilder
            .For("Bad")
            .WithBody(b => b.AddStatement("not valid c#"));

        var emit = TypeBuilder.Class("MyClass").AddMethod(method).Emit();
        var customizable = emit.DefineUserTemplate("Test/MyType", new { });

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
        var customizable = emit.DefineUserTemplate("Test/MyType", new { });

        var texts = CreateAdditionalTexts(
            ("Templates/Test/MyType.scriban-cs", "{{ for }}"));

        var templates = UserTemplates.From(texts);
        var resolved = customizable.ResolveFrom(templates);

        await Assert.That(resolved.Success).IsFalse();
    }

    #endregion

    #region Helpers

    private static ImmutableArray<AdditionalText> CreateAdditionalTexts(
        params (string Path, string Content)[] files)
    {
        return [..files.Select(f => (AdditionalText)new InMemoryAdditionalText(f.Path, f.Content))];
    }

    private sealed class InMemoryAdditionalText(string path, string content) : AdditionalText
    {
        public override string Path => path;

        public override SourceText? GetText(CancellationToken cancellationToken = default)
            => SourceText.From(content);
    }

    #endregion
}
