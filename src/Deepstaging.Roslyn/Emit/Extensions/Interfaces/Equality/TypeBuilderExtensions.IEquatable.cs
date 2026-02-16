// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Equality;

/// <summary>
/// TypeBuilder extensions for implementing IEquatable&lt;T&gt;.
/// Adds Equals(T), Equals(object?), GetHashCode(), and equality operators.
/// </summary>
public static class TypeBuilderEquatableExtensions
{
    /// <summary>
    /// Implements IEquatable&lt;T&gt; using semantic analysis of the backing type.
    /// Automatically generates appropriate Equals, GetHashCode, and operators.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol to delegate to.</param>
    /// <param name="valueAccessor">The property/field name to access the backing value (e.g., "Value").</param>
    /// <param name="stringComparison">For string-backed types, the comparison type to use. Defaults to Ordinal.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIEquatable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        var typeName = builder.Name;
        var info = EquatableTypeInfo.From(backingType);

        return builder
            .Implements($"global::System.IEquatable<{typeName}>")
            .AddEqualityOperator("left.Equals(right)")
            .AddInequalityOperator("!(left == right)")
            .AddMethod(BuildEqualsMethod(typeName, info, valueAccessor, stringComparison))
            .AddMethod(BuildEqualsObjectMethod(typeName))
            .AddMethod(BuildGetHashCodeMethod(info, valueAccessor, stringComparison));
    }

    /// <summary>
    /// Implements IEquatable&lt;T&gt; using custom expression bodies.
    /// Use when semantic detection isn't sufficient for your type.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="equalsExpression">Expression body for Equals(T) method (e.g., "Value.Equals(other.Value)").</param>
    /// <param name="hashCodeExpression">Expression body for GetHashCode() method (e.g., "Value.GetHashCode()").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIEquatable(
        this TypeBuilder builder,
        string equalsExpression,
        string hashCodeExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.IEquatable<{typeName}>")
            .AddEqualityOperator("left.Equals(right)")
            .AddInequalityOperator("!(left == right)")
            .AddMethod(MethodBuilder
                .Parse($"public bool Equals({typeName} other)")
                .WithInheritDoc("global::System.IEquatable{{T}}")
                .WithExpressionBody(equalsExpression))
            .AddMethod(BuildEqualsObjectMethod(typeName))
            .AddMethod(MethodBuilder
                .Parse("public override int GetHashCode()")
                .WithExpressionBody(hashCodeExpression));
    }

    private static MethodBuilder BuildEqualsMethod(
        string typeName,
        EquatableTypeInfo info,
        string valueAccessor,
        StringComparison stringComparison)
    {
        var method = MethodBuilder
            .Parse($"public bool Equals({typeName} other)")
            .WithInheritDoc("global::System.IEquatable{{T}}");

        if (info.RequiresNullHandling)
        {
            var comparisonName = $"global::System.StringComparison.{stringComparison}";

            // Null-safe pattern for reference types
            return method.WithBody(b => b.AddStatements($$"""
                                                          return ({{valueAccessor}}, other.{{valueAccessor}}) switch
                                                          {
                                                              (null, null) => true,
                                                              (null, _) => false,
                                                              (_, null) => false,
                                                              (_, _) => {{valueAccessor}}.Equals(other.{{valueAccessor}}, {{comparisonName}}),
                                                          };
                                                          """));
        }

        // Simple delegation for value types
        return method.WithExpressionBody($"{valueAccessor}.Equals(other.{valueAccessor})");
    }

    private static MethodBuilder BuildEqualsObjectMethod(string typeName) =>
        MethodBuilder
            .Parse("public override bool Equals(object? obj)")
            .WithBody(b => b
                .AddStatement("if (ReferenceEquals(null, obj)) return false;")
                .AddStatement($"return obj is {typeName} other && Equals(other);"));

    private static MethodBuilder BuildGetHashCodeMethod(
        EquatableTypeInfo info,
        string valueAccessor,
        StringComparison stringComparison)
    {
        if (info.RequiresNullHandling)
        {
            // For case-insensitive comparisons, we need a case-insensitive hash
            if (stringComparison is StringComparison.OrdinalIgnoreCase or StringComparison.CurrentCultureIgnoreCase
                or StringComparison.InvariantCultureIgnoreCase)
            {
                var comparerName = stringComparison switch
                {
                    StringComparison.OrdinalIgnoreCase => "global::System.StringComparer.OrdinalIgnoreCase",
                    StringComparison.CurrentCultureIgnoreCase =>
                        "global::System.StringComparer.CurrentCultureIgnoreCase",
                    StringComparison.InvariantCultureIgnoreCase =>
                        "global::System.StringComparer.InvariantCultureIgnoreCase",
                    _ => "global::System.StringComparer.Ordinal"
                };

                return MethodBuilder
                    .Parse("public override int GetHashCode()")
                    .WithExpressionBody($"{valueAccessor} is null ? 0 : {comparerName}.GetHashCode({valueAccessor})");
            }

            return MethodBuilder
                .Parse("public override int GetHashCode()")
                .WithExpressionBody($"{valueAccessor}?.GetHashCode() ?? 0");
        }

        return MethodBuilder
            .Parse("public override int GetHashCode()")
            .WithExpressionBody($"{valueAccessor}.GetHashCode()");
    }

    /// <summary>
    /// Implements IEquatable&lt;T&gt; using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsIEquatable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property,
        StringComparison stringComparison = StringComparison.Ordinal) =>
        builder.ImplementsIEquatable(backingType, property.Name, stringComparison);

    /// <summary>
    /// Implements IEquatable&lt;T&gt; using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsIEquatable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field,
        StringComparison stringComparison = StringComparison.Ordinal) =>
        builder.ImplementsIEquatable(backingType, field.Name, stringComparison);
}