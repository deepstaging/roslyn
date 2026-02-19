// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Types;

/// <summary>
/// A type-safe wrapper representing a <c>Seq&lt;A&gt;</c> type reference.
/// Carries the element type so expression builders and type declarations
/// can distinguish "this is a Seq" at compile time.
/// </summary>
public readonly record struct SeqTypeRef
{
    /// <summary>Gets the element type (e.g., <c>"string"</c>).</summary>
    public TypeRef ElementType { get; }

    /// <summary>Creates a <c>SeqTypeRef</c> wrapping the given element type.</summary>
    public SeqTypeRef(TypeRef elementType) => ElementType = elementType;

    /// <summary>Gets the globally qualified <c>Seq&lt;A&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::LanguageExt.Seq<{ElementType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(SeqTypeRef seq) =>
        TypeRef.From(seq.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(SeqTypeRef seq) =>
        seq.ToString();
}