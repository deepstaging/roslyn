// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for identifying async types (Task, ValueTask).
/// </summary>
public static class TypeSymbolAsyncExtensions
{
    /// <param name="typeSymbol">The type symbol to check.</param>
    extension(ITypeSymbol typeSymbol)
    {
        /// <summary>
        /// Checks if the type is Task or ValueTask (with or without type arguments).
        /// </summary>
        public bool IsTaskType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Task" or "ValueTask" };
        }

        /// <summary>
        /// Checks if the type is ValueTask or ValueTask&lt;T&gt;.
        /// </summary>
        public bool IsValueTaskType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "ValueTask" };
        }

        /// <summary>
        /// Checks if the type is Task&lt;T&gt;.
        /// </summary>
        public bool IsGenericTaskType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Task" or "ValueTask", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is ValueTask&lt;T&gt;.
        /// </summary>
        public bool IsGenericValueTaskType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "ValueTask", IsGenericType: true, TypeArguments.Length: 1 };
        }

        /// <summary>
        /// Checks if the type is Task (non-generic).
        /// </summary>
        public bool IsNonGenericTaskType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "Task" or "ValueTask", IsGenericType: false };
        }

        /// <summary>
        /// Checks if the type is ValueTask (non-generic).
        /// </summary>
        public bool IsNonGenericValueTaskType()
        {
            return typeSymbol is INamedTypeSymbol { Name: "ValueTask", IsGenericType: false };
        }
    }
}
