// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ITypeSymbol to start fluent query builders.
/// </summary>
public static class TypeSymbolQueryExtensions
{
    /// <param name="typeSymbol">The type symbol to query.</param>
    extension(ITypeSymbol typeSymbol)
    {
        /// <summary>
        /// Starts a fluent query for methods on this type.
        /// </summary>
        public MethodQuery QueryMethods()
        {
            return MethodQuery.From(typeSymbol);
        }

        /// <summary>
        /// Starts a fluent query for properties on this type.
        /// </summary>
        public PropertyQuery QueryProperties()
        {
            return PropertyQuery.From(typeSymbol);
        }

        /// <summary>
        /// Starts a fluent query for constructors on this type.
        /// </summary>
        public ConstructorQuery QueryConstructors()
        {
            return ConstructorQuery.From(typeSymbol);
        }

        /// <summary>
        /// Starts a fluent query for fields on this type.
        /// </summary>
        public FieldQuery QueryFields()
        {
            return FieldQuery.From(typeSymbol);
        }

        /// <summary>
        /// Starts a fluent query for events on this type.
        /// </summary>
        public EventQuery QueryEvents()
        {
            return EventQuery.From(typeSymbol);
        }

        /// <summary>
        /// Gets all attributes on this type as optional attributes.
        /// </summary>
        public ImmutableArray<AttributeData> QueryAttributes()
        {
            return typeSymbol.GetAttributes();
        }

        /// <summary>
        /// Checks if the type is Task or ValueTask (with or without type arguments).
        /// </summary>
        public bool IsTaskType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Task" or "ValueTask" };
        }

        /// <summary>
        /// Checks if the type is ValueTask or ValueTask&lt;T&gt;.
        /// </summary>
        public bool IsValueTaskType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "ValueTask" };
        }

        /// <summary>
        /// Checks if the type is Task&lt;T&gt;.
        /// </summary>
        public bool IsGenericTaskType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Task", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is ValueTask&lt;T&gt;.
        /// </summary>
        public bool IsGenericValueTaskType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "ValueTask", IsGenericType: true, TypeArguments.Length: 1 };
        }
        
        /// <summary>
        /// Checks if the type is Task (non-generic).
        /// </summary>
        public bool IsNonGenericTaskType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Task", IsGenericType: false };
        }
        
        /// <summary>
        /// Checks if the type is ValueTask (non-generic).
        /// </summary>
        public bool IsNonGenericValueTaskType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "ValueTask", IsGenericType: false };
        }

        /// <summary>
        /// Checks if the type is IEnumerable&lt;T&gt;.
        /// </summary>
        public bool IsEnumerableType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IEnumerable", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is ICollection&lt;T&gt;.
        /// </summary>
        public bool IsCollectionType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "ICollection", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is IList&lt;T&gt;.
        /// </summary>
        public bool IsListType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IList", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is IDictionary&lt;TKey, TValue&gt;.
        /// </summary>
        public bool IsDictionaryType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IDictionary", IsGenericType: true, TypeArguments.Length: 2 };
        }

        /// <summary>
        /// Checks if the type is IQueryable&lt;T&gt;.
        /// </summary>
        public bool IsQueryableType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IQueryable", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is IObservable&lt;T&gt;.
        /// </summary>
        public bool IsObservableType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "IObservable", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is Nullable&lt;T&gt; (value type nullable).
        /// </summary>
        public bool IsNullableValueType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Nullable", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is Func&lt;...&gt;.
        /// </summary>
        public bool IsFuncType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Func", IsGenericType: true };
        }

        /// <summary>
        /// Checks if the type is Action&lt;...&gt;.
        /// </summary>
        public bool IsActionType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Action" };
        }

        /// <summary>
        /// Checks if the type is Lazy&lt;T&gt;.
        /// </summary>
        public bool IsLazyType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Lazy", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is a tuple (ValueTuple).
        /// </summary>
        public bool IsTupleType()
        {
            return typeSymbol is INamedTypeSymbol { IsTupleType: true };
        }

        /// <summary>
        /// Checks if the type is an array type.
        /// </summary>
        public bool IsArrayType()
        {
            return typeSymbol is IArrayTypeSymbol;
        }

        /// <summary>
        /// Checks if the type is a pointer type.
        /// </summary>
        public bool IsPointerType()
        {
            return typeSymbol is IPointerTypeSymbol;
        }

        /// <summary>
        /// Checks if the type is a delegate type.
        /// </summary>
        public bool IsDelegateType()
        {
            return typeSymbol.TypeKind == TypeKind.Delegate;
        }

        /// <summary>
        /// Checks if the type is an enum type.
        /// </summary>
        public bool IsEnumType()
        {
            return typeSymbol.TypeKind == TypeKind.Enum;
        }

        /// <summary>
        /// Checks if the type is an interface type.
        /// </summary>
        public bool IsInterfaceType()
        {
            return typeSymbol.TypeKind == TypeKind.Interface;
        }

        /// <summary>
        /// Checks if the type is a record type (class or struct).
        /// </summary>
        public bool IsRecordType()
        {
            return typeSymbol is INamedTypeSymbol { IsRecord: true };
        }

        /// <summary>
        /// Checks if the type is a struct/value type (excluding enums).
        /// </summary>
        public bool IsStructType()
        {
            return typeSymbol.IsValueType && typeSymbol.TypeKind == TypeKind.Struct;
        }

        /// <summary>
        /// Checks if the type is a class type.
        /// </summary>
        public bool IsClassType()
        {
            return typeSymbol.TypeKind == TypeKind.Class;
        }

        /// <summary>
        /// Checks if the type is marked as abstract.
        /// </summary>
        public bool IsAbstractType()
        {
            return typeSymbol.IsAbstract;
        }

        /// <summary>
        /// Checks if the type is marked as sealed.
        /// </summary>
        public bool IsSealedType()
        {
            return typeSymbol.IsSealed;
        }

        /// <summary>
        /// Checks if the type is static.
        /// </summary>
        public bool IsStaticType()
        {
            return typeSymbol.IsStatic;
        }

        /// <summary>
        /// Checks if the type implements or inherits from the specified base type.
        /// </summary>
        public bool ImplementsOrInheritsFrom(ITypeSymbol baseType)
        {
            if (SymbolEqualityComparer.Default.Equals(typeSymbol, baseType))
                return true;

            // Check base types
            var current = typeSymbol.BaseType;
            while (current != null)
            {
                if (SymbolEqualityComparer.Default.Equals(current, baseType))
                    return true;
                current = current.BaseType;
            }

            // Check interfaces
            return typeSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, baseType));
        }

        /// <summary>
        /// Checks if the type is or inherits from a type with the specified name.
        /// </summary>
        public bool IsOrInheritsFrom(string typeName, string? containingNamespace = null)
        {
            var current = typeSymbol;
            while (current != null)
            {
                if (current.Name == typeName &&
                    (containingNamespace == null || current.ContainingNamespace?.ToString() == containingNamespace))
                    return true;
                current = current.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Checks if the type inherits from a type with the specified name (excludes the type itself).
        /// </summary>
        public bool InheritsFrom(string typeName, string? containingNamespace = null)
        {
            var current = typeSymbol.BaseType;
            while (current != null)
            {
                if (current.Name == typeName &&
                    (containingNamespace == null || current.ContainingNamespace?.ToString() == containingNamespace))
                    return true;
                current = current.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Gets the base type with the specified name from the inheritance chain.
        /// Returns null if no matching base type is found.
        /// </summary>
        public ITypeSymbol? GetBaseTypeByName(string typeName)
        {
            var current = typeSymbol.BaseType;
            while (current != null)
            {
                if (current.Name == typeName)
                    return current;
                current = current.BaseType;
            }

            return null;
        }

        /// <summary>
        /// Extracts the first type argument from a generic type with a single type parameter.
        /// Returns null if not a generic type or doesn't have exactly one type argument.
        /// </summary>
        public ITypeSymbol? GetSingleTypeArgument()
        {
            return typeSymbol is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } namedType
                ? namedType.TypeArguments[0]
                : null;
        }
    }
}