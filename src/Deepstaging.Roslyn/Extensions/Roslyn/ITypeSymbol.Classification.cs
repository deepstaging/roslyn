// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for classifying type kinds and modifiers.
/// </summary>
public static class TypeSymbolClassificationExtensions
{
    /// <param name="typeSymbol">The type symbol to check.</param>
    extension(ITypeSymbol typeSymbol)
    {
        // ── Type kinds ───────────────────────────────────────────────────

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
        /// Checks if the type is a tuple (ValueTuple).
        /// </summary>
        public bool IsTupleType()
        {
            return typeSymbol is INamedTypeSymbol { IsTupleType: true };
        }

        /// <summary>
        /// Checks if the type is <c>Nullable&lt;T&gt;</c> (value type nullable).
        /// </summary>
        public bool IsNullableValueType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Nullable", IsGenericType: true, TypeArguments.Length: 1 };
        }

        // ── Modifiers ────────────────────────────────────────────────────

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
    }
}
