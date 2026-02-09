// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Testing;
using Deepstaging.RoslynKit.Analyzers;
using Microsoft.CodeAnalysis;

namespace Deepstaging.RoslynKit.Tests;

/// <summary>
/// Tests for the GenerateWithAnalyzer.
/// </summary>
public class GenerateWithAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotPartial()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [GenerateWith]
                              public class Person
                              {
                                  public string Name { get; init; }
                              }
                              """;

        await AnalyzeWith<GenerateWithAnalyzer>(source)
            .ShouldReportDiagnostic(Diagnostics.GenerateWithMustBePartial)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithMessage("*Person*partial*");
    }

    [Test]
    public async Task ReportsDiagnostic_WhenStructIsNotPartial()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [GenerateWith]
                              public struct Point
                              {
                                  public int X { get; init; }
                              }
                              """;

        await AnalyzeWith<GenerateWithAnalyzer>(source)
            .ShouldReportDiagnostic(Diagnostics.GenerateWithMustBePartial)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithMessage("*Point*partial*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsPartial()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [GenerateWith]
                              public partial class Person
                              {
                                  public string Name { get; init; }
                              }
                              """;

        await AnalyzeWith<GenerateWithAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenStructIsPartial()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [GenerateWith]
                              public partial struct Point
                              {
                                  public int X { get; init; }
                              }
                              """;

        await AnalyzeWith<GenerateWithAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoAttribute()
    {
        const string source = """
                              namespace TestApp;

                              public class Person
                              {
                                  public string Name { get; init; }
                              }
                              """;

        await AnalyzeWith<GenerateWithAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}