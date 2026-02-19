// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Collections.Immutable;
using System.Linq;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Fluent builder for composing TypeScript method/function body statements.
/// Immutable — each method returns a new instance via <c>with</c> expressions.
/// Statements are accumulated as raw strings and emitted as a TypeScript block.
/// </summary>
/// <example>
/// <code>
/// TsBodyBuilder.Empty()
///     .AddStatement("const result = items.filter(x => x > 0)")
///     .AddReturn("result.length")
/// </code>
/// </example>
public readonly record struct TsBodyBuilder
{
    /// <summary>Gets the accumulated statements.</summary>
    public ImmutableArray<string> Statements { get; init; }

    /// <summary>Initializes a new empty body builder.</summary>
    public TsBodyBuilder()
    {
        Statements = ImmutableArray<string>.Empty;
    }

    /// <summary>Creates an empty body builder.</summary>
    public static TsBodyBuilder Empty() => new();

    // ── Statements ──────────────────────────────────────────────────────

    /// <summary>Adds a single statement to the body.</summary>
    /// <param name="statement">The TypeScript statement (semicolons are added if missing).</param>
    public TsBodyBuilder AddStatement(string statement)
    {
        if (string.IsNullOrWhiteSpace(statement))
            throw new ArgumentException("Statement cannot be null or whitespace.", nameof(statement));

        return this with { Statements = Statements.Add(NormalizeSemicolon(statement)) };
    }

    /// <summary>Adds multiple statements to the body.</summary>
    /// <param name="statements">The TypeScript statements to add.</param>
    public TsBodyBuilder AddStatements(params string[] statements) =>
        statements.Aggregate(this, (builder, stmt) => builder.AddStatement(stmt));

    // ── Returns ─────────────────────────────────────────────────────────

    /// <summary>Adds a return statement: <c>return expression;</c>.</summary>
    /// <param name="expression">The expression to return.</param>
    public TsBodyBuilder AddReturn(string expression) =>
        this with { Statements = Statements.Add($"return {expression};") };

    /// <summary>Adds an empty return statement: <c>return;</c>.</summary>
    public TsBodyBuilder AddReturn() =>
        this with { Statements = Statements.Add("return;") };

    // ── Throws ──────────────────────────────────────────────────────────

    /// <summary>Adds a throw statement: <c>throw expression;</c>.</summary>
    /// <param name="expression">The expression to throw.</param>
    public TsBodyBuilder AddThrow(string expression) =>
        this with { Statements = Statements.Add($"throw {expression};") };

    // ── Control Flow ────────────────────────────────────────────────────

    /// <summary>Adds an if block.</summary>
    /// <param name="condition">The condition expression.</param>
    /// <param name="configureBody">Builder for the if block body.</param>
    public TsBodyBuilder AddIf(string condition, Func<TsBodyBuilder, TsBodyBuilder> configureBody)
    {
        var body = configureBody(Empty());
        var block = FormatBlock(body);
        return this with { Statements = Statements.Add($"if ({condition}) {block}") };
    }

    /// <summary>Adds an if/else block.</summary>
    /// <param name="condition">The condition expression.</param>
    /// <param name="configureThen">Builder for the if block body.</param>
    /// <param name="configureElse">Builder for the else block body.</param>
    public TsBodyBuilder AddIfElse(
        string condition,
        Func<TsBodyBuilder, TsBodyBuilder> configureThen,
        Func<TsBodyBuilder, TsBodyBuilder> configureElse)
    {
        var thenBody = configureThen(Empty());
        var elseBody = configureElse(Empty());
        var thenBlock = FormatBlock(thenBody);
        var elseBlock = FormatBlock(elseBody);
        return this with { Statements = Statements.Add($"if ({condition}) {thenBlock} else {elseBlock}") };
    }

    /// <summary>Adds a for...of loop.</summary>
    /// <param name="variable">The loop variable name.</param>
    /// <param name="iterable">The iterable expression.</param>
    /// <param name="configureBody">Builder for the loop body.</param>
    public TsBodyBuilder AddForOf(
        string variable,
        string iterable,
        Func<TsBodyBuilder, TsBodyBuilder> configureBody)
    {
        var body = configureBody(Empty());
        var block = FormatBlock(body);
        return this with { Statements = Statements.Add($"for (const {variable} of {iterable}) {block}") };
    }

    /// <summary>Adds a for...in loop.</summary>
    /// <param name="variable">The loop variable name.</param>
    /// <param name="obj">The object expression.</param>
    /// <param name="configureBody">Builder for the loop body.</param>
    public TsBodyBuilder AddForIn(
        string variable,
        string obj,
        Func<TsBodyBuilder, TsBodyBuilder> configureBody)
    {
        var body = configureBody(Empty());
        var block = FormatBlock(body);
        return this with { Statements = Statements.Add($"for (const {variable} in {obj}) {block}") };
    }

    /// <summary>Adds a try/catch block.</summary>
    /// <param name="configureTry">Builder for the try block body.</param>
    /// <param name="errorVariable">The catch error variable name.</param>
    /// <param name="configureCatch">Builder for the catch block body.</param>
    public TsBodyBuilder AddTryCatch(
        Func<TsBodyBuilder, TsBodyBuilder> configureTry,
        string errorVariable,
        Func<TsBodyBuilder, TsBodyBuilder> configureCatch)
    {
        var tryBody = configureTry(Empty());
        var catchBody = configureCatch(Empty());
        var tryBlock = FormatBlock(tryBody);
        var catchBlock = FormatBlock(catchBody);
        return this with { Statements = Statements.Add($"try {tryBlock} catch ({errorVariable}) {catchBlock}") };
    }

    /// <summary>Adds a try/catch/finally block.</summary>
    /// <param name="configureTry">Builder for the try block body.</param>
    /// <param name="errorVariable">The catch error variable name.</param>
    /// <param name="configureCatch">Builder for the catch block body.</param>
    /// <param name="configureFinally">Builder for the finally block body.</param>
    public TsBodyBuilder AddTryCatchFinally(
        Func<TsBodyBuilder, TsBodyBuilder> configureTry,
        string errorVariable,
        Func<TsBodyBuilder, TsBodyBuilder> configureCatch,
        Func<TsBodyBuilder, TsBodyBuilder> configureFinally)
    {
        var tryBody = configureTry(Empty());
        var catchBody = configureCatch(Empty());
        var finallyBody = configureFinally(Empty());
        var tryBlock = FormatBlock(tryBody);
        var catchBlock = FormatBlock(catchBody);
        var finallyBlock = FormatBlock(finallyBody);
        return this with
        {
            Statements = Statements.Add(
                $"try {tryBlock} catch ({errorVariable}) {catchBlock} finally {finallyBlock}")
        };
    }

    // ── Variable Declarations ───────────────────────────────────────────

    /// <summary>Adds a const declaration: <c>const name = expression;</c>.</summary>
    /// <param name="name">The variable name.</param>
    /// <param name="expression">The initializer expression.</param>
    public TsBodyBuilder AddConst(string name, string expression) =>
        this with { Statements = Statements.Add($"const {name} = {expression};") };

    /// <summary>Adds a typed const declaration: <c>const name: type = expression;</c>.</summary>
    /// <param name="name">The variable name.</param>
    /// <param name="type">The type annotation.</param>
    /// <param name="expression">The initializer expression.</param>
    public TsBodyBuilder AddConst(string name, TsTypeRef type, string expression) =>
        this with { Statements = Statements.Add($"const {name}: {type.Value} = {expression};") };

    /// <summary>Adds a let declaration: <c>let name = expression;</c>.</summary>
    /// <param name="name">The variable name.</param>
    /// <param name="expression">The initializer expression.</param>
    public TsBodyBuilder AddLet(string name, string expression) =>
        this with { Statements = Statements.Add($"let {name} = {expression};") };

    /// <summary>Adds a typed let declaration: <c>let name: type = expression;</c>.</summary>
    /// <param name="name">The variable name.</param>
    /// <param name="type">The type annotation.</param>
    /// <param name="expression">The initializer expression.</param>
    public TsBodyBuilder AddLet(string name, TsTypeRef type, string expression) =>
        this with { Statements = Statements.Add($"let {name}: {type.Value} = {expression};") };

    // ── State ───────────────────────────────────────────────────────────

    /// <summary>Returns <c>true</c> if this body contains no statements.</summary>
    public readonly bool IsEmpty => Statements.IsDefault || Statements.IsEmpty;

    // ── Formatting ──────────────────────────────────────────────────────

    private static string NormalizeSemicolon(string statement)
    {
        var trimmed = statement.TrimEnd();
        if (trimmed.EndsWith(";", StringComparison.Ordinal) ||
            trimmed.EndsWith("}", StringComparison.Ordinal) ||
            trimmed.EndsWith("{", StringComparison.Ordinal))
            return trimmed;

        return trimmed + ";";
    }

    private static string FormatBlock(TsBodyBuilder body)
    {
        if (body.IsEmpty)
            return "{ }";

        var lines = string.Join("\n  ", body.Statements);
        return $"{{\n  {lines}\n}}";
    }
}
