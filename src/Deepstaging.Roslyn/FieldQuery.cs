// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Provides a fluent API for querying fields on a type symbol.
/// Supports chainable filters for common field query scenarios.
/// </summary>
public readonly struct FieldQuery
{
    private readonly ITypeSymbol _typeSymbol;
    private readonly ImmutableArray<Func<IFieldSymbol, bool>> _filters;

    private FieldQuery(ITypeSymbol typeSymbol, ImmutableArray<Func<IFieldSymbol, bool>> filters)
    {
        _typeSymbol = typeSymbol;
        _filters = filters;
    }

    /// <summary>
    /// Creates a new field query for the specified type symbol.
    /// </summary>
    public static FieldQuery From(ITypeSymbol typeSymbol)
    {
        return new FieldQuery(typeSymbol, ImmutableArray<Func<IFieldSymbol, bool>>.Empty);
    }

    private FieldQuery AddFilter(Func<IFieldSymbol, bool> filter)
    {
        return new FieldQuery(_typeSymbol, _filters.Add(filter));
    }

    #region Accessibility Filters

    /// <summary>
    /// Filters for public fields.
    /// </summary>
    public FieldQuery ThatArePublic()
    {
        return AddFilter(f => f.DeclaredAccessibility == Accessibility.Public);
    }

    /// <summary>
    /// Filters for private fields.
    /// </summary>
    public FieldQuery ThatArePrivate()
    {
        return AddFilter(f => f.DeclaredAccessibility == Accessibility.Private);
    }

    /// <summary>
    /// Filters for protected fields.
    /// </summary>
    public FieldQuery ThatAreProtected()
    {
        return AddFilter(f => f.DeclaredAccessibility == Accessibility.Protected);
    }

    /// <summary>
    /// Filters for internal fields.
    /// </summary>
    public FieldQuery ThatAreInternal()
    {
        return AddFilter(f => f.DeclaredAccessibility == Accessibility.Internal);
    }

    #endregion

    #region Modifier Filters

    /// <summary>
    /// Filters for static fields.
    /// </summary>
    public FieldQuery ThatAreStatic()
    {
        return AddFilter(f => f.IsStatic);
    }

    /// <summary>
    /// Filters for instance fields (non-static).
    /// </summary>
    public FieldQuery ThatAreInstance()
    {
        return AddFilter(f => !f.IsStatic);
    }

    /// <summary>
    /// Filters for readonly fields.
    /// </summary>
    public FieldQuery ThatAreReadOnly()
    {
        return AddFilter(f => f.IsReadOnly);
    }

    /// <summary>
    /// Filters for const fields.
    /// </summary>
    public FieldQuery ThatAreConst()
    {
        return AddFilter(f => f.IsConst);
    }

    /// <summary>
    /// Filters for volatile fields.
    /// </summary>
    public FieldQuery ThatAreVolatile()
    {
        return AddFilter(f => f.IsVolatile);
    }

    #endregion

    #region Name Filters

    /// <summary>
    /// Filters for fields with the exact name.
    /// </summary>
    public FieldQuery WithName(string name)
    {
        return AddFilter(f => f.Name == name);
    }

    /// <summary>
    /// Filters for fields with names starting with the specified prefix.
    /// </summary>
    public FieldQuery WithNamePrefix(string prefix)
    {
        return AddFilter(f => f.Name.StartsWith(prefix));
    }

    /// <summary>
    /// Filters for fields with names ending with the specified suffix.
    /// </summary>
    public FieldQuery WithNameSuffix(string suffix)
    {
        return AddFilter(f => f.Name.EndsWith(suffix));
    }

    /// <summary>
    /// Filters for fields with names matching a predicate.
    /// </summary>
    public FieldQuery WithNameMatching(Func<string, bool> predicate)
    {
        return AddFilter(f => predicate(f.Name));
    }

    #endregion

    #region Type Filters

    /// <summary>
    /// Filters for fields of a specific type.
    /// </summary>
    public FieldQuery WithType<T>()
    {
        return AddFilter(f => f.Type.Name == typeof(T).Name);
    }

    /// <summary>
    /// Filters for fields of a specific type by name.
    /// </summary>
    public FieldQuery WithType(string typeName)
    {
        return AddFilter(f => f.Type.Name == typeName);
    }

    /// <summary>
    /// Filters for fields with generic types.
    /// </summary>
    public FieldQuery ThatAreGenericType()
    {
        return AddFilter(f => f.Type is INamedTypeSymbol { IsGenericType: true });
    }

    /// <summary>
    /// Filters for fields with nullable value types.
    /// </summary>
    public FieldQuery ThatAreNullable()
    {
        return AddFilter(f => f.Type.NullableAnnotation == NullableAnnotation.Annotated);
    }

    #endregion

    #region Attribute Filters

    /// <summary>
    /// Filters for fields that have the specified attribute.
    /// </summary>
    public FieldQuery WithAttribute<TAttribute>() where TAttribute : Attribute
    {
        return AddFilter(f => f.GetAttributesByType<TAttribute>().Any());
    }

    /// <summary>
    /// Filters for fields that have an attribute with the specified name.
    /// </summary>
    public FieldQuery WithAttribute(string attributeName)
    {
        return AddFilter(f => f.GetAttributesByName(attributeName).Any());
    }

    /// <summary>
    /// Filters for fields that do not have the specified attribute.
    /// </summary>
    public FieldQuery WithoutAttribute<TAttribute>() where TAttribute : Attribute
    {
        return AddFilter(f => !f.GetAttributesByType<TAttribute>().Any());
    }

    #endregion

    #region Custom Filter

    /// <summary>
    /// Filters for fields matching the custom predicate.
    /// Use this as an escape hatch for complex or uncommon filters.
    /// </summary>
    public FieldQuery Where(Func<IFieldSymbol, bool> predicate)
    {
        return AddFilter(predicate);
    }

    #endregion

    #region Materialization

    /// <summary>
    /// Gets all matching and returns all matching fields as optional symbols.
    /// </summary>
    public ImmutableArray<ValidSymbol<IFieldSymbol>> GetAll()
    {
        var fields = _typeSymbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f => !f.IsImplicitlyDeclared);

        // Apply all filters
        foreach (var filter in _filters) fields = fields.Where(filter);

        return [..fields.Select(ValidSymbol<IFieldSymbol>.From)];
    }

    /// <summary>
    /// Gets all matching and returns all matching field symbols (without projection wrapper).
    /// Use this when you need raw IFieldSymbol instances for Roslyn APIs.
    /// </summary>
    public ImmutableArray<IFieldSymbol> GetAllSymbols()
    {
        var fields = _typeSymbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f => !f.IsImplicitlyDeclared);

        // Apply all filters
        foreach (var filter in _filters) fields = fields.Where(filter);

        return [..fields];
    }

    /// <summary>
    /// Gets all matching and projects each field using the specified mapper.
    /// </summary>
    public ImmutableArray<TModel> Select<TModel>(Func<ValidSymbol<IFieldSymbol>, TModel> mapper)
    {
        return [..GetAll().Select(mapper)];
    }

    /// <summary>
    /// Gets all matching and projects each field to a collection, then flattens the results.
    /// </summary>
    public ImmutableArray<TModel> SelectMany<TModel>(Func<ValidSymbol<IFieldSymbol>, IEnumerable<TModel>> mapper)
    {
        return [..GetAll().SelectMany(mapper)];
    }

    /// <summary>
    /// Gets all matching and returns the first matching field, or Empty if none found.
    /// </summary>
    public OptionalSymbol<IFieldSymbol> FirstOrDefault()
    {
        var all = GetAll();
        return all.Length > 0
            ? OptionalSymbol<IFieldSymbol>.WithValue(all[0].Value)
            : OptionalSymbol<IFieldSymbol>.Empty();
    }

    /// <summary>
    /// Gets all matching and returns the first matching field, or throws if none found.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no fields match the query.</exception>
    public ValidSymbol<IFieldSymbol> First()
    {
        var all = GetAll();
        return all.Length > 0
            ? all[0]
            : throw new InvalidOperationException("No ifieldsymbols matched the query criteria.");
    }

    /// <summary>
    /// Gets all matching and returns true if any fields match, false otherwise.
    /// </summary>
    public bool Any()
    {
        return GetAll().Length > 0;
    }

    /// <summary>
    /// Gets all matching and returns the count of matching fields.
    /// </summary>
    public int Count()
    {
        return GetAll().Length;
    }

    #endregion
}