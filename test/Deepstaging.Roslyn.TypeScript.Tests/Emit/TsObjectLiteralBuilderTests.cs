// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.TypeScript.Emit;

namespace Deepstaging.Roslyn.TypeScript.Tests.Emit;

public class TsObjectLiteralBuilderTests
{
    [Test]
    public async Task EmptyObject()
    {
        var result = TsObjectLiteralBuilder.Create().Build();
        await Assert.That(result.Value).IsEqualTo("{ }");
    }

    [Test]
    public async Task SingleProperty()
    {
        var result = TsObjectLiteralBuilder.Create()
            .AddProperty("name", "'Alice'")
            .Build();

        await Assert.That(result.Value).IsEqualTo("{ name: 'Alice' }");
    }

    [Test]
    public async Task MultipleProperties_SingleLine()
    {
        var result = TsObjectLiteralBuilder.Create()
            .AddProperty("name", "'Alice'")
            .AddProperty("age", "30")
            .Build();

        await Assert.That(result.Value).IsEqualTo("{ name: 'Alice', age: 30 }");
    }

    [Test]
    public async Task ThreeProperties_SingleLine()
    {
        var result = TsObjectLiteralBuilder.Create()
            .AddProperty("a", "1")
            .AddProperty("b", "2")
            .AddProperty("c", "3")
            .Build();

        await Assert.That(result.Value).IsEqualTo("{ a: 1, b: 2, c: 3 }");
    }

    [Test]
    public async Task FourProperties_MultiLine()
    {
        var result = TsObjectLiteralBuilder.Create()
            .AddProperty("a", "1")
            .AddProperty("b", "2")
            .AddProperty("c", "3")
            .AddProperty("d", "4")
            .Build();

        await Assert.That(result.Value).Contains("{\n");
        await Assert.That(result.Value).Contains("  a: 1,\n");
        await Assert.That(result.Value).Contains("  d: 4\n");
        await Assert.That(result.Value).Contains("}");
    }

    [Test]
    public async Task ShorthandProperty()
    {
        var result = TsObjectLiteralBuilder.Create()
            .AddShorthand("name")
            .AddShorthand("age")
            .Build();

        await Assert.That(result.Value).IsEqualTo("{ name, age }");
    }

    [Test]
    public async Task ComputedProperty()
    {
        var result = TsObjectLiteralBuilder.Create()
            .AddComputedProperty("key", "'value'")
            .Build();

        await Assert.That(result.Value).IsEqualTo("{ [key]: 'value' }");
    }

    [Test]
    public async Task SpreadEntry()
    {
        var result = TsObjectLiteralBuilder.Create()
            .AddSpread("defaults")
            .AddProperty("override", "'value'")
            .Build();

        await Assert.That(result.Value).IsEqualTo("{ ...defaults, override: 'value' }");
    }

    [Test]
    public async Task SpreadFromExpressionRef()
    {
        var result = TsObjectLiteralBuilder.Create()
            .AddSpread(TsExpressionRef.From("config"))
            .AddProperty("port", "3000")
            .Build();

        await Assert.That(result.Value).IsEqualTo("{ ...config, port: 3000 }");
    }

    [Test]
    public async Task PropertyFromExpressionRef()
    {
        var result = TsObjectLiteralBuilder.Create()
            .AddExpressionProperty("baseUrl", TsExpressionRef.From("config.url"))
            .Build();

        await Assert.That(result.Value).IsEqualTo("{ baseUrl: config.url }");
    }

    [Test]
    public async Task WithArrowFunctionValue()
    {
        var arrow = TsArrowFunctionBuilder.Create()
            .Async()
            .AddParameter("cmd", "CreateCommand")
            .WithExpressionBody("this.post('/api', cmd)")
            .Build();

        var result = TsObjectLiteralBuilder.Create()
            .AddProperty("create", arrow)
            .Build();

        await Assert.That(result.Value).Contains("create: async (cmd: CreateCommand) => this.post('/api', cmd)");
    }

    [Test]
    public async Task MethodShorthand()
    {
        var result = TsObjectLiteralBuilder.Create()
            .AddMethod("greet", m => m
                .AddParameter("name", "string")
                .WithReturnType("string")
                .WithExpressionBody("`Hello, ${name}`"))
            .Build();

        await Assert.That(result.Value).Contains("greet(name: string): string {");
        await Assert.That(result.Value).Contains("return `Hello, ${name}`;");
    }

    [Test]
    public async Task AsyncMethodShorthand()
    {
        var result = TsObjectLiteralBuilder.Create()
            .AddMethod("fetch", m => m
                .Async()
                .AddParameter("url", "string")
                .WithReturnType("Promise<Response>")
                .WithBody(b => b
                    .AddConst("response", "await fetch(url)")
                    .AddReturn("response")))
            .Build();

        await Assert.That(result.Value).Contains("async fetch(url: string): Promise<Response> {");
        await Assert.That(result.Value).Contains("const response = await fetch(url);");
    }

    [Test]
    public async Task ImplicitConversion_ToExpressionRef()
    {
        TsExpressionRef expr = TsObjectLiteralBuilder.Create()
            .AddProperty("x", "1");

        await Assert.That(expr.Value).IsEqualTo("{ x: 1 }");
    }

    [Test]
    public async Task ImplicitConversion_ToString()
    {
        string str = TsObjectLiteralBuilder.Create()
            .AddProperty("x", "1");

        await Assert.That(str).IsEqualTo("{ x: 1 }");
    }

    [Test]
    public void Throws_WhenPropertyNameEmpty()
    {
        Assert.Throws<System.ArgumentException>(() =>
            TsObjectLiteralBuilder.Create().AddProperty("", "value"));
    }

    [Test]
    public void Throws_WhenShorthandNameEmpty()
    {
        Assert.Throws<System.ArgumentException>(() =>
            TsObjectLiteralBuilder.Create().AddShorthand(""));
    }

    [Test]
    public async Task ComplexComposition_EntityGroup()
    {
        // This is the real-world pattern: entity group with CRUD arrow functions
        var result = TsObjectLiteralBuilder.Create()
            .AddProperty("create", TsArrowFunctionBuilder.Create()
                .Async()
                .AddParameter("cmd", "CreateOrderCommand")
                .WithExpressionBody("this.post<Order>('/api/orders', cmd)"))
            .AddProperty("list", TsArrowFunctionBuilder.Create()
                .Async()
                .WithExpressionBody("this.get<Order[]>('/api/orders')"))
            .AddProperty("get", TsArrowFunctionBuilder.Create()
                .Async()
                .AddParameter("id", "string")
                .WithExpressionBody("this.get<Order>(`/api/orders/${id}`)"))
            .AddProperty("delete", TsArrowFunctionBuilder.Create()
                .Async()
                .AddParameter("id", "string")
                .WithExpressionBody("this.delete(`/api/orders/${id}`)"))
            .Build();

        // Should be multi-line (>3 entries)
        await Assert.That(result.Value).Contains("{\n");
        await Assert.That(result.Value).Contains("create: async (cmd: CreateOrderCommand) => this.post<Order>('/api/orders', cmd)");
        await Assert.That(result.Value).Contains("list: async () => this.get<Order[]>('/api/orders')");
        await Assert.That(result.Value).Contains("get: async (id: string) => this.get<Order>(`/api/orders/${id}`)");
        await Assert.That(result.Value).Contains("delete: async (id: string) => this.delete(`/api/orders/${id}`)");
    }
}
