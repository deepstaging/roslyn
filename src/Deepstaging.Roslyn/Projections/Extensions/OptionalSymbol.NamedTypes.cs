// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for OptionalSymbol&lt;INamedTypeSymbol&gt; exposing INamedTypeSymbol-specific functionality.
/// Note: Most properties (IsDelegate, IsEnum, Arity, etc.) are accessible via .Symbol
/// These extensions focus on complex operations like getting nested types, enum underlying types, etc.
/// </summary>
public static class ProjectedNamedTypeSymbolExtensions
{
    extension(OptionalSymbol<INamedTypeSymbol> type)
    {
        /// <summary>
        /// Gets the base type of this type.
        /// Returns Empty if there is no base type (e.g., object, interfaces).
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> BaseType =>
            type is { HasValue: true, Symbol.BaseType: { } baseType }
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(baseType)
                : OptionalSymbol<INamedTypeSymbol>.Empty();

        /// <summary>
        /// Gets all base types in the inheritance hierarchy.
        /// Returns empty if type has no value.
        /// </summary>
        public IEnumerable<ValidSymbol<INamedTypeSymbol>> GetBaseTypes()
        {
            if (!type.HasValue) yield break;
            var current = type.Symbol!.BaseType;
            while (current != null)
            {
                yield return ValidSymbol<INamedTypeSymbol>.From(current);
                current = current.BaseType;
            }
        }

        /// <summary>
        /// Gets all interfaces implemented by this type.
        /// Returns empty if type has no value.
        /// </summary>
        public IEnumerable<ValidSymbol<INamedTypeSymbol>> GetInterfaces()
        {
            if (!type.HasValue) yield break;
            foreach (var iface in type.Symbol!.Interfaces)
                yield return ValidSymbol<INamedTypeSymbol>.From(iface);
        }

        /// <summary>
        /// Gets all interfaces implemented by this type, including base type interfaces.
        /// Returns empty if type has no value.
        /// </summary>
        public IEnumerable<ValidSymbol<INamedTypeSymbol>> GetAllInterfaces()
        {
            if (!type.HasValue) yield break;
            foreach (var iface in type.Symbol!.AllInterfaces)
                yield return ValidSymbol<INamedTypeSymbol>.From(iface);
        }

        /// <summary>
        /// Checks if this type implements the specified interface by name.
        /// Checks both direct and inherited interfaces.
        /// Returns false if type has no value.
        /// </summary>
        public bool ImplementsInterface(string interfaceName)
        {
            return type.HasValue && type.Symbol!.AllInterfaces.Any(i =>
                i.Name == interfaceName ||
                i.ToDisplayString() == interfaceName ||
                i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == interfaceName);
        }

        /// <summary>
        /// Checks if this type inherits from the specified base type by name.
        /// Returns false if type has no value.
        /// </summary>
        public bool InheritsFrom(string baseTypeName)
        {
            if (!type.HasValue) return false;
            var current = type.Symbol!.BaseType;
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
        /// Gets the underlying type of the enum.
        /// Returns Empty if not an enum or underlying type is null.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> EnumUnderlyingType =>
            type is { HasValue: true, Symbol.EnumUnderlyingType: { } underlyingType }
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(underlyingType)
                : OptionalSymbol<INamedTypeSymbol>.Empty();

        /// <summary>
        /// Gets the delegate invoke method (for delegate types).
        /// Returns Empty if not a delegate or invoke method is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> DelegateInvokeMethod =>
            type is { HasValue: true, Symbol.DelegateInvokeMethod: not null }
                ? OptionalSymbol<IMethodSymbol>.WithValue(type.Symbol!.DelegateInvokeMethod)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the native integer underlying type (IntPtr or UIntPtr) for nint/nu int types.
        /// Returns Empty if not a native integer type or underlying type is null.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> NativeIntegerUnderlyingType =>
            type is { HasValue: true, Symbol.NativeIntegerUnderlyingType: not null }
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(type.Symbol!.NativeIntegerUnderlyingType)
                : OptionalSymbol<INamedTypeSymbol>.Empty();

        /// <summary>
        /// Checks if the type is partial using syntax analysis.
        /// This requires checking DeclaringSyntaxReferences unlike most other checks.
        /// </summary>
        public bool IsPartialType()
        {
            return type.HasValue && type.Symbol!.IsPartial();
        }
    }
}