// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Extensions;

using Emit;
using Refs;

/// <summary>
/// LanguageExt convenience extensions for <see cref="TypeBuilder"/>.
/// </summary>
public static class TypeBuilderExtensions
{
    /// <summary>
    /// Adds the standard LanguageExt using directives for effect code generation.
    /// </summary>
    /// <remarks>
    /// Adds <c>LanguageExt</c>, <c>LanguageExt.Effects</c>, and <c>static LanguageExt.Prelude</c>.
    /// </remarks>
    public static TypeBuilder AddLanguageExtUsings(this TypeBuilder builder) =>
        builder.AddUsings(LanguageExtRefs.Namespace, LanguageExtRefs.EffectsNamespace, LanguageExtRefs.PreludeStatic);
}
