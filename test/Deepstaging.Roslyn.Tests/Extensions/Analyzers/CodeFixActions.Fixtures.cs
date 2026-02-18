// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn.Tests.Extensions.Analyzers;

public partial class CodeFixActionsTests
{
    private static async Task<bool> ApplyCodeFixAction(
        string source,
        string expected,
        Func<Document, ValidSyntax<ClassDeclarationSyntax>, Microsoft.CodeAnalysis.CodeActions.CodeAction> createAction)
    {
        var document = CreateDocument(source);
        var root = await document.GetSyntaxRootAsync();
        var classDecl = root?.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

        if (classDecl is null)
            return false;

        var validSyntax = OptionalSyntax<ClassDeclarationSyntax>.FromNullable(classDecl);

        if (validSyntax.IsNotValid(out var valid))
            return false;

        var codeAction = createAction(document, valid);
        return await ApplyAndVerify(document, codeAction, expected);
    }

    private static async Task<bool> ApplyMethodCodeFixAction(
        string source,
        string expected,
        Func<Document, ValidSyntax<MethodDeclarationSyntax>, Microsoft.CodeAnalysis.CodeActions.CodeAction> createAction)
    {
        var document = CreateDocument(source);
        var root = await document.GetSyntaxRootAsync();
        var methodDecl = root?.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

        if (methodDecl is null)
            return false;

        var validSyntax = OptionalSyntax<MethodDeclarationSyntax>.FromNullable(methodDecl);

        if (validSyntax.IsNotValid(out var valid))
            return false;

        var codeAction = createAction(document, valid);
        return await ApplyAndVerify(document, codeAction, expected);
    }

    private static async Task<bool> ApplyFieldCodeFixAction(
        string source,
        string expected,
        Func<Document, ValidSyntax<FieldDeclarationSyntax>, Microsoft.CodeAnalysis.CodeActions.CodeAction> createAction)
    {
        var document = CreateDocument(source);
        var root = await document.GetSyntaxRootAsync();
        var fieldDecl = root?.DescendantNodes().OfType<FieldDeclarationSyntax>().FirstOrDefault();

        if (fieldDecl is null)
            return false;

        var validSyntax = OptionalSyntax<FieldDeclarationSyntax>.FromNullable(fieldDecl);

        if (validSyntax.IsNotValid(out var valid))
            return false;

        var codeAction = createAction(document, valid);
        return await ApplyAndVerify(document, codeAction, expected);
    }

    private static async Task<bool> ApplyPropertyCodeFixAction(
        string source,
        string expected,
        Func<Document, ValidSyntax<PropertyDeclarationSyntax>, Microsoft.CodeAnalysis.CodeActions.CodeAction> createAction)
    {
        var document = CreateDocument(source);
        var root = await document.GetSyntaxRootAsync();
        var propertyDecl = root?.DescendantNodes().OfType<PropertyDeclarationSyntax>().FirstOrDefault();

        if (propertyDecl is null)
            return false;

        var validSyntax = OptionalSyntax<PropertyDeclarationSyntax>.FromNullable(propertyDecl);

        if (validSyntax.IsNotValid(out var valid))
            return false;

        var codeAction = createAction(document, valid);
        return await ApplyAndVerify(document, codeAction, expected);
    }

    private static async Task<bool> ApplyStructCodeFixAction(
        string source,
        string expected,
        Func<Document, ValidSyntax<StructDeclarationSyntax>, Microsoft.CodeAnalysis.CodeActions.CodeAction> createAction)
    {
        var document = CreateDocument(source);
        var root = await document.GetSyntaxRootAsync();
        var structDecl = root?.DescendantNodes().OfType<StructDeclarationSyntax>().FirstOrDefault();

        if (structDecl is null)
            return false;

        var validSyntax = OptionalSyntax<StructDeclarationSyntax>.FromNullable(structDecl);

        if (validSyntax.IsNotValid(out var valid))
            return false;

        var codeAction = createAction(document, valid);
        return await ApplyAndVerify(document, codeAction, expected);
    }

    private static async Task<bool> ApplyUsingCodeFixAction(
        string source,
        string expected,
        string namespaceName)
    {
        var document = CreateDocument(source);

        var codeAction = document.AddUsingAction(namespaceName);
        return await ApplyAndVerify(document, codeAction, expected);
    }

    private static async Task<bool> ApplyAndVerify(
        Document document,
        Microsoft.CodeAnalysis.CodeActions.CodeAction codeAction,
        string expected)
    {
        var operations = await codeAction.GetOperationsAsync(default);

        var changedSolution = operations
            .OfType<Microsoft.CodeAnalysis.CodeActions.ApplyChangesOperation>()
            .FirstOrDefault()
            ?.ChangedSolution;

        if (changedSolution is null)
            return false;

        var changedDocument = changedSolution.GetDocument(document.Id);

        if (changedDocument is null)
            return false;

        var changedText = await changedDocument.GetTextAsync();
        var actualText = Normalize(changedText.ToString());
        var expectedText = Normalize(expected);

        return actualText == expectedText;
    }

    private static Document CreateDocument(string source)
    {
        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);

        var solution = new AdhocWorkspace()
            .CurrentSolution
            .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
            .WithProjectParseOptions(projectId,
                new Microsoft.CodeAnalysis.CSharp.CSharpParseOptions(
                    Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp14))
            .AddMetadataReference(projectId,
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddDocument(documentId, "Test.cs",
                Microsoft.CodeAnalysis.Text.SourceText.From(source));

        return solution.GetDocument(documentId)!;
    }

    private static string Normalize(string text) => text.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
}
