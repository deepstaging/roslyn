// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.Testing.Assertions;

/// <summary>
/// TUnit assertions for ValidSymbol&lt;INamedTypeSymbol&gt;.
/// These assertions provide fluent API for testing type symbols.
/// </summary>
public static partial class ValidSymbolNamedTypeAssertions
{
    /// <summary>
    /// Asserts that the type symbol is a class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be a class")]
    public static bool IsClassSymbol(this ValidSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.Value.TypeKind == TypeKind.Class;
    }

    /// <summary>
    /// Asserts that the type symbol is an interface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be an interface")]
    public static bool IsInterfaceSymbol(this ValidSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.Value.TypeKind == TypeKind.Interface;
    }

    /// <summary>
    /// Asserts that the type symbol is a struct.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be a struct")]
    public static bool IsStructSymbol(this ValidSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.Value.TypeKind == TypeKind.Struct;
    }

    /// <summary>
    /// Asserts that the type symbol is a record.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be a record")]
    public static bool IsRecordSymbol(this ValidSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.IsRecord;
    }

    /// <summary>
    /// Asserts that the type symbol is partial.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be partial")]
    public static bool IsPartialSymbol(this ValidSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.IsPartial;
    }

    /// <summary>
    /// Asserts that the type symbol is an enum.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be an enum")]
    public static bool IsEnumSymbol(this ValidSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.Value.TypeKind == TypeKind.Enum;
    }

    /// <summary>
    /// Asserts that the type symbol is a delegate.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be a delegate")]
    public static bool IsDelegateSymbol(this ValidSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.Value.TypeKind == TypeKind.Delegate;
    }

    /// <summary>
    /// Asserts that the type symbol is generic.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be generic")]
    public static bool IsGenericSymbol(this ValidSymbol<INamedTypeSymbol> symbol)
    {
        return symbol.IsGenericType;
    }

    /// <summary>
    /// Asserts that the type symbol has the specified number of type parameters.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have {count} type parameters")]
    public static bool HasTypeParameterCount(this ValidSymbol<INamedTypeSymbol> symbol, int count)
    {
        return symbol.GetTypeParameters().Count() == count;
    }
}
