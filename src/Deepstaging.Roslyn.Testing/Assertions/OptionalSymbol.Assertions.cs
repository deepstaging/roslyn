// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.Testing.Assertions;

/// <summary>
/// TUnit assertions for OptionalSymbol types.
/// These assertions provide fluent API for testing optional symbol presence.
/// </summary>
public static partial class OptionalSymbolAssertions
{
    /// <summary>
    /// Asserts that the OptionalSymbol has a value.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have a value")]
    public static bool HasValue<T>(this OptionalSymbol<T> symbol) where T : class, ISymbol
    {
        return symbol.HasValue;
    }

    /// <summary>
    /// Asserts that the OptionalSymbol is empty.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be empty")]
    public static bool IsEmpty<T>(this OptionalSymbol<T> symbol) where T : class, ISymbol
    {
        return symbol.IsEmpty;
    }
}
