// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Access = Microsoft.CodeAnalysis.Accessibility;

namespace Deepstaging.Roslyn;

/// <summary>
/// A validated Roslyn symbol where the symbol is guaranteed non-null.
/// Provides a non-nullable API surface, eliminating the need for null checks on core properties.
/// Create via OptionalSymbol.Validate() or ValidSymbol.From().
/// </summary>
/// <typeparam name="TSymbol">The type of Roslyn symbol.</typeparam>
public readonly struct ValidSymbol<TSymbol> : IProjection<TSymbol>
    where TSymbol : class, ISymbol
{
    private readonly TSymbol _symbol;

    private ValidSymbol(TSymbol symbol)
    {
        _symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
    }

    #region Factory Methods

    /// <summary>
    /// Creates a validated projection from a non-null symbol.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if symbol is null.</exception>
    public static ValidSymbol<TSymbol> From(TSymbol symbol)
    {
        return new ValidSymbol<TSymbol>(symbol);
    }

    /// <summary>
    /// Attempts to create a validated projection from a nullable symbol.
    /// Returns null if the symbol is null or an error symbol.
    /// </summary>
    public static ValidSymbol<TSymbol>? TryFrom(TSymbol? symbol)
    {
        return symbol != null && !IsErrorSymbol(symbol) ? new ValidSymbol<TSymbol>(symbol) : null;
    }

    private static bool IsErrorSymbol(TSymbol symbol)
    {
        return symbol is ITypeSymbol { TypeKind: TypeKind.Error }
            || symbol is IErrorTypeSymbol;
    }

    #endregion

    #region Core Properties

    /// <summary>
    /// Gets the underlying non-null symbol.
    /// </summary>
    public TSymbol Value => _symbol;

    #endregion

    #region IProjection Implementation

    /// <summary>
    /// Always returns true for validated symbols.
    /// </summary>
    public bool HasValue => true;

    /// <summary>
    /// Always returns false for validated symbols.
    /// </summary>
    public bool IsEmpty => false;

    /// <summary>
    /// Returns the guaranteed non-null symbol.
    /// </summary>
    public TSymbol OrThrow(string? message = null)
    {
        return _symbol;
    }

    /// <summary>
    /// Returns the guaranteed non-null symbol.
    /// </summary>
    public TSymbol OrNull()
    {
        return _symbol;
    }

    #endregion

    #region Projection Operations (Map, Filter, Cast)

    /// <summary>
    /// Maps the validated symbol to a different type.
    /// </summary>
    public TResult Map<TResult>(Func<TSymbol, TResult> mapper)
    {
        return mapper(_symbol);
    }

    /// <summary>
    /// Maps the validated symbol to another validated symbol.
    /// </summary>
    public ValidSymbol<TDerived> MapTo<TDerived>(Func<TSymbol, TDerived> mapper)
        where TDerived : class, ISymbol
    {
        return new ValidSymbol<TDerived>(mapper(_symbol));
    }

    /// <summary>
    /// Filters the symbol based on a predicate.
    /// Returns null if predicate fails.
    /// </summary>
    public ValidSymbol<TSymbol>? Where(Func<TSymbol, bool> predicate)
    {
        return predicate(_symbol) ? this : null;
    }

    /// <summary>
    /// Attempts to cast to a derived symbol type.
    /// Returns null if cast fails.
    /// </summary>
    public ValidSymbol<TDerived>? OfType<TDerived>() where TDerived : class, ISymbol
    {
        return _symbol is TDerived derived ? new ValidSymbol<TDerived>(derived) : null;
    }

    /// <summary>
    /// Executes an action with the symbol.
    /// </summary>
    public ValidSymbol<TSymbol> Do(Action<TSymbol> action)
    {
        action(_symbol);
        return this;
    }

    #endregion

    #region Equality

    /// <summary>
    /// Checks if the validated symbol equals the specified symbol using Roslyn's symbol equality.
    /// </summary>
    public bool Equals(TSymbol? other)
    {
        return other != null && SymbolEqualityComparer.Default.Equals(_symbol, other);
    }

    /// <summary>
    /// Checks if the validated symbol does not equal the specified symbol.
    /// </summary>
    public bool DoesNotEqual(TSymbol? other)
    {
        return !Equals(other);
    }

    /// <summary>
    /// Enables equality checks: validated == symbol
    /// </summary>
    public static bool operator ==(ValidSymbol<TSymbol> left, TSymbol? right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Enables inequality checks: validated != symbol
    /// </summary>
    public static bool operator !=(ValidSymbol<TSymbol> left, TSymbol? right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Determines whether the current instance equals the specified object (always returns false).
    /// </summary>
    public override bool Equals(object? obj)
    {
        return false;
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    public override int GetHashCode()
    {
        return _symbol.GetHashCode();
    }

    #endregion

    #region Common Symbol Properties

    /// <summary>
    /// Gets the name of the symbol.
    /// </summary>
    public string Name => _symbol.Name;

    /// <summary>
    /// Gets the containing namespace, or null if in global namespace.
    /// </summary>
    public string? Namespace
    {
        get
        {
            var ns = _symbol.ContainingNamespace;
            return ns?.IsGlobalNamespace == true ? null : ns?.ToDisplayString();
        }
    }

    /// <summary>
    /// Gets the fully qualified name without global namespace prefix.
    /// </summary>
    public string FullyQualifiedName => _symbol.ToDisplayString(
        SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(
            SymbolDisplayGlobalNamespaceStyle.Omitted));

    /// <summary>
    /// Gets the fully qualified name with global namespace prefix.
    /// </summary>
    public string GloballyQualifiedName => _symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    /// <summary>
    /// Gets the display name (namespace.name) of the symbol.
    /// </summary>
    public string DisplayName
    {
        get
        {
            var ns = Namespace;
            var name = Name;
            return string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
        }
    }

    /// <summary>
    /// Gets a suggested property name derived from the symbol name.
    /// </summary>
    public string PropertyName
    {
        get
        {
            var typeName = Name;
            var substring = typeName.StartsWith("I") && typeName.Length > 1 && char.IsUpper(typeName[1])
                ? typeName.Substring(1)
                : typeName;
            return substring.ToPascalCase();
        }
    }

    /// <summary>
    /// Gets a suggested parameter name (camelCase) derived from the property name.
    /// </summary>
    public string ParameterName => PropertyName.ToCamelCase();

    /// <summary>
    /// Gets the primary location of the symbol.
    /// </summary>
    public Location Location => _symbol.Locations.FirstOrDefault() ?? Location.None;

    #endregion

    #region Accessibility Properties

    /// <summary>
    /// Gets the declared accessibility of the symbol.
    /// </summary>
    public Accessibility Accessibility => _symbol.DeclaredAccessibility;

    /// <summary>
    /// Gets a value indicating whether the symbol is public.
    /// </summary>
    public bool IsPublic => _symbol.DeclaredAccessibility == Access.Public;

    /// <summary>
    /// Gets a value indicating whether the symbol is internal.
    /// </summary>
    public bool IsInternal => _symbol.DeclaredAccessibility == Access.Internal;

    /// <summary>
    /// Gets a value indicating whether the symbol is private.
    /// </summary>
    public bool IsPrivate => _symbol.DeclaredAccessibility == Access.Private;

    /// <summary>
    /// Gets a value indicating whether the symbol is protected.
    /// </summary>
    public bool IsProtected => _symbol.DeclaredAccessibility == Access.Protected;

    /// <summary>
    /// Gets the accessibility as a string keyword.
    /// </summary>
    public string AccessibilityString =>
        _symbol.DeclaredAccessibility switch
        {
            Access.Public => "public",
            Access.Internal => "internal",
            Access.Private => "private",
            Access.Protected => "protected",
            Access.ProtectedAndInternal => "private protected",
            Access.ProtectedOrInternal => "protected internal",
            _ => throw new InvalidOperationException("Unknown accessibility.")
        };

    #endregion

    #region Modifier Properties

    /// <summary>
    /// Gets a value indicating whether the symbol is static.
    /// </summary>
    public bool IsStatic => _symbol.IsStatic;

    /// <summary>
    /// Gets a value indicating whether the symbol is abstract.
    /// </summary>
    public bool IsAbstract => _symbol.IsAbstract;

    /// <summary>
    /// Gets a value indicating whether the symbol is sealed.
    /// </summary>
    public bool IsSealed => _symbol.IsSealed;

    /// <summary>
    /// Gets a value indicating whether the symbol is virtual.
    /// </summary>
    public bool IsVirtual => _symbol.IsVirtual;

    /// <summary>
    /// Gets a value indicating whether the symbol is an override.
    /// </summary>
    public bool IsOverride => _symbol.IsOverride;

    /// <summary>
    /// Gets a value indicating whether the symbol is implicitly declared.
    /// </summary>
    public bool IsImplicitlyDeclared => _symbol.IsImplicitlyDeclared;

    /// <summary>
    /// Gets a value indicating whether the symbol is extern.
    /// </summary>
    public bool IsExtern => _symbol.IsExtern;

    /// <summary>
    /// Gets a value indicating whether the symbol is readonly.
    /// </summary>
    public bool IsReadOnly => _symbol switch
    {
        IFieldSymbol fieldSymbol => fieldSymbol.IsReadOnly,
        IPropertySymbol propertySymbol => propertySymbol.IsReadOnly,
        INamedTypeSymbol namedType => namedType.IsReadOnly,
        _ => false
    };

    /// <summary>
    /// Gets a value indicating whether the symbol is a partial type.
    /// </summary>
    public bool IsPartial =>
        _symbol is INamedTypeSymbol namedType &&
        namedType.DeclaringSyntaxReferences
            .Select(r => r.GetSyntax())
            .OfType<TypeDeclarationSyntax>()
            .Any(t => t.Modifiers.Any(SyntaxKind.PartialKeyword));

    #endregion

    #region Type Classification Properties

    /// <summary>
    /// Gets a value indicating whether the symbol is a generic type.
    /// </summary>
    public bool IsGenericType => _symbol is INamedTypeSymbol { IsGenericType: true };

    /// <summary>
    /// Gets a value indicating whether the symbol is a value type.
    /// </summary>
    public bool IsValueType => _symbol is ITypeSymbol { IsValueType: true };

    /// <summary>
    /// Gets a value indicating whether the symbol is a reference type.
    /// </summary>
    public bool IsReferenceType => _symbol is ITypeSymbol { IsReferenceType: true };

    /// <summary>
    /// Gets a value indicating whether the symbol is an interface.
    /// </summary>
    public bool IsInterface => _symbol is INamedTypeSymbol { TypeKind: TypeKind.Interface };

    /// <summary>
    /// Gets a value indicating whether the symbol is a class.
    /// </summary>
    public bool IsClass => _symbol is INamedTypeSymbol { TypeKind: TypeKind.Class };

    /// <summary>
    /// Gets a value indicating whether the symbol is a struct.
    /// </summary>
    public bool IsStruct => _symbol is INamedTypeSymbol { TypeKind: TypeKind.Struct };

    /// <summary>
    /// Gets a value indicating whether the symbol is a record.
    /// </summary>
    public bool IsRecord => _symbol is INamedTypeSymbol { IsRecord: true };

    /// <summary>
    /// Gets a value indicating whether the symbol is an enum.
    /// </summary>
    public bool IsEnum => _symbol is INamedTypeSymbol namedType && namedType.TypeKind == TypeKind.Enum;

    /// <summary>
    /// Gets a value indicating whether the symbol is a delegate.
    /// </summary>
    public bool IsDelegate => _symbol is INamedTypeSymbol namedType && namedType.TypeKind == TypeKind.Delegate;

    /// <summary>
    /// Gets a value indicating whether the symbol has nullable annotation.
    /// </summary>
    public bool IsNullable => _symbol is ITypeSymbol { NullableAnnotation: NullableAnnotation.Annotated };

    /// <summary>
    /// Gets the type kind of the symbol.
    /// </summary>
    public TypeKind? SymbolTypeKind => _symbol is ITypeSymbol typeSymbol ? typeSymbol.TypeKind : null;

    /// <summary>
    /// Gets the special type classification of the symbol.
    /// </summary>
    public SpecialType SpecialType => _symbol is ITypeSymbol typeSymbol ? typeSymbol.SpecialType : SpecialType.None;

    /// <summary>
    /// Gets the kind of the symbol as a string keyword (class, struct, interface, etc.).
    /// </summary>
    public string? Kind =>
        _symbol is not INamedTypeSymbol namedType
            ? null
            : namedType.TypeKind switch
            {
                TypeKind.Class => namedType.IsRecord ? "record" : "class",
                TypeKind.Struct => namedType.IsRecord ? "record struct" : "struct",
                TypeKind.Interface => "interface",
                TypeKind.Enum => "enum",
                TypeKind.Delegate => "delegate",
                _ => namedType.TypeKind.ToString().ToLowerInvariant()
            };

    #endregion

    #region Method-Specific Properties

    /// <summary>
    /// Gets a value indicating whether the symbol is an async method.
    /// </summary>
    public bool IsAsync => _symbol is IMethodSymbol { IsAsync: true };

    /// <summary>
    /// Gets a value indicating whether the symbol is an extension method.
    /// </summary>
    public bool IsExtensionMethod => _symbol is IMethodSymbol { IsExtensionMethod: true };

    #endregion

    #region Type Hierarchy Properties

    /// <summary>
    /// Gets the containing type of the symbol.
    /// </summary>
    public OptionalSymbol<INamedTypeSymbol> ContainingType =>
        OptionalSymbol<INamedTypeSymbol>.FromNullable(_symbol.ContainingType);

    /// <summary>
    /// Gets the base type of the symbol.
    /// </summary>
    public OptionalSymbol<INamedTypeSymbol> BaseType =>
        _symbol is INamedTypeSymbol { BaseType: not null } namedType
            ? OptionalSymbol<INamedTypeSymbol>.WithValue(namedType.BaseType)
            : OptionalSymbol<INamedTypeSymbol>.Empty();

    /// <summary>
    /// Gets the interfaces implemented by the symbol.
    /// </summary>
    public ImmutableArray<ValidSymbol<INamedTypeSymbol>> Interfaces =>
        _symbol is INamedTypeSymbol namedType
            ? [..namedType.Interfaces.Select(i => new ValidSymbol<INamedTypeSymbol>(i))]
            : ImmutableArray<ValidSymbol<INamedTypeSymbol>>.Empty;

    #endregion

    #region Generic Type Support

    /// <summary>
    /// Gets the arity (number of type parameters) of the symbol.
    /// </summary>
    public int Arity => _symbol is INamedTypeSymbol namedType ? namedType.Arity : 0;

    /// <summary>
    /// Gets the type arguments of the generic type.
    /// </summary>
    public ImmutableArray<ValidSymbol<INamedTypeSymbol>> GetTypeArguments()
    {
        return _symbol is INamedTypeSymbol { IsGenericType: true } namedType
            ? [..namedType.TypeArguments.Select(t => t.AsValidNamedType())]
            : ImmutableArray<ValidSymbol<INamedTypeSymbol>>.Empty;
    }

    /// <summary>
    /// Gets a specific type argument by index.
    /// </summary>
    public OptionalSymbol<INamedTypeSymbol> GetTypeArgument(int index)
    {
        if (_symbol is not INamedTypeSymbol { IsGenericType: true } namedType)
            return OptionalSymbol<INamedTypeSymbol>.Empty();

        if (index < 0 || index >= namedType.TypeArguments.Length)
            return OptionalSymbol<INamedTypeSymbol>.Empty();

        return namedType.TypeArguments[index].AsNamedType();
    }

    /// <summary>
    /// Gets a specific type argument by index as a type symbol.
    /// </summary>
    public OptionalSymbol<INamedTypeSymbol> GetTypeArgumentSymbol(int index)
    {
        if (_symbol is not INamedTypeSymbol { IsGenericType: true } namedType)
            return OptionalSymbol<INamedTypeSymbol>.Empty();

        if (index < 0 || index >= namedType.TypeArguments.Length)
            return OptionalSymbol<INamedTypeSymbol>.Empty();

        return namedType.TypeArguments[index].AsNamedType();
    }

    /// <summary>
    /// Gets the first type argument of the generic type.
    /// </summary>
    public OptionalSymbol<INamedTypeSymbol> GetFirstTypeArgument()
    {
        if (_symbol is not INamedTypeSymbol { IsGenericType: true } namedType)
            return OptionalSymbol<INamedTypeSymbol>.Empty();

        if (namedType.TypeArguments.Length == 0)
            return OptionalSymbol<INamedTypeSymbol>.Empty();

        return namedType.TypeArguments[0] is INamedTypeSymbol firstArg
            ? OptionalSymbol<INamedTypeSymbol>.WithValue(firstArg)
            : OptionalSymbol<INamedTypeSymbol>.Empty();
    }

    /// <summary>
    /// Attempts to get the first type argument of the generic type.
    /// </summary>
    public bool TryGetFirstTypeArgument(out ValidSymbol<INamedTypeSymbol> result)
    {
        return GetFirstTypeArgument().IsValid(out result);
    }

    /// <summary>
    /// Gets the single type argument if the type has exactly one type parameter.
    /// </summary>
    public OptionalSymbol<ITypeSymbol> SingleTypeArgument =>
        _symbol is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } namedType
            ? OptionalSymbol<ITypeSymbol>.WithValue(namedType.TypeArguments[0])
            : OptionalSymbol<ITypeSymbol>.Empty();

    /// <summary>
    /// Gets the type parameters of the generic type.
    /// </summary>
    public IEnumerable<ValidSymbol<ITypeParameterSymbol>> GetTypeParameters()
    {
        if (_symbol is not INamedTypeSymbol { IsGenericType: true } namedType)
            yield break;

        foreach (var typeParam in namedType.TypeParameters)
            yield return new ValidSymbol<ITypeParameterSymbol>(typeParam);
    }

    /// <summary>
    /// Gets the type parameters of a generic method.
    /// </summary>
    public IEnumerable<ValidSymbol<ITypeParameterSymbol>> GetMethodTypeParameters()
    {
        if (_symbol is not IMethodSymbol { IsGenericMethod: true } method)
            yield break;

        foreach (var typeParam in method.TypeParameters)
            yield return new ValidSymbol<ITypeParameterSymbol>(typeParam);
    }

    #endregion

    #region Task-Specific Support

    /// <summary>
    /// Gets a value indicating whether the symbol is a Task type.
    /// </summary>
    public bool IsTask => _symbol is INamedTypeSymbol namedType && namedType.IsTaskType();

    /// <summary>
    /// Gets a value indicating whether the symbol is a ValueTask type.
    /// </summary>
    public bool IsValueTask => _symbol is INamedTypeSymbol namedType && namedType.IsValueTaskType();

    /// <summary>
    /// Gets a value indicating whether the symbol is a generic Task&lt;T&gt; type.
    /// </summary>
    public bool IsGenericTask => _symbol is INamedTypeSymbol namedType && namedType.IsGenericTaskType();

    /// <summary>
    /// Gets a value indicating whether the symbol is a generic ValueTask&lt;T&gt; type.
    /// </summary>  
    public bool IsGenericValueTask => _symbol is INamedTypeSymbol namedType && namedType.IsGenericValueTaskType();

    /// <summary>
    /// Gets a value indicating whether the symbol is a non-generic Task type.
    /// </summary>
    public bool IsNonGenericTask => IsTask && !IsGenericTask;

    /// <summary>
    /// Gets a value indicating whether the symbol is a non-generic ValueTask type.
    /// </summary>
    public bool IsNonGenericValueTask => IsValueTask && !IsGenericValueTask;

    /// <summary>
    /// Gets the inner type of a Task&lt;T&gt; or ValueTask&lt;T&gt;.
    /// </summary>
    public OptionalSymbol<ITypeSymbol> InnerTaskType
    {
        get
        {
            if (_symbol is not INamedTypeSymbol namedType || !namedType.IsGenericTaskType() && !namedType.IsGenericValueTaskType())
                return OptionalSymbol<ITypeSymbol>.Empty();

            return SingleTypeArgument;
        }
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Gets all attributes applied to the symbol.
    /// </summary>
    public IEnumerable<ValidAttribute> GetAttributes()
    {
        return _symbol.GetAttributes().Select(ValidAttribute.From);
    }

    /// <summary>
    /// Gets attributes with the specified name.
    /// </summary>
    public IEnumerable<ValidAttribute> GetAttributes(string attributeName)
    {
        return _symbol.GetAttributesByName(attributeName);
    }

    /// <summary>
    /// Gets attributes of the specified type.
    /// </summary>
    public IEnumerable<ValidAttribute> GetAttributes<TAttribute>() where TAttribute : Attribute
    {
        return _symbol.GetAttributesByType<TAttribute>();
    }

    /// <summary>
    /// Gets the first attribute with the specified name.
    /// </summary>
    public OptionalAttribute GetAttribute(string attributeName)
    {
        return _symbol.GetAttributesByName(attributeName).FirstOrDefault().Map(OptionalAttribute.FromNullable);
    }

    /// <summary>
    /// Gets the first attribute of the specified type.
    /// </summary>
    public OptionalAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
    {
        return _symbol.GetAttributesByType<TAttribute>().FirstOrDefault().Map(OptionalAttribute.FromNullable);
    }

    /// <summary>
    /// Checks if the symbol has any attributes.
    /// </summary>
    public bool HasAttributes()
    {
        return _symbol.GetAttributes().Length > 0;
    }

    /// <summary>
    /// Checks if the symbol has an attribute with the specified name.
    /// </summary>
    public bool HasAttribute(string attributeName)
    {
        return _symbol.GetAttributesByName(attributeName).Any();
    }

    /// <summary>
    /// Checks if the symbol has an attribute of the specified type.
    /// </summary>
    public bool HasAttribute<TAttribute>() where TAttribute : Attribute
    {
        return _symbol.GetAttributesByType<TAttribute>().Any();
    }

    /// <summary>
    /// Checks if the symbol has no attributes.
    /// </summary>
    public bool LacksAttributes()
    {
        return _symbol.GetAttributes().Length == 0;
    }

    /// <summary>
    /// Checks if the symbol does not have an attribute with the specified name.
    /// </summary>
    public bool LacksAttribute(string attributeName)
    {
        return !_symbol.GetAttributesByName(attributeName).Any();
    }

    /// <summary>
    /// Checks if the symbol does not have an attribute of the specified type.
    /// </summary>
    public bool LacksAttribute<TAttribute>() where TAttribute : Attribute
    {
        return !_symbol.GetAttributesByType<TAttribute>().Any();
    }

    #endregion

    #region XML Documentation

    /// <summary>
    /// Gets the raw XML documentation comment for the symbol, or null if none exists.
    /// </summary>
    public string? XmlDocumentationRaw => _symbol.GetDocumentationCommentXml();

    /// <summary>
    /// Gets the parsed XML documentation for the symbol.
    /// </summary>
    public XmlDocumentation XmlDocumentation => XmlDocumentation.FromSymbol(_symbol);

    /// <summary>
    /// Gets a value indicating whether the symbol has XML documentation.
    /// </summary>
    public bool HasXmlDocumentation => !string.IsNullOrWhiteSpace(XmlDocumentationRaw);

    #endregion
}