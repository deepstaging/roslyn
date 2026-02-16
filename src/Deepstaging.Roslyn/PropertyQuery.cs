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
    public static PropertyQuery From(ITypeSymbol typeSymbol) =>
        new(typeSymbol, ImmutableArray<Func<IPropertySymbol, bool>>.Empty);

    private PropertyQuery AddFilter(Func<IPropertySymbol, bool> filter) =>
        new(_typeSymbol, _filters.Add(filter));

    #region Accessibility Filters

    /// <summary>
    /// Filters for public properties.
    /// </summary>
    public PropertyQuery ThatArePublic() => AddFilter(p => p.DeclaredAccessibility == Accessibility.Public);

    /// <summary>
    /// Filters for non-public properties.
    /// </summary>
    public PropertyQuery ThatAreNotPublic() => AddFilter(p => p.DeclaredAccessibility != Accessibility.Public);

    /// <summary>
    /// Filters for private properties.
    /// </summary>
    public PropertyQuery ThatArePrivate() => AddFilter(p => p.DeclaredAccessibility == Accessibility.Private);

    /// <summary>
    /// Filters for non-private properties.
    /// </summary>
    public PropertyQuery ThatAreNotPrivate() => AddFilter(p => p.DeclaredAccessibility != Accessibility.Private);

    /// <summary>
    /// Filters for protected properties.
    /// </summary>
    public PropertyQuery ThatAreProtected() => AddFilter(p => p.DeclaredAccessibility == Accessibility.Protected);

    /// <summary>
    /// Filters for non-protected properties.
    /// </summary>
    public PropertyQuery ThatAreNotProtected() => AddFilter(p => p.DeclaredAccessibility != Accessibility.Protected);

    /// <summary>
    /// Filters for internal properties.
    /// </summary>
    public PropertyQuery ThatAreInternal() => AddFilter(p => p.DeclaredAccessibility == Accessibility.Internal);

    /// <summary>
    /// Filters for non-internal properties.
    /// </summary>
    public PropertyQuery ThatAreNotInternal() => AddFilter(p => p.DeclaredAccessibility != Accessibility.Internal);

    /// <summary>
    /// Filters for protected internal properties.
    /// </summary>
    public PropertyQuery ThatAreProtectedOrInternal() =>
        AddFilter(p => p.DeclaredAccessibility == Accessibility.ProtectedOrInternal);

    /// <summary>
    /// Filters for properties that are not protected internal.
    /// </summary>
    public PropertyQuery ThatAreNotProtectedOrInternal() =>
        AddFilter(p => p.DeclaredAccessibility != Accessibility.ProtectedOrInternal);

    #endregion

    #region Modifier Filters

    /// <summary>
    /// Filters for static properties.
    /// </summary>
    public PropertyQuery ThatAreStatic() => AddFilter(p => p.IsStatic);

    /// <summary>
    /// Filters for instance properties (non-static).
    /// </summary>
    public PropertyQuery ThatAreInstance() => AddFilter(p => !p.IsStatic);

    /// <summary>
    /// Filters for virtual properties.
    /// </summary>
    public PropertyQuery ThatAreVirtual() => AddFilter(p => p.IsVirtual);

    /// <summary>
    /// Filters for non-virtual properties.
    /// </summary>
    public PropertyQuery ThatAreNotVirtual() => AddFilter(p => !p.IsVirtual);

    /// <summary>
    /// Filters for abstract properties.
    /// </summary>
    public PropertyQuery ThatAreAbstract() => AddFilter(p => p.IsAbstract);

    /// <summary>
    /// Filters for non-abstract properties.
    /// </summary>
    public PropertyQuery ThatAreNotAbstract() => AddFilter(p => !p.IsAbstract);

    /// <summary>
    /// Filters for override properties.
    /// </summary>
    public PropertyQuery ThatAreOverride() => AddFilter(p => p.IsOverride);

    /// <summary>
    /// Filters for properties that are not overrides.
    /// </summary>
    public PropertyQuery ThatAreNotOverride() => AddFilter(p => !p.IsOverride);

    /// <summary>
    /// Filters for sealed properties.
    /// </summary>
    public PropertyQuery ThatAreSealed() => AddFilter(p => p.IsSealed);

    /// <summary>
    /// Filters for non-sealed properties.
    /// </summary>
    public PropertyQuery ThatAreNotSealed() => AddFilter(p => !p.IsSealed);

    /// <summary>
    /// Filters for properties that have a getter.
    /// </summary>
    public PropertyQuery ThatAreReadable() => AddFilter(p => p.GetMethod is not null);

    /// <summary>
    /// Filters for properties that have a setter (including init-only setters).
    /// </summary>
    public PropertyQuery ThatAreWritable() => AddFilter(p => p.SetMethod is not null);

    /// <summary>
    /// Filters for read-only properties (no setter).
    /// </summary>
    public PropertyQuery ThatAreReadOnly() => AddFilter(p => p.SetMethod == null);

    /// <summary>
    /// Filters for write-only properties (no getter).
    /// </summary>
    public PropertyQuery ThatAreWriteOnly() => AddFilter(p => p.GetMethod == null);

    /// <summary>
    /// Filters for read-write properties (both getter and setter).
    /// </summary>
    public PropertyQuery ThatAreReadWrite() => AddFilter(p => p.GetMethod != null && p.SetMethod != null);

    /// <summary>
    /// Filters for properties with init-only setters.
    /// </summary>
    public PropertyQuery WithInitOnlySetter() => AddFilter(p => p.SetMethod?.IsInitOnly == true);

    /// <summary>
    /// Filters for required properties.
    /// </summary>
    public PropertyQuery ThatAreRequired() => AddFilter(p => p.IsRequired);

    /// <summary>
    /// Filters for properties that are not required.
    /// </summary>
    public PropertyQuery ThatAreNotRequired() => AddFilter(p => !p.IsRequired);

    #endregion

    #region Name Filters

    /// <summary>
    /// Filters for properties with the specified name.
    /// </summary>
    public PropertyQuery WithName(string name) => AddFilter(p => p.Name == name);

    /// <summary>
    /// Filters for properties whose names start with the specified prefix.
    /// </summary>
    public PropertyQuery WithNameStartingWith(string prefix) =>
        AddFilter(p => p.Name.StartsWith(prefix, StringComparison.Ordinal));

    /// <summary>
    /// Filters for properties whose names contain the specified substring.
    /// </summary>
    public PropertyQuery WithNameContaining(string substring) => AddFilter(p => p.Name.Contains(substring));

    /// <summary>
    /// Filters for properties whose names end with the specified suffix.
    /// </summary>
    public PropertyQuery WithNameEndingWith(string suffix) =>
        AddFilter(p => p.Name.EndsWith(suffix, StringComparison.Ordinal));

    /// <summary>
    /// Filters for properties whose names match the specified regex pattern.
    /// </summary>
    public PropertyQuery WithNameMatching(Regex pattern) => AddFilter(p => pattern.IsMatch(p.Name));

    #endregion

    #region Type Filters

    /// <summary>
    /// Filters for properties of the specified type.
    /// </summary>
    public PropertyQuery OfType(ITypeSymbol typeSymbol) =>
        AddFilter(p => SymbolEqualityComparer.Default.Equals(p.Type, typeSymbol));

    /// <summary>
    /// Filters for properties whose type name matches the specified name.
    /// </summary>
    public PropertyQuery OfTypeName(string typeName) => AddFilter(p => p.Type.Name == typeName);

    /// <summary>
    /// Filters for properties whose type matches the predicate.
    /// </summary>
    public PropertyQuery OfType(Func<ITypeSymbol, bool> typePredicate) => AddFilter(p => typePredicate(p.Type));

    #endregion

    #region Attribute Filters

    /// <summary>
    /// Filters for properties that have the specified attribute.
    /// </summary>
    public PropertyQuery WithAttribute<TAttribute>() where TAttribute : Attribute =>
        AddFilter(p => p.GetAttributesByType<TAttribute>().Any());

    /// <summary>
    /// Filters for properties with the specified attribute name (with or without "Attribute" suffix).
    /// </summary>
    public PropertyQuery WithAttribute(string attributeName) =>
        AddFilter(p => p.GetAttributes().Any(a =>
            a.AttributeClass?.Name == attributeName ||
            a.AttributeClass?.Name == attributeName + "Attribute"));

    /// <summary>
    /// Filters for properties that do not have the specified attribute.
    /// </summary>
    public PropertyQuery WithoutAttribute<TAttribute>() where TAttribute : Attribute =>
        AddFilter(p => !p.GetAttributesByType<TAttribute>().Any());

    #endregion

    #region Generic Filter

    /// <summary>
    /// Filters for properties matching the custom predicate.
    /// Use this as an escape hatch for complex or uncommon filters.
    /// </summary>
    public PropertyQuery Where(Func<IPropertySymbol, bool> predicate) => AddFilter(predicate);

    #endregion

    #region Materialization

    /// <summary>
    /// Gets all matching and returns all matching properties as projected symbols.
    /// </summary>
    public ImmutableArray<ValidSymbol<IPropertySymbol>> GetAll()
    {
        if (_typeSymbol is null) return [];

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
        if (_typeSymbol is null) return [];

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
    public ImmutableArray<TModel> Select<TModel>(Func<ValidSymbol<IPropertySymbol>, TModel> mapper) =>
        [..GetAll().Select(mapper)];

    /// <summary>
    /// Gets all matching and projects each property to a collection, then flattens the results.
    /// </summary>
    public ImmutableArray<TModel> SelectMany<TModel>(Func<ValidSymbol<IPropertySymbol>, IEnumerable<TModel>> mapper) =>
        [..GetAll().SelectMany(mapper)];

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
    public bool Any() => GetAll().Length > 0;

    /// <summary>
    /// Gets all matching and returns the count of matching properties.
    /// </summary>
    public int Count() => GetAll().Length;

    #endregion
}