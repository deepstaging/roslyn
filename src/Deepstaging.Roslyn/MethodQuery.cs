// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Provides a fluent API for querying methods on a type symbol.
/// Supports chainable filters for common method query scenarios.
/// </summary>
public readonly struct MethodQuery
{
    private readonly ITypeSymbol _typeSymbol;
    private readonly ImmutableArray<Func<IMethodSymbol, bool>> _filters;

    private MethodQuery(ITypeSymbol typeSymbol, ImmutableArray<Func<IMethodSymbol, bool>> filters)
    {
        _typeSymbol = typeSymbol;
        _filters = filters;
    }

    /// <summary>
    /// Creates a new method query for the specified type symbol.
    /// </summary>
    public static MethodQuery From(ITypeSymbol typeSymbol) =>
        new(typeSymbol, ImmutableArray<Func<IMethodSymbol, bool>>.Empty);

    private MethodQuery AddFilter(Func<IMethodSymbol, bool> filter) => new(_typeSymbol, _filters.Add(filter));

    #region Accessibility Filters

    /// <summary>
    /// Filters for public methods.
    /// </summary>
    public MethodQuery ThatArePublic() => AddFilter(m => m.DeclaredAccessibility == Accessibility.Public);

    /// <summary>
    /// Filters for non-public methods.
    /// </summary>
    public MethodQuery ThatAreNotPublic() => AddFilter(m => m.DeclaredAccessibility != Accessibility.Public);

    /// <summary>
    /// Filters for private methods.
    /// </summary>
    public MethodQuery ThatArePrivate() => AddFilter(m => m.DeclaredAccessibility == Accessibility.Private);

    /// <summary>
    /// Filters for non-private methods.
    /// </summary>
    public MethodQuery ThatAreNotPrivate() => AddFilter(m => m.DeclaredAccessibility != Accessibility.Private);

    /// <summary>
    /// Filters for protected methods.
    /// </summary>
    public MethodQuery ThatAreProtected() => AddFilter(m => m.DeclaredAccessibility == Accessibility.Protected);

    /// <summary>
    /// Filters for non-protected methods.
    /// </summary>
    public MethodQuery ThatAreNotProtected() => AddFilter(m => m.DeclaredAccessibility != Accessibility.Protected);

    /// <summary>
    /// Filters for internal methods.
    /// </summary>
    public MethodQuery ThatAreInternal() => AddFilter(m => m.DeclaredAccessibility == Accessibility.Internal);

    /// <summary>
    /// Filters for non-internal methods.
    /// </summary>
    public MethodQuery ThatAreNotInternal() => AddFilter(m => m.DeclaredAccessibility != Accessibility.Internal);

    /// <summary>
    /// Filters for protected internal methods.
    /// </summary>
    public MethodQuery ThatAreProtectedOrInternal() =>
        AddFilter(m => m.DeclaredAccessibility == Accessibility.ProtectedOrInternal);

    /// <summary>
    /// Filters for methods that are not protected internal.
    /// </summary>
    public MethodQuery ThatAreNotProtectedOrInternal() =>
        AddFilter(m => m.DeclaredAccessibility != Accessibility.ProtectedOrInternal);

    #endregion

    #region Modifier Filters

    /// <summary>
    /// Filters for static methods.
    /// </summary>
    public MethodQuery ThatAreStatic() => AddFilter(m => m.IsStatic);

    /// <summary>
    /// Filters for instance methods (non-static).
    /// </summary>
    public MethodQuery ThatAreInstance() => AddFilter(m => !m.IsStatic);

    /// <summary>
    /// Filters for async methods (methods marked with async keyword).
    /// </summary>
    public MethodQuery ThatAreAsync() => AddFilter(m => m.IsAsync);

    /// <summary>
    /// Filters for methods that are not async.
    /// </summary>
    public MethodQuery ThatAreNotAsync() => AddFilter(m => !m.IsAsync);

    /// <summary>
    /// Filters for generic methods.
    /// </summary>
    public MethodQuery ThatAreGeneric() => AddFilter(m => m.IsGenericMethod);

    /// <summary>
    /// Filters for non-generic methods.
    /// </summary>
    public MethodQuery ThatAreNotGeneric() => AddFilter(m => !m.IsGenericMethod);

    /// <summary>
    /// Filters for virtual methods.
    /// </summary>
    public MethodQuery ThatAreVirtual() => AddFilter(m => m.IsVirtual);

    /// <summary>
    /// Filters for non-virtual methods.
    /// </summary>
    public MethodQuery ThatAreNotVirtual() => AddFilter(m => !m.IsVirtual);

    /// <summary>
    /// Filters for abstract methods.
    /// </summary>
    public MethodQuery ThatAreAbstract() => AddFilter(m => m.IsAbstract);

    /// <summary>
    /// Filters for non-abstract methods.
    /// </summary>
    public MethodQuery ThatAreNotAbstract() => AddFilter(m => !m.IsAbstract);

    /// <summary>
    /// Filters for override methods.
    /// </summary>
    public MethodQuery ThatAreOverrides() => AddFilter(m => m.IsOverride);

    /// <summary>
    /// Filters for methods that are not overrides.
    /// </summary>
    public MethodQuery ThatAreNotOverrides() => AddFilter(m => !m.IsOverride);

    /// <summary>
    /// Filters for sealed methods.
    /// </summary>
    public MethodQuery ThatAreSealed() => AddFilter(m => m.IsSealed);

    /// <summary>
    /// Filters for non-sealed methods.
    /// </summary>
    public MethodQuery ThatAreNotSealed() => AddFilter(m => !m.IsSealed);

    /// <summary>
    /// Filters for partial methods (either definition or implementation).
    /// </summary>
    public MethodQuery ThatArePartial() => AddFilter(m => m.IsPartialMethod());

    /// <summary>
    /// Filters for non-partial methods.
    /// </summary>
    public MethodQuery ThatAreNotPartial() => AddFilter(m => !m.IsPartialMethod());

    /// <summary>
    /// Filters for partial method definitions (declarations without implementation).
    /// These are the `partial void Foo();` declarations that get implemented elsewhere.
    /// </summary>
    public MethodQuery ThatArePartialDefinitions() => AddFilter(m => m.PartialImplementationPart != null);

    /// <summary>
    /// Filters for partial method implementations (the implementing part).
    /// These are the `partial void Foo() { ... }` implementations.
    /// </summary>
    public MethodQuery ThatArePartialImplementations() => AddFilter(m => m.PartialDefinitionPart != null);

    /// <summary>
    /// Filters for extern methods (methods with external implementation).
    /// </summary>
    public MethodQuery ThatAreExtern() => AddFilter(m => m.IsExtern);

    /// <summary>
    /// Filters for non-extern methods.
    /// </summary>
    public MethodQuery ThatAreNotExtern() => AddFilter(m => !m.IsExtern);

    /// <summary>
    /// Filters for readonly methods (struct methods that don't modify state).
    /// </summary>
    public MethodQuery ThatAreReadOnly() => AddFilter(m => m.IsReadOnly);

    /// <summary>
    /// Filters for non-readonly methods.
    /// </summary>
    public MethodQuery ThatAreNotReadOnly() => AddFilter(m => !m.IsReadOnly);

    /// <summary>
    /// Filters for extension methods.
    /// </summary>
    public MethodQuery ThatAreExtensionMethods() => AddFilter(m => m.IsExtensionMethod);

    /// <summary>
    /// Filters for non-extension methods.
    /// </summary>
    public MethodQuery ThatAreNotExtensionMethods() => AddFilter(m => !m.IsExtensionMethod);

    #endregion

    #region Name Filters

    /// <summary>
    /// Filters for methods with the specified name.
    /// </summary>
    public MethodQuery WithName(string name) => AddFilter(m => m.Name == name);

    /// <summary>
    /// Filters for methods whose name starts with the specified prefix.
    /// </summary>
    public MethodQuery WithNameStartingWith(string prefix) => AddFilter(m => m.Name.StartsWith(prefix));

    /// <summary>
    /// Filters for methods whose name ends with the specified suffix.
    /// </summary>
    public MethodQuery WithNameEndingWith(string suffix) => AddFilter(m => m.Name.EndsWith(suffix));

    /// <summary>
    /// Filters for methods whose name contains the specified substring.
    /// </summary>
    public MethodQuery WithNameContaining(string substring) => AddFilter(m => m.Name.Contains(substring));

    /// <summary>
    /// Filters for methods whose name matches the specified regex pattern.
    /// </summary>
    public MethodQuery WithNameMatching(Regex pattern) => AddFilter(m => pattern.IsMatch(m.Name));

    #endregion

    #region Parameter Filters

    /// <summary>
    /// Filters for methods with the specified number of parameters.
    /// </summary>
    public MethodQuery WithParameterCount(int count) => AddFilter(m => m.Parameters.Length == count);

    /// <summary>
    /// Filters for methods with no parameters.
    /// </summary>
    public MethodQuery WithNoParameters() => WithParameterCount(0);

    /// <summary>
    /// Filters for methods with at least one parameter.
    /// </summary>
    public MethodQuery WithParameters() => AddFilter(m => m.Parameters.Length > 0);

    /// <summary>
    /// Filters for methods where the first parameter matches the specified type name.
    /// </summary>
    public MethodQuery WithFirstParameterOfType(string typeName) =>
        AddFilter(m => m.Parameters.Length > 0 && m.Parameters[0].Type.Name == typeName);

    /// <summary>
    /// Filters for methods using a custom parameter predicate.
    /// </summary>
    public MethodQuery WithParameters(Func<ImmutableArray<IParameterSymbol>, bool> predicate) =>
        AddFilter(m => predicate(m.Parameters));

    #endregion

    #region Return Type Filters

    /// <summary>
    /// Filters for methods with the specified return type name.
    /// </summary>
    public MethodQuery WithReturnType(string typeName) => AddFilter(m => m.ReturnType.Name == typeName);

    /// <summary>
    /// Filters for methods with return type matching the specified predicate.
    /// </summary>
    public MethodQuery WithReturnType(Func<ITypeSymbol, bool> predicate) => AddFilter(m => predicate(m.ReturnType));

    /// <summary>
    /// Filters for methods returning void.
    /// </summary>
    public MethodQuery ReturningVoid() => AddFilter(m => m.ReturnsVoid);

    /// <summary>
    /// Filters for methods returning Task.
    /// </summary>
    public MethodQuery ReturningTask() => AddFilter(m => m.ReturnType.Name == "Task");

    /// <summary>
    /// Filters for methods returning ValueTask.
    /// </summary>
    public MethodQuery ReturningValueTask() => AddFilter(m => m.ReturnType.Name == "ValueTask");

    /// <summary>
    /// Filters for methods returning Task&lt;T&gt; or ValueTask&lt;T&gt;.
    /// </summary>
    public MethodQuery ReturningGenericTask() =>
        AddFilter(m => m.ReturnType is INamedTypeSymbol { IsGenericType: true } nt &&
                       (nt.Name == "Task" || nt.Name == "ValueTask"));

    #endregion

    #region Attribute Filters

    /// <summary>
    /// Filters for methods that have the specified attribute.
    /// </summary>
    public MethodQuery WithAttribute<TAttribute>() where TAttribute : Attribute =>
        AddFilter(m => m.GetAttributesByType<TAttribute>().Any());

    /// <summary>
    /// Filters for methods with the specified attribute name (with or without "Attribute" suffix).
    /// </summary>
    public MethodQuery WithAttribute(string attributeName) =>
        AddFilter(m => m.GetAttributes().Any(a =>
            a.AttributeClass?.Name == attributeName ||
            a.AttributeClass?.Name == attributeName + "Attribute"));

    /// <summary>
    /// Filters for methods that do not have the specified attribute.
    /// </summary>
    public MethodQuery WithoutAttribute<TAttribute>() where TAttribute : Attribute =>
        AddFilter(m => !m.GetAttributesByType<TAttribute>().Any());

    #endregion

    #region Generic Filter

    /// <summary>
    /// Filters for methods matching the custom predicate.
    /// Use this as an escape hatch for complex or uncommon filters.
    /// </summary>
    public MethodQuery Where(Func<IMethodSymbol, bool> predicate) => AddFilter(predicate);

    #endregion

    #region Materialization

    /// <summary>
    /// Materializes the query and returns all matching methods as validated symbols.
    /// All returned symbols are guaranteed to be non-null.
    /// </summary>
    public ImmutableArray<ValidSymbol<IMethodSymbol>> GetAll()
    {
        if (_typeSymbol is null) return [];

        var methods = _typeSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.MethodKind == MethodKind.Ordinary && !m.IsImplicitlyDeclared);

        // Apply all filters
        foreach (var filter in _filters) methods = methods.Where(filter);

        return [..methods.Select(m => ValidSymbol<IMethodSymbol>.From(m))];
    }

    /// <summary>
    /// Gets all matching and returns all matching method symbols (without projection wrapper).
    /// Use this when you need raw IMethodSymbol instances for Roslyn APIs.
    /// </summary>
    public ImmutableArray<IMethodSymbol> GetAllSymbols()
    {
        if (_typeSymbol is null) return [];

        var methods = _typeSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.MethodKind == MethodKind.Ordinary && !m.IsImplicitlyDeclared);

        // Apply all filters
        foreach (var filter in _filters) methods = methods.Where(filter);

        return [..methods];
    }

    /// <summary>
    /// Gets all matching and projects each method using the specified mapper.
    /// </summary>
    public ImmutableArray<TModel> Select<TModel>(Func<ValidSymbol<IMethodSymbol>, TModel> mapper) =>
        [..GetAll().Select(mapper)];

    /// <summary>
    /// Gets all matching and projects each method to a collection, then flattens the results.
    /// </summary>
    public ImmutableArray<TModel> SelectMany<TModel>(Func<ValidSymbol<IMethodSymbol>, IEnumerable<TModel>> mapper) =>
        [..GetAll().SelectMany(mapper)];

    /// <summary>
    /// Gets the first matching method, or an empty OptionalSymbol if none found.
    /// </summary>
    public OptionalSymbol<IMethodSymbol> FirstOrDefault()
    {
        var all = GetAll();

        return all.Length > 0
            ? OptionalSymbol<IMethodSymbol>.WithValue(all[0].Value)
            : OptionalSymbol<IMethodSymbol>.Empty();
    }

    /// <summary>
    /// Gets the first matching method, or throws if none found.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no methods match the query.</exception>
    public ValidSymbol<IMethodSymbol> First()
    {
        var all = GetAll();

        return all.Length > 0
            ? all[0]
            : throw new InvalidOperationException("No methods matched the query criteria.");
    }

    /// <summary>
    /// Gets all matching and returns true if any methods match.
    /// </summary>
    public bool Any() => GetAll().Any();

    /// <summary>
    /// Returns the count of matching methods.
    /// </summary>
    public int Count() => GetAll().Length;

    #endregion
}