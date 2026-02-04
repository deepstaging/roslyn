// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Provides a fluent API for querying constructors on a type symbol.
/// Supports chainable filters for common constructor query scenarios.
/// </summary>
public readonly struct ConstructorQuery
{
    private readonly ITypeSymbol _typeSymbol;
    private readonly ImmutableArray<Func<IMethodSymbol, bool>> _filters;

    private ConstructorQuery(ITypeSymbol typeSymbol, ImmutableArray<Func<IMethodSymbol, bool>> filters)
    {
        _typeSymbol = typeSymbol;
        _filters = filters;
    }

    /// <summary>
    /// Creates a new constructor query for the specified type symbol.
    /// </summary>
    public static ConstructorQuery From(ITypeSymbol typeSymbol)
    {
        return new ConstructorQuery(typeSymbol, ImmutableArray<Func<IMethodSymbol, bool>>.Empty);
    }

    private ConstructorQuery AddFilter(Func<IMethodSymbol, bool> filter)
    {
        return new ConstructorQuery(_typeSymbol, _filters.Add(filter));
    }

    #region Accessibility Filters

    /// <summary>
    /// Filters for public constructors.
    /// </summary>
    public ConstructorQuery ThatArePublic()
    {
        return AddFilter(c => c.DeclaredAccessibility == Accessibility.Public);
    }

    /// <summary>
    /// Filters for private constructors.
    /// </summary>
    public ConstructorQuery ThatArePrivate()
    {
        return AddFilter(c => c.DeclaredAccessibility == Accessibility.Private);
    }

    /// <summary>
    /// Filters for protected constructors.
    /// </summary>
    public ConstructorQuery ThatAreProtected()
    {
        return AddFilter(c => c.DeclaredAccessibility == Accessibility.Protected);
    }

    /// <summary>
    /// Filters for internal constructors.
    /// </summary>
    public ConstructorQuery ThatAreInternal()
    {
        return AddFilter(c => c.DeclaredAccessibility == Accessibility.Internal);
    }

    /// <summary>
    /// Filters for protected internal constructors.
    /// </summary>
    public ConstructorQuery ThatAreProtectedOrInternal()
    {
        return AddFilter(c => c.DeclaredAccessibility == Accessibility.ProtectedOrInternal);
    }

    #endregion

    #region Modifier Filters

    /// <summary>
    /// Filters for static constructors (type initializers).
    /// </summary>
    public ConstructorQuery ThatAreStatic()
    {
        return AddFilter(c => c.IsStatic);
    }

    /// <summary>
    /// Filters for instance constructors (non-static).
    /// </summary>
    public ConstructorQuery ThatAreInstance()
    {
        return AddFilter(c => !c.IsStatic);
    }

    #endregion

    #region Parameter Filters

    /// <summary>
    /// Filters for constructors with the specified number of parameters.
    /// </summary>
    public ConstructorQuery WithParameterCount(int count)
    {
        return AddFilter(c => c.Parameters.Length == count);
    }

    /// <summary>
    /// Filters for parameterless constructors.
    /// </summary>
    public ConstructorQuery WithNoParameters()
    {
        return WithParameterCount(0);
    }

    /// <summary>
    /// Filters for constructors with at least the specified number of parameters.
    /// </summary>
    public ConstructorQuery WithAtLeastParameters(int minCount)
    {
        return AddFilter(c => c.Parameters.Length >= minCount);
    }

    /// <summary>
    /// Filters for constructors where the first parameter is of the specified type.
    /// </summary>
    public ConstructorQuery WithFirstParameterOfType(ITypeSymbol typeSymbol)
    {
        return AddFilter(c => c.Parameters.Length > 0 &&
                              SymbolEqualityComparer.Default.Equals(c.Parameters[0].Type, typeSymbol));
    }

    /// <summary>
    /// Filters for constructors where any parameter matches the predicate.
    /// </summary>
    public ConstructorQuery WithParameter(Func<IParameterSymbol, bool> parameterPredicate)
    {
        return AddFilter(c => c.Parameters.Any(parameterPredicate));
    }

    /// <summary>
    /// Filters for constructors where all parameters match the predicate.
    /// </summary>
    public ConstructorQuery WhereAllParameters(Func<IParameterSymbol, bool> parameterPredicate)
    {
        return AddFilter(c => c.Parameters.All(parameterPredicate));
    }

    #endregion

    #region Attribute Filters

    /// <summary>
    /// Filters for constructors with the specified attribute name (with or without "Attribute" suffix).
    /// </summary>
    public ConstructorQuery WithAttribute(string attributeName)
    {
        return AddFilter(c => c.GetAttributes().Any(a =>
            a.AttributeClass?.Name == attributeName ||
            a.AttributeClass?.Name == attributeName + "Attribute"));
    }

    #endregion

    #region Generic Filter

    /// <summary>
    /// Filters for constructors matching the custom predicate.
    /// Use this as an escape hatch for complex or uncommon filters.
    /// </summary>
    public ConstructorQuery Where(Func<IMethodSymbol, bool> predicate)
    {
        return AddFilter(predicate);
    }

    #endregion

    #region Materialization

    /// <summary>
    /// Gets all matching and returns all matching constructors as optional symbols.
    /// </summary>
    public ImmutableArray<ValidSymbol<IMethodSymbol>> GetAll()
    {
        var constructors = _typeSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.MethodKind == MethodKind.Constructor && !m.IsImplicitlyDeclared);

        // Apply all filters
        foreach (var filter in _filters) constructors = constructors.Where(filter);

        return [..constructors.Select(c => ValidSymbol<IMethodSymbol>.From(c))];
    }

    /// <summary>
    /// Gets all matching and returns all matching constructor symbols (without projection wrapper).
    /// Use this when you need raw IMethodSymbol instances for Roslyn APIs.
    /// </summary>
    public ImmutableArray<IMethodSymbol> GetAllSymbols()
    {
        var constructors = _typeSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.MethodKind == MethodKind.Constructor && !m.IsImplicitlyDeclared);

        // Apply all filters
        foreach (var filter in _filters) constructors = constructors.Where(filter);

        return [..constructors];
    }

    /// <summary>
    /// Gets all matching and projects each constructor using the specified mapper.
    /// </summary>
    public ImmutableArray<TModel> Select<TModel>(Func<ValidSymbol<IMethodSymbol>, TModel> mapper)
    {
        return [..GetAll().Select(mapper)];
    }

    /// <summary>
    /// Gets all matching and projects each constructor to a collection, then flattens the results.
    /// </summary>
    public ImmutableArray<TModel> SelectMany<TModel>(Func<ValidSymbol<IMethodSymbol>, IEnumerable<TModel>> mapper)
    {
        return [..GetAll().SelectMany(mapper)];
    }

    /// <summary>
    /// Gets all matching and returns the first matching constructor, or Empty if none found.
    /// </summary>
    public OptionalSymbol<IMethodSymbol> FirstOrDefault()
    {
        var all = GetAll();
        return all.Length > 0
            ? OptionalSymbol<IMethodSymbol>.WithValue(all[0].Value)
            : OptionalSymbol<IMethodSymbol>.Empty();
    }

    /// <summary>
    /// Gets all matching and returns the first matching constructor, or throws if none found.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no constructors match the query.</exception>
    public ValidSymbol<IMethodSymbol> First()
    {
        var all = GetAll();
        return all.Length > 0
            ? all[0]
            : throw new InvalidOperationException("No imethodsymbols matched the query criteria.");
    }

    /// <summary>
    /// Gets all matching and returns true if any constructors match, false otherwise.
    /// </summary>
    public bool Any()
    {
        return GetAll().Length > 0;
    }

    /// <summary>
    /// Gets all matching and returns the count of matching constructors.
    /// </summary>
    public int Count()
    {
        return GetAll().Length;
    }

    #endregion
}