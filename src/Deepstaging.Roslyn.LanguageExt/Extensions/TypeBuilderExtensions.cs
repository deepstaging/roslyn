// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt;

using Emit;
using Types;

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
        builder.AddUsings(NamespaceRef.From("LanguageExt"), LanguageExtRefs.EffectsNamespace, LanguageExtRefs.PreludeStatic);
}