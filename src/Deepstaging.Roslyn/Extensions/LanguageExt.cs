// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// LanguageExt functional library extensions for Roslyn symbols.
/// Provides helpers to identify LanguageExt types like Option&lt;T&gt;, Either&lt;L,R&gt;, Eff&lt;RT,A&gt;, etc.
/// </summary>
public static class LanguageExtExtensions
{
    private const string LangExtNamespace = "LanguageExt";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static MethodQuery ReturningLanguageExtEff(this MethodQuery query) => 
        query.WithReturnType(IsLanguageExtEff);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IsLanguageExtEff(this ISymbol symbol) =>
        symbol is INamedTypeSymbol { Name: "Eff", TypeArguments.Length: 2 } type &&
        type.ContainingNamespace?.ToDisplayString() == LangExtNamespace;
}