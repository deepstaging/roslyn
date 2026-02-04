// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Provides a fluent API for querying types within a namespace or compilation.
/// Supports chainable filters for common type query scenarios.
/// </summary>
public readonly struct TypeQuery
{
    private readonly INamespaceOrTypeSymbol _container;
    private readonly ImmutableArray<Func<INamedTypeSymbol, bool>> _filters;

    private TypeQuery(INamespaceOrTypeSymbol container, ImmutableArray<Func<INamedTypeSymbol, bool>> filters)
    {
        _container = container;
        _filters = filters;
    }

    /// <summary>
    /// Creates a new type query for the specified namespace.
    /// </summary>
    public static TypeQuery From(INamespaceSymbol namespaceSymbol)
    {
        return new TypeQuery(namespaceSymbol, ImmutableArray<Func<INamedTypeSymbol, bool>>.Empty);
    }

    /// <summary>
    /// Creates a new type query for the global namespace in a compilation.
    /// </summary>
    public static TypeQuery From(Compilation compilation)
    {
        return new TypeQuery(compilation.GlobalNamespace, ImmutableArray<Func<INamedTypeSymbol, bool>>.Empty);
    }

    private TypeQuery AddFilter(Func<INamedTypeSymbol, bool> filter)
    {
        return new TypeQuery(_container, _filters.Add(filter));
    }

    #region Accessibility Filters

    /// <summary>
    /// Filters for public types.
    /// </summary>
    public TypeQuery ThatArePublic()
    {
        return AddFilter(t => t.DeclaredAccessibility == Accessibility.Public);
    }

    /// <summary>
    /// Filters for internal types.
    /// </summary>
    public TypeQuery ThatAreInternal()
    {
        return AddFilter(t => t.DeclaredAccessibility == Accessibility.Internal);
    }

    /// <summary>
    /// Filters for private types (nested types only).
    /// </summary>
    public TypeQuery ThatArePrivate()
    {
        return AddFilter(t => t.DeclaredAccessibility == Accessibility.Private);
    }

    /// <summary>
    /// Filters for protected types (nested types only).
    /// </summary>
    public TypeQuery ThatAreProtected()
    {
        return AddFilter(t => t.DeclaredAccessibility == Accessibility.Protected);
    }

    #endregion

    #region Type Kind Filters

    /// <summary>
    /// Filters for class types.
    /// </summary>
    public TypeQuery ThatAreClasses()
    {
        return AddFilter(t => t.TypeKind == TypeKind.Class);
    }

    /// <summary>
    /// Filters for interface types.
    /// </summary>
    public TypeQuery ThatAreInterfaces()
    {
        return AddFilter(t => t.TypeKind == TypeKind.Interface);
    }

    /// <summary>
    /// Filters for struct types.
    /// </summary>
    public TypeQuery ThatAreStructs()
    {
        return AddFilter(t => t.TypeKind == TypeKind.Struct);
    }

    /// <summary>
    /// Filters for enum types.
    /// </summary>
    public TypeQuery ThatAreEnums()
    {
        return AddFilter(t => t.TypeKind == TypeKind.Enum);
    }

    /// <summary>
    /// Filters for delegate types.
    /// </summary>
    public TypeQuery ThatAreDelegates()
    {
        return AddFilter(t => t.TypeKind == TypeKind.Delegate);
    }

    /// <summary>
    /// Filters for record types (record class or record struct).
    /// </summary>
    public TypeQuery ThatAreRecords()
    {
        return AddFilter(t => t.IsRecord);
    }

    #endregion

    #region Modifier Filters

    /// <summary>
    /// Filters for static types.
    /// </summary>
    public TypeQuery ThatAreStatic()
    {
        return AddFilter(t => t.IsStatic);
    }

    /// <summary>
    /// Filters for abstract types.
    /// </summary>
    public TypeQuery ThatAreAbstract()
    {
        return AddFilter(t => t.IsAbstract);
    }

    /// <summary>
    /// Filters for sealed types.
    /// </summary>
    public TypeQuery ThatAreSealed()
    {
        return AddFilter(t => t.IsSealed);
    }

    /// <summary>
    /// Filters for generic types.
    /// </summary>
    public TypeQuery ThatAreGeneric()
    {
        return AddFilter(t => t.IsGenericType);
    }

    /// <summary>
    /// Filters for partial types.
    /// </summary>
    public TypeQuery ThatArePartial()
    {
        return AddFilter(t => t.DeclaringSyntaxReferences.Length > 1);
    }

    /// <summary>
    /// Filters for ref struct types.
    /// </summary>
    public TypeQuery ThatAreRefStructs()
    {
        return AddFilter(t => t.IsRefLikeType);
    }

    /// <summary>
    /// Filters for readonly struct types.
    /// </summary>
    public TypeQuery ThatAreReadOnlyStructs()
    {
        return AddFilter(t => t.IsReadOnly);
    }

    #endregion

    #region Name Filters

    /// <summary>
    /// Filters for types with the specified name.
    /// </summary>
    public TypeQuery WithName(string name)
    {
        return AddFilter(t => t.Name == name);
    }

    /// <summary>
    /// Filters for types whose names start with the specified prefix.
    /// </summary>
    public TypeQuery WithNameStartingWith(string prefix)
    {
        return AddFilter(t => t.Name.StartsWith(prefix, StringComparison.Ordinal));
    }

    /// <summary>
    /// Filters for types whose names contain the specified substring.
    /// </summary>
    public TypeQuery WithNameContaining(string substring)
    {
        return AddFilter(t => t.Name.Contains(substring));
    }

    /// <summary>
    /// Filters for types whose names end with the specified suffix.
    /// </summary>
    public TypeQuery WithNameEndingWith(string suffix)
    {
        return AddFilter(t => t.Name.EndsWith(suffix, StringComparison.Ordinal));
    }

    #endregion

    #region Inheritance Filters

    /// <summary>
    /// Filters for types that inherit from the specified base type.
    /// </summary>
    public TypeQuery InheritingFrom(INamedTypeSymbol baseType)
    {
        return AddFilter(t => InheritsFrom(t, baseType));
    }

    /// <summary>
    /// Filters for types that implement the specified interface.
    /// </summary>
    public TypeQuery ImplementingInterface(INamedTypeSymbol interfaceType)
    {
        return AddFilter(t => t.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, interfaceType)));
    }

    /// <summary>
    /// Filters for types that implement an interface with the specified name.
    /// </summary>
    public TypeQuery ImplementingInterface(string interfaceName)
    {
        return AddFilter(t => t.AllInterfaces.Any(i => i.Name == interfaceName));
    }

    private static bool InheritsFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        var current = type.BaseType;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
                return true;
            current = current.BaseType;
        }

        return false;
    }

    #endregion

    #region Attribute Filters

    /// <summary>
    /// Filters for types with the specified attribute name (with or without "Attribute" suffix).
    /// </summary>
    public TypeQuery WithAttribute(string attributeName)
    {
        return AddFilter(t => t.GetAttributes().Any(a =>
            a.AttributeClass?.Name == attributeName ||
            a.AttributeClass?.Name == attributeName + "Attribute"));
    }

    #endregion

    #region Namespace Filters

    /// <summary>
    /// Filters for types in the specified namespace (exact match).
    /// </summary>
    public TypeQuery InNamespace(string namespaceName)
    {
        return AddFilter(t => t.ContainingNamespace?.ToDisplayString() == namespaceName);
    }

    /// <summary>
    /// Filters for types in namespaces starting with the specified prefix.
    /// </summary>
    public TypeQuery InNamespaceStartingWith(string namespacePrefix)
    {
        return AddFilter(t =>
            t.ContainingNamespace?.ToDisplayString().StartsWith(namespacePrefix, StringComparison.Ordinal) == true);
    }

    /// <summary>
    /// Includes types from nested namespaces.
    /// </summary>
    public TypeQuery IncludeNestedNamespaces()
    {
        return this;
        // Handled in GetAll() by GetAllTypes()
    }

    #endregion

    #region Generic Filter

    /// <summary>
    /// Filters for types matching the custom predicate.
    /// Use this as an escape hatch for complex or uncommon filters.
    /// </summary>
    public TypeQuery Where(Func<INamedTypeSymbol, bool> predicate)
    {
        return AddFilter(predicate);
    }

    #endregion

    #region Materialization

    /// <summary>
    /// Gets all matching and returns all matching types as optional symbols.
    /// </summary>
    public ImmutableArray<ValidSymbol<INamedTypeSymbol>> GetAll()
    {
        var types = GetAllTypes(_container);

        // Apply all filters
        foreach (var filter in _filters) types = types.Where(filter);

        return [..types.Select(t => ValidSymbol<INamedTypeSymbol>.From(t))];
    }

    /// <summary>
    /// Gets all matching and returns all matching type symbols (without projection wrapper).
    /// Use this when you need raw INamedTypeSymbol instances for Roslyn APIs.
    /// </summary>
    public ImmutableArray<INamedTypeSymbol> GetAllSymbols()
    {
        var types = GetAllTypes(_container);

        // Apply all filters
        foreach (var filter in _filters) types = types.Where(filter);

        return [..types];
    }

    /// <summary>
    /// Gets all matching and projects each type using the specified mapper.
    /// </summary>
    public ImmutableArray<TModel> Select<TModel>(Func<ValidSymbol<INamedTypeSymbol>, TModel> mapper)
    {
        return [..GetAll().Select(mapper)];
    }

    /// <summary>
    /// Gets all matching and projects each type to a collection, then flattens the results.
    /// </summary>
    public ImmutableArray<TModel> SelectMany<TModel>(Func<ValidSymbol<INamedTypeSymbol>, IEnumerable<TModel>> mapper)
    {
        return [..GetAll().SelectMany(mapper)];
    }

    /// <summary>
    /// Gets all matching and returns the first matching type, or Empty if none found.
    /// </summary>
    public OptionalSymbol<INamedTypeSymbol> FirstOrDefault()
    {
        var all = GetAll();
        return all.Length > 0
            ? OptionalSymbol<INamedTypeSymbol>.WithValue(all[0].Value)
            : OptionalSymbol<INamedTypeSymbol>.Empty();
    }

    /// <summary>
    /// Gets all matching and returns the first matching type, or throws if none found.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no types match the query.</exception>
    public ValidSymbol<INamedTypeSymbol> First()
    {
        var all = GetAll();
        return all.Length > 0
            ? all[0]
            : throw new InvalidOperationException("No inamedtypesymbols matched the query criteria.");
    }

    /// <summary>
    /// Gets all matching and returns true if any types match, false otherwise.
    /// </summary>
    public bool Any()
    {
        return GetAll().Length > 0;
    }

    /// <summary>
    /// Gets all matching and returns the count of matching types.
    /// </summary>
    public int Count()
    {
        return GetAll().Length;
    }

    #endregion

    #region Helper Methods

    private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceOrTypeSymbol container)
    {
        foreach (var member in container.GetMembers())
            if (member is INamedTypeSymbol type)
            {
                yield return type;

                // Include nested types
                foreach (var nestedType in GetAllTypes(type)) yield return nestedType;
            }
            else if (member is INamespaceSymbol ns)
            {
                // Include types from nested namespaces
                foreach (var nestedType in GetAllTypes(ns)) yield return nestedType;
            }
    }

    #endregion
}