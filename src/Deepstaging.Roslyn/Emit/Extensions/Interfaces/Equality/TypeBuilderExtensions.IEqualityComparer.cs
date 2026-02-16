// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Equality;

/// <summary>
/// TypeBuilder extensions for implementing IEqualityComparer&lt;T&gt;.
/// Creates a standalone comparer class for custom equality logic in collections.
/// </summary>
public static class TypeBuilderEqualityComparerExtensions
{
    /// <summary>
    /// Implements IEqualityComparer&lt;T&gt; by comparing a property/field of the items.
    /// Generates: bool Equals(T x, T y) and int GetHashCode(T obj).
    /// </summary>
    /// <param name="builder">The type builder (should be a comparer class).</param>
    /// <param name="comparedType">The type being compared.</param>
    /// <param name="valueAccessor">The property/field to compare (e.g., "Value", "Id").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIEqualityComparer(
        this TypeBuilder builder,
        string comparedType,
        string valueAccessor) =>
        builder
            .Implements($"global::System.Collections.Generic.IEqualityComparer<{comparedType}>")
            .AddMethod(MethodBuilder
                .Parse($"public bool Equals({comparedType}? x, {comparedType}? y)")
                .WithBody(b => b
                    .AddStatement("if (ReferenceEquals(x, y)) return true;")
                    .AddStatement("if (x is null || y is null) return false;")
                    .AddStatement(
                        $"return global::System.Collections.Generic.EqualityComparer<{GetPropertyType(valueAccessor)}>.Default.Equals(x.{valueAccessor}, y.{valueAccessor});")))
            .AddMethod(MethodBuilder
                .Parse($"public int GetHashCode({comparedType} obj)")
                .WithExpressionBody($"obj.{valueAccessor}?.GetHashCode() ?? 0"));

    /// <summary>
    /// Implements IEqualityComparer&lt;T&gt; for value types.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="comparedType">The value type being compared.</param>
    /// <param name="valueAccessor">The property/field to compare.</param>
    /// <param name="propertyType">The type of the property being compared.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIEqualityComparerForValueType(
        this TypeBuilder builder,
        string comparedType,
        string valueAccessor,
        string propertyType) => builder
        .Implements($"global::System.Collections.Generic.IEqualityComparer<{comparedType}>")
        .AddMethod(MethodBuilder
            .Parse($"public bool Equals({comparedType} x, {comparedType} y)")
            .WithExpressionBody(
                $"global::System.Collections.Generic.EqualityComparer<{propertyType}>.Default.Equals(x.{valueAccessor}, y.{valueAccessor})"))
        .AddMethod(MethodBuilder
            .Parse($"public int GetHashCode({comparedType} obj)")
            .WithExpressionBody($"obj.{valueAccessor}.GetHashCode()"));

    /// <summary>
    /// Implements IEqualityComparer&lt;T&gt; comparing multiple properties.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="comparedType">The type being compared.</param>
    /// <param name="valueAccessors">The properties/fields to compare.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIEqualityComparer(
        this TypeBuilder builder,
        string comparedType,
        params string[] valueAccessors)
    {
        var equalsConditions = string.Join(" && ",
            valueAccessors.Select(v =>
                $"global::System.Collections.Generic.EqualityComparer<object>.Default.Equals(x.{v}, y.{v})"));

        var hashCodeCombine = string.Join(", ",
            valueAccessors.Select(v => $"obj.{v}"));

        return builder
            .Implements($"global::System.Collections.Generic.IEqualityComparer<{comparedType}>")
            .AddMethod(MethodBuilder
                .Parse($"public bool Equals({comparedType}? x, {comparedType}? y)")
                .WithBody(b => b
                    .AddStatement("if (ReferenceEquals(x, y)) return true;")
                    .AddStatement("if (x is null || y is null) return false;")
                    .AddStatement($"return {equalsConditions};")))
            .AddMethod(MethodBuilder
                .Parse($"public int GetHashCode({comparedType} obj)")
                .WithExpressionBody($"global::System.HashCode.Combine({hashCodeCombine})"));
    }

    private static string GetPropertyType(string accessor) => "object";
}