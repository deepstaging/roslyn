// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
// ReSharper disable MemberCanBePrivate.Global

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for OptionalSymbol&lt;ITypeSymbol&gt; exposing ITypeSymbol-specific functionality.
/// Note: Most simple properties (IsValueType, IsReferenceType, etc.) are accessible via .Symbol
/// These extensions focus on complex operations like inheritance checks, type analysis, and conversions.
/// </summary>
public static class ProjectedTypeSymbolExtensions
{
    /// <param name="type">The projected type symbol to check.</param>
    extension(OptionalSymbol<ITypeSymbol> type)
    {
        /// <summary>
        /// Gets the base type of the type.
        /// Returns Empty if type has no base type.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> GetBaseType()
        {
            return type is { HasValue: true, Symbol.BaseType: not null }
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(type.Symbol!.BaseType)
                : OptionalSymbol<INamedTypeSymbol>.Empty();
        }

        /// <summary>
        /// Checks if the type implements or inherits from another type using symbol equality.
        /// </summary>
        public bool ImplementsOrInheritsFrom(ITypeSymbol baseType)
        {
            return type.HasValue && type.Symbol!.ImplementsOrInheritsFrom(baseType);
        }

        /// <summary>
        /// Checks if the type is or inherits from a type with the specified name.
        /// </summary>
        public bool IsOrInheritsFrom(string typeName, string? containingNamespace = null)
        {
            return type.HasValue && type.Symbol!.IsOrInheritsFrom(typeName, containingNamespace);
        }

        /// <summary>
        /// Checks if the type inherits from a type with the specified name (excludes the type itself).
        /// </summary>
        public bool InheritsFrom(string typeName, string? containingNamespace = null)
        {
            return type.HasValue && type.Symbol!.InheritsFrom(typeName, containingNamespace);
        }

        /// <summary>
        /// Gets the base type with the specified name from the inheritance chain.
        /// Returns Empty if no matching base type is found.
        /// </summary>
        public OptionalSymbol<ITypeSymbol> GetBaseTypeByName(string typeName)
        {
            return type.HasValue && type.Symbol!.GetBaseTypeByName(typeName) is { } baseType
                ? OptionalSymbol<ITypeSymbol>.WithValue(baseType)
                : OptionalSymbol<ITypeSymbol>.Empty();
        }

        /// <summary>
        /// Checks if the type is a Task or Task&lt;T&gt;.
        /// </summary>
        public bool IsTaskType()
        {
            return type.HasValue && type.Symbol!.IsTaskType();
        }

        /// <summary>
        /// Checks if the type is a generic async type (Task&lt;T&gt; or ValueTask&lt;T&gt;).
        /// </summary>
        public bool IsGenericAsyncType()
        {
            return type.HasValue && (type.Symbol!.IsGenericTaskType() || type.Symbol!.IsGenericValueTaskType());
        }

        /// <summary>
        /// Checks if the type is a generic Task&lt;T&gt;.
        /// </summary>
        public bool IsGenericTaskType()
        {
            return type.HasValue && type.Symbol!.IsGenericTaskType();
        }

        /// <summary>
        /// Checks if the type is a ValueTask or ValueTask&lt;T&gt;.
        /// </summary>
        public bool IsValueTaskType()
        {
            return type.HasValue && type.Symbol!.IsValueTaskType();
        }

        /// <summary>
        /// Checks if the type is a generic ValueTask&lt;T&gt;.
        /// </summary>
        public bool IsGenericValueTaskType()
        {
            return type.HasValue && type.Symbol!.IsGenericValueTaskType();
        }

        /// <summary>
        /// Extracts the first type argument from a generic type with a single type parameter.
        /// Returns Empty if not a generic type or doesn't have exactly one type argument.
        /// </summary>
        public OptionalSymbol<ITypeSymbol> GetSingleTypeArgument()
        {
            return type.HasValue && type.Symbol!.GetSingleTypeArgument() is { } innerType
                ? OptionalSymbol<ITypeSymbol>.WithValue(innerType)
                : OptionalSymbol<ITypeSymbol>.Empty();
        }

        /// <summary>
        /// Extracts the inner type from Task&lt;T&gt; or ValueTask&lt;T&gt;.
        /// Returns Empty if not a generic Task/ValueTask.
        /// </summary>
        public OptionalSymbol<ITypeSymbol> ExtractTaskInnerType()
        {
            return type.IsGenericTaskType() || type.IsGenericValueTaskType()
                ? type.GetSingleTypeArgument()
                : OptionalSymbol<ITypeSymbol>.Empty();
        }

        /// <summary>
        /// Gets the element type of an array.
        /// Returns Empty if not an array.
        /// </summary>
        public OptionalSymbol<ITypeSymbol> GetArrayElementType()
        {
            return type is { HasValue: true, Symbol: IArrayTypeSymbol arrayType }
                ? OptionalSymbol<ITypeSymbol>.WithValue(arrayType.ElementType)
                : OptionalSymbol<ITypeSymbol>.Empty();
        }

        /// <summary>
        /// Gets the pointed-at type of a pointer.
        /// Returns Empty if not a pointer.
        /// </summary>
        public OptionalSymbol<ITypeSymbol> GetPointedAtType()
        {
            return type is { HasValue: true, Symbol: IPointerTypeSymbol pointerType }
                ? OptionalSymbol<ITypeSymbol>.WithValue(pointerType.PointedAtType)
                : OptionalSymbol<ITypeSymbol>.Empty();
        }

        /// <summary>
        /// Gets the special type enum for built-in types (System.Int32, System.String, System.Void, etc.).
        /// Returns null if type is not present.
        /// Use this to check if a type is void: GetSpecialType() == SpecialType.System_Void
        /// </summary>
        public SpecialType? GetSpecialType()
        {
            return type.HasValue ? type.Symbol!.SpecialType : null;
        }

        /// <summary>
        /// Checks if the type is a specific special type (System.Int32, System.String, System.Void, etc.).
        /// Returns false if type is not present or doesn't match the specified special type.
        /// </summary>
        /// <param name="specialType">The special type to check against.</param>
        /// <returns>True if the type matches the specified special type, false otherwise.</returns>
        /// <example>
        /// <code>
        /// if (returnType.IsSpecialType(SpecialType.System_Void))
        /// {
        ///     // Handle void return type
        /// }
        /// </code>
        /// </example>
        public bool IsSpecialType(SpecialType specialType)
        {
            return type.HasValue && type.Symbol!.SpecialType == specialType;
        }
    }
}