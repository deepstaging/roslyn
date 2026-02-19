// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Extensions.Analyzers;

public partial class CodeFixActionsTests : RoslynTestBase
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

    #region Method Modifier Tests

    [Test]
    public async Task AddAsyncModifierAction_AddsAsyncToMethod()
    {
        const string source = "public class MyClass { public void MyMethod() { } }";
        const string expected = "public class MyClass { public async void MyMethod() { } }";

        var result = await ApplyMethodCodeFixAction(source, expected,
            (document, methodDecl) => document.AddAsyncModifierAction(methodDecl));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task AddVirtualModifierAction_AddsVirtualToMethod()
    {
        const string source = "public class MyClass { public void MyMethod() { } }";
        const string expected = "public class MyClass { public virtual void MyMethod() { } }";

        var result = await ApplyMethodCodeFixAction(source, expected,
            (document, methodDecl) => document.AddVirtualModifierAction(methodDecl));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task AddOverrideModifierAction_AddsOverrideToMethod()
    {
        const string source = "public class MyClass { public void MyMethod() { } }";
        const string expected = "public class MyClass { public override void MyMethod() { } }";

        var result = await ApplyMethodCodeFixAction(source, expected,
            (document, methodDecl) => document.AddOverrideModifierAction(methodDecl));

        await Assert.That(result).IsTrue();
    }

    #endregion

    #region Method Rename Tests

    [Test]
    public async Task RenameMethodAction_RenamesMethod()
    {
        const string source = "public class MyClass { public void OldName() { } }";
        const string expected = "public class MyClass { public void NewName() { } }";

        var result = await ApplyMethodCodeFixAction(source, expected,
            (document, methodDecl) => document.RenameMethodAction(methodDecl, "NewName"));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task AddAsyncSuffixAction_AddsAsyncSuffix()
    {
        const string source = "public class MyClass { public void GetData() { } }";
        const string expected = "public class MyClass { public void GetDataAsync() { } }";

        var result = await ApplyMethodCodeFixAction(source, expected,
            (document, methodDecl) => document.AddAsyncSuffixAction(methodDecl));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task RemoveAsyncSuffixAction_RemovesAsyncSuffix()
    {
        const string source = "public class MyClass { public void GetDataAsync() { } }";
        const string expected = "public class MyClass { public void GetData() { } }";

        var result = await ApplyMethodCodeFixAction(source, expected,
            (document, methodDecl) => document.RemoveAsyncSuffixAction(methodDecl));

        await Assert.That(result).IsTrue();
    }

    #endregion

    #region Return Type Tests

    [Test]
    public async Task ChangeReturnTypeAction_ChangesReturnType()
    {
        const string source = "public class MyClass { public void MyMethod() { } }";
        const string expected = "public class MyClass { public int MyMethod() { } }";

        var result = await ApplyMethodCodeFixAction(source, expected,
            (document, methodDecl) => document.ChangeReturnTypeAction(methodDecl, "int"));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task WrapReturnTypeInTaskAction_WrapsVoidInTask()
    {
        const string source = "public class MyClass { public void MyMethod() { } }";
        const string expected = "public class MyClass { public Task MyMethod() { } }";

        var result = await ApplyMethodCodeFixAction(source, expected,
            (document, methodDecl) => document.WrapReturnTypeInTaskAction(methodDecl));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task WrapReturnTypeInTaskAction_WrapsTypeInTaskGeneric()
    {
        const string source = "public class MyClass { public int MyMethod() { return 0; } }";
        const string expected = "public class MyClass { public Task<int> MyMethod() { return 0; } }";

        var result = await ApplyMethodCodeFixAction(source, expected,
            (document, methodDecl) => document.WrapReturnTypeInTaskAction(methodDecl));

        await Assert.That(result).IsTrue();
    }

    #endregion

    #region Field Tests

    [Test]
    public async Task MakeFieldPrivateAction_MakesFieldPrivate()
    {
        const string source = "public class MyClass { public int _value; }";
        const string expected = "public class MyClass { private int _value; }";

        var result = await ApplyFieldCodeFixAction(source, expected,
            (document, fieldDecl) => document.MakeFieldPrivateAction(fieldDecl));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task AddFieldReadonlyModifierAction_AddsReadonlyToField()
    {
        const string source = "public class MyClass { private int _value; }";
        const string expected = "public class MyClass { private readonly int _value; }";

        var result = await ApplyFieldCodeFixAction(source, expected,
            (document, fieldDecl) => document.AddFieldReadonlyModifierAction(fieldDecl));

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task RenameFieldAction_RenamesField()
    {
        const string source = "public class MyClass { private int oldName; }";
        const string expected = "public class MyClass { private int newName; }";

        var result = await ApplyFieldCodeFixAction(source, expected,
            (document, fieldDecl) => document.RenameFieldAction(fieldDecl, "newName"));

        await Assert.That(result).IsTrue();
    }

    #endregion

    #region Property Tests

    [Test]
    public async Task RenamePropertyAction_RenamesProperty()
    {
        const string source = "public class MyClass { public int OldName { get; set; } }";
        const string expected = "public class MyClass { public int NewName { get; set; } }";

        var result = await ApplyPropertyCodeFixAction(source, expected,
            (document, propertyDecl) => document.RenamePropertyAction(propertyDecl, "NewName"));

        await Assert.That(result).IsTrue();
    }

    #endregion

    #region Struct Tests

    [Test]
    public async Task AddReadonlyModifierAction_AddsReadonlyToStruct()
    {
        const string source = "public struct MyStruct { }";
        const string expected = "public readonly struct MyStruct { }";

        var result = await ApplyStructCodeFixAction(source, expected,
            (document, structDecl) => document.AddReadonlyModifierAction(structDecl));

        await Assert.That(result).IsTrue();
    }

    #endregion

    #region Type Rename Tests

    [Test]
    public async Task RenameTypeAction_RenamesClass()
    {
        const string source = "public class OldName { }";
        const string expected = "public class NewName { }";

        var result = await ApplyCodeFixAction(source, expected,
            (document, typeDecl) => document.RenameTypeAction(typeDecl, "NewName"));

        await Assert.That(result).IsTrue();
    }

    #endregion
}