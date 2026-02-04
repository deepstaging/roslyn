// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ValidSymbol&lt;IMethodSymbol&gt; exposing IMethodSymbol-specific functionality.
/// </summary>
public static class ValidMethodSymbolExtensions
{
    extension(ValidSymbol<IMethodSymbol> method)
    {
        /// <summary>
        /// Gets the return type of the method as a validated projected symbol.
        /// Methods always have a return type (void is represented as a type), so this returns a non-nullable ValidSymbol.
        /// </summary>
        public ValidSymbol<ITypeSymbol> ReturnType =>
            ValidSymbol<ITypeSymbol>.From(method.Value.ReturnType);

        /// <summary>
        /// Gets the async method kind (NotAsync, Void, or Symbol).
        /// </summary>
        public AsyncMethodKind AsyncKind => method.Value.GetAsyncKind();

        /// <summary>
        /// Checks if method is async void (returns Task with no type arguments).
        /// </summary>
        public bool IsAsyncVoid()
        {
            return method.Value.IsAsyncVoid();
        }

        /// <summary>
        /// Checks if method is async value (returns Task&lt;T&gt; or ValueTask&lt;T&gt;).
        /// </summary>
        public bool IsAsyncValue()
        {
            return method.Value.IsAsyncValue();
        }

        /// <summary>
        /// Checks if method is async (either void or value).
        /// </summary>
        public bool IsAsync()
        {
            return method.Value.IsAsyncValue() || method.Value.IsAsyncVoid();
        }

        /// <summary>
        /// Gets the return type from an async method (extracts T from Task&lt;T&gt; or ValueTask&lt;T&gt;).
        /// Returns Empty if not an async method with a return type.
        /// </summary>
        public OptionalSymbol<ITypeSymbol> AsyncReturnType =>
            method.Value.GetAsyncReturnType() is { } returnType
                ? OptionalSymbol<ITypeSymbol>.WithValue(returnType)
                : OptionalSymbol<ITypeSymbol>.Empty();

        /// <summary>
        /// Gets the overridden method, if this is an override.
        /// Returns Empty if not an override or overridden method is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> OverriddenMethod =>
            method.Value.OverriddenMethod is { } overridden
                ? OptionalSymbol<IMethodSymbol>.WithValue(overridden)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the partial implementation part of a partial method.
        /// Returns Empty if not a partial method or implementation part is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> PartialImplementation =>
            method.Value.PartialImplementationPart is { } impl
                ? OptionalSymbol<IMethodSymbol>.WithValue(impl)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the partial definition part of a partial method.
        /// Returns Empty if not a partial method or definition part is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> PartialDefinition =>
            method.Value.PartialDefinitionPart is { } def
                ? OptionalSymbol<IMethodSymbol>.WithValue(def)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the associated property or event for accessor methods.
        /// Returns Empty if not an accessor or has no associated symbol.
        /// </summary>
        public OptionalSymbol<ISymbol> AssociatedSymbol =>
            method.Value.AssociatedSymbol is { } associated
                ? OptionalSymbol<ISymbol>.WithValue(associated)
                : OptionalSymbol<ISymbol>.Empty();

        /// <summary>
        /// Checks if this is a public method.
        /// </summary>
        public bool IsPublicMethod()
        {
            return method.Value.DeclaredAccessibility == Accessibility.Public;
        }

        /// <summary>
        /// Checks if this is an internal method.
        /// </summary>
        public bool IsInternalMethod()
        {
            return method.Value.DeclaredAccessibility == Accessibility.Internal;
        }

        /// <summary>
        /// Checks if this is a private method.
        /// </summary>
        public bool IsPrivateMethod()
        {
            return method.Value.DeclaredAccessibility == Accessibility.Private;
        }

        /// <summary>
        /// Checks if this is a protected method.
        /// </summary>
        public bool IsProtectedMethod()
        {
            return method.Value.DeclaredAccessibility == Accessibility.Protected;
        }

        /// <summary>
        /// Checks if this is a virtual method.
        /// </summary>
        public bool IsVirtualMethod()
        {
            return method.Value.IsVirtual;
        }

        /// <summary>
        /// Checks if this is an abstract method.
        /// </summary>
        public bool IsAbstractMethod()
        {
            return method.Value.IsAbstract;
        }

        /// <summary>
        /// Checks if this is an override method.
        /// </summary>
        public bool IsOverrideMethod()
        {
            return method.Value.IsOverride;
        }

        /// <summary>
        /// Checks if this is a sealed method.
        /// </summary>
        public bool IsSealedMethod()
        {
            return method.Value.IsSealed;
        }

        /// <summary>
        /// Checks if this is a static method.
        /// </summary>
        public bool IsStaticMethod()
        {
            return method.Value.IsStatic;
        }

        /// <summary>
        /// Checks if this is an extension method.
        /// </summary>
        public bool IsExtensionMethod()
        {
            return method.Value.IsExtensionMethod;
        }

        /// <summary>
        /// Checks if this is a generic method.
        /// </summary>
        public bool IsGenericMethod()
        {
            return method.Value.IsGenericMethod;
        }

        /// <summary>
        /// Checks if this is a partial method.
        /// </summary>
        public bool IsPartialMethod()
        {
            return method.Value.PartialDefinitionPart != null || method.Value.PartialImplementationPart != null;
        }

        /// <summary>
        /// Gets the parameters of the method as validated symbols.
        /// </summary>
        public ImmutableArray<ValidSymbol<IParameterSymbol>> Parameters => 
            method.Value.Parameters.Select(p => ValidSymbol<IParameterSymbol>.From(p)).ToImmutableArray();

        /// <summary>
        /// Gets whether the method returns void.
        /// </summary>
        public bool ReturnsVoid => method.Value.ReturnsVoid;

        /// <summary>
        /// Gets the method kind.
        /// </summary>
        public MethodKind MethodKind => method.Value.MethodKind;
    }
}