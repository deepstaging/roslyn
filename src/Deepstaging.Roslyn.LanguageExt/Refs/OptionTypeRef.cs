// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Refs;

using Emit.Refs;

/// <summary>
/// A type-safe wrapper representing a <c>Option&lt;T&gt;</c> type reference.
/// Carries the inner type so expression builders (e.g., <c>AsyncOptional</c>) and type declarations
/// can distinguish "this is an Option" at compile time.
/// </summary>
public readonly record struct OptionTypeRef
{
    /// <summary>Gets the unwrapped inner type (e.g., <c>"User"</c>).</summary>
    public TypeRef InnerType { get; }

    /// <summary>Creates an <c>OptionTypeRef</c> wrapping the given inner type.</summary>
    public OptionTypeRef(TypeRef innerType) => InnerType = innerType;
    /// <summary>Gets the globally qualified <c>Option&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::LanguageExt.Option<{InnerType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(OptionTypeRef option) =>
        TypeRef.From(option.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(OptionTypeRef option) =>
        option.ToString();
}
