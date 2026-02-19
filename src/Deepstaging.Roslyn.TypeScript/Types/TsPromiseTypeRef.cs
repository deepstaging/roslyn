// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Types;

/// <summary>
/// A type-safe wrapper representing a TypeScript <c>Promise&lt;T&gt;</c> type reference.
/// Carries the result type so expression builders can produce typed promise patterns.
/// </summary>
public readonly record struct TsPromiseTypeRef
{
    /// <summary>Gets the result type (e.g., <c>"string"</c>).</summary>
    public TsTypeRef ResultType { get; }

    /// <summary>Creates a <c>TsPromiseTypeRef</c> for the given result type.</summary>
    /// <param name="resultType">The type that the promise resolves to.</param>
    public TsPromiseTypeRef(TsTypeRef resultType) => ResultType = resultType;

    /// <summary>Gets the <c>Promise&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"Promise<{ResultType.Value}>";

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsPromiseTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsPromiseTypeRef self) =>
        self.ToString();
}
