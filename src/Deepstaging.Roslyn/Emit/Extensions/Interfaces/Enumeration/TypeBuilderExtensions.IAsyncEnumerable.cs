// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Enumeration;

/// <summary>
/// TypeBuilder extensions for implementing IAsyncEnumerable&lt;T&gt; (NET Core 3.0+).
/// </summary>
public static class TypeBuilderAsyncEnumerableExtensions
{
    /// <summary>
    /// Implements IAsyncEnumerable&lt;T&gt; by delegating to an inner async collection.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="elementType">The element type.</param>
    /// <param name="collectionAccessor">The property/field that holds the async enumerable.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIAsyncEnumerable(
        this TypeBuilder builder,
        string elementType,
        string collectionAccessor)
    {
        return builder
            .Implements($"global::System.Collections.Generic.IAsyncEnumerable<{elementType}>", Directives.NetCoreApp30OrGreater)
            .AddMethod(MethodBuilder
                .Parse($"public global::System.Collections.Generic.IAsyncEnumerator<{elementType}> GetAsyncEnumerator(global::System.Threading.CancellationToken cancellationToken = default)")
                .When(Directives.NetCoreApp30OrGreater)
                .WithExpressionBody($"{collectionAccessor}.GetAsyncEnumerator(cancellationToken)"));
    }

    /// <summary>
    /// Implements IAsyncEnumerable&lt;T&gt; with a custom async enumerator expression.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="elementType">The element type.</param>
    /// <param name="enumeratorExpression">Expression that returns an IAsyncEnumerator&lt;T&gt;.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIAsyncEnumerableWith(
        this TypeBuilder builder,
        string elementType,
        string enumeratorExpression)
    {
        return builder
            .Implements($"global::System.Collections.Generic.IAsyncEnumerable<{elementType}>", Directives.NetCoreApp30OrGreater)
            .AddMethod(MethodBuilder
                .Parse($"public global::System.Collections.Generic.IAsyncEnumerator<{elementType}> GetAsyncEnumerator(global::System.Threading.CancellationToken cancellationToken = default)")
                .When(Directives.NetCoreApp30OrGreater)
                .WithExpressionBody(enumeratorExpression));
    }

    /// <summary>
    /// Implements IAsyncEnumerable&lt;T&gt; using an async iterator method body.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="elementType">The element type.</param>
    /// <param name="iteratorBody">Configure the async iterator body (yield return statements).</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIAsyncEnumerableWithIterator(
        this TypeBuilder builder,
        string elementType,
        Func<BodyBuilder, BodyBuilder> iteratorBody)
    {
        return builder
            .Implements($"global::System.Collections.Generic.IAsyncEnumerable<{elementType}>", Directives.NetCoreApp30OrGreater)
            .AddMethod(MethodBuilder
                .Parse($"public async global::System.Collections.Generic.IAsyncEnumerator<{elementType}> GetAsyncEnumerator(global::System.Threading.CancellationToken cancellationToken = default)")
                .When(Directives.NetCoreApp30OrGreater)
                .WithBody(iteratorBody));
    }
}
