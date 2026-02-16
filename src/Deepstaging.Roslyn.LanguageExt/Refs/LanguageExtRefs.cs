// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.LanguageExt.Refs;

using Emit.Refs;

/// <summary>
/// Factory methods for LanguageExt core types.
/// </summary>
/// <remarks>
/// Provides foundational <see cref="TypeRef"/> and <see cref="NamespaceRef"/> factories
/// for the LanguageExt functional programming library. These are library-universal references —
/// project-specific convenience shortcuts (e.g., <c>Eff&lt;RT, Unit&gt;</c> with a fixed runtime
/// type parameter) should be defined locally in consuming generators.
/// </remarks>
public static class LanguageExtRefs
{
    /// <summary>Gets the <c>LanguageExt</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("LanguageExt");

    /// <summary>Gets the <c>LanguageExt.Effects</c> namespace.</summary>
    public static NamespaceRef EffectsNamespace => Namespace.Append("Effects");

    /// <summary>Gets a <c>static LanguageExt.Prelude</c> using reference.</summary>
    public static string PreludeStatic => Namespace.Append("Prelude").AsStatic();

    // ── Core Types ──────────────────────────────────────────────────────

    /// <summary>Creates an <c>Eff&lt;RT, A&gt;</c> type reference.</summary>
    public static TypeRef Eff(TypeRef rt, TypeRef result) =>
        Namespace.GlobalType($"Eff<{rt.Value}, {result.Value}>");

    /// <summary>Creates an <c>Option&lt;T&gt;</c> type reference that carries the inner type.</summary>
    public static OptionTypeRef Option(TypeRef innerType) => new(innerType);

    /// <summary>Creates a <c>Fin&lt;A&gt;</c> type reference.</summary>
    public static TypeRef Fin(TypeRef innerType) =>
        Namespace.GlobalType($"Fin<{innerType.Value}>");

    /// <summary>Creates a <c>Seq&lt;A&gt;</c> type reference.</summary>
    public static TypeRef Seq(TypeRef elementType) =>
        Namespace.GlobalType($"Seq<{elementType.Value}>");

    /// <summary>Creates an <c>Either&lt;L, R&gt;</c> type reference.</summary>
    public static TypeRef Either(TypeRef left, TypeRef right) =>
        Namespace.GlobalType($"Either<{left.Value}, {right.Value}>");

    /// <summary>Creates a <c>HashMap&lt;K, V&gt;</c> type reference.</summary>
    public static TypeRef HashMap(TypeRef keyType, TypeRef valueType) =>
        Namespace.GlobalType($"HashMap<{keyType.Value}, {valueType.Value}>");

    /// <summary>Gets a <c>Unit</c> type reference.</summary>
    public static TypeRef Unit => Namespace.GlobalType("Unit");
}
