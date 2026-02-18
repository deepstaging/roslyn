// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.CodeFixes;

public class ProjectCodeFixActionsTests : RoslynTestBase
{
    private const string Id = NeedsTypeFixAnalyzer.DiagnosticId;

    private const string ClassSource = """
                                       using Deepstaging.Roslyn.Tests.CodeFixes;

                                       namespace TestApp;

                                       [NeedsTypeFix]
                                       public class Foo { }
                                       """;

    [Test]
    public async Task AddProjectProperty_OffersFixWithTitle() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddProjectPropertyFix>(ClassSource)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Add <UserSecretsId> to project");

    [Test]
    public async Task WriteFile_OffersFixWithTitle() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, WriteFileFix>(ClassSource)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Create appsettings.json");

    [Test]
    public async Task WriteFiles_OffersFixWithTitle() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, WriteFilesFix>(ClassSource)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Generate config files");

    [Test]
    public async Task FileActions_OffersFixWithTitle() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, FileActionsFix>(ClassSource)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Setup project files");

    [Test]
    public async Task ModifyXmlFile_OffersFixWithTitle() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, ModifyXmlFileFix>(ClassSource)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Add build property");
}
