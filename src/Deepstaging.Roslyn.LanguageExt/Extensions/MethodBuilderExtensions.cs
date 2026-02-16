// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Extensions;

using Emit;
using Emit.Refs;
using Refs;

/// <summary>
/// LanguageExt convenience extensions for <see cref="MethodBuilder"/>.
/// </summary>
public static class MethodBuilderExtensions
{
    /// <summary>
    /// Configures this method as an Eff-returning effect method with a constrained runtime type parameter.
    /// </summary>
    /// <remarks>
    /// <para>Adds a generic type parameter (e.g., <c>RT</c>) with a constraint to the specified capability
    /// interface, and sets the return type to <c>Eff&lt;RT, A&gt;</c>.</para>
    /// <para>This is the standard shape for every LanguageExt effect method:</para>
    /// <code>
    /// public static Eff&lt;RT, int&gt; GetCount&lt;RT&gt;() where RT : IHasDb
    /// </code>
    /// </remarks>
    /// <param name="builder">The method builder to configure.</param>
    /// <param name="rt">The runtime type parameter name (e.g., <c>"RT"</c>).</param>
    /// <param name="capability">The capability interface that <paramref name="rt"/> must implement (e.g., <c>"IHasDb"</c>).</param>
    /// <param name="resultType">The result type for the <c>Eff</c> (e.g., <c>"int"</c>, <c>Option&lt;User&gt;</c>).</param>
    public static MethodBuilder AsEffMethod(
        this MethodBuilder builder,
        string rt,
        TypeRef capability,
        TypeRef resultType) => builder
        .AsStatic()
        .AddTypeParameter(rt, tp => tp.WithConstraint(capability))
        .WithReturnType(LanguageExtRefs.Eff(rt, resultType));
}
