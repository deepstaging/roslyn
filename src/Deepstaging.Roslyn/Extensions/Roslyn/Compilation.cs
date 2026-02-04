// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for Compilation to query types.
/// </summary>
public static class CompilationQueryExtensions
{
    extension(Compilation compilation)
    {
        /// <summary>
        /// Creates a TypeQuery to search for types in the compilation.
        /// </summary>
        public TypeQuery QueryTypes()
        {
            return TypeQuery.From(compilation);
        }
    }
}