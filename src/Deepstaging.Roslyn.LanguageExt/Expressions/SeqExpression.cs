// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Expressions;

using Types;

/// <summary>
/// Builds <c>Seq</c> construction expressions using LanguageExt Prelude functions and static members.
/// </summary>
/// <remarks>
/// Prelude functions require <c>using static LanguageExt.Prelude</c> in the generated code.
/// Use <see cref="TypeBuilderExtensions.AddLanguageExtUsings"/> to add the standard usings.
/// </remarks>
public static class SeqExpression
{
    /// <summary>
    /// <c>Seq({items})</c> — constructs a <c>Seq&lt;A&gt;</c> from one or more items.
    /// </summary>
    /// <param name="items">The item expressions.</param>
    public static ExpressionRef Seq(params ExpressionRef[] items)
    {
        var args = string.Join(", ", items.Select(i => i.Value));
        return ExpressionRef.From($"Seq({args})");
    }

    /// <summary>
    /// <c>toSeq({expr})</c> — converts an <c>IEnumerable&lt;T&gt;</c> to <c>Seq&lt;T&gt;</c>.
    /// </summary>
    /// <param name="expr">The enumerable expression.</param>
    public static ExpressionRef toSeq(ExpressionRef expr) =>
        ExpressionRef.From($"toSeq({expr.Value})");

    /// <summary>
    /// <c>{seqType}.Empty</c> — the empty <c>Seq&lt;T&gt;</c> singleton.
    /// </summary>
    /// <param name="seqType">The Seq type reference providing the element type.</param>
    public static ExpressionRef Empty(SeqTypeRef seqType) =>
        ((TypeRef)seqType).Member("Empty");
}