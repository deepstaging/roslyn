// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Comparison;

/// <summary>
/// TypeBuilder extensions for implementing IComparable&lt;T&gt;.
/// Adds CompareTo(T) and comparison operators.
/// </summary>
public static class TypeBuilderComparableExtensions
{
    /// <summary>
    /// Implements IComparable&lt;T&gt; using semantic analysis of the backing type.
    /// Automatically generates appropriate CompareTo and comparison operators.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="backingType">The backing type symbol to delegate to.</param>
    /// <param name="valueAccessor">The property/field name to access the backing value (e.g., "Value").</param>
    /// <param name="stringComparison">For string-backed types, the comparison type to use. Defaults to Ordinal.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIComparable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        string valueAccessor,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        var typeName = builder.Name;
        var info = ComparableTypeInfo.From(backingType);

        return builder
            .Implements($"global::System.IComparable<{typeName}>")
            .AddOperator(OperatorBuilder.GreaterThan(typeName, "a", "b").WithExpressionBody("a.CompareTo(b) > 0"))
            .AddOperator(OperatorBuilder.LessThan(typeName, "a", "b").WithExpressionBody("a.CompareTo(b) < 0"))
            .AddOperator(OperatorBuilder.GreaterThanOrEqual(typeName, "a", "b")
                .WithExpressionBody("a.CompareTo(b) >= 0"))
            .AddOperator(OperatorBuilder.LessThanOrEqual(typeName, "a", "b").WithExpressionBody("a.CompareTo(b) <= 0"))
            .AddMethod(BuildCompareToMethod(typeName, info, valueAccessor, stringComparison));
    }

    /// <summary>
    /// Implements IComparable&lt;T&gt; using a custom expression body.
    /// Use when semantic detection isn't sufficient for your type.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="compareToExpression">Expression body for CompareTo(T) method (e.g., "Value.CompareTo(other.Value)").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIComparable(
        this TypeBuilder builder,
        string compareToExpression)
    {
        var typeName = builder.Name;

        return builder
            .Implements($"global::System.IComparable<{typeName}>")
            .AddOperator(OperatorBuilder.GreaterThan(typeName, "a", "b").WithExpressionBody("a.CompareTo(b) > 0"))
            .AddOperator(OperatorBuilder.LessThan(typeName, "a", "b").WithExpressionBody("a.CompareTo(b) < 0"))
            .AddOperator(OperatorBuilder.GreaterThanOrEqual(typeName, "a", "b")
                .WithExpressionBody("a.CompareTo(b) >= 0"))
            .AddOperator(OperatorBuilder.LessThanOrEqual(typeName, "a", "b").WithExpressionBody("a.CompareTo(b) <= 0"))
            .AddMethod(MethodBuilder
                .Parse($"public int CompareTo({typeName} other)")
                .WithInheritDoc("global::System.IComparable{{TSelf}}")
                .WithExpressionBody(compareToExpression));
    }

    private static MethodBuilder BuildCompareToMethod(
        string typeName,
        ComparableTypeInfo info,
        string valueAccessor,
        StringComparison stringComparison)
    {
        var method = MethodBuilder
            .Parse($"public int CompareTo({typeName} other)")
            .WithInheritDoc("global::System.IComparable{{TSelf}}");

        if (info.RequiresNullHandling)
        {
            var compareCall = GetStringCompareCall(valueAccessor, stringComparison);

            // Null-safe pattern for reference types (like string)
            return method.WithBody(b => b.AddStatements($$"""
                                                          return ({{valueAccessor}}, other.{{valueAccessor}}) switch
                                                          {
                                                              (null, null) => 0,
                                                              (null, _) => -1,
                                                              (_, null) => 1,
                                                              (_, _) => {{compareCall}},
                                                          };
                                                          """));
        }

        // Simple delegation for value types
        return method.WithExpressionBody($"{valueAccessor}.CompareTo(other.{valueAccessor})");
    }

    private static string GetStringCompareCall(string valueAccessor, StringComparison stringComparison) =>
        stringComparison switch
        {
            StringComparison.Ordinal => $"string.CompareOrdinal({valueAccessor}, other.{valueAccessor})",
            StringComparison.OrdinalIgnoreCase =>
                $"string.Compare({valueAccessor}, other.{valueAccessor}, global::System.StringComparison.OrdinalIgnoreCase)",
            _ =>
                $"string.Compare({valueAccessor}, other.{valueAccessor}, global::System.StringComparison.{stringComparison})"
        };

    /// <summary>
    /// Implements IComparable&lt;T&gt; using a property as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsIComparable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        PropertyBuilder property,
        StringComparison stringComparison = StringComparison.Ordinal) =>
        builder.ImplementsIComparable(backingType, property.Name, stringComparison);

    /// <summary>
    /// Implements IComparable&lt;T&gt; using a field as the backing value.
    /// </summary>
    public static TypeBuilder ImplementsIComparable(
        this TypeBuilder builder,
        TypeSnapshot backingType,
        FieldBuilder field,
        StringComparison stringComparison = StringComparison.Ordinal) =>
        builder.ImplementsIComparable(backingType, field.Name, stringComparison);
}