// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Types;

/// <summary>
/// A type-safe wrapper representing an <c>Eff&lt;RT, A&gt;</c> type reference.
/// Carries the runtime and result types so expression builders (e.g., <see cref="Expressions.EffLiftIO"/>)
/// can enforce correct construction at compile time.
/// </summary>
public readonly record struct EffTypeRef
{
    /// <summary>Gets the runtime type parameter (e.g., <c>"RT"</c>).</summary>
    public TypeRef Rt { get; }

    /// <summary>Gets the result type (e.g., <c>"int"</c>, <c>"Option&lt;User&gt;"</c>).</summary>
    public TypeRef Result { get; }

    /// <summary>Creates an <c>EffTypeRef</c> with the given runtime and result types.</summary>
    public EffTypeRef(TypeRef rt, TypeRef result)
    {
        Rt = rt;
        Result = result;
    }

    /// <summary>Gets the globally qualified <c>Eff&lt;RT, A&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::LanguageExt.Eff<{Rt.Value}, {Result.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(EffTypeRef eff) =>
        TypeRef.From(eff.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(EffTypeRef eff) =>
        eff.ToString();
}