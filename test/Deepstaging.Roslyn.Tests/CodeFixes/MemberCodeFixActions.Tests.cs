// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.CodeFixes;

public class MemberCodeFixActionsTests : RoslynTestBase
{
    private const string Id = NeedsTypeFixAnalyzer.DiagnosticId;

    private const string ClassSource = """
                                       using Deepstaging.Roslyn.Tests.CodeFixes;

                                       namespace TestApp;

                                       [NeedsTypeFix]
                                       public class Foo { }
                                       """;

    [Test]
    public async Task AddAttribute_AddsAttribute() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddObsoleteAttributeFix>(ClassSource)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Add [Obsolete] attribute");

    [Test]
    public async Task AddAttribute_WithArguments() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddObsoleteWithArgsFix>(ClassSource)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Add [Obsolete] attribute");

    [Test]
    public async Task RemoveAttribute_RemovesAttribute() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, RemoveNeedsTypeFixAttributeFix>(ClassSource)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Remove [NeedsTypeFix] attribute");

    [Test]
    public async Task ReplaceAttribute_ReplacesAttribute() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, ReplaceNeedsTypeFixAttributeFix>(ClassSource)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Replace [NeedsTypeFix] with [Serializable]");
}
