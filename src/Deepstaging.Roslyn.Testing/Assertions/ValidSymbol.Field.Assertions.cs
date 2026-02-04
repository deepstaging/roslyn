// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.Testing.Assertions;

/// <summary>
/// TUnit assertions for ValidSymbol&lt;IFieldSymbol&gt;.
/// These assertions provide fluent API for testing field symbols.
/// </summary>
public static partial class alidSymbolFieldAssertions
{
    /// <summary>
    /// Asserts that the field symbol is read-only.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be read-only")]
    public static bool IsReadOnlySymbol(this ValidSymbol<IFieldSymbol> symbol)
    {
        return symbol.IsReadOnly;
    }

    /// <summary>
    /// Asserts that the field symbol is const.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be const")]
    public static bool IsConst(this ValidSymbol<IFieldSymbol> symbol)
    {
        return symbol.Value.IsConst;
    }

    /// <summary>
    /// Asserts that the field symbol is volatile.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be volatile")]
    public static bool IsVolatile(this ValidSymbol<IFieldSymbol> symbol)
    {
        return symbol.Value.IsVolatile;
    }

    /// <summary>
    /// Asserts that the field symbol has a constant value.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have a constant value")]
    public static bool HasConstantValue(this ValidSymbol<IFieldSymbol> symbol)
    {
        return symbol.Value.HasConstantValue;
    }
}
