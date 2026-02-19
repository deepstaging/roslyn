// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Linq;

namespace Deepstaging.Roslyn.TypeScript;

/// <summary>
/// Fluent builder for composing TypeScript type reference strings.
/// Supports generics, unions, intersections, tuples, literals, and TypeScript-specific type operators.
/// Immutable — each method returns a new instance.
/// Has implicit conversions to/from <see cref="string"/> for seamless integration with existing builders.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="TsTypeRef"/> is the TypeScript counterpart to <see cref="TypeRef"/>.
/// On top of it, specialized wrappers in <c>Types/</c> carry constituent generic arguments
/// (e.g., <see cref="Types.TsPromiseTypeRef"/> carries <c>ResultType</c>).
/// </para>
/// </remarks>
/// <example>
/// <code>
/// TsTypeRef.From("Map").Of(TsTypeRef.From("string"), TsTypeRef.From("number"))  // "Map&lt;string, number&gt;"
/// TsTypeRef.Union(TsTypeRef.From("string"), TsTypeRef.From("number"))            // "string | number"
/// TsTypeRef.From("string").Array()                                                // "string[]"
/// TsTypeRef.From("string").Nullable()                                             // "string | null"
/// </code>
/// </example>
public readonly partial record struct TsTypeRef
{
    /// <summary>Gets the string representation of the type reference.</summary>
    public string Value { get; }

    private TsTypeRef(string value) => Value = value;

    // ── Factories ───────────────────────────────────────────────────────

    /// <summary>Creates a type reference from a TypeScript type name.</summary>
    /// <param name="typeName">The type name (e.g., "string", "number", "MyInterface").</param>
    public static TsTypeRef From(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            throw new ArgumentException("Type name cannot be null or whitespace.", nameof(typeName));

        return new TsTypeRef(typeName);
    }

    /// <summary>Creates a type reference from a type name string.</summary>
    /// <param name="typeName">The type name as a string.</param>
    /// <returns>A <see cref="TsTypeRef"/> instance representing the provided type name.</returns>
    /// <remarks>Semantic alias for <see cref="From(string)"/> that reads naturally when <c>using static TsTypeRef</c> is in scope.</remarks>
    public static TsTypeRef CreateTypeRef(string typeName) => From(typeName);

    // ── Generics ────────────────────────────────────────────────────────

    /// <summary>Adds generic type arguments to this type reference.</summary>
    /// <param name="typeArguments">The type arguments.</param>
    /// <example><c>TsTypeRef.From("Map").Of(TsTypeRef.From("string"), TsTypeRef.From("number"))</c> produces <c>"Map&lt;string, number&gt;"</c>.</example>
    public TsTypeRef Of(params TsTypeRef[] typeArguments)
    {
        if (typeArguments.Length == 0)
            throw new ArgumentException("At least one type argument is required.", nameof(typeArguments));

        var args = string.Join(", ", typeArguments.Select(t => t.Value));
        return new TsTypeRef($"{Value}<{args}>");
    }

    // ── TypeScript Type Operators ───────────────────────────────────────

    /// <summary>Creates a union type: <c>A | B | C</c>.</summary>
    /// <param name="types">The types to combine into a union.</param>
    public static TsTypeRef Union(params TsTypeRef[] types)
    {
        if (types.Length < 2)
            throw new ArgumentException("Union requires at least two types.", nameof(types));

        return new TsTypeRef(string.Join(" | ", types.Select(t => t.Value)));
    }

    /// <summary>Creates an intersection type: <c>A &amp; B</c>.</summary>
    /// <param name="types">The types to combine into an intersection.</param>
    public static TsTypeRef Intersection(params TsTypeRef[] types)
    {
        if (types.Length < 2)
            throw new ArgumentException("Intersection requires at least two types.", nameof(types));

        return new TsTypeRef(string.Join(" & ", types.Select(t => t.Value)));
    }

    /// <summary>Creates a tuple type: <c>[string, number]</c>.</summary>
    /// <param name="elements">The tuple element types.</param>
    public static TsTypeRef Tuple(params TsTypeRef[] elements)
    {
        if (elements.Length == 0)
            throw new ArgumentException("Tuple requires at least one element.", nameof(elements));

        return new TsTypeRef($"[{string.Join(", ", elements.Select(e => e.Value))}]");
    }

    /// <summary>Creates a named tuple type: <c>[name: string, age: number]</c>.</summary>
    /// <param name="elements">The tuple elements as (Type, Name) pairs.</param>
    public static TsTypeRef NamedTuple(params (TsTypeRef Type, string Name)[] elements)
    {
        if (elements.Length == 0)
            throw new ArgumentException("Named tuple requires at least one element.", nameof(elements));

        var parts = string.Join(", ", elements.Select(e => $"{e.Name}: {e.Type.Value}"));
        return new TsTypeRef($"[{parts}]");
    }

    /// <summary>Creates a string literal type: <c>"success"</c> (with quotes).</summary>
    /// <param name="value">The literal string value.</param>
    public static TsTypeRef Literal(string value) => new($"\"{value}\"");

    /// <summary>Creates a numeric literal type: <c>42</c> (no quotes).</summary>
    /// <param name="value">The numeric literal value as a string.</param>
    public static TsTypeRef NumericLiteral(string value) => new(value);

    /// <summary>Creates a template literal type: <c>`prefix-${string}`</c>.</summary>
    /// <param name="template">The template literal string including backticks.</param>
    public static TsTypeRef TemplateLiteral(string template) => new(template);

    // ── Modifiers ───────────────────────────────────────────────────────

    /// <summary>Makes this type an array by appending <c>[]</c>.</summary>
    public TsTypeRef Array() => new($"{Value}[]");

    /// <summary>Makes this type nullable: <c>T | null</c>.</summary>
    public TsTypeRef Nullable() => new($"{Value} | null");

    /// <summary>Makes this type optional: <c>T | undefined</c>.</summary>
    public TsTypeRef Optional() => new($"{Value} | undefined");

    /// <summary>Makes this type nullable and optional: <c>T | null | undefined</c>.</summary>
    public TsTypeRef NullableOptional() => new($"{Value} | null | undefined");

    /// <summary>Adds the <c>readonly</c> modifier to this type.</summary>
    /// <remarks>For arrays, produces <c>readonly T[]</c>.</remarks>
    public TsTypeRef Readonly() => new($"readonly {Value}");

    /// <summary>Wraps this type in parentheses: <c>(T)</c> — useful before <c>[]</c> or other operators.</summary>
    public TsTypeRef Parenthesize() => new($"({Value})");

    // ── Type Operators ──────────────────────────────────────────────────

    /// <summary>Produces a <c>keyof T</c> type operator expression.</summary>
    public TsTypeRef KeyOf() => new($"keyof {Value}");

    /// <summary>Produces a <c>typeof T</c> type operator expression.</summary>
    public TsTypeRef TypeOf() => new($"typeof {Value}");

    // ── Expression Gateways ─────────────────────────────────────────────
    // These methods cross from the type domain into the expression domain.
    // All return TsExpressionRef — the one-way gate.

    /// <summary>Produces a constructor call expression: <c>new T(args)</c>.</summary>
    /// <param name="arguments">The constructor arguments.</param>
    public TsExpressionRef New(params TsExpressionRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return TsExpressionRef.From($"new {Value}({args})");
    }

    /// <summary>Produces a member access expression: <c>T.name</c>.</summary>
    /// <param name="name">The member name (property, field, or nested type).</param>
    public TsExpressionRef Member(string name) => TsExpressionRef.From($"{Value}.{name}");

    /// <summary>Produces a static method call expression: <c>T.method(args)</c>.</summary>
    /// <param name="method">The method name.</param>
    /// <param name="arguments">The method arguments.</param>
    public TsExpressionRef Call(string method, params TsExpressionRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return TsExpressionRef.From($"{Value}.{method}({args})");
    }

    // ── Conversions ─────────────────────────────────────────────────────

    /// <summary>Returns the string representation of this type reference.</summary>
    public override string ToString() => Value;

    /// <summary>Implicitly converts a <see cref="TsTypeRef"/> to a <see cref="string"/>.</summary>
    public static implicit operator string(TsTypeRef typeRef) => typeRef.Value;

    /// <summary>Implicitly converts a <see cref="string"/> to a <see cref="TsTypeRef"/>.</summary>
    public static implicit operator TsTypeRef(string typeName) => From(typeName);
}
