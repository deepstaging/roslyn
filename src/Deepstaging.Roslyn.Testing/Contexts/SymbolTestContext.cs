// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5


// ReSharper disable MemberCanBePrivate.Global

namespace Deepstaging.Roslyn.Testing;

/// <summary>
/// Provides convenient APIs for querying and extracting symbols from a compilation in tests.
/// </summary>
public class SymbolTestContext
{
    /// <summary>
    /// The underlying compilation.
    /// </summary>
    public Compilation Compilation { get; }
    
    internal SymbolTestContext(Compilation compilation)
    {
        Compilation = compilation;
    }
    
    /// <summary>
    /// Execute a query projection that requires both a symbol and compilation.
    /// Returns the result directly.
    /// </summary>
    /// <example>
    /// <code>
    /// var systemInfo = SymbolsFor(source).Query(
    ///     s => s.RequireNamedType("Runtime"),
    ///     (symbol, compilation) => symbol.QueryEffectsSystemInfo(compilation));
    /// </code>
    /// </example>
    public TResult Query<TSymbol, TResult>(
        Func<SymbolTestContext, ValidSymbol<TSymbol>> symbolSelector,
        Func<TSymbol, Compilation, TResult> projection)
        where TSymbol : class, ISymbol
    {
        var validSymbol = symbolSelector(this);
        return projection(validSymbol.Value, Compilation);
    }
    
    
    /// <summary>
    /// Get a namespace by name from the compilation.
    /// Returns Empty if not found.
    /// </summary>
    /// <param name="namespaceName">The namespace name (e.g., "MyApp.Services").</param>
    /// <returns>The namespace symbol, or Empty if not found.</returns>
    public OptionalSymbol<INamespaceSymbol> GetNamespace(string namespaceName)
    {
        var parts = namespaceName.Split('.');
        INamespaceSymbol current = Compilation.GlobalNamespace;
        
        foreach (var part in parts)
        {
            var next = current.GetNamespaceMembers().FirstOrDefault(ns => ns.Name == part);
            if (next is null)
                return OptionalSymbol<INamespaceSymbol>.Empty();
            current = next;
        }
        
        return OptionalSymbol<INamespaceSymbol>.WithValue(current);
    }
    
    /// <summary>
    /// Get a namespace by name from the compilation.
    /// Throws if not found.
    /// </summary>
    /// <param name="namespaceName">The namespace name (e.g., "MyApp.Services").</param>
    /// <returns>The namespace symbol.</returns>
    public ValidSymbol<INamespaceSymbol> RequireNamespace(string namespaceName) =>
        GetNamespace(namespaceName).ValidateOrThrow();
    
    /// <summary>
    /// Start a fluent query for a specific type by name.
    /// </summary>
    /// <param name="typeName">The type name to query.</param>
    public TypeQueryContext Type(string typeName)
    {
        return new TypeQueryContext(this, typeName);
    }
    
    /// <summary>s
    /// Start a fluent query for types in the source code under test (excludes referenced assemblies).
    /// </summary>
    public TypeQuery Types()
    {
        var sourceTreePaths = Compilation.SyntaxTrees.Select(t => t.FilePath).ToHashSet();
        
        return TypeQuery.From(Compilation)
            .Where(t => t.Locations.Any(loc => 
                loc.IsInSource && 
                sourceTreePaths.Contains(loc.SourceTree?.FilePath ?? "")));
    }
    
    /// <summary>
    /// Start a fluent query for all types in the entire compilation (includes referenced assemblies).
    /// Use this when you need to query system types or types from dependencies.
    /// For querying only the code under test, use Types() instead.
    /// </summary>
    public TypesQueryContext AllTypesInCompilation()
    {
        return new TypesQueryContext(this);
    }
    
    /// <summary>
    /// Execute a custom projection on a type.
    /// </summary>
    public T Map<T>(string typeName, Func<OptionalSymbol<ITypeSymbol>, T> projection)
    {
        var type = GetType(typeName);
        return projection(type);
    }
    
    /// <summary>
    /// Get a symbol by name and cast to INamedTypeSymbol.
    /// Returns Empty if not found or not a named type.
    /// </summary>
    public OptionalSymbol<INamedTypeSymbol> GetNamedType(string typeName)
    {
        var types = GetAllTypes(Compilation.GlobalNamespace)
            .Where(t => t.Name == typeName)
            .ToArray();

        var optionalSymbol = types.Length == 1
            ? OptionalSymbol<INamedTypeSymbol>.WithValue(types[0])
            : OptionalSymbol<INamedTypeSymbol>.Empty();
        
        return optionalSymbol;
    }
    
    /// <summary>
    /// Get a symbol by name and cast to INamedTypeSymbol.
    /// Throws if not found or not a named type.
    /// </summary>
    public ValidSymbol<INamedTypeSymbol> RequireNamedType(string typeName) =>
        GetNamedType(typeName).ValidateOrThrow();
    
    /// <summary>
    /// Get a symbol by name and cast to ITypeSymbol.
    /// Returns Empty if not found or not a type.
    /// </summary>
    public OptionalSymbol<ITypeSymbol> GetType(string typeName)
    {
        var types = GetAllTypes(Compilation.GlobalNamespace)
            .Where(t => t.Name == typeName)
            .ToArray();
        
        return types.Length == 1
            ? OptionalSymbol<ITypeSymbol>.WithValue(types[0])
            : OptionalSymbol<ITypeSymbol>.Empty();
    }
    
    /// <summary>
    /// Get a symbol by name and cast to ITypeSymbol.
    /// Throws if not found or not a type.
    /// </summary>
    public ValidSymbol<ITypeSymbol> RequireType(string typeName) =>
        GetType(typeName).ValidateOrThrow();
    
    /// <summary>
    /// Get a method by name from all types in compilation and cast to IMethodSymbol.
    /// Returns Empty if not found or not a method.
    /// </summary>
    public OptionalSymbol<IMethodSymbol> GetMethod(string methodName)
    {
        var methods = GetAllTypes(Compilation.GlobalNamespace)
            .SelectMany(t => t.GetMembers(methodName))
            .OfType<IMethodSymbol>()
            .ToArray();
        
        return methods.Length == 1
            ? OptionalSymbol<IMethodSymbol>.WithValue(methods[0])
            : OptionalSymbol<IMethodSymbol>.Empty();
    }
    
    /// <summary>
    /// Get a method by name from all types in compilation and cast to IMethodSymbol.
    /// Throws if not found or not a method.
    /// </summary>
    public ValidSymbol<IMethodSymbol> RequireMethod(string methodName) =>
        GetMethod(methodName).ValidateOrThrow();
    
    /// <summary>
    /// Get a property by name from all types in compilation and cast to IPropertySymbol.
    /// Returns Empty if not found or not a property.
    /// </summary>
    public OptionalSymbol<IPropertySymbol> GetProperty(string propertyName)
    {
        var properties = GetAllTypes(Compilation.GlobalNamespace)
            .SelectMany(t => t.GetMembers(propertyName))
            .OfType<IPropertySymbol>()
            .ToArray();
        
        return properties.Length == 1
            ? OptionalSymbol<IPropertySymbol>.WithValue(properties[0])
            : OptionalSymbol<IPropertySymbol>.Empty();
    }
    
    /// <summary>
    /// Get a property by name from all types in compilation and cast to IPropertySymbol.
    /// Throws if not found or not a property.
    /// </summary>
    public ValidSymbol<IPropertySymbol> RequireProperty(string propertyName) =>
        GetProperty(propertyName).ValidateOrThrow();
    
    /// <summary>
    /// Get a field by name from all types in compilation and cast to IFieldSymbol.
    /// Returns Empty if not found or not a field.
    /// </summary>
    public OptionalSymbol<IFieldSymbol> GetField(string fieldName)
    {
        var fields = GetAllTypes(Compilation.GlobalNamespace)
            .SelectMany(t => t.GetMembers(fieldName))
            .OfType<IFieldSymbol>()
            .ToArray();
        
        return fields.Length == 1
            ? OptionalSymbol<IFieldSymbol>.WithValue(fields[0])
            : OptionalSymbol<IFieldSymbol>.Empty();
    }
    
    /// <summary>
    /// Get a field by name from all types in compilation and cast to IFieldSymbol.
    /// Throws if not found or not a field.
    /// </summary>
    public ValidSymbol<IFieldSymbol> RequireField(string fieldName) =>
        GetField(fieldName).ValidateOrThrow();
    
    /// <summary>
    /// Get a parameter by name from all methods in compilation and cast to IParameterSymbol.
    /// Returns Empty if not found or not a parameter.
    /// </summary>
    public OptionalSymbol<IParameterSymbol> GetParameter(string parameterName)
    {
        var parameters = GetAllTypes(Compilation.GlobalNamespace)
            .SelectMany(t => t.GetMembers())
            .OfType<IMethodSymbol>()
            .SelectMany(m => m.Parameters)
            .Where(p => p.Name == parameterName)
            .ToArray();
        
        return parameters.Length == 1
            ? OptionalSymbol<IParameterSymbol>.WithValue(parameters[0])
            : OptionalSymbol<IParameterSymbol>.Empty();
    }
    
    /// <summary>
    /// Get a parameter by name from all methods in compilation and cast to IParameterSymbol.
    /// Throws if not found or not a parameter.
    /// </summary>
    public ValidSymbol<IParameterSymbol> RequireParameter(string parameterName) =>
        GetParameter(parameterName).ValidateOrThrow();
    
    private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol ns)
    {
        foreach (var type in ns.GetTypeMembers())
        {
            yield return type;
            
            // Recursively get nested types
            foreach (var nested in GetNestedTypes(type))
            {
                yield return nested;
            }
        }
        
        foreach (var childNs in ns.GetNamespaceMembers())
        {
            foreach (var type in GetAllTypes(childNs))
            {
                yield return type;
            }
        }
    }
    
    private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
    {
        foreach (var nested in type.GetTypeMembers())
        {
            yield return nested;
            foreach (var child in GetNestedTypes(nested))
            {
                yield return child;
            }
        }
    }
}

/// <summary>
/// Fluent query context for a specific type.
/// </summary>
public class TypeQueryContext
{
    private readonly SymbolTestContext _context;
    private readonly string _typeName;
    
    internal TypeQueryContext(SymbolTestContext context, string typeName)
    {
        _context = context;
        _typeName = typeName;
    }
    
    /// <summary>
    /// Start querying properties on this type.
    /// </summary>
    public PropertyQuery Properties()
    {
        return _context.GetType(_typeName).QueryProperties();
    }
    
    /// <summary>
    /// Start querying methods on this type.
    /// </summary>
    public MethodQuery Methods()
    {
        return _context.GetType(_typeName).QueryMethods();
    }
    
    /// <summary>
    /// Start querying constructors on this type.
    /// </summary>
    public ConstructorQuery Constructors()
    {
        return _context.GetType(_typeName).QueryConstructors();
    }
}

/// <summary>
/// Fluent query context for all types in the compilation.
/// </summary>
public class TypesQueryContext
{
    private readonly SymbolTestContext _context;
    private readonly TypeQuery _query;
    
    internal TypesQueryContext(SymbolTestContext context)
    {
        _context = context;
        _query = TypeQuery.From(_context.Compilation);
    }
    
    private TypesQueryContext(SymbolTestContext context, TypeQuery query)
    {
        _context = context;
        _query = query;
    }
    
    /// <summary>
    /// Get the underlying TypeQuery for further operations.
    /// </summary>
    public TypeQuery Query()
    {
        return _query;
    }
    
    /// <summary>
    /// Filter to only public types.
    /// </summary>
    public TypesQueryContext ThatArePublic()
    {
        return new TypesQueryContext(_context, _query.ThatArePublic());
    }
    
    /// <summary>
    /// Filter to only class types.
    /// </summary>
    public TypesQueryContext ThatAreClasses()
    {
        return new TypesQueryContext(_context, _query.ThatAreClasses());
    }
    
    /// <summary>
    /// Filter to only interface types.
    /// </summary>
    public TypesQueryContext ThatAreInterfaces()
    {
        return new TypesQueryContext(_context, _query.ThatAreInterfaces());
    }
    
    /// <summary>
    /// Filter to types with a specific attribute.
    /// </summary>
    public TypesQueryContext WithAttribute(string attributeName)
    {
        return new TypesQueryContext(_context, _query.WithAttribute(attributeName));
    }
}
