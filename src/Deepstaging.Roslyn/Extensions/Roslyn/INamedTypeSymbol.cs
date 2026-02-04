// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.CodeDom.Compiler;

// ReSharper disable MemberCanBePrivate.Global

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for INamedTypeSymbol - provides syntax-based checks not available on symbols.
/// For rich querying, use symbol.AsNamedType() to access type-specific extensions.
/// </summary>
public static class NamedTypeSymbolExtensions
{
    extension(INamedTypeSymbol symbol)
    {
        /// <summary>
        /// Determines whether a named type symbol is declared as partial.
        /// This requires syntax analysis as it's not available on the symbol itself.
        /// </summary>
        public bool IsPartial()
        {
            if (symbol == null) throw new ArgumentNullException(nameof(symbol));

            return symbol.DeclaringSyntaxReferences
                .Select(r => r.GetSyntax())
                .OfType<TypeDeclarationSyntax>()
                .Any(d => d.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)));
        }

        /// <summary>
        /// Checks if the type is generated code (has GeneratedCodeAttribute).
        /// </summary>
        public bool IsGeneratedCode()
        {
            var fullName = typeof(GeneratedCodeAttribute).FullName;
            return symbol.GetAttributes()
                .Any(attr => attr.AttributeClass?.ToDisplayString() == fullName);
        }

        /// <summary>
        /// Checks if the type is NOT generated code (inverse of IsGeneratedCode).
        /// </summary>
        public bool IsNotGeneratedCode()
        {
            return !symbol.IsGeneratedCode();
        }

        /// <summary>
        /// Checks if the type has a parameterless constructor.
        /// </summary>
        public bool HasParameterlessConstructor()
        {
            return symbol.Constructors.Any(c => c.Parameters.Length == 0);
        }

        /// <summary>
        /// Checks if the type is generic (has type parameters).
        /// </summary>
        public bool IsGeneric()
        {
            return symbol.IsGenericType;
        }

        /// <summary>
        /// Checks if the type is an open generic (unbound type parameters).
        /// </summary>
        public bool IsOpenGeneric()
        {
            return symbol.IsUnboundGenericType;
        }

        /// <summary>
        /// Checks if the type is nested within another type.
        /// </summary>
        public bool IsNestedType()
        {
            return symbol.ContainingType != null;
        }

        /// <summary>
        /// Gets the arity (number of type parameters) of the type.
        /// </summary>
        public int GetArity()
        {
            return symbol.Arity;
        }

        /// <summary>
        /// Checks if the type implements the specified interface by name.
        /// </summary>
        public bool ImplementsInterface(string interfaceName)
        {
            return symbol.AllInterfaces.Any(i => i.Name == interfaceName);
        }

        /// <summary>
        /// Checks if the type has any attributes.
        /// </summary>
        public bool HasAttributes()
        {
            return symbol.GetAttributes().Length > 0;
        }

        /// <summary>
        /// Checks if the type has the specified attribute by name.
        /// </summary>
        public bool HasAttribute(string attributeName)
        {
            return symbol.GetAttributes().Any(a =>
                a.AttributeClass?.Name == attributeName ||
                a.AttributeClass?.Name == attributeName + "Attribute");
        }
    }
}