// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Scriban;

namespace Deepstaging.Roslyn.Tests.Scriban;

/// <summary>
/// Tests for <see cref="ScaffoldTemplateCodeFix"/>.
/// </summary>
public class ScaffoldTemplateCodeFixTests : RoslynTestBase
{
    private const string Source = """
                                  [assembly: System.Reflection.AssemblyMetadata(
                                      "Deepstaging.Scaffold:TestProject/Widget",
                                      "TestProject.WidgetAttribute")]
                                  [assembly: System.Reflection.AssemblyMetadata(
                                      "Deepstaging.Scaffold:TestProject/Widget:Content",
                                      "// scaffold content for {{ TypeName }}")]
                                  namespace TestProject
                                  {
                                      [System.AttributeUsage(System.AttributeTargets.Struct)]
                                      public class WidgetAttribute : System.Attribute { }
                                  }
                                  namespace TestApp
                                  {
                                      [TestProject.Widget]
                                      public partial struct MyWidget { }
                                  }
                                  """;

    [Test]
    public async Task CreatesTemplateFile_WithScaffoldContent() =>
        await AnalyzeAndFixWith<ScaffoldAvailableAnalyzer, ScaffoldTemplateCodeFix>(Source)
            .ForDiagnostic(ScaffoldDiagnostics.ScaffoldAvailable)
            .ShouldAddAdditionalDocument()
            .WithPathContaining("Widget")
            .WithContentContaining("scaffold content for {{ TypeName }}");

    [Test]
    public async Task NoFix_WhenUserTemplateExists() =>
        // When user template exists, the analyzer doesn't fire → no diagnostic → no code fix offered
        await AnalyzeWith<ScaffoldAvailableAnalyzer>(Source)
            .WithAdditionalText("Templates/TestProject/Widget.scriban-cs", "// user template")
            .ShouldHaveNoDiagnostics();
}