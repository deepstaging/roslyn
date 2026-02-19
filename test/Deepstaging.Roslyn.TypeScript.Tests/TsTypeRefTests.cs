// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Tests;

public class TsTypeRefTests
{
    // ── Factories ───────────────────────────────────────────────────────

    [Test]
    public async Task From_CreatesTypeRef() =>
        await Assert.That(TsTypeRef.From("string")).IsEqualTo("string");

    [Test]
    public async Task CreateTypeRef_IsAliasForFrom() =>
        await Assert.That(TsTypeRef.CreateTypeRef("MyType")).IsEqualTo("MyType");

    [Test]
    public void From_ThrowsOnNull() =>
        Assert.Throws<ArgumentException>(() => TsTypeRef.From(""));

    [Test]
    public void From_ThrowsOnWhitespace() =>
        Assert.Throws<ArgumentException>(() => TsTypeRef.From("  "));

    // ── Generics ────────────────────────────────────────────────────────

    [Test]
    public async Task Of_SingleTypeArg() =>
        await Assert.That(TsTypeRef.From("Promise").Of(TsTypeRef.From("string"))).IsEqualTo("Promise<string>");

    [Test]
    public async Task Of_MultipleTypeArgs() =>
        await Assert.That(TsTypeRef.From("Map").Of(TsTypeRef.From("string"), TsTypeRef.From("number"))).IsEqualTo("Map<string, number>");

    // ── Type Operators ──────────────────────────────────────────────────

    [Test]
    public async Task Union_TwoTypes() =>
        await Assert.That(TsTypeRef.Union(TsTypeRef.From("string"), TsTypeRef.From("number"))).IsEqualTo("string | number");

    [Test]
    public async Task Union_ThreeTypes() =>
        await Assert.That(TsTypeRef.Union("string", "number", "boolean")).IsEqualTo("string | number | boolean");

    [Test]
    public async Task Intersection_TwoTypes() =>
        await Assert.That(TsTypeRef.Intersection("A", "B")).IsEqualTo("A & B");

    [Test]
    public async Task Tuple_Elements() =>
        await Assert.That(TsTypeRef.Tuple("string", "number")).IsEqualTo("[string, number]");

    [Test]
    public async Task NamedTuple_Elements() =>
        await Assert.That(TsTypeRef.NamedTuple(
            (TsTypeRef.From("string"), "name"),
            (TsTypeRef.From("number"), "age"))).IsEqualTo("[name: string, age: number]");

    [Test]
    public async Task Literal_StringValue() =>
        await Assert.That(TsTypeRef.Literal("success")).IsEqualTo("\"success\"");

    [Test]
    public async Task NumericLiteral() =>
        await Assert.That(TsTypeRef.NumericLiteral("42")).IsEqualTo("42");

    [Test]
    public async Task TemplateLiteral() =>
        await Assert.That(TsTypeRef.TemplateLiteral("`prefix-${string}`")).IsEqualTo("`prefix-${string}`");

    // ── Modifiers ───────────────────────────────────────────────────────

    [Test]
    public async Task Array_Modifier() =>
        await Assert.That(TsTypeRef.From("string").Array()).IsEqualTo("string[]");

    [Test]
    public async Task Nullable_Modifier() =>
        await Assert.That(TsTypeRef.From("string").Nullable()).IsEqualTo("string | null");

    [Test]
    public async Task Optional_Modifier() =>
        await Assert.That(TsTypeRef.From("string").Optional()).IsEqualTo("string | undefined");

    [Test]
    public async Task NullableOptional_Modifier() =>
        await Assert.That(TsTypeRef.From("string").NullableOptional()).IsEqualTo("string | null | undefined");

    [Test]
    public async Task Readonly_Modifier() =>
        await Assert.That(TsTypeRef.From("string[]").Readonly()).IsEqualTo("readonly string[]");

    [Test]
    public async Task Parenthesize_Modifier() =>
        await Assert.That(TsTypeRef.From("A | B").Parenthesize()).IsEqualTo("(A | B)");

    // ── Type Operators ──────────────────────────────────────────────────

    [Test]
    public async Task KeyOf_Operator() =>
        await Assert.That(TsTypeRef.From("User").KeyOf()).IsEqualTo("keyof User");

    [Test]
    public async Task TypeOf_Operator() =>
        await Assert.That(TsTypeRef.From("myVar").TypeOf()).IsEqualTo("typeof myVar");

    // ── Expression Gateways ─────────────────────────────────────────────

    [Test]
    public async Task New_Gateway() =>
        await Assert.That(TsTypeRef.From("Error").New("\"message\"")).IsEqualTo("new Error(\"message\")");

    [Test]
    public async Task Member_Gateway() =>
        await Assert.That(TsTypeRef.From("Math").Member("PI")).IsEqualTo("Math.PI");

    [Test]
    public async Task Call_Gateway() =>
        await Assert.That(TsTypeRef.From("Math").Call("max", "a", "b")).IsEqualTo("Math.max(a, b)");

    // ── Conversions ─────────────────────────────────────────────────────

    [Test]
    public async Task ImplicitToString()
    {
        string value = TsTypeRef.From("number");
        await Assert.That(value).IsEqualTo("number");
    }

    [Test]
    public async Task ImplicitFromString()
    {
        TsTypeRef typeRef = "boolean";
        await Assert.That(typeRef).IsEqualTo("boolean");
    }

    // ── Chaining ────────────────────────────────────────────────────────

    [Test]
    public async Task Chained_Union_Array_Nullable() =>
        await Assert.That(TsTypeRef.Union("string", "number").Parenthesize().Array().Nullable())
            .IsEqualTo("(string | number)[] | null");
}
