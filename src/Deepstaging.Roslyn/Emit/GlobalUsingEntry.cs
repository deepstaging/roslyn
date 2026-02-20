// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Represents a global using directive that may be conditionally compiled.
/// Use <see cref="NamespaceRef.When"/> to create conditional entries, or
/// rely on implicit conversions from <see cref="string"/> and <see cref="NamespaceRef"/>
/// for unconditional entries.
/// </summary>
/// <example>
/// <code>
/// GlobalUsings.Emit([
///     "System.Text.Json",                                          // unconditional (from string)
///     NamespaceRef.From("System.Linq"),                            // unconditional (from NamespaceRef)
///     NamespaceRef.From("LanguageExt").When(Directives.Net5OrGreater), // conditional
///     NamespaceRef.From("System.Math").AsStaticWhen(Directives.Net6OrGreater) // conditional static
/// ]);
/// </code>
/// </example>
public readonly record struct GlobalUsingEntry
{
    /// <summary>Gets the namespace string (e.g., "System.Collections.Generic").</summary>
    public string Namespace { get; }

    /// <summary>Gets the optional preprocessor directive condition. When null, the using is unconditional.</summary>
    public Directive? Condition { get; }

    /// <summary>Gets whether this is a <c>static</c> using directive.</summary>
    public bool IsStatic { get; }

    /// <summary>
    /// Creates a global using entry with the specified namespace, optional condition, and static flag.
    /// </summary>
    /// <param name="ns">The namespace string.</param>
    /// <param name="condition">Optional preprocessor directive condition.</param>
    /// <param name="isStatic">Whether this is a static using.</param>
    public GlobalUsingEntry(string ns, Directive? condition = null, bool isStatic = false)
    {
        Namespace = ns;
        Condition = condition;
        IsStatic = isStatic;
    }

    /// <summary>
    /// Wraps this entry in a preprocessor directive condition.
    /// </summary>
    /// <param name="directive">The directive condition (e.g., <c>Directives.Net6OrGreater</c>).</param>
    public GlobalUsingEntry When(Directive directive) => new(Namespace, directive, IsStatic);

    /// <summary>
    /// Returns a new entry marked as a <c>static</c> using.
    /// </summary>
    public GlobalUsingEntry AsStatic() => new(Namespace, Condition, isStatic: true);

    /// <summary>Creates an unconditional entry from a namespace string.</summary>
    public static implicit operator GlobalUsingEntry(string ns) => new(ns);

    /// <summary>Creates an unconditional entry from a <see cref="NamespaceRef"/>.</summary>
    public static implicit operator GlobalUsingEntry(NamespaceRef ns) => new(ns.Value);
}
