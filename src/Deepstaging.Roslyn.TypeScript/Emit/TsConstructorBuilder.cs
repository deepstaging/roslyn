// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Collections.Immutable;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Fluent builder for composing TypeScript constructor declarations.
/// Immutable — each method returns a new instance via <c>with</c> expressions.
/// </summary>
/// <example>
/// <code>
/// TsConstructorBuilder.Create()
///     .AddParameter("name", "string")
///     .WithBody(b => b.AddStatement("this.name = name"))             // constructor(name: string) { this.name = name; }
/// TsConstructorBuilder.Create()
///     .AddParameter("name", "string", p => p.AsParameterProperty(TsAccessibility.Public))
///     .AddParameter("age", "number", p => p.AsParameterProperty(TsAccessibility.Private))
///                                                                     // constructor(public name: string, private age: number) { }
/// TsConstructorBuilder.Create()
///     .AddParameter("config", "Config")
///     .CallsSuper("config")                                           // constructor(config: Config) { super(config); ... }
/// </code>
/// </example>
public readonly record struct TsConstructorBuilder
{
    /// <summary>Gets the parameters for the constructor.</summary>
    public ImmutableArray<TsParameterBuilder> Parameters { get; init; }

    /// <summary>Gets the accessibility modifier for the constructor.</summary>
    public TsAccessibility Accessibility { get; init; }

    /// <summary>Gets the body builder for the constructor.</summary>
    public TsBodyBuilder? Body { get; init; }

    /// <summary>Gets the arguments passed to the <c>super(...)</c> call.</summary>
    public ImmutableArray<string> SuperArguments { get; init; }

    /// <summary>Gets the pre-rendered overload signatures.</summary>
    public ImmutableArray<string> OverloadSignatures { get; init; }

    /// <summary>Initializes a new <see cref="TsConstructorBuilder"/> with empty collections.</summary>
    public TsConstructorBuilder()
    {
        Parameters = ImmutableArray<TsParameterBuilder>.Empty;
        SuperArguments = ImmutableArray<string>.Empty;
        OverloadSignatures = ImmutableArray<string>.Empty;
    }

    /// <summary>Creates a new empty <see cref="TsConstructorBuilder"/>.</summary>
    public static TsConstructorBuilder Create() => new();

    /// <summary>Sets the accessibility modifier for the constructor.</summary>
    /// <param name="accessibility">The accessibility level.</param>
    public TsConstructorBuilder WithAccessibility(TsAccessibility accessibility) =>
        this with { Accessibility = accessibility };

    /// <summary>Adds a parameter with the given name and type.</summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The TypeScript type annotation.</param>
    public TsConstructorBuilder AddParameter(string name, string type) =>
        this with { Parameters = Parameters.Add(TsParameterBuilder.For(name, type)) };

    /// <summary>Adds a parameter with the given name and type, further configured via a builder function.</summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The TypeScript type annotation.</param>
    /// <param name="configure">A function that configures the parameter builder.</param>
    public TsConstructorBuilder AddParameter(string name, string type, Func<TsParameterBuilder, TsParameterBuilder> configure) =>
        this with { Parameters = Parameters.Add(configure(TsParameterBuilder.For(name, type))) };

    /// <summary>Adds a pre-built parameter to the constructor.</summary>
    /// <param name="parameter">The parameter builder to add.</param>
    public TsConstructorBuilder AddParameter(TsParameterBuilder parameter) =>
        this with { Parameters = Parameters.Add(parameter) };

    /// <summary>Sets the body of the constructor via a builder function.</summary>
    /// <param name="configure">A function that configures the body builder.</param>
    public TsConstructorBuilder WithBody(Func<TsBodyBuilder, TsBodyBuilder> configure) =>
        this with { Body = configure(TsBodyBuilder.Empty()) };

    /// <summary>Configures the constructor to call <c>super(...)</c> with the given arguments.</summary>
    /// <param name="arguments">The arguments to pass to the super call.</param>
    public TsConstructorBuilder CallsSuper(params string[] arguments) =>
        this with { SuperArguments = ImmutableArray.Create(arguments) };

    /// <summary>Adds a raw TypeScript overload signature string.</summary>
    /// <param name="signature">The pre-rendered overload signature.</param>
    public TsConstructorBuilder AddOverloadSignature(string signature) =>
        this with { OverloadSignatures = OverloadSignatures.Add(signature) };
}
