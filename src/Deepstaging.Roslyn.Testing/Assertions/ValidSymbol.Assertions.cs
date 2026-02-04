// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.Testing.Assertions;

/// <summary>
/// TUnit assertions for ValidSymbol types.
/// These assertions provide fluent API for testing symbol properties.
/// </summary>
public static partial class ValidSymbolAssertions
{
    #region Name Assertions

    /// <summary>
    /// Asserts that the symbol has the specified name.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be named '{expectedName}'")]
    public static bool IsNamed<T>(this ValidSymbol<T> symbol, string expectedName) where T : class, ISymbol
    {
        return symbol.Name == expectedName;
    }

    /// <summary>
    /// Asserts that the symbol name starts with the specified prefix.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have name starting with '{prefix}'")]
    public static bool NameStartsWith<T>(this ValidSymbol<T> symbol, string prefix) where T : class, ISymbol
    {
        return symbol.Name.StartsWith(prefix);
    }

    /// <summary>
    /// Asserts that the symbol name ends with the specified suffix.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have name ending with '{suffix}'")]
    public static bool NameEndsWith<T>(this ValidSymbol<T> symbol, string suffix) where T : class, ISymbol
    {
        return symbol.Name.EndsWith(suffix);
    }

    /// <summary>
    /// Asserts that the symbol name contains the specified substring.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have name containing '{substring}'")]
    public static bool NameContains<T>(this ValidSymbol<T> symbol, string substring) where T : class, ISymbol
    {
        return symbol.Name.Contains(substring);
    }

    #endregion

    #region Accessibility Assertions

    /// <summary>
    /// Asserts that the symbol is public.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be public")]
    public static bool IsPublicSymbol<T>(this ValidSymbol<T> symbol) where T : class, ISymbol
    {
        return symbol.Accessibility == Accessibility.Public;
    }

    /// <summary>
    /// Asserts that the symbol is private.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be private")]
    public static bool IsPrivateSymbol<T>(this ValidSymbol<T> symbol) where T : class, ISymbol
    {
        return symbol.Accessibility == Accessibility.Private;
    }

    /// <summary>
    /// Asserts that the symbol is internal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be internal")]
    public static bool IsInternalSymbol<T>(this ValidSymbol<T> symbol) where T : class, ISymbol
    {
        return symbol.Accessibility == Accessibility.Internal;
    }

    /// <summary>
    /// Asserts that the symbol is protected.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be protected")]
    public static bool IsProtectedSymbol<T>(this ValidSymbol<T> symbol) where T : class, ISymbol
    {
        return symbol.Accessibility == Accessibility.Protected;
    }

    #endregion

    #region Modifier Assertions

    /// <summary>
    /// Asserts that the symbol is static.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be static")]
    public static bool IsStaticSymbol<T>(this ValidSymbol<T> symbol) where T : class, ISymbol
    {
        return symbol.IsStatic;
    }

    /// <summary>
    /// Asserts that the symbol is abstract.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be abstract")]
    public static bool IsAbstractSymbol<T>(this ValidSymbol<T> symbol) where T : class, ISymbol
    {
        return symbol.IsAbstract;
    }

    /// <summary>
    /// Asserts that the symbol is sealed.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be sealed")]
    public static bool IsSealedSymbol<T>(this ValidSymbol<T> symbol) where T : class, ISymbol
    {
        return symbol.IsSealed;
    }

    /// <summary>
    /// Asserts that the symbol is virtual.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be virtual")]
    public static bool IsVirtualSymbol<T>(this ValidSymbol<T> symbol) where T : class, ISymbol
    {
        return symbol.IsVirtual;
    }

    #endregion

    #region Attribute Assertions

    /// <summary>
    /// Asserts that the symbol has an attribute with the specified name.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have attribute '{attributeName}'")]
    public static bool HasAttribute<T>(this ValidSymbol<T> symbol, string attributeName) where T : class, ISymbol
    {
        return symbol.GetAttributes().Any(attr => 
            attr.AttributeClass?.Name == attributeName || 
            attr.AttributeClass?.Name == attributeName + "Attribute");
    }

    /// <summary>
    /// Asserts that the symbol does not have an attribute with the specified name.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to not have attribute '{attributeName}'")]
    public static bool DoesNotHaveAttribute<T>(this ValidSymbol<T> symbol, string attributeName) where T : class, ISymbol
    {
        return !symbol.GetAttributes().Any(attr => 
            attr.AttributeClass?.Name == attributeName || 
            attr.AttributeClass?.Name == attributeName + "Attribute");
    }

    #endregion
}
