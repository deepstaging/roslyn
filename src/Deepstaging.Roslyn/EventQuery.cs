// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Provides a fluent API for querying events on a type symbol.
/// Supports chainable filters for common event query scenarios.
/// </summary>
public readonly struct EventQuery
{
    private readonly ITypeSymbol _typeSymbol;
    private readonly ImmutableArray<Func<IEventSymbol, bool>> _filters;

    private EventQuery(ITypeSymbol typeSymbol, ImmutableArray<Func<IEventSymbol, bool>> filters)
    {
        _typeSymbol = typeSymbol;
        _filters = filters;
    }

    /// <summary>
    /// Creates a new event query for the specified type symbol.
    /// </summary>
    public static EventQuery From(ITypeSymbol typeSymbol) =>
        new(typeSymbol, ImmutableArray<Func<IEventSymbol, bool>>.Empty);

    private EventQuery AddFilter(Func<IEventSymbol, bool> filter) => new(_typeSymbol, _filters.Add(filter));

    #region Accessibility Filters

    /// <summary>
    /// Filters for public events.
    /// </summary>
    public EventQuery ThatArePublic() => AddFilter(e => e.DeclaredAccessibility == Accessibility.Public);

    /// <summary>
    /// Filters for non-public events.
    /// </summary>
    public EventQuery ThatAreNotPublic() => AddFilter(e => e.DeclaredAccessibility != Accessibility.Public);

    /// <summary>
    /// Filters for private events.
    /// </summary>
    public EventQuery ThatArePrivate() => AddFilter(e => e.DeclaredAccessibility == Accessibility.Private);

    /// <summary>
    /// Filters for non-private events.
    /// </summary>
    public EventQuery ThatAreNotPrivate() => AddFilter(e => e.DeclaredAccessibility != Accessibility.Private);

    /// <summary>
    /// Filters for protected events.
    /// </summary>
    public EventQuery ThatAreProtected() => AddFilter(e => e.DeclaredAccessibility == Accessibility.Protected);

    /// <summary>
    /// Filters for non-protected events.
    /// </summary>
    public EventQuery ThatAreNotProtected() => AddFilter(e => e.DeclaredAccessibility != Accessibility.Protected);

    /// <summary>
    /// Filters for internal events.
    /// </summary>
    public EventQuery ThatAreInternal() => AddFilter(e => e.DeclaredAccessibility == Accessibility.Internal);

    /// <summary>
    /// Filters for non-internal events.
    /// </summary>
    public EventQuery ThatAreNotInternal() => AddFilter(e => e.DeclaredAccessibility != Accessibility.Internal);

    #endregion

    #region Modifier Filters

    /// <summary>
    /// Filters for static events.
    /// </summary>
    public EventQuery ThatAreStatic() => AddFilter(e => e.IsStatic);

    /// <summary>
    /// Filters for instance events (non-static).
    /// </summary>
    public EventQuery ThatAreInstance() => AddFilter(e => !e.IsStatic);

    /// <summary>
    /// Filters for abstract events.
    /// </summary>
    public EventQuery ThatAreAbstract() => AddFilter(e => e.IsAbstract);

    /// <summary>
    /// Filters for non-abstract events.
    /// </summary>
    public EventQuery ThatAreNotAbstract() => AddFilter(e => !e.IsAbstract);

    /// <summary>
    /// Filters for virtual events.
    /// </summary>
    public EventQuery ThatAreVirtual() => AddFilter(e => e.IsVirtual);

    /// <summary>
    /// Filters for non-virtual events.
    /// </summary>
    public EventQuery ThatAreNotVirtual() => AddFilter(e => !e.IsVirtual);

    /// <summary>
    /// Filters for sealed events.
    /// </summary>
    public EventQuery ThatAreSealed() => AddFilter(e => e.IsSealed);

    /// <summary>
    /// Filters for non-sealed events.
    /// </summary>
    public EventQuery ThatAreNotSealed() => AddFilter(e => !e.IsSealed);

    /// <summary>
    /// Filters for override events.
    /// </summary>
    public EventQuery ThatAreOverride() => AddFilter(e => e.IsOverride);

    /// <summary>
    /// Filters for events that are not overrides.
    /// </summary>
    public EventQuery ThatAreNotOverride() => AddFilter(e => !e.IsOverride);

    #endregion

    #region Name Filters

    /// <summary>
    /// Filters for events with the exact name.
    /// </summary>
    public EventQuery WithName(string name) => AddFilter(e => e.Name == name);

    /// <summary>
    /// Filters for events with names starting with the specified prefix.
    /// </summary>
    public EventQuery WithNamePrefix(string prefix) => AddFilter(e => e.Name.StartsWith(prefix));

    /// <summary>
    /// Filters for events with names ending with the specified suffix.
    /// </summary>
    public EventQuery WithNameSuffix(string suffix) => AddFilter(e => e.Name.EndsWith(suffix));

    /// <summary>
    /// Filters for events with names matching a predicate.
    /// </summary>
    public EventQuery WithNameMatching(Func<string, bool> predicate) => AddFilter(e => predicate(e.Name));

    #endregion

    #region Type Filters

    /// <summary>
    /// Filters for events of a specific type.
    /// </summary>
    public EventQuery WithType<T>() => AddFilter(e => e.Type.Name == typeof(T).Name);

    /// <summary>
    /// Filters for events of a specific type by name.
    /// </summary>
    public EventQuery WithType(string typeName) => AddFilter(e => e.Type.Name == typeName);

    #endregion

    #region Attribute Filters

    /// <summary>
    /// Filters for events that have the specified attribute.
    /// </summary>
    public EventQuery WithAttribute<TAttribute>() where TAttribute : Attribute =>
        AddFilter(e => e.GetAttributesByType<TAttribute>().Any());

    /// <summary>
    /// Filters for events that have an attribute with the specified name.
    /// </summary>
    public EventQuery WithAttribute(string attributeName) => AddFilter(e => e.GetAttributesByName(attributeName).Any());

    /// <summary>
    /// Filters for events that do not have the specified attribute.
    /// </summary>
    public EventQuery WithoutAttribute<TAttribute>() where TAttribute : Attribute =>
        AddFilter(e => !e.GetAttributesByType<TAttribute>().Any());

    #endregion

    #region Custom Filter

    /// <summary>
    /// Filters for events matching the custom predicate.
    /// Use this as an escape hatch for complex or uncommon filters.
    /// </summary>
    public EventQuery Where(Func<IEventSymbol, bool> predicate) => AddFilter(predicate);

    #endregion

    #region Materialization

    /// <summary>
    /// Gets all matching and returns all matching events as optional symbols.
    /// </summary>
    public ImmutableArray<ValidSymbol<IEventSymbol>> GetAll()
    {
        if (_typeSymbol is null) return [];

        var events = _typeSymbol.GetMembers()
            .OfType<IEventSymbol>();

        // Apply all filters
        foreach (var filter in _filters) events = events.Where(filter);

        return [..events.Select(e => ValidSymbol<IEventSymbol>.From(e))];
    }

    /// <summary>
    /// Gets all matching and returns all matching event symbols (without projection wrapper).
    /// Use this when you need raw IEventSymbol instances for Roslyn APIs.
    /// </summary>
    public ImmutableArray<IEventSymbol> GetAllSymbols()
    {
        if (_typeSymbol is null) return [];

        var events = _typeSymbol.GetMembers()
            .OfType<IEventSymbol>();

        // Apply all filters
        foreach (var filter in _filters) events = events.Where(filter);

        return [..events];
    }

    /// <summary>
    /// Gets all matching and projects each event using the specified mapper.
    /// </summary>
    public ImmutableArray<TModel> Select<TModel>(Func<ValidSymbol<IEventSymbol>, TModel> mapper) =>
        [..GetAll().Select(mapper)];

    /// <summary>
    /// Gets all matching and projects each event to a collection, then flattens the results.
    /// </summary>
    public ImmutableArray<TModel> SelectMany<TModel>(Func<ValidSymbol<IEventSymbol>, IEnumerable<TModel>> mapper) =>
        [..GetAll().SelectMany(mapper)];

    /// <summary>
    /// Gets all matching and returns the first matching event, or Empty if none found.
    /// </summary>
    public OptionalSymbol<IEventSymbol> FirstOrDefault()
    {
        var all = GetAll();

        return all.Length > 0
            ? OptionalSymbol<IEventSymbol>.WithValue(all[0].Value)
            : OptionalSymbol<IEventSymbol>.Empty();
    }

    /// <summary>
    /// Gets all matching and returns the first matching event, or throws if none found.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no events match the query.</exception>
    public ValidSymbol<IEventSymbol> First()
    {
        var all = GetAll();

        return all.Length > 0
            ? all[0]
            : throw new InvalidOperationException("No ieventsymbols matched the query criteria.");
    }

    /// <summary>
    /// Gets all matching and returns true if any events match, false otherwise.
    /// </summary>
    public bool Any() => GetAll().Length > 0;

    /// <summary>
    /// Gets all matching and returns the count of matching events.
    /// </summary>
    public int Count() => GetAll().Length;

    #endregion
}