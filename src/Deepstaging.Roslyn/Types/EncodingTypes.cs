// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience <see cref="TypeRef"/> constants for <c>System.Text.Encoding</c> types.
/// </summary>
public static class EncodingTypes
{
    /// <summary>Gets a <see cref="NamespaceRef"/> for <c>System.Text</c>.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Text");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>Encoding</c>.</summary>
    public static TypeRef Encoding => TypeRef.Global("System.Text.Encoding");

    /// <summary>Gets a <see cref="ExpressionRef"/> for <c>Encoding.UTF8</c>.</summary>
    public static ExpressionRef UTF8 => ExpressionRef.From("global::System.Text.Encoding.UTF8");

    /// <summary>Gets a <see cref="ExpressionRef"/> for <c>Encoding.ASCII</c>.</summary>
    public static ExpressionRef ASCII => ExpressionRef.From("global::System.Text.Encoding.ASCII");

    /// <summary>Gets a <see cref="ExpressionRef"/> for <c>Encoding.Unicode</c> (UTF-16).</summary>
    public static ExpressionRef Unicode => ExpressionRef.From("global::System.Text.Encoding.Unicode");

    /// <summary>Gets a <see cref="ExpressionRef"/> for <c>Encoding.UTF32</c>.</summary>
    public static ExpressionRef UTF32 => ExpressionRef.From("global::System.Text.Encoding.UTF32");

    /// <summary>Gets a <see cref="ExpressionRef"/> for <c>Encoding.Latin1</c>.</summary>
    public static ExpressionRef Latin1 => ExpressionRef.From("global::System.Text.Encoding.Latin1");
}
