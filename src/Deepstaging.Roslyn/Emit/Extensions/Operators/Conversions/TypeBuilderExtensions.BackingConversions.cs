// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Operators.Conversions;

/// <summary>
/// TypeBuilder extensions for adding implicit and explicit conversion operators
/// between the type and its backing type.
/// </summary>
public static class TypeBuilderBackingConversionsExtensions
{
    /// <summary>
    /// Adds implicit and explicit conversion operators between this type and its backing type.
    /// Generates: implicit operator T(BackingType value) and explicit operator BackingType(T value).
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol.</param>
    /// <param name="valueAccessor">The property/field name to access the backing value (e.g., "Value").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder WithBackingConversions(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var backingTypeName = backingType.CodeName;

        return builder
            .AddImplicitConversion(backingTypeName, op => op
                .WithExpressionBody($"new {typeName}(value)")
                .WithXmlDoc($"Implicitly converts a <see cref=\"{backingType.Name}\"/> to a <see cref=\"{typeName}\"/>."))
            .AddExplicitConversionTo(backingTypeName, op => op
                .WithExpressionBody($"value.{valueAccessor}")
                .WithXmlDoc($"Explicitly converts a <see cref=\"{typeName}\"/> to its underlying <see cref=\"{backingType.Name}\"/> value."));
    }

    /// <summary>
    /// Adds implicit and explicit conversion operators using custom expressions.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingTypeName">The fully qualified backing type name.</param>
    /// <param name="fromBackingExpression">Expression to convert from backing type to this type (e.g., "new MyType(value)").</param>
    /// <param name="toBackingExpression">Expression to convert from this type to backing type (e.g., "value.Value").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder WithBackingConversions(
        this TypeBuilder builder,
        string backingTypeName,
        string fromBackingExpression,
        string toBackingExpression)
    {
        return builder
            .AddImplicitConversion(backingTypeName, op => op
                .WithExpressionBody(fromBackingExpression))
            .AddExplicitConversionTo(backingTypeName, op => op
                .WithExpressionBody(toBackingExpression));
    }

    /// <summary>
    /// Adds only an implicit conversion from the backing type to this type.
    /// Generates: implicit operator T(BackingType value).
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder WithImplicitConversionFromBacking(
        this TypeBuilder builder,
        TypeSnapshot backingType)
    {
        var typeName = builder.Name;
        var backingTypeName = backingType.CodeName;

        return builder
            .AddImplicitConversion(backingTypeName, op => op
                .WithExpressionBody($"new {typeName}(value)")
                .WithXmlDoc($"Implicitly converts a <see cref=\"{backingType.Name}\"/> to a <see cref=\"{typeName}\"/>."));
    }

    /// <summary>
    /// Adds only an explicit conversion from this type to the backing type.
    /// Generates: explicit operator BackingType(T value).
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol.</param>
    /// <param name="valueAccessor">The property/field name to access the backing value (e.g., "Value").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder WithExplicitConversionToBacking(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor)
    {
        var typeName = builder.Name;
        var backingTypeName = backingType.CodeName;

        return builder
            .AddExplicitConversionTo(backingTypeName, op => op
                .WithExpressionBody($"value.{valueAccessor}")
                .WithXmlDoc($"Explicitly converts a <see cref=\"{typeName}\"/> to its underlying <see cref=\"{backingType.Name}\"/> value."));
    }

    /// <summary>
    /// Adds implicit and explicit conversion operators using a property as the backing value.
    /// </summary>
    public static TypeBuilder WithBackingConversions(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.WithBackingConversions(backingType, property.Name);

    /// <summary>
    /// Adds implicit and explicit conversion operators using a field as the backing value.
    /// </summary>
    public static TypeBuilder WithBackingConversions(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.WithBackingConversions(backingType, field.Name);

    /// <summary>
    /// Adds only an explicit conversion from this type to the backing type using a property.
    /// </summary>
    public static TypeBuilder WithExplicitConversionToBacking(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property) =>
        builder.WithExplicitConversionToBacking(backingType, property.Name);

    /// <summary>
    /// Adds only an explicit conversion from this type to the backing type using a field.
    /// </summary>
    public static TypeBuilder WithExplicitConversionToBacking(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field) =>
        builder.WithExplicitConversionToBacking(backingType, field.Name);
}
