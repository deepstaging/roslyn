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
        public bool IsSymbol(ISymbol? other) => SymbolEqualityComparer.Default.Equals(symbol, other);

        /// <summary>
        /// Checks if this symbol does not equal another symbol using semantic comparison.
        /// </summary>
        public bool DoesNotEqual(ISymbol? other) => !SymbolEqualityComparer.Default.Equals(symbol, other);

        /// <summary>
        /// Gets all attributes on the symbol as validated attribute projections.
        /// </summary>
        public IEnumerable<ValidAttribute> GetAttributes() => symbol.GetAttributes().Select(ValidAttribute.From);

        /// <summary>
        /// Gets attributes by name (supports both "Name" and "NameAttribute" forms).
        /// </summary>
        public IEnumerable<ValidAttribute> GetAttributesByName(string attributeName) =>
            symbol.GetAttributes().GetByName(attributeName);

        /// <summary>
        /// Gets attributes by type.
        /// </summary>
        public IEnumerable<ValidAttribute> GetAttributesByType<TAttribute>()
            where TAttribute : Attribute => symbol.GetAttributesByName(typeof(TAttribute).Name);

        /// <summary>
        /// Gets attributes by System.Type, supporting open generic types.
        /// For generic attributes, use typeof(MyAttribute&lt;&gt;) to match any instantiation.
        /// </summary>
        /// <param name="attributeType">The attribute type. Can be an open generic like typeof(MyAttribute&lt;&gt;).</param>
        public IEnumerable<ValidAttribute> GetAttributesByType(Type attributeType) =>
            symbol.GetAttributes().GetByType(attributeType);

        /// <summary>
        /// Gets attributes by metadata name (supports generic arity notation like "MyAttribute`1").
        /// </summary>
        public IEnumerable<ValidAttribute> GetAttributesByMetadataName(string metadataName) =>
            symbol.GetAttributes().GetByMetadataName(metadataName);

        /// <summary>
        /// Checks if the symbol is public.
        /// </summary>
        public bool IsPublic() => symbol.DeclaredAccessibility == Accessibility.Public;

        /// <summary>
        /// Checks if the symbol is private.
        /// </summary>
        public bool IsPrivate() => symbol.DeclaredAccessibility == Accessibility.Private;

        /// <summary>
        /// Checks if the symbol is protected.
        /// </summary>
        public bool IsProtected() => symbol.DeclaredAccessibility == Accessibility.Protected;

        /// <summary>
        /// Checks if the symbol is internal.
        /// </summary>
        public bool IsInternal() => symbol.DeclaredAccessibility == Accessibility.Internal;

        /// <summary>
        /// Checks if the symbol is protected internal.
        /// </summary>
        public bool IsProtectedInternal() => symbol.DeclaredAccessibility == Accessibility.ProtectedOrInternal;

        /// <summary>
        /// Checks if the symbol is private protected.
        /// </summary>
        public bool IsPrivateProtected() => symbol.DeclaredAccessibility == Accessibility.ProtectedAndInternal;

        /// <summary>
        /// Checks if the symbol is marked as virtual.
        /// </summary>
        public bool IsVirtual() => symbol.IsVirtual;

        /// <summary>
        /// Checks if the symbol is marked as override.
        /// </summary>
        public bool IsOverride() => symbol.IsOverride;

        /// <summary>
        /// Checks if the symbol is marked as sealed.
        /// </summary>
        public bool IsSealed() => symbol.IsSealed;

        /// <summary>
        /// Checks if the symbol is marked as abstract.
        /// </summary>
        public bool IsAbstract() => symbol.IsAbstract;

        /// <summary>
        /// Checks if the symbol is marked as static.
        /// </summary>
        public bool IsStatic() => symbol.IsStatic;

        /// <summary>
        /// Checks if the symbol is marked as extern.
        /// </summary>
        public bool IsExtern() => symbol.IsExtern;

        /// <summary>
        /// Checks if the symbol is obsolete (has ObsoleteAttribute).
        /// </summary>
        public bool IsObsolete() => symbol.GetAttributes().Any(a => a.AttributeClass?.Name == "ObsoleteAttribute");

        /// <summary>
        /// Checks if the symbol is implicitly declared (compiler-generated).
        /// </summary>
        public bool IsImplicitlyDeclared() => symbol.IsImplicitlyDeclared;

        /// <summary>
        /// Checks if the symbol is defined in source code (not from metadata).
        /// </summary>
        public bool IsFromSource() => symbol.Locations.Any(l => l.IsInSource);

        /// <summary>
        /// Checks if the symbol is defined in a referenced assembly (from metadata).
        /// </summary>
        public bool IsFromMetadata() => symbol.Locations.Any(l => l.IsInMetadata);

        /// <summary>
        /// Checks if the symbol has any attributes.
        /// </summary>
        public bool HasAttributes() => symbol.GetAttributes().Length > 0;

        /// <summary>
        /// Checks if the symbol has an attribute with the specified name.
        /// Supports both "Name" and "NameAttribute" forms.
        /// </summary>
        public bool HasAttribute(string attributeName) => symbol.GetAttributesByName(attributeName).Any();

        /// <summary>
        /// Checks if the symbol has an attribute of the specified type.
        /// </summary>
        public bool HasAttribute<TAttribute>() where TAttribute : Attribute =>
            symbol.GetAttributesByType<TAttribute>().Any();

        /// <summary>
        /// Checks if the symbol has an attribute of the specified System.Type.
        /// Supports open generic types - use typeof(MyAttribute&lt;&gt;) to match any instantiation.
        /// </summary>
        /// <param name="attributeType">The attribute type. Can be an open generic like typeof(MyAttribute&lt;&gt;).</param>
        public bool HasAttribute(Type attributeType) => symbol.GetAttributesByType(attributeType).Any();

        /// <summary>
        /// Checks if the symbol has no attributes.
        /// </summary>
        public bool LacksAttributes() => symbol.GetAttributes().Length == 0;

        /// <summary>
        /// Checks if the symbol does not have an attribute with the specified name.
        /// </summary>
        public bool LacksAttribute(string attributeName) => !symbol.GetAttributesByName(attributeName).Any();

        /// <summary>
        /// Checks if the symbol does not have an attribute of the specified type.
        /// </summary>
        public bool LacksAttribute<TAttribute>() where TAttribute : Attribute =>
            !symbol.GetAttributesByType<TAttribute>().Any();

        /// <summary>
        /// Checks if the symbol does not have an attribute of the specified System.Type.
        /// Supports open generic types - use typeof(MyAttribute&lt;&gt;) to match any instantiation.
        /// </summary>
        /// <param name="attributeType">The attribute type. Can be an open generic like typeof(MyAttribute&lt;&gt;).</param>
        public bool LacksAttribute(Type attributeType) => !symbol.GetAttributesByType(attributeType).Any();
    }
}