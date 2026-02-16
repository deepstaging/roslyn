// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Scriban;
using Deepstaging.Roslyn.Testing;

namespace Deepstaging.Roslyn.Tests.Scriban;

/// <summary>
/// Tests for <see cref="ScaffoldAvailableAnalyzer"/>.
/// </summary>
public class ScaffoldAvailableAnalyzerTests : RoslynTestBase
{
    private const string ScaffoldMetadataSource = """
                                                  [assembly: System.Reflection.AssemblyMetadata(
                                                      "Deepstaging.Scaffold:TestProject/Widget",
                                                      "TestProject.WidgetAttribute")]
                                                  [assembly: System.Reflection.AssemblyMetadata(
                                                      "Deepstaging.Scaffold:TestProject/Widget:Content",
                                                      "// scaffold content")]
                                                  """;

    private const string TriggerAttributeSource = """
                                                  namespace TestProject
                                                  {
                                                      [System.AttributeUsage(System.AttributeTargets.Struct)]
                                                      public class WidgetAttribute : System.Attribute { }
                                                  }
                                                  """;

    [Test]
    public async Task ReportsDiagnostic_WhenTypeHasTriggerAttribute_AndNoTemplate()
    {
        var source = $$"""
                       {{ScaffoldMetadataSource}}
                       {{TriggerAttributeSource}}
                       namespace TestApp
                       {
                           [TestProject.Widget]
                           public partial struct MyWidget { }
                       }
                       """;

        await AnalyzeWith<ScaffoldAvailableAnalyzer>(source)
            .ShouldReportDiagnostic("DSRK005")
            .WithSeverity(DiagnosticSeverity.Info)
            .WithMessage("*MyWidget*TestProject/Widget*");
    }

    [Test]
    public async Task NoDiagnostic_WhenUserTemplateExists()
    {
        var source = $$"""
                       {{ScaffoldMetadataSource}}
                       {{TriggerAttributeSource}}
                       namespace TestApp
                       {
                           [TestProject.Widget]
                           public partial struct MyWidget { }
                       }
                       """;

        await AnalyzeWith<ScaffoldAvailableAnalyzer>(source)
            .WithAdditionalText("Templates/TestProject/Widget.scriban-cs", "// user template")
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoScaffoldMetadata()
    {
        var source = """
                     namespace TestApp
                     {
                         public partial struct MyWidget { }
                     }
                     """;

        await AnalyzeWith<ScaffoldAvailableAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenTypeDoesNotHaveTriggerAttribute()
    {
        var source = $$"""
                       {{ScaffoldMetadataSource}}
                       {{TriggerAttributeSource}}
                       namespace TestApp
                       {
                           public partial struct PlainStruct { }
                       }
                       """;

        await AnalyzeWith<ScaffoldAvailableAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task ReportsMultipleDiagnostics_ForMultipleTypes()
    {
        var source = $$"""
                       {{ScaffoldMetadataSource}}
                       {{TriggerAttributeSource}}
                       namespace TestApp
                       {
                           [TestProject.Widget]
                           public partial struct WidgetA { }

                           [TestProject.Widget]
                           public partial struct WidgetB { }
                       }
                       """;

        await AnalyzeWith<ScaffoldAvailableAnalyzer>(source)
            .ShouldReportDiagnostic("DSRK005")
            .WithMessage("*Widget*");
    }
}