// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn.Tests.Extensions.Analyzers;

public class CodeFixActionsTests : RoslynTestBase
{
    #region Modifier Tests

    [Test]
    public async Task AddPartialModifierAction_AddsPartialToClass()
    {
        const string source = "public class MyClass { }";
        const string expected = "public partial class MyClass { }";

        var result = await ApplyCodeFixAction(source, expected,
            (document, typeDecl) => document.AddPartialModifierAction(typeDecl));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task AddSealedModifierAction_AddsSealedToClass()
    {
        const string source = "public class MyClass { }";
        const string expected = "public sealed class MyClass { }";

        var result = await ApplyCodeFixAction(source, expected,
            (document, typeDecl) => document.AddSealedModifierAction(typeDecl));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task AddStaticModifierAction_AddsStaticToClass()
    {
        const string source = "public class MyClass { }";
        const string expected = "public static class MyClass { }";

        var result = await ApplyCodeFixAction(source, expected,
            (document, typeDecl) => document.AddStaticModifierAction(typeDecl));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task RemoveModifierAction_RemovesPublicFromClass()
    {
        const string source = "public class MyClass { }";
        const string expected = "class MyClass { }";

        var result = await ApplyCodeFixAction(source, expected,
            (document, typeDecl) => document.RemoveModifierAction(
                typeDecl, 
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword, 
                "Remove 'public' modifier"));

        await Assert.That(result).IsTrue();
    }

    #endregion

    #region Using Directive Tests

    [Test]
    public async Task AddUsingAction_AddsUsingDirective()
    {
        const string source = """
            namespace TestApp
            {
                public class MyClass { }
            }
            """;

        const string expected = """
            using System.Linq;
            namespace TestApp
            {
                public class MyClass { }
            }
            """;

        var result = await ApplyUsingCodeFixAction(source, expected, "System.Linq");
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task AddUsingAction_DoesNotDuplicateExistingUsing()
    {
        const string source = """
            using System.Linq;
            namespace TestApp
            {
                public class MyClass { }
            }
            """;

        // Should remain unchanged
        var result = await ApplyUsingCodeFixAction(source, source, "System.Linq");
        await Assert.That(result).IsTrue();
    }

    #endregion

    #region Base Type Tests

    [Test]
    public async Task AddBaseTypeAction_AddsBaseClass()
    {
        const string source = "public class MyClass { }";
        const string expected = "public class MyClass : BaseClass { }";

        var result = await ApplyCodeFixAction(source, expected,
            (document, typeDecl) => document.AddBaseTypeAction(typeDecl, "BaseClass"));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task AddInterfaceAction_AddsInterface()
    {
        const string source = "public class MyClass { }";
        const string expected = "public class MyClass : IDisposable { }";

        var result = await ApplyCodeFixAction(source, expected,
            (document, typeDecl) => document.AddInterfaceAction(typeDecl, "IDisposable"));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task AddInterfaceAction_AppendsToExistingBaseList()
    {
        const string source = "public class MyClass : BaseClass { }";
        const string expected = "public class MyClass : BaseClass, IDisposable { }";

        var result = await ApplyCodeFixAction(source, expected,
            (document, typeDecl) => document.AddInterfaceAction(typeDecl, "IDisposable"));

        await Assert.That(result).IsTrue();
    }

    #endregion

    #region Helper Methods

    private static async Task<bool> ApplyCodeFixAction(
        string source,
        string expected,
        Func<Microsoft.CodeAnalysis.Document, ValidSyntax<ClassDeclarationSyntax>, Microsoft.CodeAnalysis.CodeActions.CodeAction> createAction)
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

    private static async Task<bool> ApplyUsingCodeFixAction(
        string source,
        string expected,
        string namespaceName)
    {
        var document = CreateDocument(source);

        var codeAction = document.AddUsingAction(namespaceName);
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

    private static Microsoft.CodeAnalysis.Document CreateDocument(string source)
    {
        var projectId = Microsoft.CodeAnalysis.ProjectId.CreateNewId();
        var documentId = Microsoft.CodeAnalysis.DocumentId.CreateNewId(projectId);

        var solution = new Microsoft.CodeAnalysis.AdhocWorkspace()
            .CurrentSolution
            .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
            .WithProjectParseOptions(projectId, 
                new Microsoft.CodeAnalysis.CSharp.CSharpParseOptions(
                    Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp14))
            .AddMetadataReference(projectId, 
                Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddDocument(documentId, "Test.cs", 
                Microsoft.CodeAnalysis.Text.SourceText.From(source));

        return solution.GetDocument(documentId)!;
    }

    private static string Normalize(string text) => 
        text.Replace("\r\n", "\n").Replace("\r", "\n").Trim();

    #endregion
}
