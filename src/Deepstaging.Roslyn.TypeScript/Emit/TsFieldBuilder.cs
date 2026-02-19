// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Fluent builder for composing TypeScript class field declarations.
/// Immutable — each method returns a new instance via <c>with</c> expressions.
/// </summary>
/// <example>
/// <code>
/// TsFieldBuilder.For("name", "string")                        // name: string;
/// TsFieldBuilder.For("id", "number").AsReadonly()              // readonly id: number;
/// TsFieldBuilder.For("_count", "number")
///     .WithAccessibility(TsAccessibility.Private)
///     .WithInitializer("0")                                    // private _count: number = 0;
/// TsFieldBuilder.For("secret", "string").AsEsPrivate()         // #secret: string;
/// TsFieldBuilder.For("field", "Type").AsDeclare()              // declare field: Type;
/// </code>
/// </example>
public readonly record struct TsFieldBuilder
{
    /// <summary>Gets the field name.</summary>
    public string Name { get; init; }

    /// <summary>Gets the TypeScript type annotation.</summary>
    public string Type { get; init; }

    /// <summary>Gets the optional initializer expression.</summary>
    public string? Initializer { get; init; }

    /// <summary>Gets the accessibility modifier for the field.</summary>
    public TsAccessibility Accessibility { get; init; }

    /// <summary>Gets a value indicating whether the field is readonly.</summary>
    public bool IsReadonly { get; init; }

    /// <summary>Gets a value indicating whether the field is static.</summary>
    public bool IsStatic { get; init; }

    /// <summary>Gets a value indicating whether the field is optional (<c>?</c> suffix).</summary>
    public bool IsOptional { get; init; }

    /// <summary>Gets a value indicating whether the field uses ES private syntax (<c>#</c> prefix).</summary>
    public bool IsEsPrivate { get; init; }

    /// <summary>Gets a value indicating whether the field has the <c>declare</c> modifier.</summary>
    public bool IsDeclare { get; init; }

    /// <summary>Gets a value indicating whether the field has the <c>override</c> modifier.</summary>
    public bool IsOverride { get; init; }

    /// <summary>Gets a value indicating whether the field has the <c>abstract</c> modifier.</summary>
    public bool IsAbstract { get; init; }

    /// <summary>Creates a new <see cref="TsFieldBuilder"/> for a field with the given name and type.</summary>
    /// <param name="name">The field name.</param>
    /// <param name="type">The TypeScript type annotation.</param>
    public static TsFieldBuilder For(string name, string type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name cannot be null or whitespace.", nameof(name));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Field type cannot be null or whitespace.", nameof(type));

        return new TsFieldBuilder { Name = name, Type = type };
    }

    /// <summary>Sets the accessibility modifier for the field.</summary>
    /// <param name="accessibility">The accessibility level.</param>
    public TsFieldBuilder WithAccessibility(TsAccessibility accessibility) =>
        this with { Accessibility = accessibility };

    /// <summary>Marks the field as <c>readonly</c>.</summary>
    public TsFieldBuilder AsReadonly() =>
        this with { IsReadonly = true };

    /// <summary>Marks the field as <c>static</c>.</summary>
    public TsFieldBuilder AsStatic() =>
        this with { IsStatic = true };

    /// <summary>Marks the field as optional (<c>name?: Type</c>).</summary>
    public TsFieldBuilder AsOptional() =>
        this with { IsOptional = true };

    /// <summary>Marks the field as ES private (<c>#name</c> syntax).</summary>
    public TsFieldBuilder AsEsPrivate() =>
        this with { IsEsPrivate = true };

    /// <summary>Marks the field with the <c>declare</c> modifier.</summary>
    public TsFieldBuilder AsDeclare() =>
        this with { IsDeclare = true };

    /// <summary>Marks the field with the <c>override</c> modifier.</summary>
    public TsFieldBuilder AsOverride() =>
        this with { IsOverride = true };

    /// <summary>Marks the field with the <c>abstract</c> modifier.</summary>
    public TsFieldBuilder AsAbstract() =>
        this with { IsAbstract = true };

    /// <summary>Sets the initializer expression for the field.</summary>
    /// <param name="initializer">The initializer expression (e.g., <c>"0"</c>, <c>"[]"</c>).</param>
    public TsFieldBuilder WithInitializer(string initializer) =>
        this with { Initializer = initializer };
}
