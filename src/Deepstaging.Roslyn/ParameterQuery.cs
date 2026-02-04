// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Provides a fluent API for querying parameters on a method symbol.
/// Supports chainable filters for common parameter query scenarios.
/// </summary>
public readonly struct ParameterQuery
{
    private readonly IMethodSymbol _methodSymbol;
    private readonly ImmutableArray<Func<IParameterSymbol, bool>> _filters;

    private ParameterQuery(IMethodSymbol methodSymbol, ImmutableArray<Func<IParameterSymbol, bool>> filters)
    {
        _methodSymbol = methodSymbol;
        _filters = filters;
    }

    /// <summary>
    /// Creates a new parameter query for the specified method symbol.
    /// </summary>
    public static ParameterQuery From(IMethodSymbol methodSymbol)
    {
        return new ParameterQuery(methodSymbol, ImmutableArray<Func<IParameterSymbol, bool>>.Empty);
    }

    private ParameterQuery AddFilter(Func<IParameterSymbol, bool> filter)
    {
        return new ParameterQuery(_methodSymbol, _filters.Add(filter));
    }

    #region Modifier Filters

    /// <summary>
    /// Filters for ref parameters.
    /// </summary>
    public ParameterQuery ThatAreRef()
    {
        return AddFilter(p => p.RefKind == RefKind.Ref);
    }

    /// <summary>
    /// Filters for out parameters.
    /// </summary>
    public ParameterQuery ThatAreOut()
    {
        return AddFilter(p => p.RefKind == RefKind.Out);
    }

    /// <summary>
    /// Filters for in parameters.
    /// </summary>
    public ParameterQuery ThatAreIn()
    {
        return AddFilter(p => p.RefKind == RefKind.In);
    }

    /// <summary>
    /// Filters for params parameters (parameter arrays).
    /// </summary>
    public ParameterQuery ThatAreParams()
    {
        return AddFilter(p => p.IsParams);
    }

    /// <summary>
    /// Filters for optional parameters (have default values).
    /// </summary>
    public ParameterQuery ThatAreOptional()
    {
        return AddFilter(p => p.IsOptional);
    }

    /// <summary>
    /// Filters for required parameters (no default values).
    /// </summary>
    public ParameterQuery ThatAreRequired()
    {
        return AddFilter(p => !p.IsOptional);
    }

    /// <summary>
    /// Filters for this parameters (extension method receivers).
    /// </summary>
    public ParameterQuery ThatAreThis()
    {
        return AddFilter(p => p.IsThis);
    }

    /// <summary>
    /// Filters for parameters that discard their value (named _).
    /// </summary>
    public ParameterQuery ThatAreDiscards()
    {
        return AddFilter(p => p.IsDiscard);
    }

    #endregion

    #region Name Filters

    /// <summary>
    /// Filters for parameters with the exact name.
    /// </summary>
    public ParameterQuery WithName(string name)
    {
        return AddFilter(p => p.Name == name);
    }

    /// <summary>
    /// Filters for parameters with names starting with the specified prefix.
    /// </summary>
    public ParameterQuery WithNamePrefix(string prefix)
    {
        return AddFilter(p => p.Name.StartsWith(prefix));
    }

    /// <summary>
    /// Filters for parameters with names ending with the specified suffix.
    /// </summary>
    public ParameterQuery WithNameSuffix(string suffix)
    {
        return AddFilter(p => p.Name.EndsWith(suffix));
    }

    /// <summary>
    /// Filters for parameters with names matching a predicate.
    /// </summary>
    public ParameterQuery WithNameMatching(Func<string, bool> predicate)
    {
        return AddFilter(p => predicate(p.Name));
    }

    #endregion

    #region Type Filters

    /// <summary>
    /// Filters for parameters of a specific type.
    /// </summary>
    public ParameterQuery WithType<T>()
    {
        return AddFilter(p => p.Type.Name == typeof(T).Name);
    }

    /// <summary>
    /// Filters for parameters of a specific type by name.
    /// </summary>
    public ParameterQuery WithType(string typeName)
    {
        return AddFilter(p => p.Type.Name == typeName);
    }

    /// <summary>
    /// Filters for parameters with generic types.
    /// </summary>
    public ParameterQuery ThatAreGenericType()
    {
        return AddFilter(p => p.Type is INamedTypeSymbol { IsGenericType: true });
    }

    /// <summary>
    /// Filters for parameters with nullable value types.
    /// </summary>
    public ParameterQuery ThatAreNullable()
    {
        return AddFilter(p => p.Type.NullableAnnotation == NullableAnnotation.Annotated);
    }

    #endregion

    #region Position Filters

    /// <summary>
    /// Filters for parameters at a specific position (0-based index).
    /// </summary>
    public ParameterQuery AtPosition(int index)
    {
        return AddFilter(p => p.Ordinal == index);
    }

    /// <summary>
    /// Filters for the first parameter (position 0).
    /// </summary>
    public ParameterQuery ThatAreFirst()
    {
        return AddFilter(p => p.Ordinal == 0);
    }

    /// <summary>
    /// Filters for the last parameter.
    /// </summary>
    public ParameterQuery ThatAreLast()
    {
        var paramCount = _methodSymbol.Parameters.Length;
        return AddFilter(p => p.Ordinal == paramCount - 1);
    }

    #endregion

    #region Attribute Filters

    /// <summary>
    /// Filters for parameters that have the specified attribute.
    /// </summary>
    public ParameterQuery WithAttribute<TAttribute>() where TAttribute : Attribute
    {
        return AddFilter(p => p.GetAttributesByType<TAttribute>().Any());
    }

    /// <summary>
    /// Filters for parameters that have an attribute with the specified name.
    /// </summary>
    public ParameterQuery WithAttribute(string attributeName)
    {
        return AddFilter(p => p.GetAttributesByName(attributeName).Any());
    }

    /// <summary>
    /// Filters for parameters that do not have the specified attribute.
    /// </summary>
    public ParameterQuery WithoutAttribute<TAttribute>() where TAttribute : Attribute
    {
        return AddFilter(p => !p.GetAttributesByType<TAttribute>().Any());
    }

    #endregion

    #region Custom Filter

    /// <summary>
    /// Filters for parameters matching the custom predicate.
    /// Use this as an escape hatch for complex or uncommon filters.
    /// </summary>
    public ParameterQuery Where(Func<IParameterSymbol, bool> predicate)
    {
        return AddFilter(predicate);
    }

    #endregion

    #region Materialization

    /// <summary>
    /// Gets all matching and returns all matching parameters as optional symbols.
    /// </summary>
    public ImmutableArray<ValidSymbol<IParameterSymbol>> GetAll()
    {
        var parameters = _methodSymbol.Parameters.AsEnumerable();

        // Apply all filters
        foreach (var filter in _filters) parameters = parameters.Where(filter);

        return [..parameters.Select(p => ValidSymbol<IParameterSymbol>.From(p))];
    }

    /// <summary>
    /// Gets all matching and returns all matching parameter symbols (without projection wrapper).
    /// Use this when you need raw IParameterSymbol instances for Roslyn APIs.
    /// </summary>
    public ImmutableArray<IParameterSymbol> GetAllSymbols()
    {
        var parameters = _methodSymbol.Parameters.AsEnumerable();

        // Apply all filters
        foreach (var filter in _filters) parameters = parameters.Where(filter);

        return [..parameters];
    }

    /// <summary>
    /// Gets all matching and projects each parameter using the specified mapper.
    /// </summary>
    public ImmutableArray<TModel> Select<TModel>(Func<ValidSymbol<IParameterSymbol>, TModel> mapper)
    {
        return [..GetAll().Select(mapper)];
    }

    /// <summary>
    /// Gets all matching and projects each parameter to a collection, then flattens the results.
    /// </summary>
    public ImmutableArray<TModel> SelectMany<TModel>(Func<ValidSymbol<IParameterSymbol>, IEnumerable<TModel>> mapper)
    {
        return [..GetAll().SelectMany(mapper)];
    }

    /// <summary>
    /// Gets all matching and returns the first matching parameter, or Empty if none found.
    /// </summary>
    public OptionalSymbol<IParameterSymbol> FirstOrDefault()
    {
        var all = GetAll();
        return all.Length > 0
            ? OptionalSymbol<IParameterSymbol>.WithValue(all[0].Value)
            : OptionalSymbol<IParameterSymbol>.Empty();
    }

    /// <summary>
    /// Gets all matching and returns the first matching parameter, or throws if none found.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no parameters match the query.</exception>
    public ValidSymbol<IParameterSymbol> First()
    {
        var all = GetAll();
        return all.Length > 0
            ? all[0]
            : throw new InvalidOperationException("No iparametersymbols matched the query criteria.");
    }

    /// <summary>
    /// Gets all matching and returns true if any parameters match, false otherwise.
    /// </summary>
    public bool Any()
    {
        return GetAll().Length > 0;
    }

    /// <summary>
    /// Gets all matching and returns the count of matching parameters.
    /// </summary>
    public int Count()
    {
        return GetAll().Length;
    }

    #endregion
}