// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for checking equality semantics on types.
/// </summary>
public static class TypeSymbolEqualityExtensions
{
    /// <param name="typeSymbol">The type symbol to check.</param>
    extension(ITypeSymbol typeSymbol)
    {
        /// <summary>
        /// Checks if the type implements <c>IEquatable&lt;T&gt;</c> where T is the type itself.
        /// Handles primitives, enums, strings, and <c>Nullable&lt;T&gt;</c> transparently.
        /// </summary>
        public bool ImplementsIEquatable()
        {
            if (typeSymbol.SpecialType is
                SpecialType.System_Boolean or
                SpecialType.System_Byte or
                SpecialType.System_SByte or
                SpecialType.System_Int16 or
                SpecialType.System_UInt16 or
                SpecialType.System_Int32 or
                SpecialType.System_UInt32 or
                SpecialType.System_Int64 or
                SpecialType.System_UInt64 or
                SpecialType.System_Single or
                SpecialType.System_Double or
                SpecialType.System_Char or
                SpecialType.System_String)
                return true;

            if (typeSymbol.TypeKind == TypeKind.Enum)
                return true;

            foreach (var iface in typeSymbol.AllInterfaces)
                if (iface is { Name: "IEquatable", IsGenericType: true, Arity: 1 } &&
                    iface.ContainingNamespace.ToDisplayString() == "System" &&
                    SymbolEqualityComparer.Default.Equals(iface.TypeArguments[0], typeSymbol))
                    return true;

            // Nullable<T> is equatable if T is equatable
            if (typeSymbol is INamedTypeSymbol { IsGenericType: true, Name: "Nullable", Arity: 1 } nullable &&
                nullable.ContainingNamespace.ToDisplayString() == "System")
                return nullable.TypeArguments[0].ImplementsIEquatable();

            return false;
        }
    }
}