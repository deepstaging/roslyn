// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Cloning;

/// <summary>
/// TypeBuilder extensions for implementing ICloneable.
/// Adds Clone method for value types.
/// </summary>
public static class TypeBuilderCloneableExtensions
{
    /// <summary>
    /// Implements ICloneable for a value type.
    /// For value types (structs), Clone simply returns 'this' since structs are copied by value.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsICloneable(this TypeBuilder builder)
    {
        var typeName = builder.Name;

        return builder
            .Implements("global::System.ICloneable")
            .AddMethod(MethodBuilder
                .Parse("public object Clone()")
                .WithInheritDoc("global::System.ICloneable")
                .WithExpressionBody("this"));
    }

    /// <summary>
    /// Implements ICloneable with a custom clone expression.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="cloneExpression">The expression body for Clone (e.g., "new(Value)").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsICloneable(
        this TypeBuilder builder,
        string cloneExpression)
    {
        return builder
            .Implements("global::System.ICloneable")
            .AddMethod(MethodBuilder
                .Parse("public object Clone()")
                .WithInheritDoc("global::System.ICloneable")
                .WithExpressionBody(cloneExpression));
    }
}
