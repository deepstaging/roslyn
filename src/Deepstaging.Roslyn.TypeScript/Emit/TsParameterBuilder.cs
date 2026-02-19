// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Fluent builder for composing TypeScript function/method parameter declarations.
/// Immutable — each method returns a new instance via <c>with</c> expressions.
/// </summary>
/// <example>
/// <code>
/// TsParameterBuilder.For("name", "string")                             // name: string
/// TsParameterBuilder.For("name", "string").AsOptional()                // name?: string
/// TsParameterBuilder.For("name", "string").WithDefaultValue("\"default\"") // name: string = "default"
/// TsParameterBuilder.For("args", "string[]").AsRest()                  // ...args: string[]
/// TsParameterBuilder.For("name", "string")
///     .AsParameterProperty(TsAccessibility.Public)                     // public name: string
/// TsParameterBuilder.For("name", "string").AsReadonlyParameterProperty() // readonly name: string
/// </code>
/// </example>
public readonly record struct TsParameterBuilder
{
    /// <summary>Gets the parameter name.</summary>
    public string Name { get; init; }

    /// <summary>Gets the TypeScript type annotation.</summary>
    public string Type { get; init; }

    /// <summary>Gets the optional default value expression.</summary>
    public string? DefaultValue { get; init; }

    /// <summary>Gets a value indicating whether the parameter is optional (<c>?</c> suffix).</summary>
    public bool IsOptional { get; init; }

    /// <summary>Gets a value indicating whether the parameter is a rest parameter (<c>...</c> prefix).</summary>
    public bool IsRest { get; init; }

    /// <summary>Gets the accessibility modifier when used as a constructor parameter property.</summary>
    public TsAccessibility? ParameterPropertyAccessibility { get; init; }

    /// <summary>Gets a value indicating whether the parameter is a readonly constructor parameter property.</summary>
    public bool IsReadonlyParameterProperty { get; init; }

    /// <summary>Creates a new <see cref="TsParameterBuilder"/> for a parameter with the given name and type.</summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The TypeScript type annotation.</param>
    public static TsParameterBuilder For(string name, string type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Parameter name cannot be null or whitespace.", nameof(name));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Parameter type cannot be null or whitespace.", nameof(type));

        return new TsParameterBuilder { Name = name, Type = type };
    }

    /// <summary>Sets the default value expression for the parameter.</summary>
    /// <param name="defaultValue">The default value expression (e.g., <c>"\"hello\""</c>, <c>"0"</c>).</param>
    public TsParameterBuilder WithDefaultValue(string defaultValue) =>
        this with { DefaultValue = defaultValue };

    /// <summary>Marks the parameter as optional (<c>name?: Type</c>).</summary>
    public TsParameterBuilder AsOptional() =>
        this with { IsOptional = true };

    /// <summary>Marks the parameter as a rest parameter (<c>...name: Type</c>).</summary>
    public TsParameterBuilder AsRest() =>
        this with { IsRest = true };

    /// <summary>Marks the parameter as a constructor parameter property with the given accessibility.</summary>
    /// <param name="accessibility">The accessibility level for the parameter property.</param>
    public TsParameterBuilder AsParameterProperty(TsAccessibility accessibility) =>
        this with { ParameterPropertyAccessibility = accessibility };

    /// <summary>Marks the parameter as a readonly constructor parameter property.</summary>
    public TsParameterBuilder AsReadonlyParameterProperty() =>
        this with { IsReadonlyParameterProperty = true };
}
