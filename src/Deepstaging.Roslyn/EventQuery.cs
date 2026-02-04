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
    public static EventQuery From(ITypeSymbol typeSymbol)
    {
        return new EventQuery(typeSymbol, ImmutableArray<Func<IEventSymbol, bool>>.Empty);
    }

    private EventQuery AddFilter(Func<IEventSymbol, bool> filter)
    {
        return new EventQuery(_typeSymbol, _filters.Add(filter));
    }

    #region Accessibility Filters

    /// <summary>
    /// Filters for public events.
    /// </summary>
    public EventQuery ThatArePublic()
    {
        return AddFilter(e => e.DeclaredAccessibility == Accessibility.Public);
    }

    /// <summary>
    /// Filters for private events.
    /// </summary>
    public EventQuery ThatArePrivate()
    {
        return AddFilter(e => e.DeclaredAccessibility == Accessibility.Private);
    }

    /// <summary>
    /// Filters for protected events.
    /// </summary>
    public EventQuery ThatAreProtected()
    {
        return AddFilter(e => e.DeclaredAccessibility == Accessibility.Protected);
    }

    /// <summary>
    /// Filters for internal events.
    /// </summary>
    public EventQuery ThatAreInternal()
    {
        return AddFilter(e => e.DeclaredAccessibility == Accessibility.Internal);
    }

    #endregion

    #region Modifier Filters

    /// <summary>
    /// Filters for static events.
    /// </summary>
    public EventQuery ThatAreStatic()
    {
        return AddFilter(e => e.IsStatic);
    }

    /// <summary>
    /// Filters for instance events (non-static).
    /// </summary>
    public EventQuery ThatAreInstance()
    {
        return AddFilter(e => !e.IsStatic);
    }

    /// <summary>
    /// Filters for abstract events.
    /// </summary>
    public EventQuery ThatAreAbstract()
    {
        return AddFilter(e => e.IsAbstract);
    }

    /// <summary>
    /// Filters for virtual events.
    /// </summary>
    public EventQuery ThatAreVirtual()
    {
        return AddFilter(e => e.IsVirtual);
    }

    /// <summary>
    /// Filters for sealed events.
    /// </summary>
    public EventQuery ThatAreSealed()
    {
        return AddFilter(e => e.IsSealed);
    }

    /// <summary>
    /// Filters for override events.
    /// </summary>
    public EventQuery ThatAreOverride()
    {
        return AddFilter(e => e.IsOverride);
    }

    #endregion

    #region Name Filters

    /// <summary>
    /// Filters for events with the exact name.
    /// </summary>
    public EventQuery WithName(string name)
    {
        return AddFilter(e => e.Name == name);
    }

    /// <summary>
    /// Filters for events with names starting with the specified prefix.
    /// </summary>
    public EventQuery WithNamePrefix(string prefix)
    {
        return AddFilter(e => e.Name.StartsWith(prefix));
    }

    /// <summary>
    /// Filters for events with names ending with the specified suffix.
    /// </summary>
    public EventQuery WithNameSuffix(string suffix)
    {
        return AddFilter(e => e.Name.EndsWith(suffix));
    }

    /// <summary>
    /// Filters for events with names matching a predicate.
    /// </summary>
    public EventQuery WithNameMatching(Func<string, bool> predicate)
    {
        return AddFilter(e => predicate(e.Name));
    }

    #endregion

    #region Type Filters

    /// <summary>
    /// Filters for events of a specific type.
    /// </summary>
    public EventQuery WithType<T>()
    {
        return AddFilter(e => e.Type.Name == typeof(T).Name);
    }

    /// <summary>
    /// Filters for events of a specific type by name.
    /// </summary>
    public EventQuery WithType(string typeName)
    {
        return AddFilter(e => e.Type.Name == typeName);
    }

    #endregion

    #region Attribute Filters

    /// <summary>
    /// Filters for events that have the specified attribute.
    /// </summary>
    public EventQuery WithAttribute<TAttribute>() where TAttribute : Attribute
    {
        return AddFilter(e => e.GetAttributesByType<TAttribute>().Any());
    }

    /// <summary>
    /// Filters for events that have an attribute with the specified name.
    /// </summary>
    public EventQuery WithAttribute(string attributeName)
    {
        return AddFilter(e => e.GetAttributesByName(attributeName).Any());
    }

    /// <summary>
    /// Filters for events that do not have the specified attribute.
    /// </summary>
    public EventQuery WithoutAttribute<TAttribute>() where TAttribute : Attribute
    {
        return AddFilter(e => !e.GetAttributesByType<TAttribute>().Any());
    }

    #endregion

    #region Custom Filter

    /// <summary>
    /// Filters for events matching the custom predicate.
    /// Use this as an escape hatch for complex or uncommon filters.
    /// </summary>
    public EventQuery Where(Func<IEventSymbol, bool> predicate)
    {
        return AddFilter(predicate);
    }

    #endregion

    #region Materialization

    /// <summary>
    /// Gets all matching and returns all matching events as optional symbols.
    /// </summary>
    public ImmutableArray<ValidSymbol<IEventSymbol>> GetAll()
    {
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
        var events = _typeSymbol.GetMembers()
            .OfType<IEventSymbol>();

        // Apply all filters
        foreach (var filter in _filters) events = events.Where(filter);

        return [..events];
    }

    /// <summary>
    /// Gets all matching and projects each event using the specified mapper.
    /// </summary>
    public ImmutableArray<TModel> Select<TModel>(Func<ValidSymbol<IEventSymbol>, TModel> mapper)
    {
        return [..GetAll().Select(mapper)];
    }

    /// <summary>
    /// Gets all matching and projects each event to a collection, then flattens the results.
    /// </summary>
    public ImmutableArray<TModel> SelectMany<TModel>(Func<ValidSymbol<IEventSymbol>, IEnumerable<TModel>> mapper)
    {
        return [..GetAll().SelectMany(mapper)];
    }

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
    public bool Any()
    {
        return GetAll().Length > 0;
    }

    /// <summary>
    /// Gets all matching and returns the count of matching events.
    /// </summary>
    public int Count()
    {
        return GetAll().Length;
    }

    #endregion
}