// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Analyzers;
using Deepstaging.Roslyn.Testing;

namespace Deepstaging.Roslyn.Tests.Analyzers;

public class PipelineModelAnalyzerTests : RoslynTestBase
{
    // ── DSRK001: ImmutableArray ──────────────────────────────────────────

    [Test]
    public async Task DSRK001_ReportsDiagnostic_WhenPropertyUsesImmutableArray()
    {
        const string source = """
            using System.Collections.Immutable;
            using Deepstaging.Roslyn;

            namespace TestApp;

            [PipelineModel]
            public sealed record MyModel(string Name, ImmutableArray<string> Items);
            """;

        await AnalyzeWith<PipelineModelImmutableArrayAnalyzer>(source)
            .ShouldReportDiagnostic("DSRK001")
            .WithSeverity(DiagnosticSeverity.Error)
            .WithMessage("*Items*MyModel*ImmutableArray*EquatableArray*");
    }

    [Test]
    public async Task DSRK001_NoDiagnostic_WhenPropertyUsesEquatableArray()
    {
        const string source = """
            using Deepstaging.Roslyn;

            namespace TestApp;

            [PipelineModel]
            public sealed record MyModel(string Name, EquatableArray<string> Items);
            """;

        await AnalyzeWith<PipelineModelImmutableArrayAnalyzer>(source)
            .ShouldNotReportDiagnostic("DSRK001");
    }

    // ── DSRK002: ValidSymbol ─────────────────────────────────────────────

    [Test]
    public async Task DSRK002_ReportsDiagnostic_WhenPropertyUsesValidSymbol()
    {
        const string source = """
            using Deepstaging.Roslyn;
            using Microsoft.CodeAnalysis;

            namespace TestApp;

            [PipelineModel]
            public sealed record MyModel(string Name, ValidSymbol<INamedTypeSymbol> Symbol);
            """;

        await AnalyzeWith<PipelineModelValidSymbolAnalyzer>(source)
            .ShouldReportDiagnostic("DSRK002")
            .WithSeverity(DiagnosticSeverity.Error)
            .WithMessage("*Symbol*MyModel*ValidSymbol*snapshot*");
    }

    // ── DSRK003: ISymbol ─────────────────────────────────────────────────

    [Test]
    public async Task DSRK003_ReportsDiagnostic_WhenPropertyUsesISymbol()
    {
        const string source = """
            using Deepstaging.Roslyn;
            using Microsoft.CodeAnalysis;

            namespace TestApp;

            [PipelineModel]
            public sealed record MyModel(string Name, INamedTypeSymbol Symbol);
            """;

        await AnalyzeWith<PipelineModelSymbolAnalyzer>(source)
            .ShouldReportDiagnostic("DSRK003")
            .WithSeverity(DiagnosticSeverity.Error)
            .WithMessage("*Symbol*MyModel*ISymbol*");
    }

    [Test]
    public async Task DSRK003_ReportsDiagnostic_WhenPropertyUsesIMethodSymbol()
    {
        const string source = """
            using Deepstaging.Roslyn;
            using Microsoft.CodeAnalysis;

            namespace TestApp;

            [PipelineModel]
            public sealed record MyModel(string Name, IMethodSymbol Method);
            """;

        await AnalyzeWith<PipelineModelSymbolAnalyzer>(source)
            .ShouldReportDiagnostic("DSRK003")
            .WithSeverity(DiagnosticSeverity.Error);
    }

    // ── DSRK004: Non-IEquatable ──────────────────────────────────────────

    [Test]
    public async Task DSRK004_ReportsDiagnostic_WhenPropertyTypeIsNotEquatable()
    {
        const string source = """
            using Deepstaging.Roslyn;
            using System.Xml.Linq;

            namespace TestApp;

            [PipelineModel]
            public sealed record MyModel(string Name, XElement Docs);
            """;

        await AnalyzeWith<PipelineModelNonEquatableAnalyzer>(source)
            .ShouldReportDiagnostic("DSRK004")
            .WithSeverity(DiagnosticSeverity.Warning)
            .WithMessage("*Docs*MyModel*XElement*IEquatable*");
    }

    // ── Clean models: no diagnostics ─────────────────────────────────────

    [Test]
    public async Task NoDiagnostic_WhenAllPropertiesAreClean()
    {
        const string source = """
            using Deepstaging.Roslyn;

            namespace TestApp;

            [PipelineModel]
            public sealed record CleanModel(string Name, int Count, bool IsActive);
            """;

        await AnalyzeWith<PipelineModelImmutableArrayAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenRecordDoesNotHavePipelineModelAttribute()
    {
        const string source = """
            using System.Collections.Immutable;

            namespace TestApp;

            public sealed record NotAModel(string Name, ImmutableArray<string> Items);
            """;

        await AnalyzeWith<PipelineModelImmutableArrayAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenPropertyIsEnum()
    {
        const string source = """
            using Deepstaging.Roslyn;

            namespace TestApp;

            public enum Status { Active, Inactive }

            [PipelineModel]
            public sealed record MyModel(string Name, Status Status);
            """;

        await AnalyzeWith<PipelineModelNonEquatableAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenPropertyIsNullableValueType()
    {
        const string source = """
            using Deepstaging.Roslyn;

            namespace TestApp;

            [PipelineModel]
            public sealed record MyModel(string Name, int? OptionalCount);
            """;

        await AnalyzeWith<PipelineModelNonEquatableAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenPropertyIsNestedPipelineModel()
    {
        const string source = """
            using Deepstaging.Roslyn;

            namespace TestApp;

            [PipelineModel]
            public sealed record ChildModel(string Value);

            [PipelineModel]
            public sealed record ParentModel(string Name, ChildModel Child);
            """;

        await AnalyzeWith<PipelineModelNonEquatableAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    // ── Multiple diagnostics ─────────────────────────────────────────────

    [Test]
    public async Task ReportsMultipleDiagnostics_ForMultipleProblematicProperties()
    {
        const string source = """
            using System.Collections.Immutable;
            using Deepstaging.Roslyn;
            using Microsoft.CodeAnalysis;

            namespace TestApp;

            [PipelineModel]
            public sealed record BrokenModel(
                string Name,
                ImmutableArray<string> Items,
                INamedTypeSymbol Symbol);
            """;

        await AnalyzeWith<PipelineModelImmutableArrayAnalyzer>(source)
            .ShouldReportDiagnostic("DSRK001");

        await AnalyzeWith<PipelineModelSymbolAnalyzer>(source)
            .ShouldReportDiagnostic("DSRK003");
    }
}
