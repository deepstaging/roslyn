// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.Testing.Assertions;

/// <summary>
/// TUnit assertions for OptionalSymbol&lt;IPropertySymbol&gt;.
/// These assertions provide fluent API for testing optional property symbols.
/// </summary>
public static partial class OptionalSymbolPropertyAssertions
{
    /// <summary>
    /// Asserts that the property symbol is read-only (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be read-only")]
    public static bool IsReadOnlySymbol(this OptionalSymbol<IPropertySymbol> symbol)
    {
        return symbol.IsReadOnly;
    }

    /// <summary>
    /// Asserts that the property symbol is write-only (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be write-only")]
    public static bool IsWriteOnly(this OptionalSymbol<IPropertySymbol> symbol)
    {
        return symbol.Map(x => x.Value.IsWriteOnly).Value;
    }

    /// <summary>
    /// Asserts that the property symbol has a getter (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have a getter")]
    public static bool HasGetter(this OptionalSymbol<IPropertySymbol> symbol)
    {
        return symbol.GetMethod.HasValue;
    }

    /// <summary>
    /// Asserts that the property symbol has a setter (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have a setter")]
    public static bool HasSetter(this OptionalSymbol<IPropertySymbol> symbol)
    {
        return symbol.SetMethod.HasValue;
    }

    /// <summary>
    /// Asserts that the property symbol is an indexer (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be an indexer")]
    public static bool IsIndexer(this OptionalSymbol<IPropertySymbol> symbol)
    {
        return symbol.Map(x => x.Value.IsIndexer).Value;
    }

    /// <summary>
    /// Asserts that the property symbol has an init-only setter (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have an init-only setter")]
    public static bool HasInitOnlySetter(this OptionalSymbol<IPropertySymbol> symbol)
    {
        return symbol.SetMethod.Map(x => x.Value.IsInitOnly).HasValue;
    }

    /// <summary>
    /// Asserts that the property symbol is required (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be required")]
    public static bool IsRequired(this OptionalSymbol<IPropertySymbol> symbol)
    {
        return symbol.Map(x => x.Value.IsRequired).Value;
    }
}