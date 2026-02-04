// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for OptionalSymbol&lt;IMethodSymbol&gt; exposing IMethodSymbol-specific functionality.
/// Note: Most properties (IsVirtual, IsOverride, IsAsync, etc.) are accessible via .Symbol
/// These extensions focus on complex operations and type conversions.
/// </summary>
public static class ProjectedMethodSymbolExtensions
{
    extension(OptionalSymbol<IMethodSymbol> method)
    {
        /// <summary>
        /// Gets the return type of the method as a projected symbol.
        /// Returns Empty if method symbol is not present, otherwise returns the return type (which is always present for valid methods, including void).
        /// </summary>
        public OptionalSymbol<ITypeSymbol> ReturnType =>
            method.HasValue
                ? OptionalSymbol<ITypeSymbol>.WithValue(method.Symbol!.ReturnType)
                : OptionalSymbol<ITypeSymbol>.Empty();

        /// <summary>
        /// Gets the async method kind (NotAsync, Void, or Symbol).
        /// </summary>
        public AsyncMethodKind AsyncKind =>
            method.HasValue
                ? method.Symbol!.GetAsyncKind()
                : AsyncMethodKind.NotAsync;

        /// <summary>
        /// Checks if method is async void (returns Task with no type arguments).
        /// </summary>
        public bool IsAsyncVoid()
        {
            return method.HasValue && method.Symbol!.IsAsyncVoid();
        }

        /// <summary>
        /// Checks if method is async value (returns Task&lt;T&gt; or ValueTask&lt;T&gt;).
        /// </summary>
        public bool IsAsyncValue()
        {
            return method.HasValue && method.Symbol!.IsAsyncValue();
        }

        /// <summary>
        /// Checks if the method is async (returns Task or ValueTask).
        /// </summary>
        public bool IsAsync()
        {
            return method.HasValue && (method.Symbol!.IsAsyncValue() || method.Symbol!.IsAsyncVoid());
        }

        /// <summary>
        /// Gets the return type from an async method (extracts T from Task&lt;T&gt; or ValueTask&lt;T&gt;).
        /// Returns Empty if not an async method with a return type.
        /// </summary>
        public OptionalSymbol<ITypeSymbol> AsyncReturnType =>
            method.HasValue && method.Symbol!.GetAsyncReturnType() is { } returnType
                ? OptionalSymbol<ITypeSymbol>.WithValue(returnType)
                : OptionalSymbol<ITypeSymbol>.Empty();

        /// <summary>
        /// Gets the overridden method, if this is an override.
        /// Returns Empty if not an override or overridden method is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> OverriddenMethod =>
            method is { HasValue: true, Symbol.OverriddenMethod: not null }
                ? OptionalSymbol<IMethodSymbol>.WithValue(method.Symbol!.OverriddenMethod)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the partial implementation part of a partial method.
        /// Returns Empty if not a partial method or implementation part is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> PartialImplementation =>
            method is { HasValue: true, Symbol.PartialImplementationPart: not null }
                ? OptionalSymbol<IMethodSymbol>.WithValue(method.Symbol!.PartialImplementationPart)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the partial definition part of a partial method.
        /// Returns Empty if not a partial method or definition part is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> PartialDefinition =>
            method is { HasValue: true, Symbol.PartialDefinitionPart: not null }
                ? OptionalSymbol<IMethodSymbol>.WithValue(method.Symbol!.PartialDefinitionPart)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the associated property or event for accessor methods.
        /// Returns Empty if not an accessor or has no associated symbol.
        /// </summary>
        public OptionalSymbol<ISymbol> AssociatedSymbol =>
            method is { HasValue: true, Symbol.AssociatedSymbol: not null }
                ? OptionalSymbol<ISymbol>.WithValue(method.Symbol!.AssociatedSymbol)
                : OptionalSymbol<ISymbol>.Empty();

        /// <summary>
        /// Gets the parameters of the method as valid symbols (empty array if method symbol is not present).
        /// Parameters are always valid when the method exists, so they're returned as ValidSymbol.
        /// </summary>
        public ImmutableArray<ValidSymbol<IParameterSymbol>> Parameters => 
            method.HasValue 
                ? method.Symbol!.Parameters.Select(p => ValidSymbol<IParameterSymbol>.From(p)).ToImmutableArray()
                : ImmutableArray<ValidSymbol<IParameterSymbol>>.Empty;

        /// <summary>
        /// Gets whether the method returns void (false if method symbol is not present).
        /// </summary>
        public bool ReturnsVoid => method.HasValue && method.Symbol!.ReturnsVoid;

        /// <summary>
        /// Gets the method kind (returns default if method symbol is not present).
        /// </summary>
        public MethodKind MethodKind => method.HasValue ? method.Symbol!.MethodKind : default;
    }
}