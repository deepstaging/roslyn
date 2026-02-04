// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
// ReSharper disable MemberCanBePrivate.Global

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ISymbol - provides convenient attribute and accessibility checks.
/// </summary>
public static class SymbolExtensions
{
    extension(ISymbol symbol)
    {
        /// <summary>
        /// Checks if this symbol equals another symbol using semantic comparison.
        /// </summary>
        public bool IsSymbol(ISymbol? other)
        {
            return SymbolEqualityComparer.Default.Equals(symbol, other);
        }

        /// <summary>
        /// Checks if this symbol does not equal another symbol using semantic comparison.
        /// </summary>
        public bool DoesNotEqual(ISymbol? other)
        {
            return !SymbolEqualityComparer.Default.Equals(symbol, other);
        }

        /// <summary>
        /// Gets all attributes on the symbol as validated attribute projections.
        /// </summary>
        public IEnumerable<ValidAttribute> GetAttributes()
        {
            return symbol.GetAttributes().Select(ValidAttribute.From);
        }

        /// <summary>
        /// Gets attributes by name (supports both "Name" and "NameAttribute" forms).
        /// </summary>
        public IEnumerable<ValidAttribute> GetAttributesByName(string attributeName)
        {
            return symbol.GetAttributes().GetByName(attributeName);
        }

        /// <summary>
        /// Gets attributes by type.
        /// </summary>
        public IEnumerable<ValidAttribute> GetAttributesByType<TAttribute>()
            where TAttribute : Attribute
        {
            return symbol.GetAttributesByName(typeof(TAttribute).Name);
        }

        /// <summary>
        /// Checks if the symbol is public.
        /// </summary>
        public bool IsPublic()
        {
            return symbol.DeclaredAccessibility == Accessibility.Public;
        }

        /// <summary>
        /// Checks if the symbol is private.
        /// </summary>
        public bool IsPrivate()
        {
            return symbol.DeclaredAccessibility == Accessibility.Private;
        }

        /// <summary>
        /// Checks if the symbol is protected.
        /// </summary>
        public bool IsProtected()
        {
            return symbol.DeclaredAccessibility == Accessibility.Protected;
        }

        /// <summary>
        /// Checks if the symbol is internal.
        /// </summary>
        public bool IsInternal()
        {
            return symbol.DeclaredAccessibility == Accessibility.Internal;
        }

        /// <summary>
        /// Checks if the symbol is protected internal.
        /// </summary>
        public bool IsProtectedInternal()
        {
            return symbol.DeclaredAccessibility == Accessibility.ProtectedOrInternal;
        }

        /// <summary>
        /// Checks if the symbol is private protected.
        /// </summary>
        public bool IsPrivateProtected()
        {
            return symbol.DeclaredAccessibility == Accessibility.ProtectedAndInternal;
        }

        /// <summary>
        /// Checks if the symbol is marked as virtual.
        /// </summary>
        public bool IsVirtual()
        {
            return symbol.IsVirtual;
        }

        /// <summary>
        /// Checks if the symbol is marked as override.
        /// </summary>
        public bool IsOverride()
        {
            return symbol.IsOverride;
        }

        /// <summary>
        /// Checks if the symbol is marked as sealed.
        /// </summary>
        public bool IsSealed()
        {
            return symbol.IsSealed;
        }

        /// <summary>
        /// Checks if the symbol is marked as abstract.
        /// </summary>
        public bool IsAbstract()
        {
            return symbol.IsAbstract;
        }

        /// <summary>
        /// Checks if the symbol is marked as static.
        /// </summary>
        public bool IsStatic()
        {
            return symbol.IsStatic;
        }

        /// <summary>
        /// Checks if the symbol is marked as extern.
        /// </summary>
        public bool IsExtern()
        {
            return symbol.IsExtern;
        }

        /// <summary>
        /// Checks if the symbol is obsolete (has ObsoleteAttribute).
        /// </summary>
        public bool IsObsolete()
        {
            return symbol.GetAttributes().Any(a => a.AttributeClass?.Name == "ObsoleteAttribute");
        }

        /// <summary>
        /// Checks if the symbol is implicitly declared (compiler-generated).
        /// </summary>
        public bool IsImplicitlyDeclared()
        {
            return symbol.IsImplicitlyDeclared;
        }

        /// <summary>
        /// Checks if the symbol is defined in source code (not from metadata).
        /// </summary>
        public bool IsFromSource()
        {
            return symbol.Locations.Any(l => l.IsInSource);
        }

        /// <summary>
        /// Checks if the symbol is defined in a referenced assembly (from metadata).
        /// </summary>
        public bool IsFromMetadata()
        {
            return symbol.Locations.Any(l => l.IsInMetadata);
        }
    }
}