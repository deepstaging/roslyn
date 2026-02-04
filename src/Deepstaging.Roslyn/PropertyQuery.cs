// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Provides a fluent API for querying properties on a type symbol.
/// Supports chainable filters for common property query scenarios.
/// </summary>
public readonly struct PropertyQuery
{
    private readonly ITypeSymbol _typeSymbol;
    private readonly ImmutableArray<Func<IPropertySymbol, bool>> _filters;

    private PropertyQuery(ITypeSymbol typeSymbol, ImmutableArray<Func<IPropertySymbol, bool>> filters)
    {
        _typeSymbol = typeSymbol;
        _filters = filters;
    }

    /// <summary>
    /// Creates a new property query for the specified type symbol.
    /// </summary>
    public static PropertyQuery From(ITypeSymbol typeSymbol)
    {
        return new PropertyQuery(typeSymbol, ImmutableArray<Func<IPropertySymbol, bool>>.Empty);
    }

    private PropertyQuery AddFilter(Func<IPropertySymbol, bool> filter)
    {
        return new PropertyQuery(_typeSymbol, _filters.Add(filter));
    }

    #region Accessibility Filters

    /// <summary>
    /// Filters for public properties.
    /// </summary>
    public PropertyQuery ThatArePublic()
    {
        return AddFilter(p => p.DeclaredAccessibility == Accessibility.Public);
    }

    /// <summary>
    /// Filters for private properties.
    /// </summary>
    public PropertyQuery ThatArePrivate()
    {
        return AddFilter(p => p.DeclaredAccessibility == Accessibility.Private);
    }

    /// <summary>
    /// Filters for protected properties.
    /// </summary>
    public PropertyQuery ThatAreProtected()
    {
        return AddFilter(p => p.DeclaredAccessibility == Accessibility.Protected);
    }

    /// <summary>
    /// Filters for internal properties.
    /// </summary>
    public PropertyQuery ThatAreInternal()
    {
        return AddFilter(p => p.DeclaredAccessibility == Accessibility.Internal);
    }

    /// <summary>
    /// Filters for protected internal properties.
    /// </summary>
    public PropertyQuery ThatAreProtectedOrInternal()
    {
        return AddFilter(p => p.DeclaredAccessibility == Accessibility.ProtectedOrInternal);
    }

    #endregion

    #region Modifier Filters

    /// <summary>
    /// Filters for static properties.
    /// </summary>
    public PropertyQuery ThatAreStatic()
    {
        return AddFilter(p => p.IsStatic);
    }

    /// <summary>
    /// Filters for instance properties (non-static).
    /// </summary>
    public PropertyQuery ThatAreInstance()
    {
        return AddFilter(p => !p.IsStatic);
    }

    /// <summary>
    /// Filters for virtual properties.
    /// </summary>
    public PropertyQuery ThatAreVirtual()
    {
        return AddFilter(p => p.IsVirtual);
    }

    /// <summary>
    /// Filters for abstract properties.
    /// </summary>
    public PropertyQuery ThatAreAbstract()
    {
        return AddFilter(p => p.IsAbstract);
    }

    /// <summary>
    /// Filters for override properties.
    /// </summary>
    public PropertyQuery ThatAreOverride()
    {
        return AddFilter(p => p.IsOverride);
    }

    /// <summary>
    /// Filters for sealed properties.
    /// </summary>
    public PropertyQuery ThatAreSealed()
    {
        return AddFilter(p => p.IsSealed);
    }

    /// <summary>
    /// Filters for read-only properties (no setter).
    /// </summary>
    public PropertyQuery ThatAreReadOnly()
    {
        return AddFilter(p => p.SetMethod == null);
    }

    /// <summary>
    /// Filters for write-only properties (no getter).
    /// </summary>
    public PropertyQuery ThatAreWriteOnly()
    {
        return AddFilter(p => p.GetMethod == null);
    }

    /// <summary>
    /// Filters for read-write properties (both getter and setter).
    /// </summary>
    public PropertyQuery ThatAreReadWrite()
    {
        return AddFilter(p => p.GetMethod != null && p.SetMethod != null);
    }

    /// <summary>
    /// Filters for properties with init-only setters.
    /// </summary>
    public PropertyQuery WithInitOnlySetter()
    {
        return AddFilter(p => p.SetMethod?.IsInitOnly == true);
    }

    /// <summary>
    /// Filters for required properties.
    /// </summary>
    public PropertyQuery ThatAreRequired()
    {
        return AddFilter(p => p.IsRequired);
    }

    #endregion

    #region Name Filters

    /// <summary>
    /// Filters for properties with the specified name.
    /// </summary>
    public PropertyQuery WithName(string name)
    {
        return AddFilter(p => p.Name == name);
    }

    /// <summary>
    /// Filters for properties whose names start with the specified prefix.
    /// </summary>
    public PropertyQuery WithNameStartingWith(string prefix)
    {
        return AddFilter(p => p.Name.StartsWith(prefix, StringComparison.Ordinal));
    }

    /// <summary>
    /// Filters for properties whose names contain the specified substring.
    /// </summary>
    public PropertyQuery WithNameContaining(string substring)
    {
        return AddFilter(p => p.Name.Contains(substring));
    }

    /// <summary>
    /// Filters for properties whose names end with the specified suffix.
    /// </summary>
    public PropertyQuery WithNameEndingWith(string suffix)
    {
        return AddFilter(p => p.Name.EndsWith(suffix, StringComparison.Ordinal));
    }

    #endregion

    #region Type Filters

    /// <summary>
    /// Filters for properties of the specified type.
    /// </summary>
    public PropertyQuery OfType(ITypeSymbol typeSymbol)
    {
        return AddFilter(p => SymbolEqualityComparer.Default.Equals(p.Type, typeSymbol));
    }

    /// <summary>
    /// Filters for properties whose type name matches the specified name.
    /// </summary>
    public PropertyQuery OfTypeName(string typeName)
    {
        return AddFilter(p => p.Type.Name == typeName);
    }

    /// <summary>
    /// Filters for properties whose type matches the predicate.
    /// </summary>
    public PropertyQuery OfType(Func<ITypeSymbol, bool> typePredicate)
    {
        return AddFilter(p => typePredicate(p.Type));
    }

    #endregion

    #region Attribute Filters

    /// <summary>
    /// Filters for properties that have the specified attribute.
    /// </summary>
    public PropertyQuery WithAttribute<TAttribute>() where TAttribute : Attribute
    {
        return AddFilter(p => p.GetAttributesByType<TAttribute>().Any());
    }

    /// <summary>
    /// Filters for properties with the specified attribute name (with or without "Attribute" suffix).
    /// </summary>
    public PropertyQuery WithAttribute(string attributeName)
    {
        return AddFilter(p => p.GetAttributes().Any(a =>
            a.AttributeClass?.Name == attributeName ||
            a.AttributeClass?.Name == attributeName + "Attribute"));
    }

    /// <summary>
    /// Filters for properties that do not have the specified attribute.
    /// </summary>
    public PropertyQuery WithoutAttribute<TAttribute>() where TAttribute : Attribute
    {
        return AddFilter(p => !p.GetAttributesByType<TAttribute>().Any());
    }

    #endregion

    #region Generic Filter

    /// <summary>
    /// Filters for properties matching the custom predicate.
    /// Use this as an escape hatch for complex or uncommon filters.
    /// </summary>
    public PropertyQuery Where(Func<IPropertySymbol, bool> predicate)
    {
        return AddFilter(predicate);
    }

    #endregion

    #region Materialization

    /// <summary>
    /// Gets all matching and returns all matching properties as projected symbols.
    /// </summary>
    public ImmutableArray<ValidSymbol<IPropertySymbol>> GetAll()
    {
        var properties = _typeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => !p.IsImplicitlyDeclared);

        // Apply all filters
        foreach (var filter in _filters) properties = properties.Where(filter);

        return [..properties.Select(p => ValidSymbol<IPropertySymbol>.From(p))];
    }

    /// <summary>
    /// Gets all matching and returns all matching property symbols (without projection wrapper).
    /// Use this when you need raw IPropertySymbol instances for Roslyn APIs.
    /// </summary>
    public ImmutableArray<IPropertySymbol> GetAllSymbols()
    {
        var properties = _typeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => !p.IsImplicitlyDeclared);

        // Apply all filters
        foreach (var filter in _filters) properties = properties.Where(filter);

        return [..properties];
    }

    /// <summary>
    /// Gets all matching and projects each property using the specified mapper.
    /// </summary>
    public ImmutableArray<TModel> Select<TModel>(Func<ValidSymbol<IPropertySymbol>, TModel> mapper)
    {
        return [..GetAll().Select(mapper)];
    }

    /// <summary>
    /// Gets all matching and projects each property to a collection, then flattens the results.
    /// </summary>
    public ImmutableArray<TModel> SelectMany<TModel>(Func<ValidSymbol<IPropertySymbol>, IEnumerable<TModel>> mapper)
    {
        return [..GetAll().SelectMany(mapper)];
    }

    /// <summary>
    /// Gets the first matching property, or an empty OptionalSymbol if none found.
    /// </summary>
    public OptionalSymbol<IPropertySymbol> FirstOrDefault()
    {
        var all = GetAll();
        return all.Length > 0
            ? OptionalSymbol<IPropertySymbol>.WithValue(all[0].Value)
            : OptionalSymbol<IPropertySymbol>.Empty();
    }

    /// <summary>
    /// Gets the first matching property, or throws if none found.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no properties match the query.</exception>
    public ValidSymbol<IPropertySymbol> First()
    {
        var all = GetAll();
        return all.Length > 0
            ? all[0]
            : throw new InvalidOperationException("No properties matched the query criteria.");
    }

    /// <summary>
    /// Gets all matching and returns true if any properties match, false otherwise.
    /// </summary>
    public bool Any()
    {
        return GetAll().Length > 0;
    }

    /// <summary>
    /// Gets all matching and returns the count of matching properties.
    /// </summary>
    public int Count()
    {
        return GetAll().Length;
    }

    #endregion
}