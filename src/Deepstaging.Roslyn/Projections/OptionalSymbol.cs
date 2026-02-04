// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Access = Microsoft.CodeAnalysis.Accessibility;

namespace Deepstaging.Roslyn;

/// <summary>
/// An optional Roslyn symbol that may or may not contain a value.
/// Provides a fluent API for querying and transforming symbols with null-safety.
/// Use Validate() or IsNotValid() to get a ValidSymbol&lt;T&gt; with guaranteed non-null access.
/// </summary>
/// <typeparam name="TSymbol">The type of Roslyn symbol.</typeparam>
public readonly struct OptionalSymbol<TSymbol> : IValidatableProjection<TSymbol, ValidSymbol<TSymbol>>
    where TSymbol : class, ISymbol
{
    private readonly TSymbol? _symbol;

    private OptionalSymbol(TSymbol? symbol)
    {
        _symbol = symbol;
    }

    #region Factory Methods

    /// <summary>
    /// Creates an optional symbol with a value.
    /// </summary>
    public static OptionalSymbol<TSymbol> WithValue(TSymbol symbol) => new(symbol);
    
    /// <summary>
    /// Creates an empty optional symbol without a value.
    /// </summary>
    public static OptionalSymbol<TSymbol> Empty() => new(null);
    
    /// <summary>
    /// Creates an optional symbol from a nullable symbol reference.
    /// </summary>
    public static OptionalSymbol<TSymbol> FromNullable(TSymbol? symbol) => symbol != null ? WithValue(symbol) : Empty();

    #endregion

    #region Core Properties

    /// <summary>
    /// Gets the underlying symbol, which may be null.
    /// </summary>
    public TSymbol? Symbol => _symbol;

    #endregion

    #region IValidatableProjection Implementation

    /// <summary>
    /// Gets a value indicating whether the symbol is present.
    /// </summary>
    public bool HasValue => _symbol != null;
    
    /// <summary>
    /// Gets a value indicating whether the symbol is absent.
    /// </summary>
    public bool IsEmpty => _symbol == null;

    /// <summary>
    /// Returns the symbol or throws an exception if absent.
    /// </summary>
    public TSymbol OrThrow(string? message = null)
    {
        return _symbol ?? throw new InvalidOperationException(message ?? "Symbol is not present.");
    }

    /// <summary>
    /// Returns the symbol or throws an exception with a lazily-computed message if absent.
    /// </summary>
    /// <param name="messageFactory">Factory function to create the error message.</param>
    /// <exception cref="InvalidOperationException">Thrown when symbol is not present.</exception>
    public TSymbol OrThrow(Func<string> messageFactory)
    {
        return _symbol ?? throw new InvalidOperationException(messageFactory());
    }

    /// <summary>
    /// Returns the symbol or null if absent.
    /// </summary>
    public TSymbol? OrNull() => _symbol;

    /// <summary>
    /// Validates the optional symbol to a ValidSymbol with guaranteed non-null, non-error access.
    /// </summary>
    public ValidSymbol<TSymbol>? Validate()
    {
        return _symbol != null && !IsErrorSymbol(_symbol)
            ? ValidSymbol<TSymbol>.From(_symbol)
            : null;
    }

    /// <summary>
    /// Validates the optional symbol or throws an exception if absent or an error symbol.
    /// </summary>
    public ValidSymbol<TSymbol> ValidateOrThrow(string? message = null)
    {
        return _symbol != null && !IsErrorSymbol(_symbol)
            ? ValidSymbol<TSymbol>.From(_symbol)
            : throw new InvalidOperationException(message ?? "Cannot validate an empty or error projection.");
    }

    /// <summary>
    /// Attempts to validate the optional symbol. Returns true if successful (non-null and non-error).
    /// </summary>
    public bool TryValidate(out ValidSymbol<TSymbol> validated)
    {
        if (_symbol != null && !IsErrorSymbol(_symbol))
        {
            validated = ValidSymbol<TSymbol>.From(_symbol);
            return true;
        }

        validated = default;
        return false;
    }

    private static bool IsErrorSymbol(TSymbol symbol)
    {
        return symbol is ITypeSymbol { TypeKind: TypeKind.Error } or IErrorTypeSymbol;
    }

    /// <summary>
    /// Checks if the optional symbol is not valid (empty). Returns true if invalid.
    /// </summary>
    public bool IsNotValid(out ValidSymbol<TSymbol> validated) => !IsValid(out validated);
    
    /// <summary>
    /// Checks if the optional symbol is valid (has value). Returns true if valid.
    /// </summary>
    public bool IsValid(out ValidSymbol<TSymbol> validated) => TryValidate(out validated);

    #endregion

    #region Projection Operations (Map, Filter, Cast)

    /// <summary>
    /// Maps the symbol to a different type using the provided function.
    /// </summary>
    public OptionalValue<TResult> Map<TResult>(Func<ValidSymbol<TSymbol>, TResult> mapper)
    {
        return _symbol != null
            ? OptionalValue<TResult>.WithValue(mapper(ValidSymbol<TSymbol>.From(_symbol)))
            : OptionalValue<TResult>.Empty();
    }

    /// <summary>
    /// Alias for Map. Maps the symbol to a different type.
    /// </summary>
    public OptionalValue<TResult> Select<TResult>(Func<ValidSymbol<TSymbol>, TResult> selector) => Map(selector);

    /// <summary>
    /// Filters the symbol based on a predicate.
    /// </summary>
    public OptionalSymbol<TSymbol> Where(Func<TSymbol, bool> predicate)
    {
        return _symbol != null && predicate(_symbol) ? this : Empty();
    }

    /// <summary>
    /// Attempts to cast the symbol to a derived symbol type.
    /// </summary>
    public OptionalSymbol<TDerived> OfType<TDerived>() where TDerived : class, ISymbol
    {
        return _symbol is TDerived derived
            ? OptionalSymbol<TDerived>.WithValue(derived)
            : OptionalSymbol<TDerived>.Empty();
    }

    #endregion

    #region Equality

    /// <summary>
    /// Checks if the symbol equals another symbol using Roslyn's symbol equality.
    /// </summary>
    public bool Equals(TSymbol? other)
    {
        return _symbol != null && other != null && SymbolEqualityComparer.Default.Equals(_symbol, other);
    }

    /// <summary>
    /// Checks if the symbol does not equal another symbol.
    /// </summary>
    public bool DoesNotEqual(TSymbol? other) => !Equals(other);

    /// <summary>
    /// Enables equality checks: optional == symbol
    /// </summary>
    public static bool operator ==(OptionalSymbol<TSymbol> left, TSymbol? right) => left.Equals(right);
    
    /// <summary>
    /// Enables inequality checks: optional != symbol
    /// </summary>
    public static bool operator !=(OptionalSymbol<TSymbol> left, TSymbol? right) => !left.Equals(right);

    /// <summary>
    /// Determines whether the current instance equals the specified object (always returns false).
    /// </summary>
    public override bool Equals(object? obj) => false;
    
    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    public override int GetHashCode() => _symbol?.GetHashCode() ?? 0;

    #endregion

    #region Common Symbol Properties

    /// <summary>
    /// Gets the name of the symbol, or null if absent.
    /// </summary>
    public string? Name => _symbol?.Name;
    
    /// <summary>
    /// Gets the containing namespace of the symbol, or null if absent or in global namespace.
    /// </summary>
    public string? Namespace
    {
        get
        {
            if (_symbol?.ContainingNamespace is null)
                return null;
            var ns = _symbol.ContainingNamespace;
            return ns.IsGlobalNamespace ? null : ns.ToDisplayString();
        }
    }

    /// <summary>
    /// Gets the fully qualified name of the symbol without global namespace prefix.
    /// </summary>
    public string? FullyQualifiedName => _symbol?.ToDisplayString(
        SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(
            SymbolDisplayGlobalNamespaceStyle.Omitted));

    /// <summary>
    /// Gets the fully qualified name of the symbol with global namespace prefix.
    /// </summary>
    public string? GloballyQualifiedName => _symbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    /// <summary>
    /// Gets the display name (namespace.name) of the symbol.
    /// </summary>
    public string? DisplayName
    {
        get
        {
            if (_symbol == null) return null;
            var ns = Namespace;
            var name = Name;
            return string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
        }
    }

    /// <summary>
    /// Gets a suggested property name derived from the symbol name.
    /// </summary>
    public string? PropertyName
    {
        get
        {
            var typeName = Name;
            if (typeName == null) return null;
            return (typeName.StartsWith("I") && typeName.Length > 1 && char.IsUpper(typeName[1])
                ? typeName.Substring(1)
                : typeName).ToPascalCase();
        }
    }

    /// <summary>
    /// Gets a suggested parameter name derived from the property name.
    /// </summary>
    public string? ParameterName => PropertyName?.ToCamelCase();
    
    /// <summary>
    /// Gets the primary location of the symbol.
    /// </summary>
    public Location Location => _symbol?.Locations.FirstOrDefault() ?? Location.None;

    #endregion

    #region Accessibility Properties

    /// <summary>
    /// Gets the declared accessibility of the symbol.
    /// </summary>
    public Accessibility? Accessibility => _symbol?.DeclaredAccessibility;
    
    /// <summary>
    /// Gets a value indicating whether the symbol is public.
    /// </summary>
    public bool IsPublic => _symbol?.DeclaredAccessibility == Access.Public;
    
    /// <summary>
    /// Gets a value indicating whether the symbol is internal.
    /// </summary>
    public bool IsInternal => _symbol?.DeclaredAccessibility == Access.Internal;
    
    /// <summary>
    /// Gets a value indicating whether the symbol is private.
    /// </summary>
    public bool IsPrivate => _symbol?.DeclaredAccessibility == Access.Private;
    
    /// <summary>
    /// Gets a value indicating whether the symbol is protected.
    /// </summary>
    public bool IsProtected => _symbol?.DeclaredAccessibility == Access.Protected;

    /// <summary>
    /// Gets the accessibility as a string keyword.
    /// </summary>
    public string? AccessibilityString =>
        _symbol == null ? null : _symbol.DeclaredAccessibility switch
        {
            Access.Public => "public",
            Access.Internal => "internal",
            Access.Private => "private",
            Access.Protected => "protected",
            Access.ProtectedAndInternal => "private protected",
            Access.ProtectedOrInternal => "protected internal",
            _ => null
        };

    #endregion

    #region Modifier Properties

    /// <summary>
    /// Gets a value indicating whether the symbol is static.
    /// </summary>
    public bool IsStatic => _symbol?.IsStatic ?? false;
    
    /// <summary>
    /// Gets a value indicating whether the symbol is abstract.
    /// </summary>
    public bool IsAbstract => _symbol?.IsAbstract ?? false;
    
    /// <summary>
    /// Gets a value indicating whether the symbol is sealed.
    /// </summary>
    public bool IsSealed => _symbol?.IsSealed ?? false;
    
    /// <summary>
    /// Gets a value indicating whether the symbol is virtual.
    /// </summary>
    public bool IsVirtual => _symbol?.IsVirtual ?? false;
    
    /// <summary>
    /// Gets a value indicating whether the symbol is an override.
    /// </summary>
    public bool IsOverride => _symbol?.IsOverride ?? false;
    
    /// <summary>
    /// Gets a value indicating whether the symbol is implicitly declared.
    /// </summary>
    public bool IsImplicitlyDeclared => _symbol?.IsImplicitlyDeclared ?? false;
    
    /// <summary>
    /// Gets a value indicating whether the symbol is extern.
    /// </summary>
    public bool IsExtern => _symbol?.IsExtern ?? false;

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
    public bool IsPartial
    {
        get
        {
            if (_symbol is not INamedTypeSymbol namedType)
                return false;
            return namedType.DeclaringSyntaxReferences
                .Select(r => r.GetSyntax())
                .OfType<TypeDeclarationSyntax>()
                .Any(t => t.Modifiers.Any(SyntaxKind.PartialKeyword));
        }
    }

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
    public bool IsEnum => _symbol is INamedTypeSymbol { TypeKind: TypeKind.Enum };
    
    /// <summary>
    /// Gets a value indicating whether the symbol is a delegate.
    /// </summary>
    public bool IsDelegate => _symbol is INamedTypeSymbol { TypeKind: TypeKind.Delegate };
    
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
        _symbol is not INamedTypeSymbol namedType ? null : namedType.TypeKind switch
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
        _symbol?.ContainingType != null
            ? OptionalSymbol<INamedTypeSymbol>.WithValue(_symbol.ContainingType)
            : OptionalSymbol<INamedTypeSymbol>.Empty();

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
    public ImmutableArray<OptionalSymbol<INamedTypeSymbol>> Interfaces =>
        _symbol is INamedTypeSymbol namedType
            ? [..namedType.Interfaces.Select(OptionalSymbol<INamedTypeSymbol>.WithValue)]
            : ImmutableArray<OptionalSymbol<INamedTypeSymbol>>.Empty;

    #endregion

    #region Generic Type Support

    /// <summary>
    /// Gets the arity (number of type parameters) of the symbol.
    /// </summary>
    public int Arity => _symbol is INamedTypeSymbol namedType ? namedType.Arity : 0;

    /// <summary>
    /// Gets the type arguments of the generic type.
    /// </summary>
    public ImmutableArray<OptionalSymbol<INamedTypeSymbol>> GetTypeArguments()
    {
        return _symbol is INamedTypeSymbol { IsGenericType: true } namedType
            ? [..namedType.TypeArguments.Select(t => t.AsNamedType())]
            : ImmutableArray<OptionalSymbol<INamedTypeSymbol>>.Empty;
    }

    /// <summary>
    /// Gets a specific type argument by index.
    /// </summary>
    public OptionalValue<INamedTypeSymbol> GetTypeArgument(int index)
    {
        if (_symbol is not INamedTypeSymbol { IsGenericType: true } namedType)
            return OptionalValue<INamedTypeSymbol>.Empty();

        if (index < 0 || index >= namedType.TypeArguments.Length)
            return OptionalValue<INamedTypeSymbol>.Empty();

        return namedType.TypeArguments[index] is INamedTypeSymbol named
            ? OptionalValue<INamedTypeSymbol>.WithValue(named)
            : OptionalValue<INamedTypeSymbol>.Empty();
    }

    /// <summary>
    /// Gets a specific type argument by index as a type symbol.
    /// </summary>
    public OptionalSymbol<ITypeSymbol> GetTypeArgumentSymbol(int index)
    {
        if (_symbol is not INamedTypeSymbol { IsGenericType: true } namedType)
            return OptionalSymbol<ITypeSymbol>.Empty();

        if (index < 0 || index >= namedType.TypeArguments.Length)
            return OptionalSymbol<ITypeSymbol>.Empty();

        return OptionalSymbol<ITypeSymbol>.WithValue(namedType.TypeArguments[index]);
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
    /// Gets the single type argument if the type has exactly one type parameter.
    /// </summary>
    public OptionalSymbol<ITypeSymbol> SingleTypeArgument =>
        _symbol.AsNamedType()
            .Where(x => x.IsGenericType && x.TypeArguments.Length == 1)
            .Map(x => OptionalSymbol<ITypeSymbol>.WithValue(x.Value.TypeArguments[0]))
            .OrDefault(OptionalSymbol<ITypeSymbol>.Empty());

    /// <summary>
    /// Gets the type parameters of the generic type.
    /// </summary>
    public IEnumerable<OptionalSymbol<ITypeParameterSymbol>> GetTypeParameters()
    {
        if (_symbol is not INamedTypeSymbol { IsGenericType: true } namedType)
            yield break;

        foreach (var typeParam in namedType.TypeParameters)
            yield return OptionalSymbol<ITypeParameterSymbol>.WithValue(typeParam);
    }

    /// <summary>
    /// Gets the type parameters of a generic method.
    /// </summary>
    public IEnumerable<OptionalSymbol<ITypeParameterSymbol>> GetMethodTypeParameters()
    {
        if (_symbol is not IMethodSymbol { IsGenericMethod: true } method)
            yield break;

        foreach (var typeParam in method.TypeParameters)
            yield return OptionalSymbol<ITypeParameterSymbol>.WithValue(typeParam);
    }

    #endregion

    #region Task-Specific Support

    /// <summary>
    /// Gets a value indicating whether the symbol is a Task type.
    /// </summary>
    public bool IsTask => _symbol is INamedTypeSymbol namedType && namedType.IsTaskType();

    /// <summary>
    /// Gets the inner type of a Task&lt;T&gt; or ValueTask&lt;T&gt;.
    /// </summary>
    public OptionalSymbol<ITypeSymbol> InnerTaskType =>
        _symbol.AsNamedType()
            .Where(x => x.IsGenericTaskType() || x.IsGenericValueTaskType())
            .Map(x => x.Value.AsType().SingleTypeArgument)
            .OrDefault(OptionalSymbol<ITypeSymbol>.Empty());

    #endregion

    #region Attributes

    /// <summary>
    /// Gets all attributes applied to the symbol.
    /// </summary>
    public IEnumerable<OptionalAttribute> GetAttributes()
    {
        if (_symbol == null)
            yield break;

        foreach (var attr in _symbol.GetAttributes())
            yield return OptionalAttribute.WithValue(attr);
    }

    /// <summary>
    /// Gets attributes with the specified name.
    /// </summary>
    public IEnumerable<ValidAttribute> GetAttributes(string attributeName)
    {
        return _symbol?.GetAttributesByName(attributeName) ?? [];
    }

    /// <summary>
    /// Gets attributes of the specified type.
    /// </summary>
    public IEnumerable<ValidAttribute> GetAttributes<TAttribute>() where TAttribute : Attribute
    {
        return _symbol?.GetAttributesByType<TAttribute>() ?? [];
    }

    /// <summary>
    /// Gets the first attribute with the specified name.
    /// </summary>
    public OptionalAttribute GetAttribute(string attributeName)
    {
        return _symbol == null
            ? OptionalAttribute.Empty()
            : OptionalAttribute.FromNullable(_symbol.GetAttributesByName(attributeName).FirstOrDefault().Value);
    }

    /// <summary>
    /// Gets the first attribute of the specified type.
    /// </summary>
    public OptionalAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
    {
        return _symbol == null
            ? OptionalAttribute.Empty()
            : OptionalAttribute.FromNullable(_symbol.GetAttributesByType<TAttribute>().FirstOrDefault().Value);
    }

    /// <summary>
    /// Checks if the symbol has any attributes.
    /// </summary>
    public bool HasAttributes()
    {
        return _symbol?.GetAttributes().Length > 0;
    }

    /// <summary>
    /// Checks if the symbol has an attribute with the specified name.
    /// </summary>
    public bool HasAttribute(string attributeName)
    {
        return _symbol?.GetAttributesByName(attributeName).Any() == true;
    }

    /// <summary>
    /// Checks if the symbol has an attribute of the specified type.
    /// </summary>
    public bool HasAttribute<TAttribute>() where TAttribute : Attribute
    {
        return _symbol?.GetAttributesByType<TAttribute>().Any() == true;
    }

    /// <summary>
    /// Checks if the symbol has no attributes (or symbol is empty).
    /// </summary>
    public bool LacksAttributes()
    {
        return _symbol == null || _symbol.GetAttributes().Length == 0;
    }

    /// <summary>
    /// Checks if the symbol does not have an attribute with the specified name (or symbol is empty).
    /// </summary>
    public bool LacksAttribute(string attributeName)
    {
        return _symbol == null || !_symbol.GetAttributesByName(attributeName).Any();
    }

    /// <summary>
    /// Checks if the symbol does not have an attribute of the specified type (or symbol is empty).
    /// </summary>
    public bool LacksAttribute<TAttribute>() where TAttribute : Attribute
    {
        return _symbol == null || !_symbol.GetAttributesByType<TAttribute>().Any();
    }

    #endregion

    #region XML Documentation

    /// <summary>
    /// Gets the raw XML documentation comment for the symbol, or null if none exists or symbol is empty.
    /// </summary>
    public string? XmlDocumentationRaw => _symbol?.GetDocumentationCommentXml();

    /// <summary>
    /// Gets the parsed XML documentation for the symbol.
    /// </summary>
    public XmlDocumentation XmlDocumentation => XmlDocumentation.FromSymbol(_symbol);

    /// <summary>
    /// Gets a value indicating whether the symbol has XML documentation.
    /// </summary>
    public bool HasXmlDocumentation => !string.IsNullOrWhiteSpace(XmlDocumentationRaw);

    #endregion

    #region Utility Methods

    /// <summary>
    /// Executes an action if the symbol is present.
    /// </summary>
    public OptionalSymbol<TSymbol> Do(Action<TSymbol> action)
    {
        if (_symbol != null)
            action(_symbol);
        return this;
    }

    /// <summary>
    /// Pattern matching with discriminated union semantics.
    /// </summary>
    public void Match(Action<TSymbol> whenPresent, Action whenEmpty)
    {
        if (_symbol != null)
            whenPresent(_symbol);
        else
            whenEmpty();
    }

    #endregion
}
