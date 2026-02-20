// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.TypeScript.Emit;

namespace Deepstaging.Roslyn.TypeScript.Tests.Emit;

public class TsArrowFunctionBuilderTests
{
    [Test]
    public async Task ExpressionBody_NoParams()
    {
        var result = TsArrowFunctionBuilder.Create()
            .WithExpressionBody("Date.now()")
            .Build();

        await Assert.That(result.Value).IsEqualTo("() => Date.now()");
    }

    [Test]
    public async Task ExpressionBody_SingleParam()
    {
        var result = TsArrowFunctionBuilder.Create()
            .AddParameter("x", "number")
            .WithExpressionBody("x * 2")
            .Build();

        await Assert.That(result.Value).IsEqualTo("(x: number) => x * 2");
    }

    [Test]
    public async Task ExpressionBody_MultipleParams()
    {
        var result = TsArrowFunctionBuilder.Create()
            .AddParameter("a", "number")
            .AddParameter("b", "number")
            .WithExpressionBody("a + b")
            .Build();

        await Assert.That(result.Value).IsEqualTo("(a: number, b: number) => a + b");
    }

    [Test]
    public async Task Async_ExpressionBody()
    {
        var result = TsArrowFunctionBuilder.Create()
            .Async()
            .AddParameter("url", "string")
            .WithExpressionBody("fetch(url)")
            .Build();

        await Assert.That(result.Value).IsEqualTo("async (url: string) => fetch(url)");
    }

    [Test]
    public async Task BlockBody()
    {
        var result = TsArrowFunctionBuilder.Create()
            .AddParameter("x", "number")
            .WithBody(b => b
                .AddConst("doubled", "x * 2")
                .AddReturn("doubled"))
            .Build();

        await Assert.That(result.Value).Contains("(x: number) => {");
        await Assert.That(result.Value).Contains("const doubled = x * 2;");
        await Assert.That(result.Value).Contains("return doubled;");
    }

    [Test]
    public async Task Async_BlockBody()
    {
        var result = TsArrowFunctionBuilder.Create()
            .Async()
            .AddParameter("cmd", "CreateCommand")
            .WithBody(b => b
                .AddConst("response", "await fetch('/api', { method: 'POST', body: JSON.stringify(cmd) })")
                .AddReturn("response.json()"))
            .Build();

        await Assert.That(result.Value).StartsWith("async (cmd: CreateCommand) => {");
        await Assert.That(result.Value).Contains("const response");
        await Assert.That(result.Value).Contains("return response.json();");
    }

    [Test]
    public async Task WithReturnType()
    {
        var result = TsArrowFunctionBuilder.Create()
            .AddParameter("x", "number")
            .WithReturnType("number")
            .WithExpressionBody("x * 2")
            .Build();

        await Assert.That(result.Value).IsEqualTo("(x: number): number => x * 2");
    }

    [Test]
    public async Task WithTypeParameters()
    {
        var result = TsArrowFunctionBuilder.Create()
            .AddTypeParameter("T")
            .AddParameter("items", "T[]")
            .WithReturnType("T")
            .WithExpressionBody("items[0]")
            .Build();

        await Assert.That(result.Value).IsEqualTo("<T>(items: T[]): T => items[0]");
    }

    [Test]
    public async Task OptionalParameter()
    {
        var result = TsArrowFunctionBuilder.Create()
            .AddParameter("name", "string", p => p.AsOptional())
            .WithExpressionBody("name ?? 'default'")
            .Build();

        await Assert.That(result.Value).IsEqualTo("(name?: string) => name ?? 'default'");
    }

    [Test]
    public async Task DefaultValueParameter()
    {
        var result = TsArrowFunctionBuilder.Create()
            .AddParameter("count", "number", p => p.WithDefaultValue("10"))
            .WithExpressionBody("count")
            .Build();

        await Assert.That(result.Value).IsEqualTo("(count: number = 10) => count");
    }

    [Test]
    public async Task RestParameter()
    {
        var result = TsArrowFunctionBuilder.Create()
            .AddParameter("args", "string[]", p => p.AsRest())
            .WithExpressionBody("args.join(', ')")
            .Build();

        await Assert.That(result.Value).IsEqualTo("(...args: string[]) => args.join(', ')");
    }

    [Test]
    public void Throws_WhenNoBody()
    {
        Assert.Throws<System.InvalidOperationException>(() =>
            TsArrowFunctionBuilder.Create()
                .AddParameter("x", "number")
                .Build());
    }

    [Test]
    public async Task ImplicitConversion_ToExpressionRef()
    {
        TsExpressionRef expr = TsArrowFunctionBuilder.Create()
            .AddParameter("x", "number")
            .WithExpressionBody("x + 1");

        await Assert.That(expr.Value).IsEqualTo("(x: number) => x + 1");
    }

    [Test]
    public async Task ImplicitConversion_ToString()
    {
        string str = TsArrowFunctionBuilder.Create()
            .AddParameter("x", "number")
            .WithExpressionBody("x + 1");

        await Assert.That(str).IsEqualTo("(x: number) => x + 1");
    }

    [Test]
    public async Task UsedAsFieldInitializer()
    {
        var arrow = TsArrowFunctionBuilder.Create()
            .AddParameter("x", "number")
            .WithExpressionBody("x * 2")
            .Build();

        var field = TsFieldBuilder.For("transform", "(x: number) => number")
            .AsReadonly()
            .WithInitializer(arrow);

        await Assert.That(field.Initializer).IsEqualTo("(x: number) => x * 2");
    }

    [Test]
    public async Task ExpressionBodyFromTsExpressionRef()
    {
        var fetchExpr = TsExpressionRef.From("fetch").Invoke("url");
        var result = TsArrowFunctionBuilder.Create()
            .Async()
            .AddParameter("url", "string")
            .WithExpressionBody(fetchExpr)
            .Build();

        await Assert.That(result.Value).IsEqualTo("async (url: string) => fetch(url)");
    }
}
