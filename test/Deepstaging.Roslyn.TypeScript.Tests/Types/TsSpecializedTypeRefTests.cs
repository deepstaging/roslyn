// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.TypeScript.Types;

namespace Deepstaging.Roslyn.TypeScript.Tests.Types;

public class TsSpecializedTypeRefTests
{
    // ── TsPromiseTypeRef ────────────────────────────────────────────────

    [Test]
    public async Task Promise_ToString() =>
        await Assert.That(new TsPromiseTypeRef("string")).IsEqualTo("Promise<string>");

    [Test]
    public async Task Promise_CarriesResultType() =>
        await Assert.That(new TsPromiseTypeRef("number").ResultType).IsEqualTo("number");

    [Test]
    public async Task Promise_ImplicitToTsTypeRef()
    {
        TsTypeRef typeRef = new TsPromiseTypeRef("void");
        await Assert.That(typeRef).IsEqualTo("Promise<void>");
    }

    // ── TsArrayTypeRef ──────────────────────────────────────────────────

    [Test]
    public async Task Array_ToString() =>
        await Assert.That(new TsArrayTypeRef("number")).IsEqualTo("number[]");

    [Test]
    public async Task Array_CarriesElementType() =>
        await Assert.That(new TsArrayTypeRef("string").ElementType).IsEqualTo("string");

    // ── TsReadonlyArrayTypeRef ──────────────────────────────────────────

    [Test]
    public async Task ReadonlyArray_ToString() =>
        await Assert.That(new TsReadonlyArrayTypeRef("string")).IsEqualTo("readonly string[]");

    // ── TsMapTypeRef ────────────────────────────────────────────────────

    [Test]
    public async Task Map_ToString() =>
        await Assert.That(new TsMapTypeRef("string", "number")).IsEqualTo("Map<string, number>");

    [Test]
    public async Task Map_CarriesKeyAndValueTypes()
    {
        var map = new TsMapTypeRef("string", "boolean");
        await Assert.That(map.KeyType).IsEqualTo("string");
        await Assert.That(map.ValueType).IsEqualTo("boolean");
    }

    // ── TsSetTypeRef ────────────────────────────────────────────────────

    [Test]
    public async Task Set_ToString() =>
        await Assert.That(new TsSetTypeRef("number")).IsEqualTo("Set<number>");

    // ── TsRecordTypeRef ─────────────────────────────────────────────────

    [Test]
    public async Task Record_ToString() =>
        await Assert.That(new TsRecordTypeRef("string", "unknown")).IsEqualTo("Record<string, unknown>");

    // ── TsNullableTypeRef ───────────────────────────────────────────────

    [Test]
    public async Task Nullable_ToString() =>
        await Assert.That(new TsNullableTypeRef("string")).IsEqualTo("string | null");

    [Test]
    public async Task Nullable_CarriesInnerType() =>
        await Assert.That(new TsNullableTypeRef("number").InnerType).IsEqualTo("number");

    // ── TsFunctionTypeRef ───────────────────────────────────────────────

    [Test]
    public async Task Function_ToString()
    {
        var fn = new TsFunctionTypeRef(
            [(TsTypeRef.From("string"), "name"), (TsTypeRef.From("number"), "age")],
            "void");
        await Assert.That(fn.ToString()).IsEqualTo("(name: string, age: number) => void");
    }

    [Test]
    public async Task Function_NoParams()
    {
        var fn = new TsFunctionTypeRef([], "boolean");
        await Assert.That(fn.ToString()).IsEqualTo("() => boolean");
    }

    // ── Utility Types (TsUtilityTypeRefs.cs) ────────────────────────────

    [Test]
    public async Task Partial_ToString() =>
        await Assert.That(new TsPartialTypeRef("User")).IsEqualTo("Partial<User>");

    [Test]
    public async Task Partial_ImplicitToTsTypeRef()
    {
        TsTypeRef typeRef = new TsPartialTypeRef("Config");
        await Assert.That(typeRef).IsEqualTo("Partial<Config>");
    }
}
