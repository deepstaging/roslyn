// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.CodeFixes;

public class DocumentCodeFixActionsTests : RoslynTestBase
{
    private const string Id = NeedsTypeFixAnalyzer.DiagnosticId;

    private const string ClassSource = """
                                       using Deepstaging.Roslyn.Tests.CodeFixes;

                                       namespace TestApp;

                                       [NeedsTypeFix]
                                       public class Foo { }
                                       """;

    [Test]
    public async Task AddUsing_AddsUsingDirective() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddUsingFix>(ClassSource)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Add 'using System.Collections.Generic;'");

    [Test]
    public async Task SuppressWithPragma_OffersPragmaSuppression() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, SuppressWithPragmaFix>(ClassSource)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Suppress 'TEST_TYPE' with #pragma");
}