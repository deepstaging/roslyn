// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Represents the kind of async method.
/// </summary>
public enum AsyncMethodKind
{
    /// <summary>
    /// Not an async method (doesn't return Task or ValueTask).
    /// </summary>
    NotAsync,

    /// <summary>
    /// Async void method (returns Task or ValueTask with no type arguments).
    /// </summary>
    Void,

    /// <summary>
    /// Async value method (returns Task&lt;T&gt; or ValueTask&lt;T&gt;).
    /// </summary>
    Value
}

/// <summary>
/// Extension methods for IMethodSymbol to query parameters and check async characteristics.
/// </summary>
public static class MethodSymbolQueryExtensions
{
    extension(IMethodSymbol methodSymbol)
    {
        /// <summary>
        /// Creates a ParameterQuery to search method parameters.
        /// </summary>
        public ParameterQuery QueryParameters()
        {
            return ParameterQuery.From(methodSymbol);
        }

        /// <summary>
        /// Gets the async method kind (NotAsync, Void, or Value).
        /// </summary>
        public AsyncMethodKind GetAsyncKind()
        {
            if (methodSymbol.ReturnType is not INamedTypeSymbol { Name: "Task" or "ValueTask" } namedType)
                return AsyncMethodKind.NotAsync;

            return namedType.TypeArguments.Length == 0
                ? AsyncMethodKind.Void
                : AsyncMethodKind.Value;
        }

        /// <summary>
        /// Checks if a method is async (returns Task or ValueTask).
        /// </summary>
        public bool IsAsync()
        {
            return methodSymbol.GetAsyncKind() != AsyncMethodKind.NotAsync;
        }

        /// <summary>
        /// Checks if a method is async void (returns Task with no type arguments).
        /// </summary>
        public bool IsAsyncVoid()
        {
            return methodSymbol.GetAsyncKind() == AsyncMethodKind.Void;
        }

        /// <summary>
        /// Checks if a method is async value (returns Task&lt;T&gt; or ValueTask&lt;T&gt;).
        /// </summary>
        public bool IsAsyncValue()
        {
            return methodSymbol.GetAsyncKind() == AsyncMethodKind.Value;
        }

        /// <summary>
        /// Gets the return type from an async method (extracts T from Task&lt;T&gt; or ValueTask&lt;T&gt;).
        /// </summary>
        /// <returns>The type argument T, or null if not an async method with a return type.</returns>
        public ITypeSymbol? GetAsyncReturnType()
        {
            if (methodSymbol.ReturnType is not INamedTypeSymbol { Name: "Task" or "ValueTask" } namedType)
                return null;

            return namedType.TypeArguments.Length == 1
                ? namedType.TypeArguments[0]
                : null;
        }

        /// <summary>
        /// Checks if the method is an extension method.
        /// </summary>
        public bool IsExtensionMethod()
        {
            return methodSymbol.IsExtensionMethod;
        }

        /// <summary>
        /// Checks if the method is a generic method.
        /// </summary>
        public bool IsGenericMethod()
        {
            return methodSymbol.IsGenericMethod;
        }

        /// <summary>
        /// Checks if the method is an operator overload.
        /// </summary>
        public bool IsOperator()
        {
            return methodSymbol.MethodKind == MethodKind.UserDefinedOperator ||
                   methodSymbol.MethodKind == MethodKind.Conversion;
        }

        /// <summary>
        /// Checks if the method is a constructor.
        /// </summary>
        public bool IsConstructor()
        {
            return methodSymbol.MethodKind == MethodKind.Constructor;
        }

        /// <summary>
        /// Checks if the method is a destructor/finalizer.
        /// </summary>
        public bool IsDestructor()
        {
            return methodSymbol.MethodKind == MethodKind.Destructor;
        }

        /// <summary>
        /// Checks if the method is a property accessor (getter or setter).
        /// </summary>
        public bool IsPropertyAccessor()
        {
            return methodSymbol.MethodKind == MethodKind.PropertyGet ||
                   methodSymbol.MethodKind == MethodKind.PropertySet;
        }

        /// <summary>
        /// Checks if the method is an event accessor (add or remove).
        /// </summary>
        public bool IsEventAccessor()
        {
            return methodSymbol.MethodKind == MethodKind.EventAdd ||
                   methodSymbol.MethodKind == MethodKind.EventRemove;
        }

        /// <summary>
        /// Checks if the method returns void.
        /// </summary>
        public bool ReturnsVoid()
        {
            return methodSymbol.ReturnsVoid;
        }

        /// <summary>
        /// Checks if the method is partial (declaration or implementation).
        /// </summary>
        public bool IsPartialMethod()
        {
            return methodSymbol.PartialDefinitionPart != null ||
                   methodSymbol.PartialImplementationPart != null;
        }

        /// <summary>
        /// Checks if the method has parameters.
        /// </summary>
        public bool HasParameters()
        {
            return methodSymbol.Parameters.Length > 0;
        }

        /// <summary>
        /// Checks if the method has ref or out parameters.
        /// </summary>
        public bool HasRefOrOutParameters()
        {
            return methodSymbol.Parameters.Any(p => p.RefKind is RefKind.Ref or RefKind.Out);
        }
    }
}