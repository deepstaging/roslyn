// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.CodeFixes;

public class FieldCodeFixActionsTests : RoslynTestBase
{
    private const string Id = NeedsFieldFixAnalyzer.DiagnosticId;

    [Test]
    public async Task MakePrivate_ReplacesAccessibility() =>
        await AnalyzeAndFixWith<NeedsFieldFixAnalyzer, MakeFieldPrivateFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsFieldFix]
                    public int _value;
                }
                """)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Make field private");

    [Test]
    public async Task AddReadonly_AddsReadonlyModifier() =>
        await AnalyzeAndFixWith<NeedsFieldFixAnalyzer, AddReadonlyToFieldFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsFieldFix]
                    private int _value;
                }
                """)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Add 'readonly' modifier");

    [Test]
    public async Task RenameField_RenamesFirstVariable() =>
        await AnalyzeAndFixWith<NeedsFieldFixAnalyzer, RenameFieldFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsFieldFix]
                    private int x;
                }
                """)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Rename to '_value'");

    [Test]
    public async Task RenameField_WithIndex_RenamesSpecificVariable() =>
        await AnalyzeAndFixWith<NeedsFieldFixAnalyzer, RenameFieldWithIndexFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsFieldFix]
                    private int x, y;
                }
                """)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Rename to '_second'");
}
