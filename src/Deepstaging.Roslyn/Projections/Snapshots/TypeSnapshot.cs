// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Pipeline-safe snapshot of a named type symbol (class, struct, interface, enum, delegate).
/// Mirrors <see cref="ValidSymbol{TSymbol}"/> for <c>INamedTypeSymbol</c>.
/// </summary>
public sealed record TypeSnapshot : SymbolSnapshot, IEquatable<TypeSnapshot>
{
    #region Type Identity

    /// <summary>
    /// Gets a value indicating whether the symbol is an interface.
    /// </summary>
    public bool IsInterface { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is a class.
    /// </summary>
    public bool IsClass { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is a struct.
    /// </summary>
    public bool IsStruct { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is a record.
    /// </summary>
    public bool IsRecord { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is an enum.
    /// </summary>
    public bool IsEnum { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is a delegate.
    /// </summary>
    public bool IsDelegate { get; init; }

    #endregion

    #region Generics

    /// <summary>
    /// Gets a value indicating whether the symbol is a generic type.
    /// </summary>
    public bool IsGenericType { get; init; }

    /// <summary>
    /// Gets the arity (number of type parameters) of the type.
    /// </summary>
    public int Arity { get; init; }

    /// <summary>
    /// Gets the globally qualified type argument names.
    /// </summary>
    public EquatableArray<string> TypeArgumentNames { get; init; } = [];

    #endregion

    #region Type Hierarchy

    /// <summary>
    /// Gets the base type globally qualified name, or null if none.
    /// </summary>
    public string? BaseTypeName { get; init; }

    /// <summary>
    /// Gets the globally qualified names of directly implemented interfaces.
    /// </summary>
    public EquatableArray<string> InterfaceNames { get; init; } = [];

    #endregion
}
