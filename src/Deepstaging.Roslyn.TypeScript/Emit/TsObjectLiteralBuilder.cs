// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Collections.Immutable;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Fluent builder for composing TypeScript object literal expressions.
/// Immutable — each method returns a new instance via <c>with</c> expressions.
/// Builds to a <see cref="TsExpressionRef"/> for composition with fields, variables, and other expressions.
/// </summary>
/// <example>
/// <code>
/// // Simple object literal
/// TsObjectLiteralBuilder.Create()
///     .AddProperty("name", "'Alice'")
///     .AddProperty("age", "30")
///     .Build()                                          // { name: 'Alice', age: 30 }
///
/// // With arrow function methods
/// TsObjectLiteralBuilder.Create()
///     .AddProperty("create", TsArrowFunctionBuilder.Create()
///         .Async()
///         .AddParameter("cmd", "CreateCommand")
///         .WithExpressionBody("this.post('/api', cmd)")
///         .Build())
///     .AddProperty("list", TsArrowFunctionBuilder.Create()
///         .Async()
///         .WithExpressionBody("this.get('/api')")
///         .Build())
///     .Build()                                          // { create: async (cmd: CreateCommand) => ..., list: async () => ... }
///
/// // Shorthand properties
/// TsObjectLiteralBuilder.Create()
///     .AddShorthand("name")
///     .AddShorthand("age")
///     .Build()                                          // { name, age }
///
/// // Spread
/// TsObjectLiteralBuilder.Create()
///     .AddSpread("defaults")
///     .AddProperty("override", "'value'")
///     .Build()                                          // { ...defaults, override: 'value' }
///
/// // Computed property
/// TsObjectLiteralBuilder.Create()
///     .AddComputedProperty("key", "'value'")
///     .Build()                                          // { [key]: 'value' }
/// </code>
/// </example>
public readonly record struct TsObjectLiteralBuilder
{
    /// <summary>Gets the entries in this object literal.</summary>
    public ImmutableArray<ObjectEntry> Entries { get; init; }

    /// <summary>Initializes a new <see cref="TsObjectLiteralBuilder"/> with empty entries.</summary>
    public TsObjectLiteralBuilder()
    {
        Entries = ImmutableArray<ObjectEntry>.Empty;
    }

    /// <summary>Creates a new empty object literal builder.</summary>
    public static TsObjectLiteralBuilder Create() => new();

    // ── Properties ──────────────────────────────────────────────────────

    /// <summary>Adds a property with a name and value expression: <c>name: value</c>.</summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The value expression.</param>
    public TsObjectLiteralBuilder AddProperty(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Property name cannot be null or whitespace.", nameof(name));

        return this with { Entries = Entries.Add(new ObjectEntry(ObjectEntryKind.Property, name, value)) };
    }

    /// <summary>Adds a property with a name and a <see cref="TsExpressionRef"/> value.</summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The value expression.</param>
    /// <remarks>
    /// Use this overload when you have a <see cref="TsExpressionRef"/> (e.g., from <see cref="TsArrowFunctionBuilder.Build"/>).
    /// For raw string values, use <see cref="AddProperty(string, string)"/>.
    /// The overloads are disambiguated by requiring an explicit <see cref="TsExpressionRef"/> parameter.
    /// </remarks>
    public TsObjectLiteralBuilder AddExpressionProperty(string name, TsExpressionRef value) =>
        AddProperty(name, value.Value);

    /// <summary>Adds a shorthand property: <c>name</c> (equivalent to <c>name: name</c>).</summary>
    /// <param name="name">The property name.</param>
    public TsObjectLiteralBuilder AddShorthand(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Property name cannot be null or whitespace.", nameof(name));

        return this with { Entries = Entries.Add(new ObjectEntry(ObjectEntryKind.Shorthand, name, null)) };
    }

    /// <summary>Adds a computed property: <c>[expression]: value</c>.</summary>
    /// <param name="keyExpression">The computed key expression.</param>
    /// <param name="value">The value expression.</param>
    public TsObjectLiteralBuilder AddComputedProperty(string keyExpression, string value) =>
        this with { Entries = Entries.Add(new ObjectEntry(ObjectEntryKind.Computed, keyExpression, value)) };

    /// <summary>Adds a spread entry: <c>...expression</c>.</summary>
    /// <param name="expression">The expression to spread.</param>
    public TsObjectLiteralBuilder AddSpread(string expression) =>
        this with { Entries = Entries.Add(new ObjectEntry(ObjectEntryKind.Spread, expression, null)) };

    /// <summary>Adds a spread entry from a <see cref="TsExpressionRef"/>.</summary>
    /// <param name="expression">The expression to spread.</param>
    public TsObjectLiteralBuilder AddSpread(TsExpressionRef expression) =>
        AddSpread(expression.Value);

    // ── Method shorthand ────────────────────────────────────────────────

    /// <summary>
    /// Adds a method shorthand property: <c>name(params) { body }</c>.
    /// Configured via a <see cref="TsMethodBuilder"/> for full control over parameters, async, body, etc.
    /// </summary>
    /// <param name="name">The method name.</param>
    /// <param name="configure">A function that configures the method builder.</param>
    public TsObjectLiteralBuilder AddMethod(string name, Func<TsMethodBuilder, TsMethodBuilder> configure)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Method name cannot be null or whitespace.", nameof(name));

        var method = configure(TsMethodBuilder.For(name));
        var rendered = RenderMethodShorthand(method);
        return this with { Entries = Entries.Add(new ObjectEntry(ObjectEntryKind.Method, name, rendered)) };
    }

    // ── Build ───────────────────────────────────────────────────────────

    /// <summary>
    /// Builds the object literal expression and returns it as a <see cref="TsExpressionRef"/>.
    /// </summary>
    /// <remarks>
    /// Objects with 3 or fewer simple entries are rendered on a single line.
    /// Objects with more entries or multi-line values are rendered in multi-line format.
    /// </remarks>
    public TsExpressionRef Build()
    {
        if (Entries.IsDefaultOrEmpty)
            return TsExpressionRef.From("{ }");

        var useMultiLine = Entries.Length > 3 || HasMultiLineEntries();

        return useMultiLine ? BuildMultiLine() : BuildSingleLine();
    }

    /// <summary>
    /// Implicitly converts a built object literal to a <see cref="TsExpressionRef"/>.
    /// Equivalent to calling <see cref="Build"/>.
    /// </summary>
    public static implicit operator TsExpressionRef(TsObjectLiteralBuilder builder) => builder.Build();

    /// <summary>
    /// Implicitly converts a built object literal to a <see cref="string"/>.
    /// Equivalent to calling <see cref="Build"/> and accessing <see cref="TsExpressionRef.Value"/>.
    /// </summary>
    public static implicit operator string(TsObjectLiteralBuilder builder) => builder.Build().Value;

    // ── Rendering ───────────────────────────────────────────────────────

    private TsExpressionRef BuildSingleLine()
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("{ ");

        for (var i = 0; i < Entries.Length; i++)
        {
            if (i > 0)
                sb.Append(", ");

            AppendEntry(sb, Entries[i]);
        }

        sb.Append(" }");
        return TsExpressionRef.From(sb.ToString());
    }

    private TsExpressionRef BuildMultiLine()
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("{\n");

        for (var i = 0; i < Entries.Length; i++)
        {
            sb.Append("  ");
            AppendEntry(sb, Entries[i]);

            if (i < Entries.Length - 1)
                sb.Append(',');

            sb.Append('\n');
        }

        sb.Append('}');
        return TsExpressionRef.From(sb.ToString());
    }

    private static void AppendEntry(System.Text.StringBuilder sb, ObjectEntry entry)
    {
        switch (entry.Kind)
        {
            case ObjectEntryKind.Property:
                sb.Append(entry.Key);
                sb.Append(": ");
                sb.Append(entry.Value);
                break;

            case ObjectEntryKind.Shorthand:
                sb.Append(entry.Key);
                break;

            case ObjectEntryKind.Computed:
                sb.Append('[');
                sb.Append(entry.Key);
                sb.Append("]: ");
                sb.Append(entry.Value);
                break;

            case ObjectEntryKind.Spread:
                sb.Append("...");
                sb.Append(entry.Key);
                break;

            case ObjectEntryKind.Method:
                sb.Append(entry.Value);
                break;
        }
    }

    private bool HasMultiLineEntries()
    {
        foreach (var entry in Entries)
        {
            if (entry.Value != null && entry.Value.Contains("\n"))
                return true;
            if (entry.Kind == ObjectEntryKind.Method)
                return true;
        }

        return false;
    }

    private static string RenderMethodShorthand(TsMethodBuilder method)
    {
        var sb = new System.Text.StringBuilder();

        if (method.IsAsync)
            sb.Append("async ");
        if (method.IsGenerator)
            sb.Append('*');

        sb.Append(method.Name);
        sb.Append('(');

        for (var i = 0; i < method.Parameters.Length; i++)
        {
            if (i > 0)
                sb.Append(", ");

            var param = method.Parameters[i];
            if (param.IsRest)
                sb.Append("...");
            sb.Append(param.Name);
            if (param.IsOptional)
                sb.Append('?');
            sb.Append(": ");
            sb.Append(param.Type);
            if (param.DefaultValue != null)
            {
                sb.Append(" = ");
                sb.Append(param.DefaultValue);
            }
        }

        sb.Append(')');

        if (method.ReturnType != null)
        {
            sb.Append(": ");
            sb.Append(method.ReturnType);
        }

        if (method.ExpressionBody != null)
        {
            sb.Append(" {\n    return ");
            sb.Append(method.ExpressionBody);
            sb.Append(";\n  }");
        }
        else if (method.Body != null && !method.Body.Value.IsEmpty)
        {
            sb.Append(" {\n");
            foreach (var statement in method.Body.Value.Statements)
            {
                var lines = statement.Split('\n');
                foreach (var line in lines)
                {
                    sb.Append("    ");
                    sb.Append(line.TrimEnd('\r'));
                    sb.Append('\n');
                }
            }
            sb.Append("  }");
        }
        else
        {
            sb.Append(" { }");
        }

        return sb.ToString();
    }

    /// <summary>Represents a single entry in an object literal.</summary>
    public readonly record struct ObjectEntry(ObjectEntryKind Kind, string Key, string? Value);

    /// <summary>The kind of object literal entry.</summary>
    public enum ObjectEntryKind
    {
        /// <summary>A standard property: <c>key: value</c>.</summary>
        Property,

        /// <summary>A shorthand property: <c>key</c>.</summary>
        Shorthand,

        /// <summary>A computed property: <c>[expr]: value</c>.</summary>
        Computed,

        /// <summary>A spread entry: <c>...expr</c>.</summary>
        Spread,

        /// <summary>A method shorthand: <c>name() { }</c>.</summary>
        Method
    }
}
