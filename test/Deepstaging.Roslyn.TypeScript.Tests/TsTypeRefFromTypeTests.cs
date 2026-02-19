// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deepstaging.Roslyn.TypeScript.Tests;

public class TsTypeRefFromTypeTests
{
    // ── Primitives ──────────────────────────────────────────────────────

    [Test]
    public async Task Maps_String() =>
        await Assert.That(TsTypeRef.From(typeof(string))).IsEqualTo("string");

    [Test]
    public async Task Maps_Bool() =>
        await Assert.That(TsTypeRef.From(typeof(bool))).IsEqualTo("boolean");

    [Test]
    public async Task Maps_Int() =>
        await Assert.That(TsTypeRef.From(typeof(int))).IsEqualTo("number");

    [Test]
    public async Task Maps_Long() =>
        await Assert.That(TsTypeRef.From(typeof(long))).IsEqualTo("number");

    [Test]
    public async Task Maps_Double() =>
        await Assert.That(TsTypeRef.From(typeof(double))).IsEqualTo("number");

    [Test]
    public async Task Maps_Decimal() =>
        await Assert.That(TsTypeRef.From(typeof(decimal))).IsEqualTo("number");

    [Test]
    public async Task Maps_Float() =>
        await Assert.That(TsTypeRef.From(typeof(float))).IsEqualTo("number");

    [Test]
    public async Task Maps_Byte() =>
        await Assert.That(TsTypeRef.From(typeof(byte))).IsEqualTo("number");

    [Test]
    public async Task Maps_Char() =>
        await Assert.That(TsTypeRef.From(typeof(char))).IsEqualTo("string");

    [Test]
    public async Task Maps_Void() =>
        await Assert.That(TsTypeRef.From(typeof(void))).IsEqualTo("void");

    // ── Well-known BCL types ────────────────────────────────────────────

    [Test]
    public async Task Maps_DateTime() =>
        await Assert.That(TsTypeRef.From(typeof(DateTime))).IsEqualTo("Date");

    [Test]
    public async Task Maps_DateTimeOffset() =>
        await Assert.That(TsTypeRef.From(typeof(DateTimeOffset))).IsEqualTo("Date");

    [Test]
    public async Task Maps_Guid() =>
        await Assert.That(TsTypeRef.From(typeof(Guid))).IsEqualTo("string");

    [Test]
    public async Task Maps_Uri() =>
        await Assert.That(TsTypeRef.From(typeof(Uri))).IsEqualTo("string");

    [Test]
    public async Task Maps_Object() =>
        await Assert.That(TsTypeRef.From(typeof(object))).IsEqualTo("unknown");

    [Test]
    public async Task Maps_TimeSpan() =>
        await Assert.That(TsTypeRef.From(typeof(TimeSpan))).IsEqualTo("number");

    // ── Nullable<T> ─────────────────────────────────────────────────────

    [Test]
    public async Task Maps_NullableInt() =>
        await Assert.That(TsTypeRef.From(typeof(int?))).IsEqualTo("number | null");

    [Test]
    public async Task Maps_NullableBool() =>
        await Assert.That(TsTypeRef.From(typeof(bool?))).IsEqualTo("boolean | null");

    [Test]
    public async Task Maps_NullableDateTime() =>
        await Assert.That(TsTypeRef.From(typeof(DateTime?))).IsEqualTo("Date | null");

    // ── Arrays ──────────────────────────────────────────────────────────

    [Test]
    public async Task Maps_StringArray() =>
        await Assert.That(TsTypeRef.From(typeof(string[]))).IsEqualTo("string[]");

    [Test]
    public async Task Maps_IntArray() =>
        await Assert.That(TsTypeRef.From(typeof(int[]))).IsEqualTo("number[]");

    [Test]
    public async Task Maps_ByteArray() =>
        await Assert.That(TsTypeRef.From(typeof(byte[]))).IsEqualTo("Uint8Array");

    // ── Task / Promise ──────────────────────────────────────────────────

    [Test]
    public async Task Maps_Task() =>
        await Assert.That(TsTypeRef.From(typeof(Task))).IsEqualTo("Promise<void>");

    [Test]
    public async Task Maps_TaskOfString() =>
        await Assert.That(TsTypeRef.From(typeof(Task<string>))).IsEqualTo("Promise<string>");

    [Test]
    public async Task Maps_TaskOfInt() =>
        await Assert.That(TsTypeRef.From(typeof(Task<int>))).IsEqualTo("Promise<number>");

    // ── Collections ─────────────────────────────────────────────────────

    [Test]
    public async Task Maps_ListOfString() =>
        await Assert.That(TsTypeRef.From(typeof(List<string>))).IsEqualTo("string[]");

    [Test]
    public async Task Maps_IEnumerableOfInt() =>
        await Assert.That(TsTypeRef.From(typeof(IEnumerable<int>))).IsEqualTo("number[]");

    [Test]
    public async Task Maps_IListOfBool() =>
        await Assert.That(TsTypeRef.From(typeof(IList<bool>))).IsEqualTo("boolean[]");

    [Test]
    public async Task Maps_IReadOnlyListOfString() =>
        await Assert.That(TsTypeRef.From(typeof(IReadOnlyList<string>))).IsEqualTo("string[]");

    // ── Dictionary / Record ─────────────────────────────────────────────

    [Test]
    public async Task Maps_DictionaryStringInt() =>
        await Assert.That(TsTypeRef.From(typeof(Dictionary<string, int>))).IsEqualTo("Record<string, number>");

    [Test]
    public async Task Maps_IDictionaryStringBool() =>
        await Assert.That(TsTypeRef.From(typeof(IDictionary<string, bool>))).IsEqualTo("Record<string, boolean>");

    [Test]
    public async Task Maps_IReadOnlyDictionaryGuidString() =>
        await Assert.That(TsTypeRef.From(typeof(IReadOnlyDictionary<Guid, string>))).IsEqualTo("Record<string, string>");

    // ── Set ─────────────────────────────────────────────────────────────

    [Test]
    public async Task Maps_HashSetOfString() =>
        await Assert.That(TsTypeRef.From(typeof(HashSet<string>))).IsEqualTo("Set<string>");

    [Test]
    public async Task Maps_ISetOfInt() =>
        await Assert.That(TsTypeRef.From(typeof(ISet<int>))).IsEqualTo("Set<number>");

    // ── KeyValuePair ────────────────────────────────────────────────────

    [Test]
    public async Task Maps_KeyValuePair() =>
        await Assert.That(TsTypeRef.From(typeof(KeyValuePair<string, int>))).IsEqualTo("[string, number]");

    // ── Nested generics ─────────────────────────────────────────────────

    [Test]
    public async Task Maps_TaskOfListOfString() =>
        await Assert.That(TsTypeRef.From(typeof(Task<List<string>>))).IsEqualTo("Promise<string[]>");

    [Test]
    public async Task Maps_DictionaryStringListOfInt() =>
        await Assert.That(TsTypeRef.From(typeof(Dictionary<string, List<int>>))).IsEqualTo("Record<string, number[]>");

    // ── Unknown type fallback ───────────────────────────────────────────

    [Test]
    public async Task Maps_UnknownType_UsesTypeName() =>
        await Assert.That(TsTypeRef.From(typeof(System.Text.StringBuilder))).IsEqualTo("StringBuilder");
}
