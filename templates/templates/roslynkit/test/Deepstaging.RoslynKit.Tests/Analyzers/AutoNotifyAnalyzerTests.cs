// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Testing;
using Deepstaging.RoslynKit.Analyzers;
using Microsoft.CodeAnalysis;

namespace Deepstaging.RoslynKit.Tests.Analyzers;

public class AutoNotifyAnalyzerTests : RoslynTestBase
{
    // ── RK001: Type must be partial ─────────────────────────────────────

    [Test]
    public async Task RK001_Reports_when_class_not_partial() =>
        await AnalyzeWith<MustBePartialAnalyzer>(
                """
                using Deepstaging.RoslynKit;

                namespace TestApp;

                [AutoNotify]
                public class Person
                {
                    private string _name;
                }
                """)
            .ShouldReportDiagnostic("RK001")
            .WithSeverity(DiagnosticSeverity.Error)
            .WithMessage("*Person*partial*");

    [Test]
    public async Task RK001_No_diagnostic_when_class_is_partial() =>
        await AnalyzeWith<MustBePartialAnalyzer>(
                """
                using Deepstaging.RoslynKit;

                namespace TestApp;

                [AutoNotify]
                public partial class Person
                {
                    private string _name;
                }
                """)
            .ShouldNotReportDiagnostic("RK001");

    [Test]
    public async Task RK001_No_diagnostic_without_attribute() =>
        await AnalyzeWith<MustBePartialAnalyzer>(
                """
                namespace TestApp;

                public class Person
                {
                    private string _name;
                }
                """)
            .ShouldNotReportDiagnostic("RK001");

    // ── RK002: Field must be private ────────────────────────────────────

    [Test]
    public async Task RK002_Reports_when_field_not_private() =>
        await AnalyzeWith<FieldMustBePrivateAnalyzer>(
                """
                using Deepstaging.RoslynKit;

                namespace TestApp;

                [AutoNotify]
                public partial class Person
                {
                    public string _name;
                }
                """)
            .ShouldReportDiagnostic("RK002")
            .WithSeverity(DiagnosticSeverity.Error);

    [Test]
    public async Task RK002_No_diagnostic_when_field_is_private() =>
        await AnalyzeWith<FieldMustBePrivateAnalyzer>(
                """
                using Deepstaging.RoslynKit;

                namespace TestApp;

                [AutoNotify]
                public partial class Person
                {
                    private string _name;
                }
                """)
            .ShouldNotReportDiagnostic("RK002");

    [Test]
    public async Task RK002_No_diagnostic_without_attribute() =>
        await AnalyzeWith<FieldMustBePrivateAnalyzer>(
                """
                namespace TestApp;

                public partial class Person
                {
                    public string _name;
                }
                """)
            .ShouldNotReportDiagnostic("RK002");
}