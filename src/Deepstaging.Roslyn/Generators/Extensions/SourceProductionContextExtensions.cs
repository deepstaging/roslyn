// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Roslyn.Emit;

namespace Deepstaging.Roslyn.Generators;

/// <summary>
/// Extension methods for Roslyn's source production context to support fluent code generation.
/// </summary>
public static class SourceProductionContextExtensions
{
    extension(SourceProductionContext ctx)
    {
        /// <summary>
        /// Adds generated source from an OptionalEmit result, reporting any diagnostics.
        /// </summary>
        /// <param name="hintName">Unique hint name for the generated file.</param>
        /// <param name="emit">The emit result containing code or diagnostics.</param>
        public void AddFromEmit(string hintName, OptionalEmit emit)
        {
            if (emit.IsNotValid(out var validCode))
            {
                foreach (var diagnostic in emit.Diagnostics)
                    ctx.ReportDiagnostic(diagnostic);
                return;
            }

            ctx.AddSource(hintName, validCode.Code);
        }
    }
}
