// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit;

namespace Deepstaging.Roslyn.Generators;

/// <summary>
/// Extension methods for Roslyn's source production context to support fluent code generation.
/// </summary>
public static class SourceProductionContextExtensions
{
    extension(OptionalEmit emit)
    {
        /// <summary>
        /// Adds this emit result as generated source to the production context.
        /// Reports any diagnostics and only adds source if the emit was successful.
        /// </summary>
        /// <param name="ctx">The source production context.</param>
        /// <param name="filename">Unique hint name for the generated file.</param>
        public void AddSourceTo(SourceProductionContext ctx, string filename)
        {
            if (emit.IsNotValid(out var validCode))
            {
                foreach (var diagnostic in emit.Diagnostics)
                    ctx.ReportDiagnostic(diagnostic);
                return;
            }

            ctx.AddSource(filename, validCode.Code);
        }
    }

    extension(SourceProductionContext ctx)
    {
        /// <summary>
        /// Adds generated source from an OptionalEmit result, reporting any diagnostics.
        /// </summary>
        /// <param name="emit">The emit result containing code or diagnostics.</param>
        /// <param name="hintName">Unique hint name for the generated file.</param>
        public void AddSourceFrom(OptionalEmit emit, string hintName)
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