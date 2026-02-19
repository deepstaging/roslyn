// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.CodeFixes;

public class PropertyCodeFixActionsTests : RoslynTestBase
{
    private const string Id = NeedsPropertyFixAnalyzer.DiagnosticId;

    private const string ClassWithProperty = """
                                             using Deepstaging.Roslyn.Tests.CodeFixes;

                                             namespace TestApp;

                                             public class Foo
                                             {
                                                 [NeedsPropertyFix]
                                                 public int Value { get; set; }
                                             }
                                             """;

    [Test]
    public async Task AddPartial_AddsPartialModifier() =>
        await AnalyzeAndFixWith<NeedsPropertyFixAnalyzer, AddPartialToPropertyFix>(ClassWithProperty)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsPropertyFix]
                    public partial int Value { get; set; }
                }
                """);

    [Test]
    public async Task AddRequired_AddsRequiredModifier() =>
        await AnalyzeAndFixWith<NeedsPropertyFixAnalyzer, AddRequiredToPropertyFix>(ClassWithProperty)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Add 'required' modifier");

    [Test]
    public async Task MakeInitOnly_ReplacesSetWithInit() =>
        await AnalyzeAndFixWith<NeedsPropertyFixAnalyzer, MakePropertyInitOnlyFix>(ClassWithProperty)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Make property init-only");

    [Test]
    public async Task RenameProperty_RenamesProperty() =>
        await AnalyzeAndFixWith<NeedsPropertyFixAnalyzer, RenamePropertyFix>(ClassWithProperty)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsPropertyFix]
                    public int NewName { get; set; }
                }
                """);
}