// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.TypeScript.Expressions;

namespace Deepstaging.Roslyn.TypeScript.Tests.Expressions;

public class TsExpressionFactoryTests
{
    // ── TsPromiseExpression ─────────────────────────────────────────────

    [Test]
    public async Task Promise_Resolve() =>
        await Assert.That((string)TsPromiseExpression.Resolve("value")).IsEqualTo("Promise.resolve(value)");

    [Test]
    public async Task Promise_Reject() =>
        await Assert.That((string)TsPromiseExpression.Reject("reason")).IsEqualTo("Promise.reject(reason)");

    [Test]
    public async Task Promise_All() =>
        await Assert.That((string)TsPromiseExpression.All("a", "b", "c")).IsEqualTo("Promise.all([a, b, c])");

    [Test]
    public async Task Promise_AllSettled() =>
        await Assert.That((string)TsPromiseExpression.AllSettled("a", "b")).IsEqualTo("Promise.allSettled([a, b])");

    [Test]
    public async Task Promise_Race() =>
        await Assert.That((string)TsPromiseExpression.Race("a", "b")).IsEqualTo("Promise.race([a, b])");

    [Test]
    public async Task Promise_Any() =>
        await Assert.That((string)TsPromiseExpression.Any("a", "b")).IsEqualTo("Promise.any([a, b])");

    [Test]
    public async Task Promise_New() =>
        await Assert.That((string)TsPromiseExpression.New("(resolve) => resolve(42)"))
            .IsEqualTo("new Promise((resolve) => resolve(42))");

    // ── TsJsonExpression ────────────────────────────────────────────────

    [Test]
    public async Task Json_Stringify() =>
        await Assert.That((string)TsJsonExpression.Stringify("data")).IsEqualTo("JSON.stringify(data)");

    [Test]
    public async Task Json_StringifyWithIndent() =>
        await Assert.That((string)TsJsonExpression.Stringify("data", 2)).IsEqualTo("JSON.stringify(data, null, 2)");

    [Test]
    public async Task Json_Parse() =>
        await Assert.That((string)TsJsonExpression.Parse("text")).IsEqualTo("JSON.parse(text)");

    [Test]
    public async Task Json_ParseAs() =>
        await Assert.That((string)TsJsonExpression.ParseAs("text", "User")).IsEqualTo("JSON.parse(text) as User");

    // ── TsFetchExpression ───────────────────────────────────────────────

    [Test]
    public async Task Fetch_Get() =>
        await Assert.That((string)TsFetchExpression.Get("url")).IsEqualTo("fetch(url)");

    [Test]
    public async Task Fetch_Post() =>
        await Assert.That((string)TsFetchExpression.Post("url", "body")).Contains("method: 'POST'");

    [Test]
    public async Task Fetch_Put() =>
        await Assert.That((string)TsFetchExpression.Put("url", "body")).Contains("method: 'PUT'");

    [Test]
    public async Task Fetch_Delete() =>
        await Assert.That((string)TsFetchExpression.Delete("url")).Contains("method: 'DELETE'");

    [Test]
    public async Task Fetch_ResponseJson() =>
        await Assert.That((string)TsFetchExpression.ResponseJson("response")).IsEqualTo("response.json()");

    [Test]
    public async Task Fetch_ResponseText() =>
        await Assert.That((string)TsFetchExpression.ResponseText("response")).IsEqualTo("response.text()");

    // ── TsConsoleExpression ─────────────────────────────────────────────

    [Test]
    public async Task Console_Log() =>
        await Assert.That((string)TsConsoleExpression.Log("msg")).IsEqualTo("console.log(msg)");

    [Test]
    public async Task Console_Error() =>
        await Assert.That((string)TsConsoleExpression.Error("msg")).IsEqualTo("console.error(msg)");

    [Test]
    public async Task Console_Warn() =>
        await Assert.That((string)TsConsoleExpression.Warn("msg")).IsEqualTo("console.warn(msg)");

    [Test]
    public async Task Console_Info() =>
        await Assert.That((string)TsConsoleExpression.Info("msg")).IsEqualTo("console.info(msg)");

    [Test]
    public async Task Console_Debug() =>
        await Assert.That((string)TsConsoleExpression.Debug("msg")).IsEqualTo("console.debug(msg)");

    [Test]
    public async Task Console_Table() =>
        await Assert.That((string)TsConsoleExpression.Table("data")).IsEqualTo("console.table(data)");

    // ── TsArrayExpression ───────────────────────────────────────────────

    [Test]
    public async Task Array_From() =>
        await Assert.That((string)TsArrayExpression.From("iterable")).IsEqualTo("Array.from(iterable)");

    [Test]
    public async Task Array_IsArray() =>
        await Assert.That((string)TsArrayExpression.IsArray("value")).IsEqualTo("Array.isArray(value)");

    [Test]
    public async Task Array_Spread() =>
        await Assert.That((string)TsArrayExpression.Spread("source")).IsEqualTo("[...source]");

    [Test]
    public async Task Array_Frozen() =>
        await Assert.That((string)TsArrayExpression.Frozen("arr")).IsEqualTo("Object.freeze(arr)");

    // ── TsObjectExpression ──────────────────────────────────────────────

    [Test]
    public async Task Object_Keys() =>
        await Assert.That((string)TsObjectExpression.Keys("obj")).IsEqualTo("Object.keys(obj)");

    [Test]
    public async Task Object_Values() =>
        await Assert.That((string)TsObjectExpression.Values("obj")).IsEqualTo("Object.values(obj)");

    [Test]
    public async Task Object_Entries() =>
        await Assert.That((string)TsObjectExpression.Entries("obj")).IsEqualTo("Object.entries(obj)");

    [Test]
    public async Task Object_Assign() =>
        await Assert.That((string)TsObjectExpression.Assign("target", "source")).IsEqualTo("Object.assign(target, source)");

    [Test]
    public async Task Object_Freeze() =>
        await Assert.That((string)TsObjectExpression.Freeze("obj")).IsEqualTo("Object.freeze(obj)");

    [Test]
    public async Task Object_Spread() =>
        await Assert.That((string)TsObjectExpression.Spread("source")).IsEqualTo("{ ...source }");

    [Test]
    public async Task Object_FromEntries() =>
        await Assert.That((string)TsObjectExpression.FromEntries("entries")).IsEqualTo("Object.fromEntries(entries)");

    // ── TsErrorExpression ───────────────────────────────────────────────

    [Test]
    public async Task Error_New() =>
        await Assert.That((string)TsErrorExpression.New("\"message\"")).IsEqualTo("new Error(\"message\")");

    [Test]
    public async Task Error_NewTypeError() =>
        await Assert.That((string)TsErrorExpression.NewTypeError("\"bad type\"")).IsEqualTo("new TypeError(\"bad type\")");

    [Test]
    public async Task Error_NewRangeError() =>
        await Assert.That((string)TsErrorExpression.NewRangeError("\"out of range\"")).IsEqualTo("new RangeError(\"out of range\")");

    [Test]
    public async Task Error_NewReferenceError() =>
        await Assert.That((string)TsErrorExpression.NewReferenceError("\"not defined\"")).IsEqualTo("new ReferenceError(\"not defined\")");
}
