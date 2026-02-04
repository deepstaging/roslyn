// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.Testing.Assertions;

/// <summary>
/// TUnit assertions for OptionalSymbol&lt;IMethodSymbol&gt;.
/// These assertions provide fluent API for testing optional method symbols.
/// </summary>
public static partial class OptionalSymbolMethodAssertions
{
    /// <summary>
    /// Asserts that the method symbol is async (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be async")]
    public static bool IsAsyncSymbol(this OptionalSymbol<IMethodSymbol> symbol)
    {
        return symbol.IsAsync;
    }

    /// <summary>
    /// Asserts that the method symbol returns void (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to return void")]
    public static bool ReturnsVoid(this OptionalSymbol<IMethodSymbol> symbol)
    {
        return symbol.Map(x => x.Value.ReturnsVoid).Value;
    }

    /// <summary>
    /// Asserts that the method symbol has the specified number of parameters (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have {count} parameters")]
    public static bool HasParameterCount(this OptionalSymbol<IMethodSymbol> symbol, int count)
    {
        return symbol.Parameters.Length == count;
    }

    /// <summary>
    /// Asserts that the method symbol is an extension method (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be an extension method")]
    public static bool IsExtensionMethod(this OptionalSymbol<IMethodSymbol> symbol)
    {
        return symbol.IsExtensionMethod;
    }

    /// <summary>
    /// Asserts that the method symbol is generic (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be generic")]
    public static bool IsGenericMethod(this OptionalSymbol<IMethodSymbol> symbol)
    {
        return symbol.Map(x => x.Value.IsGenericMethod).Value;
    }

    /// <summary>
    /// Asserts that the method symbol is a constructor (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be a constructor")]
    public static bool IsConstructor(this OptionalSymbol<IMethodSymbol> symbol)
    {
        return symbol.MethodKind == MethodKind.Constructor;
    }

    /// <summary>
    /// Asserts that the method symbol is an operator (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be an operator")]
    public static bool IsOperator(this OptionalSymbol<IMethodSymbol> symbol)
    {
        return (symbol.MethodKind == MethodKind.UserDefinedOperator ||
                symbol.MethodKind == MethodKind.BuiltinOperator);
    }

    /// <summary>
    /// Asserts that the method symbol has a return type (returns false if empty).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have a return type")]
    public static bool HasReturnType(this OptionalSymbol<IMethodSymbol> symbol)
    {
        return !symbol.Map(x => x.Value.ReturnsVoid).Value;
    }
}