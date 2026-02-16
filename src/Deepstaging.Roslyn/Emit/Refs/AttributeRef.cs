// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit;

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Represents an attribute type reference for use in code generation.
/// Provides a fluent bridge to <see cref="AttributeBuilder"/> for configuring arguments.
/// Has implicit conversions to <see cref="string"/> and <see cref="AttributeBuilder"/>
/// so it works directly with existing <c>.WithAttribute()</c> overloads on all builders.
/// </summary>
/// <example>
/// <code>
/// // Simple attribute — implicit string conversion
/// builder.WithAttribute(EntityFrameworkRefs.Key)
///
/// // With positional argument — .WithArgument returns AttributeBuilder
/// builder.WithAttribute(EntityFrameworkRefs.Column.WithArgument("\"name\""))
///
/// // With configure callback — implicit string conversion for name
/// builder.WithAttribute(EntityFrameworkRefs.Column, a => a
///     .WithNamedArgument("TypeName", "\"nvarchar(100)\""))
/// </code>
/// </example>
public readonly record struct AttributeRef
{
    /// <summary>Gets the attribute type name (e.g., "global::System.ComponentModel.DataAnnotations.KeyAttribute").</summary>
    public string Value { get; }

    private AttributeRef(string value) => Value = value;

    #region Factory Methods

    /// <summary>Creates an attribute reference from a type name.</summary>
    /// <param name="name">The attribute type name (e.g., "System.ComponentModel.DataAnnotations.KeyAttribute").</param>
    public static AttributeRef From(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Attribute name cannot be null or whitespace.", nameof(name));

        return new AttributeRef(name);
    }

    /// <summary>Creates a globally-qualified attribute reference with <c>global::</c> prefix.</summary>
    /// <param name="fullyQualifiedName">The fully qualified attribute type name without the global:: prefix.</param>
    public static AttributeRef Global(string fullyQualifiedName)
    {
        if (string.IsNullOrWhiteSpace(fullyQualifiedName))
            throw new ArgumentException("Fully qualified name cannot be null or whitespace.",
                nameof(fullyQualifiedName));

        return new AttributeRef($"global::{fullyQualifiedName}");
    }

    #endregion

    #region Bridge to AttributeBuilder

    /// <summary>
    /// Adds a positional argument, returning an <see cref="AttributeBuilder"/> for further configuration.
    /// </summary>
    /// <param name="value">The argument value as a code expression (e.g., "\"message\"", "typeof(int)", "42").</param>
    public AttributeBuilder WithArgument(string value) =>
        AttributeBuilder.For(Value).WithArgument(value);

    /// <summary>
    /// Adds multiple positional arguments, returning an <see cref="AttributeBuilder"/> for further configuration.
    /// </summary>
    /// <param name="values">The argument values as code expressions.</param>
    public AttributeBuilder WithArguments(params string[] values) =>
        AttributeBuilder.For(Value).WithArguments(values);

    /// <summary>
    /// Adds a named argument, returning an <see cref="AttributeBuilder"/> for further configuration.
    /// </summary>
    /// <param name="name">The argument name (e.g., "ErrorMessage", "AllowMultiple").</param>
    /// <param name="value">The argument value as a code expression (e.g., "\"message\"", "true").</param>
    public AttributeBuilder WithNamedArgument(string name, string value) =>
        AttributeBuilder.For(Value).WithNamedArgument(name, value);

    /// <summary>
    /// Adds a using directive, returning an <see cref="AttributeBuilder"/> for further configuration.
    /// </summary>
    /// <param name="namespace">The namespace to add (e.g., "System.Linq", "static System.Math").</param>
    public AttributeBuilder AddUsing(string @namespace) =>
        AttributeBuilder.For(Value).AddUsing(@namespace);

    #endregion

    #region Conversions

    /// <summary>Returns the attribute type name.</summary>
    public override string ToString() => Value;

    /// <summary>Implicitly converts an <see cref="AttributeRef"/> to a <see cref="string"/>.</summary>
    public static implicit operator string(AttributeRef attr) => attr.Value;

    /// <summary>Implicitly converts an <see cref="AttributeRef"/> to an <see cref="AttributeBuilder"/>.</summary>
    public static implicit operator AttributeBuilder(AttributeRef attr) => AttributeBuilder.For(attr.Value);

    #endregion
}