// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Pipeline-safe snapshot of a method symbol.
/// Mirrors <see cref="ValidSymbol{TSymbol}"/> for <c>IMethodSymbol</c>.
/// </summary>
public sealed record MethodSnapshot : SymbolSnapshot, IEquatable<MethodSnapshot>
{
    #region Return Type

    /// <summary>
    /// Gets the globally qualified return type name.
    /// </summary>
    public required string ReturnType { get; init; }

    /// <summary>
    /// Gets whether the method returns void.
    /// </summary>
    public bool ReturnsVoid { get; init; }

    #endregion

    #region Async

    /// <summary>
    /// Gets the async method kind.
    /// </summary>
    public AsyncMethodKind AsyncKind { get; init; }

    /// <summary>
    /// Gets the inner return type for async methods (T from Task&lt;T&gt;), or null.
    /// </summary>
    public string? AsyncReturnType { get; init; }

    #endregion

    #region Method Classification

    /// <summary>
    /// Gets the method kind (Ordinary, Constructor, PropertyGet, etc.).
    /// </summary>
    public MethodKind MethodKind { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is an extension method.
    /// </summary>
    public bool IsExtensionMethod { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is a generic method.
    /// </summary>
    public bool IsGenericMethod { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is a partial method.
    /// </summary>
    public bool IsPartialMethod { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is an async method.
    /// </summary>
    public bool IsAsync { get; init; }

    #endregion

    #region Parameters

    /// <summary>
    /// Gets the parameters of the method.
    /// </summary>
    public EquatableArray<ParameterSnapshot> Parameters { get; init; } = [];

    #endregion
}
