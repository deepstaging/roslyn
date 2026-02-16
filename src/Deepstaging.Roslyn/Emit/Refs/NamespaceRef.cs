// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Represents a namespace reference for use in code generation.
/// Provides a <see cref="Type"/> factory to create fully-qualified <see cref="TypeRef"/> instances
/// within this namespace.
/// </summary>
/// <example>
/// <code>
/// var ns = NamespaceRef.From("System.Collections.Generic");
/// var listRef = ns.Type("List");  // global::System.Collections.Generic.List
/// </code>
/// </example>
public readonly record struct NamespaceRef
{
    /// <summary>Gets the namespace string (e.g., "System.Collections.Generic").</summary>
    public string Value { get; }

    private NamespaceRef(string value) => Value = value;

    /// <summary>Creates a namespace reference from a dotted namespace string.</summary>
    /// <param name="ns">The namespace (e.g., "System.Collections.Generic").</param>
    public static NamespaceRef From(string ns)
    {
        if (string.IsNullOrWhiteSpace(ns))
            throw new ArgumentException("Namespace cannot be null or whitespace.", nameof(ns));

        return new NamespaceRef(ns);
    }

    /// <summary>
    /// Creates a globally-qualified <see cref="TypeRef"/> for a type in this namespace.
    /// </summary>
    /// <param name="typeName">The simple type name (e.g., "List", "Dictionary").</param>
    public TypeRef Type(string typeName) => TypeRef.From($"global::{Value}.{typeName}");

    /// <summary>Returns the namespace string.</summary>
    public override string ToString() => Value;

    /// <summary>Implicitly converts a <see cref="NamespaceRef"/> to a <see cref="string"/>.</summary>
    public static implicit operator string(NamespaceRef ns) => ns.Value;
}