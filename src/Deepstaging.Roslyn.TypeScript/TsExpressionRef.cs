// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Linq;

namespace Deepstaging.Roslyn.TypeScript;

/// <summary>
/// Fluent builder for composing TypeScript expression strings.
/// Represents a value-position expression — anything valid on the right side of an assignment.
/// Immutable — each method returns a new instance.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="TsExpressionRef"/> is the expression-domain counterpart to <see cref="TsTypeRef"/>.
/// A <see cref="TsTypeRef"/> crosses into expression domain via gateway methods like
/// <see cref="TsTypeRef.New(TsExpressionRef[])"/>, <see cref="TsTypeRef.Call(string, TsExpressionRef[])"/>,
/// and <see cref="TsTypeRef.Member(string)"/>. Once in expression domain, chaining continues
/// through <see cref="TsExpressionRef"/> methods.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Method call chain
/// TsExpressionRef.From("items").Call("filter", "x => x > 0").Call("map", "x => x * 2")
///     // "items.filter(x => x > 0).map(x => x * 2)"
///
/// // Optional chaining with nullish coalescing
/// TsExpressionRef.From("user").OptionalChain("address").OptionalChain("city").NullishCoalesce("\"unknown\"")
///     // "user?.address?.city ?? \"unknown\""
///
/// // Type assertion
/// TsExpressionRef.From("value").As(TsTypeRef.From("string"))
///     // "value as string"
///
/// // Await
/// TsExpressionRef.From("fetch").Invoke("url").Await()
///     // "await fetch(url)"
/// </code>
/// </example>
public readonly record struct TsExpressionRef
{
    /// <summary>Gets the string representation of the expression.</summary>
    public string Value { get; }

    private TsExpressionRef(string value) => Value = value;

    // ── Factories ───────────────────────────────────────────────────────

    /// <summary>Creates an expression reference from a raw expression string.</summary>
    /// <param name="expression">The expression string (e.g., "value", "x + y").</param>
    public static TsExpressionRef From(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new ArgumentException("Expression cannot be null or whitespace.", nameof(expression));

        return new TsExpressionRef(expression);
    }

    /// <summary>Creates an expression reference from a raw expression string.</summary>
    /// <param name="expression">The expression string.</param>
    /// <remarks>Semantic alias for <see cref="From(string)"/> that reads naturally when <c>using static TsExpressionRef</c> is in scope.</remarks>
    public static TsExpressionRef CreateExpressionRef(string expression) => From(expression);

    // ── Method Calls ────────────────────────────────────────────────────

    /// <summary>Produces a method call expression: <c>expr.method(args)</c>.</summary>
    /// <param name="method">The method name.</param>
    /// <param name="arguments">The arguments to pass.</param>
    public TsExpressionRef Call(string method, params TsExpressionRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return new TsExpressionRef($"{Value}.{method}({args})");
    }

    /// <summary>Produces a function invocation expression: <c>expr(args)</c>.</summary>
    /// <param name="arguments">The arguments to pass.</param>
    public TsExpressionRef Invoke(params TsExpressionRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return new TsExpressionRef($"{Value}({args})");
    }

    // ── Member Access ───────────────────────────────────────────────────

    /// <summary>Produces a member access expression: <c>expr.member</c>.</summary>
    /// <param name="name">The member name (property, field, or nested type).</param>
    public TsExpressionRef Member(string name) => new($"{Value}.{name}");

    /// <summary>Produces a computed member access expression: <c>expr[key]</c>.</summary>
    /// <param name="key">The computed key expression.</param>
    public TsExpressionRef Index(TsExpressionRef key) => new($"{Value}[{key.Value}]");

    // ── Optional Chaining ───────────────────────────────────────────────

    /// <summary>Produces optional chaining member access: <c>expr?.member</c>.</summary>
    /// <param name="name">The member name to access conditionally.</param>
    public TsExpressionRef OptionalChain(string name) => new($"{Value}?.{name}");

    /// <summary>Produces an optional chaining method call: <c>expr?.method(args)</c>.</summary>
    /// <param name="method">The method name.</param>
    /// <param name="arguments">The arguments to pass.</param>
    public TsExpressionRef OptionalCall(string method, params TsExpressionRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return new TsExpressionRef($"{Value}?.{method}({args})");
    }

    /// <summary>Produces optional chaining element access: <c>expr?.[index]</c>.</summary>
    /// <param name="index">The index expression.</param>
    public TsExpressionRef OptionalIndex(TsExpressionRef index) => new($"{Value}?.[{index.Value}]");

    // ── Nullish Coalescing ──────────────────────────────────────────────

    /// <summary>Appends a nullish coalescing fallback: <c>expr ?? fallback</c>.</summary>
    /// <param name="fallback">The fallback expression when the value is null or undefined.</param>
    public TsExpressionRef NullishCoalesce(TsExpressionRef fallback) => new($"{Value} ?? {fallback.Value}");

    // ── Non-Null Assertion ──────────────────────────────────────────────

    /// <summary>Appends the non-null assertion operator: <c>expr!</c>.</summary>
    public TsExpressionRef NonNullAssertion() => new($"{Value}!");

    // ── Type Assertions ─────────────────────────────────────────────────

    /// <summary>Produces a type assertion: <c>expr as Type</c>.</summary>
    /// <param name="target">The target type.</param>
    public TsExpressionRef As(TsTypeRef target) => new($"{Value} as {target.Value}");

    /// <summary>Produces a satisfies expression: <c>expr satisfies Type</c>.</summary>
    /// <param name="target">The type to satisfy.</param>
    public TsExpressionRef Satisfies(TsTypeRef target) => new($"{Value} satisfies {target.Value}");

    // ── Type Checks ─────────────────────────────────────────────────────

    /// <summary>Produces a typeof check: <c>typeof expr</c>.</summary>
    public TsExpressionRef TypeOf() => new($"typeof {Value}");

    /// <summary>Produces an instanceof check: <c>expr instanceof Type</c>.</summary>
    /// <param name="target">The type to check against.</param>
    public TsExpressionRef InstanceOf(TsTypeRef target) => new($"{Value} instanceof {target.Value}");

    // ── Async ───────────────────────────────────────────────────────────

    /// <summary>Produces an await expression: <c>await expr</c>.</summary>
    public TsExpressionRef Await() => new($"await {Value}");

    // ── Spread ──────────────────────────────────────────────────────────

    /// <summary>Produces a spread expression: <c>...expr</c>.</summary>
    public TsExpressionRef Spread() => new($"...{Value}");

    // ── Template Literals ───────────────────────────────────────────────

    /// <summary>Wraps the expression as an interpolated part of a template literal: <c>`...${expr}...`</c>.</summary>
    /// <param name="prefix">Text before the expression.</param>
    /// <param name="suffix">Text after the expression.</param>
    public TsExpressionRef TemplateLiteral(string prefix = "", string suffix = "") =>
        new($"`{prefix}${{{Value}}}{suffix}`");

    // ── Parenthesization ────────────────────────────────────────────────

    /// <summary>Wraps the expression in parentheses: <c>(expr)</c>.</summary>
    public TsExpressionRef Parenthesize() => new($"({Value})");

    // ── Logical / Comparison ────────────────────────────────────────────

    /// <summary>Produces a strict equality check: <c>expr === other</c>.</summary>
    /// <param name="other">The expression to compare against.</param>
    public TsExpressionRef StrictEquals(TsExpressionRef other) => new($"{Value} === {other.Value}");

    /// <summary>Produces a strict inequality check: <c>expr !== other</c>.</summary>
    /// <param name="other">The expression to compare against.</param>
    public TsExpressionRef StrictNotEquals(TsExpressionRef other) => new($"{Value} !== {other.Value}");

    /// <summary>Produces a logical AND: <c>expr &amp;&amp; other</c>.</summary>
    /// <param name="other">The right-hand expression.</param>
    public TsExpressionRef And(TsExpressionRef other) => new($"{Value} && {other.Value}");

    /// <summary>Produces a logical OR: <c>expr || other</c>.</summary>
    /// <param name="other">The right-hand expression.</param>
    public TsExpressionRef Or(TsExpressionRef other) => new($"{Value} || {other.Value}");

    /// <summary>Produces a logical NOT: <c>!expr</c>.</summary>
    public TsExpressionRef Not() => new($"!{Value}");

    // ── Conversions ─────────────────────────────────────────────────────

    /// <summary>Returns the string representation of this expression.</summary>
    public override string ToString() => Value;

    /// <summary>Implicitly converts a <see cref="TsExpressionRef"/> to a <see cref="string"/>.</summary>
    public static implicit operator string(TsExpressionRef expr) => expr.Value;

    /// <summary>Implicitly converts a <see cref="string"/> to a <see cref="TsExpressionRef"/>.</summary>
    public static implicit operator TsExpressionRef(string expression) => From(expression);

    /// <summary>Implicitly converts a <see cref="TsTypeRef"/> to a <see cref="TsExpressionRef"/>. A type name is a valid expression.</summary>
    public static implicit operator TsExpressionRef(TsTypeRef typeRef) => new(typeRef.Value);
}
