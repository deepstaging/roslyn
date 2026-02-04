// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ValidSymbol&lt;INamedTypeSymbol&gt; exposing INamedTypeSymbol-specific functionality.
/// </summary>
public static class ValidNamedTypeSymbolExtensions
{
    extension(ValidSymbol<INamedTypeSymbol> type)
    {
        /// <summary>
        /// Gets the underlying type of the enum.
        /// Returns Empty if not an enum or underlying type is null.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> EnumUnderlyingType =>
            type.Value.EnumUnderlyingType is { } underlyingType
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(underlyingType)
                : OptionalSymbol<INamedTypeSymbol>.Empty();

        /// <summary>
        /// Gets the delegate invoke method (for delegate types).
        /// Returns Empty if not a delegate or invoke method is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> DelegateInvokeMethod =>
            type.Value.DelegateInvokeMethod is { } invokeMethod
                ? OptionalSymbol<IMethodSymbol>.WithValue(invokeMethod)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the native integer underlying type (IntPtr or UIntPtr) for nint/nuint types.
        /// Returns Empty if not a native integer type or underlying type is null.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> NativeIntegerUnderlyingType =>
            type.Value.NativeIntegerUnderlyingType is { } nativeType
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(nativeType)
                : OptionalSymbol<INamedTypeSymbol>.Empty();

        /// <summary>
        /// Gets the base type of this type.
        /// Returns Empty if there is no base type (e.g., object, interfaces).
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> BaseType =>
            type.Value.BaseType is { } baseType
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(baseType)
                : OptionalSymbol<INamedTypeSymbol>.Empty();

        /// <summary>
        /// Gets all base types in the inheritance hierarchy.
        /// </summary>
        public IEnumerable<ValidSymbol<INamedTypeSymbol>> GetBaseTypes()
        {
            var current = type.Value.BaseType;
            while (current != null)
            {
                yield return ValidSymbol<INamedTypeSymbol>.From(current);
                current = current.BaseType;
            }
        }

        /// <summary>
        /// Gets all interfaces implemented by this type.
        /// </summary>
        public IEnumerable<ValidSymbol<INamedTypeSymbol>> GetInterfaces()
        {
            foreach (var iface in type.Value.Interfaces) yield return ValidSymbol<INamedTypeSymbol>.From(iface);
        }

        /// <summary>
        /// Gets all interfaces implemented by this type, including base type interfaces.
        /// </summary>
        public IEnumerable<ValidSymbol<INamedTypeSymbol>> GetAllInterfaces()
        {
            foreach (var iface in type.Value.AllInterfaces) yield return ValidSymbol<INamedTypeSymbol>.From(iface);
        }

        /// <summary>
        /// Checks if this type implements the specified interface by name.
        /// Checks both direct and inherited interfaces.
        /// </summary>
        public bool ImplementsInterface(string interfaceName)
        {
            return type.Value.AllInterfaces.Any(i =>
                i.Name == interfaceName ||
                i.ToDisplayString() == interfaceName ||
                i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == interfaceName);
        }

        /// <summary>
        /// Checks if this type inherits from the specified base type by name.
        /// </summary>
        public bool InheritsFrom(string baseTypeName)
        {
            var current = type.Value.BaseType;
            while (current != null)
            {
                if (current.Name == baseTypeName ||
                    current.ToDisplayString() == baseTypeName ||
                    current.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == baseTypeName)
                    return true;
                current = current.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Gets the constructed (generic instantiated) version from an unbound generic type.
        /// Returns Empty if not an unbound generic type.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> ConstructedFrom =>
            type.Value.ConstructedFrom is { } constructed &&
                   !SymbolEqualityComparer.Default.Equals(constructed, type.Value)
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(constructed)
                : OptionalSymbol<INamedTypeSymbol>.Empty();

        /// <summary>
        /// Gets the original definition of a generic type.
        /// For List&lt;int&gt;, returns List&lt;T&gt;.
        /// Returns Empty if this is the original definition.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> OriginalDefinition =>
            type.Value.OriginalDefinition is { } original &&
                   !SymbolEqualityComparer.Default.Equals(original, type.Value)
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(original)
                : OptionalSymbol<INamedTypeSymbol>.Empty();

        /// <summary>
        /// Checks if this is a partial type using syntax analysis.
        /// </summary>
        public bool IsPartialType()
        {
            return type.Value.IsPartial();
        }

        /// <summary>
        /// Checks if this is an enum type.
        /// </summary>
        public bool IsEnum()
        {
            return type.Value.TypeKind == TypeKind.Enum;
        }

        /// <summary>
        /// Checks if this is a delegate type.
        /// </summary>
        public bool IsDelegate()
        {
            return type.Value.TypeKind == TypeKind.Delegate;
        }

        /// <summary>
        /// Checks if this is a nested type.
        /// </summary>
        public bool IsNestedType()
        {
            return type.Value.ContainingType != null;
        }

        /// <summary>
        /// Gets the containing type if this is a nested type.
        /// Returns Empty if not a nested type.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> ContainingType =>
            type.Value.ContainingType is { } containingType
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(containingType)
                : OptionalSymbol<INamedTypeSymbol>.Empty();

        /// <summary>
        /// Gets all nested types declared within this type.
        /// </summary>
        public IEnumerable<ValidSymbol<INamedTypeSymbol>> GetNestedTypes()
        {
            foreach (var member in type.Value.GetMembers())
                if (member is INamedTypeSymbol nestedType)
                    yield return ValidSymbol<INamedTypeSymbol>.From(nestedType);
        }

        /// <summary>
        /// Checks if this type is a tuple type.
        /// </summary>
        public bool IsTupleType()
        {
            return type.Value.IsTupleType;
        }

        /// <summary>
        /// Checks if this type is an anonymous type.
        /// </summary>
        public bool IsAnonymousType()
        {
            return type.Value.IsAnonymousType;
        }

        /// <summary>
        /// Checks if this type is a native integer type (nint or nuint).
        /// </summary>
        public bool IsNativeIntegerType()
        {
            return type.Value.IsNativeIntegerType;
        }

        /// <summary>
        /// Gets the arity (number of type parameters) for generic types.
        /// Returns 0 for non-generic types.
        /// </summary>
        public int Arity => type.Value.Arity;

        /// <summary>
        /// Checks if this is an unbound generic type (e.g., List&lt;&gt; without type arguments).
        /// </summary>
        public bool IsUnboundGenericType()
        {
            return type.Value.IsUnboundGenericType;
        }
    }
}