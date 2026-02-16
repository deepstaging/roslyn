// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory methods for <c>System.Func</c> and <c>System.Action</c> delegate types.
/// </summary>
public static class DelegateRefs
{
    /// <summary>Gets the <c>System</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System");

    /// <summary>Creates a <c>Func&lt;...&gt;</c> delegate type reference. The last type argument is the return type.</summary>
    /// <param name="typeArguments">The type arguments. Must contain at least one (the return type).</param>
    public static TypeRef Func(params TypeRef[] typeArguments)
    {
        if (typeArguments.Length == 0)
            throw new ArgumentException("Func requires at least one type argument (the return type).",
                nameof(typeArguments));

        var args = string.Join(", ", typeArguments.Select(t => t.Value));
        return TypeRef.From($"global::System.Func<{args}>");
    }

    /// <summary>Creates an <c>Action</c> or <c>Action&lt;...&gt;</c> delegate type reference.</summary>
    /// <param name="typeArguments">The type arguments. If empty, produces <c>Action</c>.</param>
    public static TypeRef Action(params TypeRef[] typeArguments)
    {
        if (typeArguments.Length == 0)
            return TypeRef.From("global::System.Action");

        var args = string.Join(", ", typeArguments.Select(t => t.Value));
        return TypeRef.From($"global::System.Action<{args}>");
    }
}