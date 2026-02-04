// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ValidSymbol&lt;INamespaceSymbol&gt; exposing INamespaceSymbol-specific functionality.
/// </summary>
public static class ValidNamespaceSymbolExtensions
{
    extension(ValidSymbol<INamespaceSymbol> ns)
    {
        /// <summary>
        /// Gets all types directly declared in this namespace (non-recursive).
        /// </summary>
        public IEnumerable<ValidSymbol<INamedTypeSymbol>> GetTypes()
        {
            foreach (var type in ns.Value.GetTypeMembers())
                yield return ValidSymbol<INamedTypeSymbol>.From(type);
        }

        /// <summary>
        /// Gets a named type by name from this namespace.
        /// Returns Empty if not found.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> GetNamedType(string typeName)
        {
            var types = ns.Value.GetTypeMembers(typeName).ToArray();
            return types.Length == 1
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(types[0])
                : OptionalSymbol<INamedTypeSymbol>.Empty();
        }

        /// <summary>
        /// Gets a named type by name from this namespace.
        /// Throws if not found.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> RequireNamedType(string typeName) =>
            ns.GetNamedType(typeName).ValidateOrThrow();

        /// <summary>
        /// Gets all child namespaces declared within this namespace (non-recursive).
        /// </summary>
        public IEnumerable<ValidSymbol<INamespaceSymbol>> GetNamespaces()
        {
            foreach (var childNs in ns.Value.GetNamespaceMembers())
                yield return ValidSymbol<INamespaceSymbol>.From(childNs);
        }

        /// <summary>
        /// Gets a child namespace by name.
        /// Returns Empty if not found.
        /// </summary>
        public OptionalSymbol<INamespaceSymbol> GetNamespace(string namespaceName)
        {
            var namespaces = ns.Value.GetNamespaceMembers()
                .Where(n => n.Name == namespaceName)
                .ToArray();
            
            return namespaces.Length == 1
                ? OptionalSymbol<INamespaceSymbol>.WithValue(namespaces[0])
                : OptionalSymbol<INamespaceSymbol>.Empty();
        }

        /// <summary>
        /// Gets a child namespace by name.
        /// Throws if not found.
        /// </summary>
        public ValidSymbol<INamespaceSymbol> RequireNamespace(string namespaceName) =>
            ns.GetNamespace(namespaceName).ValidateOrThrow();

        /// <summary>
        /// Checks if this is the global namespace.
        /// </summary>
        public bool IsGlobalNamespace() => ns.Value.IsGlobalNamespace;

        /// <summary>
        /// Gets the containing namespace if not the global namespace.
        /// Returns Empty if this is the global namespace.
        /// </summary>
        public OptionalSymbol<INamespaceSymbol> ContainingNamespace =>
            ns.Value.ContainingNamespace is { IsGlobalNamespace: false } containingNs
                ? OptionalSymbol<INamespaceSymbol>.WithValue(containingNs)
                : OptionalSymbol<INamespaceSymbol>.Empty();
    }
}
