// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Collections.Immutable;
using System.Linq;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Fluent builder for composing TypeScript arrow function expressions.
/// Immutable — each method returns a new instance via <c>with</c> expressions.
/// Builds to a <see cref="TsExpressionRef"/> for composition with fields, variables, and other expressions.
/// </summary>
/// <example>
/// <code>
/// // Concise expression body
/// TsArrowFunctionBuilder.Create()
///     .AddParameter("x", "number")
///     .WithExpressionBody("x * 2")
///     .Build()                                          // (x: number) => x * 2
///
/// // Async with block body
/// TsArrowFunctionBuilder.Create()
///     .Async()
///     .AddParameter("cmd", "CreateOrderCommand")
///     .WithBody(b => b
///         .AddConst("response", "await this.fetch('/api/orders', { method: 'POST', body: JSON.stringify(cmd) })")
///         .AddReturn("response.json()"))
///     .Build()                                          // async (cmd: CreateOrderCommand) => { ... }
///
/// // No parameters
/// TsArrowFunctionBuilder.Create()
///     .WithExpressionBody("Date.now()")
///     .Build()                                          // () => Date.now()
///
/// // Used as field initializer
/// TsFieldBuilder.For("transform", "(x: number) => number")
///     .AsReadonly()
///     .WithInitializer(
///         TsArrowFunctionBuilder.Create()
///             .AddParameter("x", "number")
///             .WithExpressionBody("x * 2")
///             .Build())                                 // readonly transform: (x: number) => number = (x: number) => x * 2;
/// </code>
/// </example>
public readonly record struct TsArrowFunctionBuilder
{
    /// <summary>Gets the parameters for the arrow function.</summary>
    public ImmutableArray<TsParameterBuilder> Parameters { get; init; }

    /// <summary>Gets the generic type parameter names.</summary>
    public ImmutableArray<string> TypeParameters { get; init; }

    /// <summary>Gets a value indicating whether the arrow function is async.</summary>
    public bool IsAsync { get; init; }

    /// <summary>Gets the return type annotation, if any.</summary>
    public string? ReturnType { get; init; }

    /// <summary>Gets the single expression body (concise form).</summary>
    public string? ExpressionBody { get; init; }

    /// <summary>Gets the block body builder.</summary>
    public TsBodyBuilder? Body { get; init; }

    /// <summary>Initializes a new <see cref="TsArrowFunctionBuilder"/> with empty collections.</summary>
    public TsArrowFunctionBuilder()
    {
        Parameters = ImmutableArray<TsParameterBuilder>.Empty;
        TypeParameters = ImmutableArray<string>.Empty;
    }

    /// <summary>Creates a new empty arrow function builder.</summary>
    public static TsArrowFunctionBuilder Create() => new();

    // ── Modifiers ───────────────────────────────────────────────────────

    /// <summary>Marks the arrow function as <c>async</c>.</summary>
    public TsArrowFunctionBuilder Async() =>
        this with { IsAsync = true };

    /// <summary>Sets the return type annotation for the arrow function.</summary>
    /// <param name="returnType">The TypeScript return type.</param>
    public TsArrowFunctionBuilder WithReturnType(string returnType) =>
        this with { ReturnType = returnType };

    // ── Type Parameters ─────────────────────────────────────────────────

    /// <summary>Adds a generic type parameter to the arrow function.</summary>
    /// <param name="name">The type parameter name (e.g., <c>"T"</c>).</param>
    public TsArrowFunctionBuilder AddTypeParameter(string name) =>
        this with { TypeParameters = TypeParameters.Add(name) };

    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Adds a parameter with the given name and type.</summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The TypeScript type annotation.</param>
    public TsArrowFunctionBuilder AddParameter(string name, string type) =>
        this with { Parameters = Parameters.Add(TsParameterBuilder.For(name, type)) };

    /// <summary>Adds a parameter configured via a builder function.</summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The TypeScript type annotation.</param>
    /// <param name="configure">A function that configures the parameter builder.</param>
    public TsArrowFunctionBuilder AddParameter(string name, string type, Func<TsParameterBuilder, TsParameterBuilder> configure) =>
        this with { Parameters = Parameters.Add(configure(TsParameterBuilder.For(name, type))) };

    /// <summary>Adds a pre-built parameter to the arrow function.</summary>
    /// <param name="parameter">The parameter builder to add.</param>
    public TsArrowFunctionBuilder AddParameter(TsParameterBuilder parameter) =>
        this with { Parameters = Parameters.Add(parameter) };

    // ── Body ────────────────────────────────────────────────────────────

    /// <summary>Sets a single expression body for the arrow function (concise form: <c>() =&gt; expr</c>).</summary>
    /// <param name="expression">The expression body.</param>
    public TsArrowFunctionBuilder WithExpressionBody(string expression) =>
        this with { ExpressionBody = expression };

    /// <summary>Sets a single expression body using a <see cref="TsExpressionRef"/>.</summary>
    /// <param name="expression">The expression body.</param>
    public TsArrowFunctionBuilder WithExpressionBody(TsExpressionRef expression) =>
        this with { ExpressionBody = expression.Value };

    /// <summary>Sets the block body via a builder function.</summary>
    /// <param name="configure">A function that configures the body builder.</param>
    public TsArrowFunctionBuilder WithBody(Func<TsBodyBuilder, TsBodyBuilder> configure) =>
        this with { Body = configure(TsBodyBuilder.Empty()) };

    // ── Build ───────────────────────────────────────────────────────────

    /// <summary>
    /// Builds the arrow function expression string and returns it as a <see cref="TsExpressionRef"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when neither expression body nor block body is set.</exception>
    public TsExpressionRef Build()
    {
        if (ExpressionBody == null && Body == null)
            throw new InvalidOperationException("Arrow function must have either an expression body or a block body.");

        var sb = new System.Text.StringBuilder();

        if (IsAsync)
            sb.Append("async ");

        // Type parameters
        if (!TypeParameters.IsDefaultOrEmpty)
        {
            sb.Append('<');
            sb.Append(string.Join(", ", TypeParameters));
            sb.Append('>');
        }

        // Parameters
        sb.Append('(');
        AppendParameters(sb);
        sb.Append(')');

        // Return type
        if (ReturnType != null)
        {
            sb.Append(": ");
            sb.Append(ReturnType);
        }

        sb.Append(" => ");

        // Body
        if (ExpressionBody != null)
        {
            sb.Append(ExpressionBody);
        }
        else if (Body != null)
        {
            AppendBlockBody(sb, Body.Value);
        }

        return TsExpressionRef.From(sb.ToString());
    }

    /// <summary>
    /// Implicitly converts a built arrow function to a <see cref="TsExpressionRef"/>.
    /// Equivalent to calling <see cref="Build"/>.
    /// </summary>
    public static implicit operator TsExpressionRef(TsArrowFunctionBuilder builder) => builder.Build();

    /// <summary>
    /// Implicitly converts a built arrow function to a <see cref="string"/>.
    /// Equivalent to calling <see cref="Build"/> and accessing <see cref="TsExpressionRef.Value"/>.
    /// </summary>
    public static implicit operator string(TsArrowFunctionBuilder builder) => builder.Build().Value;

    private void AppendParameters(System.Text.StringBuilder sb)
    {
        if (Parameters.IsDefaultOrEmpty)
            return;

        for (var i = 0; i < Parameters.Length; i++)
        {
            if (i > 0)
                sb.Append(", ");

            var param = Parameters[i];

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
    }

    private static void AppendBlockBody(System.Text.StringBuilder sb, TsBodyBuilder body)
    {
        if (body.IsEmpty)
        {
            sb.Append("{ }");
            return;
        }

        sb.Append("{\n");
        foreach (var statement in body.Statements)
        {
            var lines = statement.Split('\n');
            foreach (var line in lines)
            {
                sb.Append("  ");
                sb.Append(line.TrimEnd('\r'));
                sb.Append('\n');
            }
        }
        sb.Append('}');
    }
}
