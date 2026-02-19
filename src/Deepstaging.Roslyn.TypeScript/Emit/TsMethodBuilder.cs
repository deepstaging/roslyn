// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Collections.Immutable;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Fluent builder for composing TypeScript method/function declarations.
/// Immutable — each method returns a new instance via <c>with</c> expressions.
/// </summary>
/// <example>
/// <code>
/// TsMethodBuilder.For("getName").WithReturnType("string")
///     .WithExpressionBody("this.name")                               // getName(): string { return this.name; }
/// TsMethodBuilder.For("fetchData").Async()
///     .AddParameter("url", "string")
///     .WithReturnType("Promise&lt;Response&gt;")                     // async fetchData(url: string): Promise&lt;Response&gt; { ... }
/// TsMethodBuilder.For("create").AsStatic()
///     .WithReturnType("MyClass")                                     // static create(): MyClass { ... }
/// TsMethodBuilder.For("process").AsAbstract()
///     .AddParameter("data", "Buffer")
///     .WithReturnType("void")                                        // abstract process(data: Buffer): void;
/// TsMethodBuilder.For("generate").AsGenerator()
///     .WithReturnType("Generator&lt;number&gt;")                     // *generate(): Generator&lt;number&gt; { ... }
/// </code>
/// </example>
public readonly record struct TsMethodBuilder
{
    /// <summary>Gets the method name.</summary>
    public string Name { get; init; }

    /// <summary>Gets the TypeScript return type annotation.</summary>
    public string? ReturnType { get; init; }

    /// <summary>Gets the parameters for the method.</summary>
    public ImmutableArray<TsParameterBuilder> Parameters { get; init; }

    /// <summary>Gets the generic type parameter names.</summary>
    public ImmutableArray<string> TypeParameters { get; init; }

    /// <summary>Gets the accessibility modifier for the method.</summary>
    public TsAccessibility Accessibility { get; init; }

    /// <summary>Gets a value indicating whether the method is static.</summary>
    public bool IsStatic { get; init; }

    /// <summary>Gets a value indicating whether the method is async.</summary>
    public bool IsAsync { get; init; }

    /// <summary>Gets a value indicating whether the method is abstract.</summary>
    public bool IsAbstract { get; init; }

    /// <summary>Gets a value indicating whether the method has the <c>override</c> modifier.</summary>
    public bool IsOverride { get; init; }

    /// <summary>Gets a value indicating whether the method is a generator (<c>*</c> prefix).</summary>
    public bool IsGenerator { get; init; }

    /// <summary>Gets a value indicating whether the method is optional (<c>name?(): void</c> — for interfaces).</summary>
    public bool IsOptional { get; init; }

    /// <summary>Gets the single expression body (no braces needed — emitter wraps).</summary>
    public string? ExpressionBody { get; init; }

    /// <summary>Gets the body builder for the method.</summary>
    public TsBodyBuilder? Body { get; init; }

    /// <summary>Gets the pre-rendered overload signatures.</summary>
    public ImmutableArray<string> OverloadSignatures { get; init; }

    /// <summary>Gets the decorator names applied to the method.</summary>
    public ImmutableArray<string> Decorators { get; init; }

    /// <summary>Initializes a new <see cref="TsMethodBuilder"/> with empty collections.</summary>
    public TsMethodBuilder()
    {
        Name = string.Empty;
        Parameters = ImmutableArray<TsParameterBuilder>.Empty;
        TypeParameters = ImmutableArray<string>.Empty;
        OverloadSignatures = ImmutableArray<string>.Empty;
        Decorators = ImmutableArray<string>.Empty;
    }

    /// <summary>Creates a new <see cref="TsMethodBuilder"/> for a method with the given name.</summary>
    /// <param name="name">The method name.</param>
    public static TsMethodBuilder For(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Method name cannot be null or whitespace.", nameof(name));

        return new TsMethodBuilder { Name = name };
    }

    /// <summary>Sets the return type annotation for the method.</summary>
    /// <param name="returnType">The TypeScript return type.</param>
    public TsMethodBuilder WithReturnType(string returnType) =>
        this with { ReturnType = returnType };

    /// <summary>Sets the accessibility modifier for the method.</summary>
    /// <param name="accessibility">The accessibility level.</param>
    public TsMethodBuilder WithAccessibility(TsAccessibility accessibility) =>
        this with { Accessibility = accessibility };

    /// <summary>Marks the method as <c>static</c>.</summary>
    public TsMethodBuilder AsStatic() =>
        this with { IsStatic = true };

    /// <summary>Marks the method as <c>async</c>.</summary>
    public TsMethodBuilder Async() =>
        this with { IsAsync = true };

    /// <summary>Marks the method as <c>abstract</c>.</summary>
    public TsMethodBuilder AsAbstract() =>
        this with { IsAbstract = true };

    /// <summary>Marks the method with the <c>override</c> modifier.</summary>
    public TsMethodBuilder AsOverride() =>
        this with { IsOverride = true };

    /// <summary>Marks the method as a generator (<c>*name()</c>).</summary>
    public TsMethodBuilder AsGenerator() =>
        this with { IsGenerator = true };

    /// <summary>Marks the method as optional (<c>name?(): void</c> — for interface members).</summary>
    public TsMethodBuilder AsOptional() =>
        this with { IsOptional = true };

    /// <summary>Adds a generic type parameter to the method.</summary>
    /// <param name="name">The type parameter name (e.g., <c>"T"</c>).</param>
    public TsMethodBuilder AddTypeParameter(string name) =>
        this with { TypeParameters = TypeParameters.Add(name) };

    /// <summary>Adds a parameter with the given name and type.</summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The TypeScript type annotation.</param>
    public TsMethodBuilder AddParameter(string name, string type) =>
        this with { Parameters = Parameters.Add(TsParameterBuilder.For(name, type)) };

    /// <summary>Adds a parameter with the given name and type, further configured via a builder function.</summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The TypeScript type annotation.</param>
    /// <param name="configure">A function that configures the parameter builder.</param>
    public TsMethodBuilder AddParameter(string name, string type, Func<TsParameterBuilder, TsParameterBuilder> configure) =>
        this with { Parameters = Parameters.Add(configure(TsParameterBuilder.For(name, type))) };

    /// <summary>Adds a pre-built parameter to the method.</summary>
    /// <param name="parameter">The parameter builder to add.</param>
    public TsMethodBuilder AddParameter(TsParameterBuilder parameter) =>
        this with { Parameters = Parameters.Add(parameter) };

    /// <summary>Sets the body of the method via a builder function.</summary>
    /// <param name="configure">A function that configures the body builder.</param>
    public TsMethodBuilder WithBody(Func<TsBodyBuilder, TsBodyBuilder> configure) =>
        this with { Body = configure(TsBodyBuilder.Empty()) };

    /// <summary>Sets a single expression body for the method (emitter wraps in braces and return).</summary>
    /// <param name="expression">The expression to use as the body.</param>
    public TsMethodBuilder WithExpressionBody(string expression) =>
        this with { ExpressionBody = expression };

    /// <summary>Adds a raw TypeScript overload signature string.</summary>
    /// <param name="signature">The pre-rendered overload signature.</param>
    public TsMethodBuilder AddOverloadSignature(string signature) =>
        this with { OverloadSignatures = OverloadSignatures.Add(signature) };

    /// <summary>Adds a decorator to the method.</summary>
    /// <param name="decorator">The decorator name or expression (e.g., <c>"@Log"</c>).</param>
    public TsMethodBuilder WithDecorator(string decorator) =>
        this with { Decorators = Decorators.Add(decorator) };
}
