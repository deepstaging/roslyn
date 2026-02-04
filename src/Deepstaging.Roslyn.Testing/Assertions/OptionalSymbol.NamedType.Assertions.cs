// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.Testing.Assertions;

/// <summary>
/// TUnit assertions for OptionalSymbol&lt;INamedTypeSymbol&gt;.
/// These assertions provide fluent API for testing optional type symbols.
/// </summary>
public static partial class OptionalSymbolNamedTypeAssertions
{
    /// <summary>
    /// Asserts that the type symbol is a class (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be a class")]
    public static bool IsClassSymbol(this OptionalSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.Map(x => x.Value.TypeKind == TypeKind.Class).Value;
    }

    /// <summary>
    /// Asserts that the type symbol is an interface (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be an interface")]
    public static bool IsInterfaceSymbol(this OptionalSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.Map(x => x.Value.TypeKind == TypeKind.Interface).Value;
    }

    /// <summary>
    /// Asserts that the type symbol is a struct (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be a struct")]
    public static bool IsStructSymbol(this OptionalSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.Map(x => x.Value.TypeKind == TypeKind.Struct).Value;
    }

    /// <summary>
    /// Asserts that the type symbol is a record (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be a record")]
    public static bool IsRecordSymbol(this OptionalSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.Map(x => x.Value.IsRecord).Value;
    }

    /// <summary>
    /// Asserts that the type symbol is partial (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be partial")]
    public static bool IsPartialSymbol(this OptionalSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.Map(x => x.Value.DeclaringSyntaxReferences.Length > 1).Value;
    }

    /// <summary>
    /// Asserts that the type symbol is an enum (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be an enum")]
    public static bool IsEnumSymbol(this OptionalSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.Map(x => x.Value.TypeKind == TypeKind.Enum).Value;
    }

    /// <summary>
    /// Asserts that the type symbol is a delegate (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be a delegate")]
    public static bool IsDelegateSymbol(this OptionalSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.Map(x => x.Value.TypeKind == TypeKind.Delegate).Value;
    }

    /// <summary>
    /// Asserts that the type symbol is generic (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be generic")]
    public static bool IsGenericSymbol(this OptionalSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.IsGenericType;
    }

    /// <summary>
    /// Asserts that the type symbol has the specified number of type parameters (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have {count} type parameters")]
    public static bool HasTypeParameterCount(this OptionalSymbol<INamedTypeSymbol> symbol, int count)
    {
        return symbol.GetTypeParameters().Count() == count;
    }
}
