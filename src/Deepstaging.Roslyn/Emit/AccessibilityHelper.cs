// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Shared helpers for parsing accessibility keywords.
/// </summary>
internal static class AccessibilityHelper
{
    /// <summary>
    /// Parses an accessibility keyword string (e.g., "public", "private protected")
    /// into a <see cref="Accessibility"/> enum value.
    /// Symmetric with <see cref="ValidSymbol{TSymbol}.AccessibilityString"/>.
    /// </summary>
    internal static Accessibility Parse(string accessibilityKeyword) =>
        accessibilityKeyword switch
        {
            "public" => Accessibility.Public,
            "private" => Accessibility.Private,
            "protected" => Accessibility.Protected,
            "internal" => Accessibility.Internal,
            "private protected" => Accessibility.ProtectedAndInternal,
            "protected internal" => Accessibility.ProtectedOrInternal,
            _ => throw new ArgumentException(
                $"Unknown accessibility keyword: '{accessibilityKeyword}'. " +
                "Expected one of: public, private, protected, internal, private protected, protected internal.",
                nameof(accessibilityKeyword))
        };
}