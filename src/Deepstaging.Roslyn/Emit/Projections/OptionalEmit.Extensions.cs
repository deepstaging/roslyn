// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Extension methods for combining optional emit results.
/// </summary>
public static class OptionalEmitExtensions
{
    extension(OptionalEmit emit)
    {
        /// <summary>
        /// Combines this optional emit with another into a single optional emit.
        /// If either emit failed, returns a failed result with all aggregated diagnostics.
        /// If both succeed, combines using ValidEmit.Combine and returns a successful result.
        /// Chain calls for more: <c>a.Combine(b).Combine(c)</c>.
        /// </summary>
        /// <param name="other">The other optional emit to combine with.</param>
        public OptionalEmit Combine(OptionalEmit other)
        {
            List<OptionalEmit> emitList = [emit, other];

            // Collect all diagnostics
            var allDiagnostics = emitList
                .SelectMany(e => e.Diagnostics)
                .ToImmutableArray();

            // Check if any emit failed
            if (emitList.Any(e => !e.Success))
                return OptionalEmit.FromFailure(allDiagnostics);

            // All succeeded - combine the validated emits
            var validEmits = emitList.Select(e => e.ValidateOrThrow()).ToList();
            var combined = ValidEmitExtensions.CombineAll(validEmits);

            return allDiagnostics.Length > 0
                ? OptionalEmit.FromDiagnostics(combined.Syntax, combined.Code, allDiagnostics)
                : OptionalEmit.FromSuccess(combined.Syntax, combined.Code);
        }
    }
}