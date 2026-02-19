// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.TypeScript.Emit;

namespace Deepstaging.Roslyn.TypeScript.Tests.Emit;

public class TsBodyBuilderTests
{
    // ── Statements ──────────────────────────────────────────────────────

    [Test]
    public async Task AddStatement_AppendsSemicolon()
    {
        var body = TsBodyBuilder.Empty().AddStatement("console.log('hello')");
        await Assert.That(body.Statements[0]).IsEqualTo("console.log('hello');");
    }

    [Test]
    public async Task AddStatement_PreservesSemicolon()
    {
        var body = TsBodyBuilder.Empty().AddStatement("x++;");
        await Assert.That(body.Statements[0]).IsEqualTo("x++;");
    }

    [Test]
    public async Task AddStatement_PreservesBrace()
    {
        var body = TsBodyBuilder.Empty().AddStatement("if (x) {}");
        await Assert.That(body.Statements[0]).IsEqualTo("if (x) {}");
    }

    [Test]
    public async Task AddStatements_Multiple()
    {
        var body = TsBodyBuilder.Empty().AddStatements("a", "b", "c");
        await Assert.That(body.Statements).Count().IsEqualTo(3);
    }

    [Test]
    public void AddStatement_ThrowsOnEmpty() =>
        Assert.Throws<ArgumentException>(() => TsBodyBuilder.Empty().AddStatement(""));

    // ── Returns ─────────────────────────────────────────────────────────

    [Test]
    public async Task AddReturn_WithExpression()
    {
        var body = TsBodyBuilder.Empty().AddReturn("result");
        await Assert.That(body.Statements[0]).IsEqualTo("return result;");
    }

    [Test]
    public async Task AddReturn_Empty()
    {
        var body = TsBodyBuilder.Empty().AddReturn();
        await Assert.That(body.Statements[0]).IsEqualTo("return;");
    }

    // ── Throws ──────────────────────────────────────────────────────────

    [Test]
    public async Task AddThrow()
    {
        var body = TsBodyBuilder.Empty().AddThrow("new Error('fail')");
        await Assert.That(body.Statements[0]).IsEqualTo("throw new Error('fail');");
    }

    // ── Variable Declarations ───────────────────────────────────────────

    [Test]
    public async Task AddConst_Untyped()
    {
        var body = TsBodyBuilder.Empty().AddConst("x", "42");
        await Assert.That(body.Statements[0]).IsEqualTo("const x = 42;");
    }

    [Test]
    public async Task AddConst_Typed()
    {
        var body = TsBodyBuilder.Empty().AddConst("x", TsTypeRef.From("number"), "42");
        await Assert.That(body.Statements[0]).IsEqualTo("const x: number = 42;");
    }

    [Test]
    public async Task AddLet_Untyped()
    {
        var body = TsBodyBuilder.Empty().AddLet("x", "0");
        await Assert.That(body.Statements[0]).IsEqualTo("let x = 0;");
    }

    [Test]
    public async Task AddLet_Typed()
    {
        var body = TsBodyBuilder.Empty().AddLet("x", TsTypeRef.From("string"), "''");
        await Assert.That(body.Statements[0]).IsEqualTo("let x: string = '';");
    }

    // ── Control Flow ────────────────────────────────────────────────────

    [Test]
    public async Task AddIf()
    {
        var body = TsBodyBuilder.Empty()
            .AddIf("x > 0", b => b.AddReturn("x"));
        await Assert.That(body.Statements[0]).Contains("if (x > 0)");
        await Assert.That(body.Statements[0]).Contains("return x;");
    }

    [Test]
    public async Task AddIfElse()
    {
        var body = TsBodyBuilder.Empty()
            .AddIfElse("x > 0",
                b => b.AddReturn("x"),
                b => b.AddReturn("-x"));
        await Assert.That(body.Statements[0]).Contains("if (x > 0)");
        await Assert.That(body.Statements[0]).Contains("else");
        await Assert.That(body.Statements[0]).Contains("return -x;");
    }

    [Test]
    public async Task AddForOf()
    {
        var body = TsBodyBuilder.Empty()
            .AddForOf("item", "items", b => b.AddStatement("process(item)"));
        await Assert.That(body.Statements[0]).Contains("for (const item of items)");
        await Assert.That(body.Statements[0]).Contains("process(item);");
    }

    [Test]
    public async Task AddForIn()
    {
        var body = TsBodyBuilder.Empty()
            .AddForIn("key", "obj", b => b.AddStatement("console.log(key)"));
        await Assert.That(body.Statements[0]).Contains("for (const key in obj)");
    }

    [Test]
    public async Task AddTryCatch()
    {
        var body = TsBodyBuilder.Empty()
            .AddTryCatch(
                b => b.AddStatement("riskyOp()"),
                "err",
                b => b.AddStatement("console.error(err)"));
        await Assert.That(body.Statements[0]).Contains("try");
        await Assert.That(body.Statements[0]).Contains("catch (err)");
    }

    [Test]
    public async Task AddTryCatchFinally()
    {
        var body = TsBodyBuilder.Empty()
            .AddTryCatchFinally(
                b => b.AddStatement("open()"),
                "err",
                b => b.AddStatement("log(err)"),
                b => b.AddStatement("close()"));
        await Assert.That(body.Statements[0]).Contains("try");
        await Assert.That(body.Statements[0]).Contains("catch (err)");
        await Assert.That(body.Statements[0]).Contains("finally");
        await Assert.That(body.Statements[0]).Contains("close();");
    }

    // ── State ───────────────────────────────────────────────────────────

    [Test]
    public async Task IsEmpty_WhenNew() =>
        await Assert.That(TsBodyBuilder.Empty().IsEmpty).IsTrue();

    [Test]
    public async Task IsEmpty_FalseAfterStatement() =>
        await Assert.That(TsBodyBuilder.Empty().AddStatement("x").IsEmpty).IsFalse();

    // ── Immutability ────────────────────────────────────────────────────

    [Test]
    public async Task AddStatement_DoesNotMutate()
    {
        var original = TsBodyBuilder.Empty();
        _ = original.AddStatement("x");
        await Assert.That(original.IsEmpty).IsTrue();
    }
}
