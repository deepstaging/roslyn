// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory methods for <c>System.Text.Encoding</c> types.
/// </summary>
public static class EncodingRefs
{
    /// <summary>Gets the <c>System.Text</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Text");

    /// <summary>Gets an <c>Encoding.UTF8</c> expression.</summary>
    public static ExpressionRef UTF8 => ExpressionRef.From("global::System.Text.Encoding.UTF8");

    /// <summary>Gets an <c>Encoding.ASCII</c> expression.</summary>
    public static ExpressionRef ASCII => ExpressionRef.From("global::System.Text.Encoding.ASCII");

    /// <summary>Gets an <c>Encoding.Unicode</c> (UTF-16) expression.</summary>
    public static ExpressionRef Unicode => ExpressionRef.From("global::System.Text.Encoding.Unicode");
}