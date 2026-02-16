// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Patterns;

/// <summary>
/// Factory pattern extensions for TypeBuilder.
/// Provides fluent methods to add common factory patterns like Empty, Default, and New.
/// </summary>
public static partial class TypeBuilderFactoryExtensions
{
    /// <summary>
    /// Adds a static readonly Empty field with the specified initialization expression.
    /// Common pattern for value types and strongly-typed IDs.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="initExpression">The initialization expression (e.g., "new MyType(0)", "default").</param>
    /// <example>
    /// <code>
    /// TypeBuilder.Parse("public partial struct UserId")
    ///     .WithEmptyFactory("new UserId(Guid.Empty)");
    /// // Generates: public static readonly UserId Empty = new UserId(Guid.Empty);
    /// </code>
    /// </example>
    public static TypeBuilder WithEmptyFactory(this TypeBuilder builder, string initExpression)
    {
        var typeName = builder.Name;

        return builder.AddField(FieldBuilder
            .Parse($"public static readonly {typeName} Empty = {initExpression}"));
    }

    /// <summary>
    /// Adds a static readonly Default field with the specified initialization expression.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="initExpression">The initialization expression.</param>
    public static TypeBuilder WithDefaultFactory(this TypeBuilder builder, string initExpression)
    {
        var typeName = builder.Name;

        return builder.AddField(FieldBuilder
            .Parse($"public static readonly {typeName} Default = {initExpression}"));
    }

    /// <summary>
    /// Adds a static New() factory method that creates a new instance.
    /// Commonly used for types with generated values (e.g., Guid.NewGuid()).
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="expression">The expression to create a new instance (e.g., "new MyType(Guid.NewGuid())").</param>
    /// <example>
    /// <code>
    /// TypeBuilder.Parse("public partial struct UserId")
    ///     .WithNewFactory("new UserId(Guid.NewGuid())");
    /// // Generates: public static UserId New() => new UserId(Guid.NewGuid());
    /// </code>
    /// </example>
    public static TypeBuilder WithNewFactory(this TypeBuilder builder, string expression)
    {
        var typeName = builder.Name;

        return builder.AddMethod(MethodBuilder
            .Parse($"public static {typeName} New()")
            .WithExpressionBody(expression));
    }

    /// <summary>
    /// Adds both Empty field and New() factory method.
    /// Common pattern for ID types backed by Guid.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="emptyExpression">The expression for the Empty field.</param>
    /// <param name="newExpression">The expression for the New() method.</param>
    public static TypeBuilder WithEmptyAndNewFactory(
        this TypeBuilder builder,
        string emptyExpression,
        string newExpression) => builder
        .WithEmptyFactory(emptyExpression)
        .WithNewFactory(newExpression);
}