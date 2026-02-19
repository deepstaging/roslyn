// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Types;

/// <summary>
/// A type-safe wrapper representing an <c>Either&lt;L, R&gt;</c> type reference.
/// Carries the left and right types so expression builders and type declarations
/// can distinguish "this is an Either" at compile time.
/// </summary>
public readonly record struct EitherTypeRef
{
    /// <summary>Gets the left (error) type (e.g., <c>"Error"</c>).</summary>
    public TypeRef Left { get; }

    /// <summary>Gets the right (success) type (e.g., <c>"int"</c>).</summary>
    public TypeRef Right { get; }

    /// <summary>Creates an <c>EitherTypeRef</c> with the given left and right types.</summary>
    public EitherTypeRef(TypeRef left, TypeRef right)
    {
        Left = left;
        Right = right;
    }

    /// <summary>Gets the globally qualified <c>Either&lt;L, R&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::LanguageExt.Either<{Left.Value}, {Right.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(EitherTypeRef either) =>
        TypeRef.From(either.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(EitherTypeRef either) =>
        either.ToString();
}