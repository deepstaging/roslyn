// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for identifying common value types
/// (TimeSpan, DateTime, DateTimeOffset, Guid, CancellationToken, Lazy, Uri).
/// </summary>
public static class TypeSymbolCommonTypeExtensions
{
    /// <param name="typeSymbol">The type symbol to check.</param>
    extension(ITypeSymbol typeSymbol)
    {
        // ── Time ─────────────────────────────────────────────────────────

        /// <summary>
        /// Checks if the type is <c>System.TimeSpan</c>.
        /// </summary>
        public bool IsTimeSpanType() => typeSymbol.SpecialType == SpecialType.None &&
                                        typeSymbol is INamedTypeSymbol { Name: "TimeSpan" } &&
                                        typeSymbol.ContainingNamespace.ToDisplayString() == "System";

        /// <summary>
        /// Checks if the type is <c>System.DateTime</c>.
        /// </summary>
        public bool IsDateTimeType() => typeSymbol.SpecialType == SpecialType.System_DateTime;

        /// <summary>
        /// Checks if the type is <c>System.DateTimeOffset</c>.
        /// </summary>
        public bool IsDateTimeOffsetType() => typeSymbol.SpecialType == SpecialType.None &&
                                              typeSymbol is INamedTypeSymbol { Name: "DateTimeOffset" } &&
                                              typeSymbol.ContainingNamespace.ToDisplayString() == "System";

        // ── Identifiers ──────────────────────────────────────────────────

        /// <summary>
        /// Checks if the type is <c>System.Guid</c>.
        /// </summary>
        public bool IsGuidType() => typeSymbol.SpecialType == SpecialType.None &&
                                    typeSymbol is INamedTypeSymbol { Name: "Guid" } &&
                                    typeSymbol.ContainingNamespace.ToDisplayString() == "System";

        /// <summary>
        /// Checks if the type is <c>System.Uri</c>.
        /// </summary>
        public bool IsUriType() => typeSymbol is INamedTypeSymbol { Name: "Uri" } &&
                                   typeSymbol.ContainingNamespace.ToDisplayString() == "System";

        // ── Threading ────────────────────────────────────────────────────

        /// <summary>
        /// Checks if the type is <c>System.Threading.CancellationToken</c>.
        /// </summary>
        public bool IsCancellationTokenType() => typeSymbol is INamedTypeSymbol { Name: "CancellationToken" } &&
                                                 typeSymbol.ContainingNamespace.ToDisplayString() == "System.Threading";

        // ── Wrappers ────────────────────────────────────────────────────

        /// <summary>
        /// Checks if the type is <c>Lazy&lt;T&gt;</c>.
        /// </summary>
        public bool IsLazyType() => typeSymbol is INamedTypeSymbol
        {
            Name: "Lazy", IsGenericType: true, TypeArguments.Length: 1
        };
    }
}