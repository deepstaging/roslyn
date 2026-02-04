// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.Testing.Assertions;

/// <summary>
/// TUnit assertions for ValidSymbol&lt;IPropertySymbol&gt;.
/// These assertions provide fluent API for testing property symbols.
/// </summary>
public static partial class ValidSymbolPropertyAssertions
{
    /// <summary>
    /// Asserts that the property symbol is read-only.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be read-only")]
    public static bool IsReadOnlySymbol(this ValidSymbol<IPropertySymbol> symbol)
    {
        return symbol.IsReadOnly;
    }

    /// <summary>
    /// Asserts that the property symbol is write-only.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be write-only")]
    public static bool IsWriteOnly(this ValidSymbol<IPropertySymbol> symbol)
    {
        return symbol.Value.IsWriteOnly;
    }

    /// <summary>
    /// Asserts that the property symbol has a getter.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have a getter")]
    public static bool HasGetter(this ValidSymbol<IPropertySymbol> symbol)
    {
        return symbol.GetMethod.HasValue;
    }

    /// <summary>
    /// Asserts that the property symbol has a setter.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have a setter")]
    public static bool HasSetter(this ValidSymbol<IPropertySymbol> symbol)
    {
        return symbol.SetMethod.HasValue;
    }

    /// <summary>
    /// Asserts that the property symbol is an indexer.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be an indexer")]
    public static bool IsIndexer(this ValidSymbol<IPropertySymbol> symbol)
    {
        return symbol.Value.IsIndexer;
    }

    /// <summary>
    /// Asserts that the property symbol has an init-only setter.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have an init-only setter")]
    public static bool HasInitOnlySetter(this ValidSymbol<IPropertySymbol> symbol)
    {
        return symbol.SetMethod.Where(x => x.IsInitOnly).HasValue;
    }

    /// <summary>
    /// Asserts that the property symbol is required.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be required")]
    public static bool IsRequired(this ValidSymbol<IPropertySymbol> symbol)
    {
        return symbol.Value.IsRequired;
    }
}
