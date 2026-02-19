// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Fluent builder for composing TypeScript interface/class property declarations.
/// Supports simple properties, getters, setters, and accessor bodies.
/// Immutable — each method returns a new instance via <c>with</c> expressions.
/// </summary>
/// <example>
/// <code>
/// TsPropertyBuilder.For("name", "string")                              // name: string;
/// TsPropertyBuilder.For("name", "string").AsReadonly()                  // readonly name: string;
/// TsPropertyBuilder.For("name", "string").AsOptional()                  // name?: string;
/// TsPropertyBuilder.For("name", "string")
///     .WithGetter("this._name")                                         // get name(): string { return this._name; }
/// TsPropertyBuilder.For("name", "string")
///     .WithSetter(b => b.AddStatement("this._name = value"))            // set name(value: string) { this._name = value; }
/// TsPropertyBuilder.For("name", "string").AsAbstract()                  // abstract name: string;
/// </code>
/// </example>
public readonly record struct TsPropertyBuilder
{
    /// <summary>Gets the property name.</summary>
    public string Name { get; init; }

    /// <summary>Gets the TypeScript type annotation.</summary>
    public string Type { get; init; }

    /// <summary>Gets the accessibility modifier for the property.</summary>
    public TsAccessibility Accessibility { get; init; }

    /// <summary>Gets a value indicating whether the property is readonly.</summary>
    public bool IsReadonly { get; init; }

    /// <summary>Gets a value indicating whether the property is optional (<c>?</c> suffix).</summary>
    public bool IsOptional { get; init; }

    /// <summary>Gets a value indicating whether the property is static.</summary>
    public bool IsStatic { get; init; }

    /// <summary>Gets a value indicating whether the property is abstract.</summary>
    public bool IsAbstract { get; init; }

    /// <summary>Gets a value indicating whether the property has the <c>override</c> modifier.</summary>
    public bool IsOverride { get; init; }

    /// <summary>Gets the expression for an expression-style getter (<c>get name() { return expr; }</c>).</summary>
    public string? GetterExpression { get; init; }

    /// <summary>Gets the body builder for a block-style getter.</summary>
    public TsBodyBuilder? GetterBody { get; init; }

    /// <summary>Gets the body builder for a setter.</summary>
    public TsBodyBuilder? SetterBody { get; init; }

    /// <summary>Gets the optional initializer expression.</summary>
    public string? Initializer { get; init; }

    /// <summary>Creates a new <see cref="TsPropertyBuilder"/> for a property with the given name and type.</summary>
    /// <param name="name">The property name.</param>
    /// <param name="type">The TypeScript type annotation.</param>
    public static TsPropertyBuilder For(string name, string type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Property name cannot be null or whitespace.", nameof(name));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Property type cannot be null or whitespace.", nameof(type));

        return new TsPropertyBuilder { Name = name, Type = type };
    }

    /// <summary>Sets the accessibility modifier for the property.</summary>
    /// <param name="accessibility">The accessibility level.</param>
    public TsPropertyBuilder WithAccessibility(TsAccessibility accessibility) =>
        this with { Accessibility = accessibility };

    /// <summary>Marks the property as <c>readonly</c>.</summary>
    public TsPropertyBuilder AsReadonly() =>
        this with { IsReadonly = true };

    /// <summary>Marks the property as optional (<c>name?: Type</c>).</summary>
    public TsPropertyBuilder AsOptional() =>
        this with { IsOptional = true };

    /// <summary>Marks the property as <c>static</c>.</summary>
    public TsPropertyBuilder AsStatic() =>
        this with { IsStatic = true };

    /// <summary>Marks the property as <c>abstract</c>.</summary>
    public TsPropertyBuilder AsAbstract() =>
        this with { IsAbstract = true };

    /// <summary>Marks the property with the <c>override</c> modifier.</summary>
    public TsPropertyBuilder AsOverride() =>
        this with { IsOverride = true };

    /// <summary>Adds an expression-style getter: <c>get name(): Type { return expression; }</c>.</summary>
    /// <param name="expression">The expression to return from the getter.</param>
    public TsPropertyBuilder WithGetter(string expression) =>
        this with { GetterExpression = expression };

    /// <summary>Adds a block-style getter configured via a <see cref="TsBodyBuilder"/>.</summary>
    /// <param name="configure">A function that configures the getter body.</param>
    public TsPropertyBuilder WithGetter(Func<TsBodyBuilder, TsBodyBuilder> configure) =>
        this with { GetterBody = configure(TsBodyBuilder.Empty()) };

    /// <summary>Adds a setter configured via a <see cref="TsBodyBuilder"/>.</summary>
    /// <param name="configure">A function that configures the setter body.</param>
    public TsPropertyBuilder WithSetter(Func<TsBodyBuilder, TsBodyBuilder> configure) =>
        this with { SetterBody = configure(TsBodyBuilder.Empty()) };

    /// <summary>Sets the initializer expression for the property.</summary>
    /// <param name="initializer">The initializer expression (e.g., <c>"0"</c>, <c>"[]"</c>).</param>
    public TsPropertyBuilder WithInitializer(string initializer) =>
        this with { Initializer = initializer };
}
