// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Observable;

/// <summary>
/// TypeBuilder extensions for implementing IObserver&lt;T&gt;.
/// </summary>
public static class TypeBuilderObserverExtensions
{
    /// <summary>
    /// Implements IObserver&lt;T&gt; with expression bodies for each method.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="elementType">The type of elements being observed.</param>
    /// <param name="onNextExpression">Expression for OnNext (e.g., "_handler(value)").</param>
    /// <param name="onErrorExpression">Expression for OnError (default: "throw error").</param>
    /// <param name="onCompletedExpression">Expression for OnCompleted (default: "{ }").</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIObserver(
        this TypeBuilder builder,
        string elementType,
        string onNextExpression,
        string? onErrorExpression = null,
        string? onCompletedExpression = null)
    {
        return builder
            .Implements($"global::System.IObserver<{elementType}>")
            .AddMethod(MethodBuilder
                .Parse($"public void OnNext({elementType} value)")
                .WithExpressionBody(onNextExpression))
            .AddMethod(MethodBuilder
                .Parse("public void OnError(global::System.Exception error)")
                .WithBody(b => b.AddStatement(onErrorExpression ?? "throw error;")))
            .AddMethod(MethodBuilder
                .Parse("public void OnCompleted()")
                .WithBody(b => b.AddStatement(onCompletedExpression ?? "// No action on completed")));
    }

    /// <summary>
    /// Implements IObserver&lt;T&gt; with virtual methods for subclass override.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="elementType">The type of elements being observed.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIObserverVirtual(
        this TypeBuilder builder,
        string elementType)
    {
        return builder
            .Implements($"global::System.IObserver<{elementType}>")
            .AddMethod(MethodBuilder
                .Parse($"public virtual void OnNext({elementType} value)")
                .WithBody(b => b.AddStatement("// Override in derived class")))
            .AddMethod(MethodBuilder
                .Parse("public virtual void OnError(global::System.Exception error)")
                .WithBody(b => b.AddStatement("throw error;")))
            .AddMethod(MethodBuilder
                .Parse("public virtual void OnCompleted()")
                .WithBody(b => b.AddStatement("// Override in derived class")));
    }

    /// <summary>
    /// Implements IObserver&lt;T&gt; that delegates to action fields.
    /// Generates fields for the action delegates and wires them up.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="elementType">The type of elements being observed.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIObserverWithActionFields(
        this TypeBuilder builder,
        string elementType)
    {
        return builder
            .Implements($"global::System.IObserver<{elementType}>")
            .AddField(FieldBuilder.Parse($"private readonly global::System.Action<{elementType}>? _onNext"))
            .AddField(FieldBuilder.Parse("private readonly global::System.Action<global::System.Exception>? _onError"))
            .AddField(FieldBuilder.Parse("private readonly global::System.Action? _onCompleted"))
            .AddMethod(MethodBuilder
                .Parse($"public void OnNext({elementType} value)")
                .WithExpressionBody("_onNext?.Invoke(value)"))
            .AddMethod(MethodBuilder
                .Parse("public void OnError(global::System.Exception error)")
                .WithBody(b => b
                    .AddStatement("if (_onError is not null) _onError(error);")
                    .AddStatement("else throw error;")))
            .AddMethod(MethodBuilder
                .Parse("public void OnCompleted()")
                .WithExpressionBody("_onCompleted?.Invoke()"));
    }
}
