// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.Testing.Assertions;

/// <summary>
/// TUnit assertions for ValidSymbol&lt;IMethodSymbol&gt;.
/// These assertions provide fluent API for testing method symbols.
/// </summary>
public static partial class ValidSymbolMethodAssertions
{
    /// <summary>
    /// Asserts that the method symbol is async.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be async")]
    public static bool IsAsyncSymbol(this ValidSymbol<IMethodSymbol> symbol)
    {
        return symbol.IsAsync;
    }

    /// <summary>
    /// Asserts that the method symbol returns void.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to return void")]
    public static bool ReturnsVoid(this ValidSymbol<IMethodSymbol> symbol)
    {
        return symbol.Value.ReturnsVoid;
    }

    /// <summary>
    /// Asserts that the method symbol has the specified number of parameters.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have {count} parameters")]
    public static bool HasParameterCount(this ValidSymbol<IMethodSymbol> symbol, int count)
    {
        return symbol.GetTypeParameters().Count() == count;
    }

    /// <summary>
    /// Asserts that the method symbol is an extension method.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be an extension method")]
    public static bool IsExtension(this ValidSymbol<IMethodSymbol> symbol)
    {
        return symbol.IsExtensionMethod;
    }

    /// <summary>
    /// Asserts that the method symbol is generic.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be generic")]
    public static bool IsGeneric(this ValidSymbol<IMethodSymbol> symbol)
    {
        return symbol.Value.IsGenericMethod;
    }

    /// <summary>
    /// Asserts that the method symbol is a constructor.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be a constructor")]
    public static bool IsConstructor(this ValidSymbol<IMethodSymbol> symbol)
    {
        return symbol.Value.MethodKind == MethodKind.Constructor;
    }

    /// <summary>
    /// Asserts that the method symbol is an operator.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be an operator")]
    public static bool IsOperator(this ValidSymbol<IMethodSymbol> symbol)
    {
        return symbol.Value.MethodKind == MethodKind.UserDefinedOperator || 
               symbol.Value.MethodKind == MethodKind.BuiltinOperator;
    }

    /// <summary>
    /// Asserts that the method symbol has a return type.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have a return type")]
    public static bool HasReturnType(this ValidSymbol<IMethodSymbol> symbol)
    {
        return !symbol.Value.ReturnsVoid;
    }
}
