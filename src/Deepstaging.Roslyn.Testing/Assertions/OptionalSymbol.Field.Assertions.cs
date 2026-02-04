// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.Testing.Assertions;

/// <summary>
/// TUnit assertions for OptionalSymbol&lt;IFieldSymbol&gt;.
/// These assertions provide fluent API for testing optional field symbols.
/// </summary>
public static partial class OptionalSymbolFieldAssertions
{
    /// <summary>
    /// Asserts that the field symbol is read-only (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be read-only")]
    public static bool IsReadOnlySymbol(this OptionalSymbol<IFieldSymbol> symbol)
    {
        return symbol.IsReadOnly;
    }

    /// <summary>
    /// Asserts that the field symbol is const (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be const")]
    public static bool IsConst(this OptionalSymbol<IFieldSymbol> symbol)
    {
        return symbol.Map(x => x.Value.IsConst).Value;
    }

    /// <summary>
    /// Asserts that the field symbol is volatile (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be volatile")]
    public static bool IsVolatile(this OptionalSymbol<IFieldSymbol> symbol)
    {
        return symbol.Map(x => x.Value.IsVolatile).Value;
    }

    /// <summary>
    /// Asserts that the field symbol has a constant value (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have a constant value")]
    public static bool HasConstantValue(this OptionalSymbol<IFieldSymbol> symbol)
    {
        return symbol.Map(x => x.Value.HasConstantValue).Value;
    }
}