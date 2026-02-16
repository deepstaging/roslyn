// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Refs;

using Emit.Refs;

/// <summary>
/// A type-safe wrapper representing a <c>Fin&lt;A&gt;</c> type reference.
/// Carries the inner type so expression builders and type declarations
/// can distinguish "this is a Fin" at compile time.
/// </summary>
public readonly record struct FinTypeRef
{
    /// <summary>Gets the unwrapped inner type (e.g., <c>"int"</c>).</summary>
    public TypeRef InnerType { get; }

    /// <summary>Creates a <c>FinTypeRef</c> wrapping the given inner type.</summary>
    public FinTypeRef(TypeRef innerType) => InnerType = innerType;

    /// <summary>Gets the globally qualified <c>Fin&lt;A&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::LanguageExt.Fin<{InnerType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(FinTypeRef fin) =>
        TypeRef.From(fin.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(FinTypeRef fin) =>
        fin.ToString();
}
