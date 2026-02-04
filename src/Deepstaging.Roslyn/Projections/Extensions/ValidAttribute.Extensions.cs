// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ValidAttribute providing enhanced argument access.
/// </summary>
public static class ValidAttributeExtensions
{
    extension(ValidAttribute attribute)
    {
        /// <summary>
        /// Checks if a constructor argument exists at the specified index.
        /// </summary>
        public bool HasConstructorArg(int index)
        {
            return attribute.Value.ConstructorArguments.Length > index;
        }

        /// <summary>
        /// Attempts to get a constructor argument at the specified index.
        /// Returns true and sets value if successful, false otherwise.
        /// </summary>
        public bool TryGetConstructorArg<T>(int index, out T? value)
        {
            var arg = attribute.ConstructorArg<T>(index);
            if (arg.HasValue)
            {
                value = arg.OrThrow();
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets a constructor argument or returns a default value if not present.
        /// </summary>
        public T GetConstructorArgOrDefault<T>(int index, T defaultValue = default!)
        {
            var arg = attribute.ConstructorArg<T>(index);
            return arg.HasValue ? arg.OrThrow() : defaultValue;
        }

        /// <summary>
        /// Checks if a named argument exists with the specified name.
        /// </summary>
        public bool HasNamedArg(string name)
        {
            return attribute.Value.NamedArguments.Any(kvp => kvp.Key == name);
        }

        /// <summary>
        /// Attempts to get a named argument by name.
        /// Returns true and sets value if successful, false otherwise.
        /// </summary>
        public bool TryGetNamedArg<T>(string name, out T? value)
        {
            var arg = attribute.NamedArg<T>(name);
            if (arg.HasValue)
            {
                value = arg.OrThrow();
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets a named argument or returns a default value if not present.
        /// </summary>
        public T GetNamedArgOrDefault<T>(string name, T defaultValue = default!)
        {
            var arg = attribute.NamedArg<T>(name);
            return arg.HasValue ? arg.OrThrow() : defaultValue;
        }

        /// <summary>
        /// Gets all named arguments as a dictionary.
        /// </summary>
        public Dictionary<string, TypedConstant> GetAllNamedArgs()
        {
            return attribute.Value.NamedArguments.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Gets all constructor arguments as TypedConstants.
        /// </summary>
        public ImmutableArray<TypedConstant> GetAllConstructorArgs()
        {
            return attribute.Value.ConstructorArguments;
        }

        /// <summary>
        /// Gets the number of constructor arguments.
        /// </summary>
        public int GetConstructorArgCount()
        {
            return attribute.Value.ConstructorArguments.Length;
        }

        /// <summary>
        /// Gets the number of named arguments.
        /// </summary>
        public int GetNamedArgCount()
        {
            return attribute.Value.NamedArguments.Length;
        }

        /// <summary>
        /// Checks if this attribute has any constructor arguments.
        /// </summary>
        public bool HasAnyConstructorArgs()
        {
            return attribute.Value.ConstructorArguments.Length > 0;
        }

        /// <summary>
        /// Checks if this attribute has any named arguments.
        /// </summary>
        public bool HasAnyNamedArgs()
        {
            return attribute.Value.NamedArguments.Length > 0;
        }

        /// <summary>
        /// Gets the attribute constructor symbol.
        /// Returns Empty if constructor is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> AttributeConstructor =>
            attribute.Value.AttributeConstructor is { } constructor
                ? OptionalSymbol<IMethodSymbol>.WithValue(constructor)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the attribute class as a validated symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> GetAttributeClass()
        {
            return ValidSymbol<INamedTypeSymbol>.From(attribute.AttributeClass);
        }

        /// <summary>
        /// Checks if this attribute matches the specified attribute name (with or without "Attribute" suffix).
        /// </summary>
        public bool MatchesName(string name)
        {
            var attrName = attribute.AttributeClass.Name;

            // Direct match
            if (attrName == name) return true;

            // Match without "Attribute" suffix
            if (attrName.EndsWith("Attribute") &&
                attrName.Substring(0, attrName.Length - "Attribute".Length) == name)
                return true;

            // Match with "Attribute" suffix added
            if (!name.EndsWith("Attribute") && attrName == name + "Attribute")
                return true;

            return false;
        }

        /// <summary>
        /// Gets the fully qualified name of the attribute class.
        /// </summary>
        public string GetFullyQualifiedName()
        {
            return attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        /// <summary>
        /// Gets the namespace of the attribute class.
        /// Returns null if in global namespace.
        /// </summary>
        public string? GetNamespace()
        {
            var ns = attribute.AttributeClass.ContainingNamespace;
            return ns?.IsGlobalNamespace == true ? null : ns?.ToDisplayString();
        }
    }
}