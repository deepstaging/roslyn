// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for wrapping Roslyn symbols in projections for fluent querying.
/// </summary>
public static class OptionalSymbolExtensions
{
    /// <param name="symbol">The symbol to wrap.</param>
    extension(ISymbol? symbol)
    {
        /// <summary>
        /// Directly casts to INamedTypeSymbol (class, struct, interface, enum, delegate, record).
        /// Returns Empty if symbol is null or not a named type.
        /// </summary>
        /// <example>
        /// <code>
        /// var namedType = context.TargetSymbol.AsNamedType();
        /// if (namedType.IsClass) { ... }
        /// </code>
        /// </example>
        public OptionalSymbol<INamedTypeSymbol> AsNamedType()
        {
            return symbol is INamedTypeSymbol nts
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(nts)
                : OptionalSymbol<INamedTypeSymbol>.Empty();
        }

        /// <summary>
        /// Casts to INamedTypeSymbol and validates (throws if null or wrong type).
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> AsValidNamedType()
        {
            return symbol.AsNamedType().ValidateOrThrow();
        }

        /// <summary>
        /// Checks if the symbol is a named type and outputs the validated symbol.
        /// </summary>
        public bool IsNamedType(out ValidSymbol<INamedTypeSymbol> validated)
        {
            var projected = symbol.AsNamedType();
            if (projected.HasValue)
            {
                validated = projected.ValidateOrThrow();
                return true;
            }

            validated = default;
            return false;
        }

        /// <summary>
        /// Checks if the symbol is not a named type and outputs the validated symbol if it is.
        /// </summary>
        public bool IsNotNamedType(out ValidSymbol<INamedTypeSymbol> validated)
        {
            validated = default;
            var projected = symbol.AsNamedType();
            if (!projected.HasValue) return true;
            validated = projected.ValidateOrThrow();
            return false;
        }

        /// <summary>
        /// Casts the symbol to INamespaceSymbol.
        /// </summary>
        public OptionalSymbol<INamespaceSymbol> AsNamespace()
        {
            return symbol is INamespaceSymbol ns
                ? OptionalSymbol<INamespaceSymbol>.WithValue(ns)
                : OptionalSymbol<INamespaceSymbol>.Empty();
        }

        /// <summary>
        /// Casts to INamespaceSymbol and validates (throws if null or wrong type).
        /// </summary>
        public ValidSymbol<INamespaceSymbol> AsValidNamespace()
        {
            return symbol.AsNamespace().ValidateOrThrow();
        }

        /// <summary>
        /// Checks if the symbol is a namespace and outputs the validated symbol.
        /// </summary>
        public bool IsNamespace(out ValidSymbol<INamespaceSymbol> validated)
        {
            var projected = symbol.AsNamespace();
            if (projected.HasValue)
            {
                validated = projected.ValidateOrThrow();
                return true;
            }

            validated = default;
            return false;
        }

        /// <summary>
        /// Checks if the symbol is not a namespace and outputs the validated symbol if it is.
        /// </summary>
        public bool IsNotNamespace(out ValidSymbol<INamespaceSymbol> validated)
        {
            validated = default;
            var projected = symbol.AsNamespace();
            if (!projected.HasValue) return true;
            validated = projected.ValidateOrThrow();
            return false;
        }

        /// <summary>
        /// Directly casts to ITypeSymbol (any type).
        /// Returns Empty if symbol is null or not a type.
        /// </summary>
        public OptionalSymbol<ITypeSymbol> AsType()
        {
            return symbol is ITypeSymbol ts
                ? OptionalSymbol<ITypeSymbol>.WithValue(ts)
                : OptionalSymbol<ITypeSymbol>.Empty();
        }

        /// <summary>
        /// Casts to ITypeSymbol and validates (throws if null or wrong type).
        /// </summary>
        public ValidSymbol<ITypeSymbol> AsValidType()
        {
            return symbol.AsType().ValidateOrThrow();
        }

        /// <summary>
        /// Checks if the symbol is a type and outputs the validated symbol.
        /// </summary>
        public bool IsType(out ValidSymbol<ITypeSymbol> validated)
        {
            var projected = symbol.AsType();
            if (projected.HasValue)
            {
                validated = projected.ValidateOrThrow();
                return true;
            }

            validated = default;
            return false;
        }

        /// <summary>
        /// Checks if the symbol is not a type and outputs the validated symbol if it is.
        /// </summary>
        public bool IsNotType(out ValidSymbol<ITypeSymbol> validated)
        {
            validated = default;
            var projected = symbol.AsType();
            if (!projected.HasValue) return true;
            validated = projected.ValidateOrThrow();
            return false;
        }

        /// <summary>
        /// Directly casts to IMethodSymbol.
        /// Returns Empty if symbol is null or not a method.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> AsMethod()
        {
            return symbol is IMethodSymbol ms
                ? OptionalSymbol<IMethodSymbol>.WithValue(ms)
                : OptionalSymbol<IMethodSymbol>.Empty();
        }

        /// <summary>
        /// Casts to IMethodSymbol and validates (throws if null or wrong type).
        /// </summary>
        public ValidSymbol<IMethodSymbol> AsValidMethod()
        {
            return symbol.AsMethod().ValidateOrThrow();
        }

        /// <summary>
        /// Checks if the symbol is a method and outputs the validated symbol.
        /// </summary>
        public bool IsMethod(out ValidSymbol<IMethodSymbol> validated)
        {
            var projected = symbol.AsMethod();
            if (projected.HasValue)
            {
                validated = projected.ValidateOrThrow();
                return true;
            }

            validated = default;
            return false;
        }

        /// <summary>
        /// Checks if the symbol is not a method and outputs the validated symbol if it is.
        /// </summary>
        public bool IsNotMethod(out ValidSymbol<IMethodSymbol> validated)
        {
            validated = default;
            var projected = symbol.AsMethod();
            if (!projected.HasValue) return true;
            validated = projected.ValidateOrThrow();
            return false;
        }

        /// <summary>
        /// Directly casts to IPropertySymbol.
        /// Returns Empty if symbol is null or not a property.
        /// </summary>
        public OptionalSymbol<IPropertySymbol> AsProperty()
        {
            return symbol is IPropertySymbol ps
                ? OptionalSymbol<IPropertySymbol>.WithValue(ps)
                : OptionalSymbol<IPropertySymbol>.Empty();
        }

        /// <summary>
        /// Casts to IPropertySymbol and validates (throws if null or wrong type).
        /// </summary>
        public ValidSymbol<IPropertySymbol> AsValidProperty()
        {
            return symbol.AsProperty().ValidateOrThrow();
        }

        /// <summary>
        /// Checks if the symbol is a property and outputs the validated symbol.
        /// </summary>
        public bool IsProperty(out ValidSymbol<IPropertySymbol> validated)
        {
            var projected = symbol.AsProperty();
            if (projected.HasValue)
            {
                validated = projected.ValidateOrThrow();
                return true;
            }

            validated = default;
            return false;
        }

        /// <summary>
        /// Checks if the symbol is not a property and outputs the validated symbol if it is.
        /// </summary>
        public bool IsNotProperty(out ValidSymbol<IPropertySymbol> validated)
        {
            validated = default;
            var projected = symbol.AsProperty();
            if (!projected.HasValue) return true;
            validated = projected.ValidateOrThrow();
            return false;
        }

        /// <summary>
        /// Directly casts to IFieldSymbol.
        /// Returns Empty if symbol is null or not a field.
        /// </summary>
        public OptionalSymbol<IFieldSymbol> AsField()
        {
            return symbol is IFieldSymbol fs
                ? OptionalSymbol<IFieldSymbol>.WithValue(fs)
                : OptionalSymbol<IFieldSymbol>.Empty();
        }

        /// <summary>
        /// Casts to IFieldSymbol and validates (throws if null or wrong type).
        /// </summary>
        public ValidSymbol<IFieldSymbol> AsValidField()
        {
            return symbol.AsField().ValidateOrThrow();
        }

        /// <summary>
        /// Checks if the symbol is a field and outputs the validated symbol.
        /// </summary>
        public bool IsField(out ValidSymbol<IFieldSymbol> validated)
        {
            var projected = symbol.AsField();
            if (projected.HasValue)
            {
                validated = projected.ValidateOrThrow();
                return true;
            }

            validated = default;
            return false;
        }

        /// <summary>
        /// Checks if the symbol is not a field and outputs the validated symbol if it is.
        /// </summary>
        public bool IsNotField(out ValidSymbol<IFieldSymbol> validated)
        {
            validated = default;
            var projected = symbol.AsField();
            if (!projected.HasValue) return true;
            validated = projected.ValidateOrThrow();
            return false;
        }

        /// <summary>
        /// Directly casts to IParameterSymbol.
        /// Returns Empty if symbol is null or not a parameter.
        /// </summary>
        public OptionalSymbol<IParameterSymbol> AsParameter()
        {
            return symbol is IParameterSymbol ps
                ? OptionalSymbol<IParameterSymbol>.WithValue(ps)
                : OptionalSymbol<IParameterSymbol>.Empty();
        }

        /// <summary>
        /// Casts to IParameterSymbol and validates (throws if null or wrong type).
        /// </summary>
        public ValidSymbol<IParameterSymbol> AsValidParameter()
        {
            return symbol.AsParameter().ValidateOrThrow();
        }

        /// <summary>
        /// Checks if the symbol is a parameter and outputs the validated symbol.
        /// </summary>
        public bool IsParameter(out ValidSymbol<IParameterSymbol> validated)
        {
            var projected = symbol.AsParameter();
            if (projected.HasValue)
            {
                validated = projected.ValidateOrThrow();
                return true;
            }

            validated = default;
            return false;
        }

        /// <summary>
        /// Checks if the symbol is not a parameter and outputs the validated symbol if it is.
        /// </summary>
        public bool IsNotParameter(out ValidSymbol<IParameterSymbol> validated)
        {
            validated = default;
            var projected = symbol.AsParameter();
            if (!projected.HasValue) return true;
            validated = projected.ValidateOrThrow();
            return false;
        }

        /// <summary>
        /// Gets the first type argument from a generic type as INamedTypeSymbol.
        /// Common use case: extracting TEntity from DbSet&lt;TEntity&gt; or Task&lt;TEntity&gt;.
        /// Returns Empty if symbol is not a named type, not generic, has no type arguments, 
        /// or first arg is not a named type.
        /// </summary>
        /// <example>
        /// <code>
        /// // Extract entity type from DbSet&lt;Product&gt;
        /// var entityType = dbSetType.GetFirstTypeArgument().OrNull();
        /// </code>
        /// </example>
        public OptionalSymbol<INamedTypeSymbol> GetFirstTypeArgument()
        {
            return symbol is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: > 0 } nts &&
                   nts.TypeArguments[0] is INamedTypeSymbol arg
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(arg)
                : OptionalSymbol<INamedTypeSymbol>.Empty();
        }
    }

    extension(OptionalSymbol<INamedTypeSymbol> typeSymbol)
    {
        /// <summary>
        /// Creates a query to search for methods in the type.
        /// </summary>
        public MethodQuery QueryMethods()
        {
            return MethodQuery.From(typeSymbol.OrThrow());
        }

        /// <summary>
        /// Creates a query to search for properties in the type.
        /// </summary>
        public PropertyQuery QueryProperties()
        {
            return PropertyQuery.From(typeSymbol.OrThrow());
        }

        /// <summary>
        /// Creates a query to search for constructors in the type.
        /// </summary>
        public ConstructorQuery QueryConstructors()
        {
            return ConstructorQuery.From(typeSymbol.OrThrow());
        }
    }

    extension(ValidSymbol<INamedTypeSymbol> typeSymbol)
    {
        /// <summary>
        /// Creates a query to search for methods in the type.
        /// </summary>
        public MethodQuery QueryMethods()
        {
            return MethodQuery.From(typeSymbol.OrThrow());
        }

        /// <summary>
        /// Creates a query to search for properties in the type.
        /// </summary>
        public PropertyQuery QueryProperties()
        {
            return PropertyQuery.From(typeSymbol.OrThrow());
        }
    }
}