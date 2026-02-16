// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Comparison;

/// <summary>
/// TypeBuilder extensions for implementing IComparer&lt;T&gt;.
/// Creates a standalone comparer class for custom sorting logic.
/// </summary>
public static class TypeBuilderComparerExtensions
{
    /// <summary>
    /// Implements IComparer&lt;T&gt; by comparing a property/field of the items.
    /// Generates: int Compare(T x, T y) using the specified accessor.
    /// </summary>
    /// <param name="builder">The type builder (should be a comparer class).</param>
    /// <param name="comparedType">The type being compared.</param>
    /// <param name="valueAccessor">The property/field to compare (e.g., "Value", "Name").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIComparer(
        this TypeBuilder builder,
        string comparedType,
        string valueAccessor) =>
        builder
            .Implements($"global::System.Collections.Generic.IComparer<{comparedType}>")
            .AddMethod(MethodBuilder
                .Parse($"public int Compare({comparedType}? x, {comparedType}? y)")
                .WithBody(b => b
                    .AddStatement("if (ReferenceEquals(x, y)) return 0;")
                    .AddStatement("if (x is null) return -1;")
                    .AddStatement("if (y is null) return 1;")
                    .AddStatement(
                        $"return global::System.Collections.Generic.Comparer<{GetPropertyType(valueAccessor)}>.Default.Compare(x.{valueAccessor}, y.{valueAccessor});")));

    /// <summary>
    /// Implements IComparer&lt;T&gt; for value types by comparing a property/field.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="comparedType">The value type being compared.</param>
    /// <param name="valueAccessor">The property/field to compare.</param>
    /// <param name="propertyType">The type of the property being compared.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIComparerForValueType(
        this TypeBuilder builder,
        string comparedType,
        string valueAccessor,
        string propertyType) => builder
        .Implements($"global::System.Collections.Generic.IComparer<{comparedType}>")
        .AddMethod(MethodBuilder
            .Parse($"public int Compare({comparedType} x, {comparedType} y)")
            .WithExpressionBody(
                $"global::System.Collections.Generic.Comparer<{propertyType}>.Default.Compare(x.{valueAccessor}, y.{valueAccessor})"));

    /// <summary>
    /// Implements IComparer&lt;T&gt; with a custom comparison body.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="comparedType">The type being compared.</param>
    /// <param name="compareBody">Action to configure the Compare method body.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIComparer(
        this TypeBuilder builder,
        string comparedType,
        Func<BodyBuilder, BodyBuilder> compareBody) => builder
        .Implements($"global::System.Collections.Generic.IComparer<{comparedType}>")
        .AddMethod(MethodBuilder
            .Parse($"public int Compare({comparedType}? x, {comparedType}? y)")
            .WithBody(compareBody));

    // Helper to infer property type - simplified, assumes var for now
    private static string GetPropertyType(string accessor) => "var";
}