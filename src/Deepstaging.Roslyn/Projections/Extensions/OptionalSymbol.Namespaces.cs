// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for OptionalSymbol&lt;INamespaceSymbol&gt; exposing INamespaceSymbol-specific functionality.
/// </summary>
public static class ProjectedNamespaceSymbolExtensions
{
    extension(OptionalSymbol<INamespaceSymbol> ns)
    {
        /// <summary>
        /// Gets all types directly declared in this namespace (non-recursive).
        /// Returns empty enumerable if namespace is empty.
        /// </summary>
        public IEnumerable<OptionalSymbol<INamedTypeSymbol>> GetTypes()
        {
            if (!ns.HasValue)
                yield break;
            
            foreach (var type in ns.Symbol!.GetTypeMembers())
                yield return OptionalSymbol<INamedTypeSymbol>.WithValue(type);
        }

        /// <summary>
        /// Gets a named type by name from this namespace.
        /// Returns Empty if namespace is empty or type not found.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> GetNamedType(string typeName)
        {
            if (!ns.HasValue)
                return OptionalSymbol<INamedTypeSymbol>.Empty();
            
            var types = ns.Symbol!.GetTypeMembers(typeName).ToArray();
            return types.Length == 1
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(types[0])
                : OptionalSymbol<INamedTypeSymbol>.Empty();
        }

        /// <summary>
        /// Gets a named type by name from this namespace.
        /// Throws if namespace is empty or type not found.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> RequireNamedType(string typeName) =>
            ns.GetNamedType(typeName).ValidateOrThrow();

        /// <summary>
        /// Gets all child namespaces declared within this namespace (non-recursive).
        /// Returns empty enumerable if namespace is empty.
        /// </summary>
        public IEnumerable<OptionalSymbol<INamespaceSymbol>> GetNamespaces()
        {
            if (!ns.HasValue)
                yield break;
            
            foreach (var childNs in ns.Symbol!.GetNamespaceMembers())
                yield return OptionalSymbol<INamespaceSymbol>.WithValue(childNs);
        }

        /// <summary>
        /// Gets a child namespace by name.
        /// Returns Empty if namespace is empty or child not found.
        /// </summary>
        public OptionalSymbol<INamespaceSymbol> GetNamespace(string namespaceName)
        {
            if (!ns.HasValue)
                return OptionalSymbol<INamespaceSymbol>.Empty();
            
            var namespaces = ns.Symbol!.GetNamespaceMembers()
                .Where(n => n.Name == namespaceName)
                .ToArray();
            
            return namespaces.Length == 1
                ? OptionalSymbol<INamespaceSymbol>.WithValue(namespaces[0])
                : OptionalSymbol<INamespaceSymbol>.Empty();
        }

        /// <summary>
        /// Gets a child namespace by name.
        /// Throws if namespace is empty or child not found.
        /// </summary>
        public ValidSymbol<INamespaceSymbol> RequireNamespace(string namespaceName) =>
            ns.GetNamespace(namespaceName).ValidateOrThrow();

        /// <summary>
        /// Checks if this is the global namespace.
        /// Returns false if namespace is empty.
        /// </summary>
        public bool IsGlobalNamespace() =>
            ns.HasValue && ns.Symbol!.IsGlobalNamespace;

        /// <summary>
        /// Gets the containing namespace if not the global namespace.
        /// Returns Empty if this is empty, the global namespace, or has no parent.
        /// </summary>
        public OptionalSymbol<INamespaceSymbol> ContainingNamespace =>
            ns is { HasValue: true, Symbol.ContainingNamespace: { IsGlobalNamespace: false } containingNs }
                ? OptionalSymbol<INamespaceSymbol>.WithValue(containingNs)
                : OptionalSymbol<INamespaceSymbol>.Empty();
    }
}
