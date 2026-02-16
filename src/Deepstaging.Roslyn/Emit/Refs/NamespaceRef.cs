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
/// var listRef = ns.Type("List");        // System.Collections.Generic.List
/// var globalRef = ns.GlobalType("List"); // global::System.Collections.Generic.List
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
    /// Creates a fully-qualified <see cref="TypeRef"/> for a type in this namespace.
    /// </summary>
    /// <param name="typeName">The simple type name (e.g., "List", "Dictionary").</param>
    public TypeRef Type(string typeName) => TypeRef.From($"{Value}.{typeName}");

    /// <summary>
    /// Creates a globally-qualified <see cref="TypeRef"/> (with <c>global::</c> prefix) for a type in this namespace.
    /// </summary>
    /// <param name="typeName">The simple type name (e.g., "List", "Dictionary").</param>
    public TypeRef GlobalType(string typeName) => TypeRef.Global($"{Value}.{typeName}");

    /// <summary>
    /// Creates a fully-qualified <see cref="AttributeRef"/> for an attribute type in this namespace.
    /// </summary>
    /// <param name="typeName">The attribute type name (e.g., "KeyAttribute", "RequiredAttribute").</param>
    public AttributeRef Attribute(string typeName) => AttributeRef.From($"{Value}.{typeName}");

    /// <summary>
    /// Creates a globally-qualified <see cref="AttributeRef"/> (with <c>global::</c> prefix) for an attribute type in this namespace.
    /// </summary>
    /// <param name="typeName">The attribute type name (e.g., "KeyAttribute", "RequiredAttribute").</param>
    public AttributeRef GlobalAttribute(string typeName) => AttributeRef.Global($"{Value}.{typeName}");

    /// <summary>
    /// Returns a static using string for this namespace (e.g., <c>static System.Math</c>).
    /// </summary>
    public string AsStatic() => $"static {Value}";

    /// <summary>Returns the namespace string.</summary>
    public override string ToString() => Value;

    /// <summary>Implicitly converts a <see cref="NamespaceRef"/> to a <see cref="string"/>.</summary>
    public static implicit operator string(NamespaceRef ns) => ns.Value;
}