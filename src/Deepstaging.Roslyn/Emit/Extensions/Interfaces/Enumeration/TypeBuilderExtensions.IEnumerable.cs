// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Enumeration;

/// <summary>
/// TypeBuilder extensions for implementing IEnumerable&lt;T&gt;.
/// Delegates enumeration to an inner collection.
/// </summary>
public static class TypeBuilderEnumerableExtensions
{
    /// <summary>
    /// Implements IEnumerable&lt;T&gt; by delegating to an inner collection property/field.
    /// Generates: GetEnumerator() methods that delegate to the collection.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="elementType">The element type of the enumerable.</param>
    /// <param name="collectionAccessor">The property/field that holds the collection (e.g., "_items", "Items").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIEnumerable(
        this TypeBuilder builder,
        string elementType,
        string collectionAccessor) => builder
        .Implements($"global::System.Collections.Generic.IEnumerable<{elementType}>")
        .AddMethod(MethodBuilder
            .Parse($"public global::System.Collections.Generic.IEnumerator<{elementType}> GetEnumerator()")
            .WithExpressionBody($"{collectionAccessor}.GetEnumerator()"))
        .AddMethod(MethodBuilder
            .Parse("global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()")
            .WithExpressionBody("GetEnumerator()"));

    /// <summary>
    /// Implements IEnumerable&lt;T&gt; with a custom enumerator expression.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="elementType">The element type.</param>
    /// <param name="enumeratorExpression">Expression that returns an IEnumerator&lt;T&gt;.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIEnumerableWith(
        this TypeBuilder builder,
        string elementType,
        string enumeratorExpression) => builder
        .Implements($"global::System.Collections.Generic.IEnumerable<{elementType}>")
        .AddMethod(MethodBuilder
            .Parse($"public global::System.Collections.Generic.IEnumerator<{elementType}> GetEnumerator()")
            .WithExpressionBody(enumeratorExpression))
        .AddMethod(MethodBuilder
            .Parse("global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()")
            .WithExpressionBody("GetEnumerator()"));

    /// <summary>
    /// Implements IReadOnlyCollection&lt;T&gt; by delegating to an inner collection.
    /// Adds Count property in addition to enumeration.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="elementType">The element type.</param>
    /// <param name="collectionAccessor">The property/field that holds the collection.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIReadOnlyCollection(
        this TypeBuilder builder,
        string elementType,
        string collectionAccessor) => builder
        .ImplementsIEnumerable(elementType, collectionAccessor)
        .Implements($"global::System.Collections.Generic.IReadOnlyCollection<{elementType}>")
        .AddProperty(PropertyBuilder
            .For("Count", "int")
            .WithGetter($"{collectionAccessor}.Count"));

    /// <summary>
    /// Implements IReadOnlyList&lt;T&gt; by delegating to an inner list.
    /// Adds indexed access in addition to count and enumeration.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="elementType">The element type.</param>
    /// <param name="listAccessor">The property/field that holds the list.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIReadOnlyList(
        this TypeBuilder builder,
        string elementType,
        string listAccessor) => builder
        .ImplementsIReadOnlyCollection(elementType, listAccessor)
        .Implements($"global::System.Collections.Generic.IReadOnlyList<{elementType}>")
        .AddProperty(PropertyBuilder
            .For($"this[int index]", elementType)
            .WithGetter($"{listAccessor}[index]"));
}