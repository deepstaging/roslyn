// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Tests;

public class TsExpressionRefTests
{
    // ── Factories ───────────────────────────────────────────────────────

    [Test]
    public async Task From_CreatesExpression() =>
        await Assert.That(TsExpressionRef.From("value")).IsEqualTo("value");

    [Test]
    public async Task CreateExpressionRef_IsAlias() =>
        await Assert.That(TsExpressionRef.CreateExpressionRef("x")).IsEqualTo("x");

    [Test]
    public void From_ThrowsOnEmpty() =>
        Assert.Throws<ArgumentException>(() => TsExpressionRef.From(""));

    // ── Method Calls ────────────────────────────────────────────────────

    [Test]
    public async Task Call_NoArgs() =>
        await Assert.That(TsExpressionRef.From("items").Call("length")).IsEqualTo("items.length()");

    [Test]
    public async Task Call_WithArgs() =>
        await Assert.That(TsExpressionRef.From("items").Call("filter", "x => x > 0")).IsEqualTo("items.filter(x => x > 0)");

    [Test]
    public async Task Call_Chained() =>
        await Assert.That(TsExpressionRef.From("arr").Call("filter", "x => x").Call("map", "x => x * 2"))
            .IsEqualTo("arr.filter(x => x).map(x => x * 2)");

    [Test]
    public async Task Invoke_NoArgs() =>
        await Assert.That(TsExpressionRef.From("fn").Invoke()).IsEqualTo("fn()");

    [Test]
    public async Task Invoke_WithArgs() =>
        await Assert.That(TsExpressionRef.From("fetch").Invoke("url")).IsEqualTo("fetch(url)");

    // ── Member Access ───────────────────────────────────────────────────

    [Test]
    public async Task Member_Access() =>
        await Assert.That(TsExpressionRef.From("user").Member("name")).IsEqualTo("user.name");

    [Test]
    public async Task Index_Access() =>
        await Assert.That(TsExpressionRef.From("items").Index("0")).IsEqualTo("items[0]");

    // ── Optional Chaining ───────────────────────────────────────────────

    [Test]
    public async Task OptionalChain() =>
        await Assert.That(TsExpressionRef.From("user").OptionalChain("address")).IsEqualTo("user?.address");

    [Test]
    public async Task OptionalChain_Deep() =>
        await Assert.That(TsExpressionRef.From("user").OptionalChain("address").OptionalChain("city"))
            .IsEqualTo("user?.address?.city");

    [Test]
    public async Task OptionalCall() =>
        await Assert.That(TsExpressionRef.From("handler").OptionalCall("process", "data"))
            .IsEqualTo("handler?.process(data)");

    [Test]
    public async Task OptionalIndex() =>
        await Assert.That(TsExpressionRef.From("items").OptionalIndex("0")).IsEqualTo("items?.[0]");

    // ── Nullish Coalescing ──────────────────────────────────────────────

    [Test]
    public async Task NullishCoalesce() =>
        await Assert.That(TsExpressionRef.From("value").NullishCoalesce("\"default\""))
            .IsEqualTo("value ?? \"default\"");

    [Test]
    public async Task NonNullAssertion() =>
        await Assert.That(TsExpressionRef.From("el").NonNullAssertion()).IsEqualTo("el!");

    // ── Type Assertions ─────────────────────────────────────────────────

    [Test]
    public async Task As_TypeAssertion() =>
        await Assert.That(TsExpressionRef.From("value").As("string")).IsEqualTo("value as string");

    [Test]
    public async Task Satisfies_Check() =>
        await Assert.That(TsExpressionRef.From("config").Satisfies("Config")).IsEqualTo("config satisfies Config");

    // ── Type Checks ─────────────────────────────────────────────────────

    [Test]
    public async Task TypeOf_Check() =>
        await Assert.That(TsExpressionRef.From("value").TypeOf()).IsEqualTo("typeof value");

    [Test]
    public async Task InstanceOf_Check() =>
        await Assert.That(TsExpressionRef.From("err").InstanceOf("Error")).IsEqualTo("err instanceof Error");

    // ── Async / Spread / Template ───────────────────────────────────────

    [Test]
    public async Task Await_Expression() =>
        await Assert.That(TsExpressionRef.From("fetch").Invoke("url").Await()).IsEqualTo("await fetch(url)");

    [Test]
    public async Task Spread_Expression() =>
        await Assert.That(TsExpressionRef.From("args").Spread()).IsEqualTo("...args");

    [Test]
    public async Task TemplateLiteral_WithPrefixSuffix() =>
        await Assert.That(TsExpressionRef.From("name").TemplateLiteral("Hello, ", "!"))
            .IsEqualTo("`Hello, ${name}!`");

    // ── Logical / Comparison ────────────────────────────────────────────

    [Test]
    public async Task StrictEquals() =>
        await Assert.That(TsExpressionRef.From("a").StrictEquals("b")).IsEqualTo("a === b");

    [Test]
    public async Task StrictNotEquals() =>
        await Assert.That(TsExpressionRef.From("a").StrictNotEquals("b")).IsEqualTo("a !== b");

    [Test]
    public async Task And_Operator() =>
        await Assert.That(TsExpressionRef.From("a").And("b")).IsEqualTo("a && b");

    [Test]
    public async Task Or_Operator() =>
        await Assert.That(TsExpressionRef.From("a").Or("b")).IsEqualTo("a || b");

    [Test]
    public async Task Not_Operator() =>
        await Assert.That(TsExpressionRef.From("valid").Not()).IsEqualTo("!valid");

    [Test]
    public async Task Parenthesize() =>
        await Assert.That(TsExpressionRef.From("a + b").Parenthesize()).IsEqualTo("(a + b)");

    // ── Conversions ─────────────────────────────────────────────────────

    [Test]
    public async Task ImplicitToString()
    {
        string value = TsExpressionRef.From("x");
        await Assert.That(value).IsEqualTo("x");
    }

    [Test]
    public async Task ImplicitFromString()
    {
        TsExpressionRef expr = "myVar";
        await Assert.That(expr).IsEqualTo("myVar");
    }

    [Test]
    public async Task ImplicitFromTypeRef()
    {
        TsExpressionRef expr = TsTypeRef.From("Math");
        await Assert.That(expr).IsEqualTo("Math");
    }
}
