// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Fluent builder for composing type reference strings.
/// Supports generics, delegates, tuples, arrays, and nullability.
/// Immutable - each method returns a new instance.
/// Has implicit conversions to/from <see cref="string"/> for seamless integration with existing builders.
/// </summary>
/// <remarks>
/// Namespace-specific factories are organized into standalone static classes:
/// <list type="bullet">
/// <item><see cref="CollectionRefs"/> — <c>System.Collections.Generic</c></item>
/// <item><see cref="ImmutableCollectionRefs"/> — <c>System.Collections.Immutable</c></item>
/// <item><see cref="TaskRefs"/> — <c>System.Threading.Tasks</c> and <c>System.Threading</c></item>
/// <item><see cref="HttpRefs"/> — <c>System.Net.Http</c></item>
/// <item><see cref="ConfigurationRefs"/> — <c>Microsoft.Extensions.Configuration</c></item>
/// <item><see cref="DependencyInjectionRefs"/> — <c>Microsoft.Extensions.DependencyInjection</c></item>
/// <item><see cref="LoggingRefs"/> — <c>Microsoft.Extensions.Logging</c></item>
/// <item><see cref="LinqRefs"/> — <c>System.Linq</c> and <c>System.Linq.Expressions</c></item>
/// <item><see cref="DelegateRefs"/> — <c>System.Func</c> and <c>System.Action</c></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// TypeRef.From("List").Of("string")                       // "List&lt;string&gt;"
/// DelegateRefs.Func("int", "string").Nullable()           // "Func&lt;int, string&gt;?"
/// TypeRef.Tuple(("string", "Name"), ("int", "Age"))       // "(string Name, int Age)"
/// TaskRefs.Task(CollectionRefs.List("string"))             // "Task&lt;List&lt;string&gt;&gt;"
/// </code>
/// </example>
public readonly record struct TypeRef
{
    /// <summary>Gets the string representation of the type reference.</summary>
    public string Value { get; }

    private TypeRef(string value) => Value = value;

    /// <summary>Creates a type reference from a type name.</summary>
    /// <param name="typeName">The type name (e.g., "string", "List", "MyClass").</param>
    public static TypeRef From(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            throw new ArgumentException("Type name cannot be null or whitespace.", nameof(typeName));

        return new TypeRef(typeName);
    }

    /// <summary>
    ///  Creates a type reference from a type name string.
    /// </summary>
    /// <param name="typeName">The type name as a string (e.g., "string", "List", "MyClass").</param>
    /// <returns>A TypeRef instance representing the provided type name.</returns>
    public static TypeRef CreateTypeRef(string typeName) => From(typeName);

    /// <summary>
    /// Creates a type reference from a validated symbol representing a type.
    /// </summary>
    /// <param name="symbol">The validated symbol of the type. Must be a class, struct, interface, enum, or delegate.</param>
    /// <typeparam name="T">The type of the symbol, constrained to ITypeSymbol.</typeparam>
    /// <returns>A TypeRef representing the globally qualified name of the type.</returns>
    public static TypeRef From<T>(ValidSymbol<T> symbol) where T : class, ITypeSymbol =>
        From(symbol.GloballyQualifiedName);

    /// <summary>
    /// Creates a type reference from a <see cref="SymbolSnapshot"/>.
    /// Uses the globally qualified name from the snapshot.
    /// </summary>
    /// <param name="snapshot">The symbol snapshot.</param>
    public static TypeRef From(SymbolSnapshot snapshot) =>
        From(snapshot?.GloballyQualifiedName ?? throw new ArgumentNullException(nameof(snapshot)));

    /// <summary>Creates a globally qualified type reference with <c>global::</c> prefix.</summary>
    /// <param name="fullyQualifiedName">The fully qualified type name without the global:: prefix (e.g., "System.String").</param>
    public static TypeRef Global(string fullyQualifiedName)
    {
        if (string.IsNullOrWhiteSpace(fullyQualifiedName))
            throw new ArgumentException("Fully qualified name cannot be null or whitespace.",
                nameof(fullyQualifiedName));

        return new TypeRef($"global::{fullyQualifiedName}");
    }

    // ── Tuples ──────────────────────────────────────────────────────────

    /// <summary>Creates a tuple type reference with named elements.</summary>
    /// <param name="elements">The tuple elements as (type, name) pairs.</param>
    public static TypeRef Tuple(params (TypeRef Type, string Name)[] elements)
    {
        if (elements.Length < 2)
            throw new ArgumentException("Tuples require at least two elements.", nameof(elements));

        var parts = string.Join(", ", elements.Select(e => $"{e.Type.Value} {e.Name}"));
        return new TypeRef($"({parts})");
    }

    /// <summary>Adds generic type arguments to this type reference.</summary>
    /// <param name="typeArguments">The type arguments.</param>
    public TypeRef Of(params TypeRef[] typeArguments)
    {
        if (typeArguments.Length == 0)
            throw new ArgumentException("At least one type argument is required.", nameof(typeArguments));

        var args = string.Join(", ", typeArguments.Select(t => t.Value));
        return new TypeRef($"{Value}<{args}>");
    }

    // ── Expressions ────────────────────────────────────────────────────

    /// <summary>Produces a null-conditional delegate invocation expression: <c>value?.Invoke(args)</c>.</summary>
    /// <param name="arguments">The arguments to pass to the delegate.</param>
    public TypeRef Invoke(params TypeRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return new TypeRef($"{Value}?.Invoke({args})");
    }

    /// <summary>Produces a safe cast expression: <c>value as Type</c>.</summary>
    /// <param name="target">The target type to cast to.</param>
    public TypeRef As(TypeRef target) => new($"{Value} as {target.Value}");

    /// <summary>Produces a direct cast expression: <c>(Type)value</c>.</summary>
    /// <param name="target">The target type to cast to.</param>
    public TypeRef Cast(TypeRef target) => new($"({target.Value}){Value}");

    /// <summary>Appends a null-coalescing fallback: <c>value ?? fallback</c>.</summary>
    /// <param name="fallback">The fallback expression when the value is null.</param>
    public TypeRef OrDefault(TypeRef fallback) => new($"{Value} ?? {fallback.Value}");

    // ── Modifiers ───────────────────────────────────────────────────────

    /// <summary>Makes this type reference nullable by appending <c>?</c>.</summary>
    public TypeRef Nullable() => new($"{Value}?");

    /// <summary>Makes this type reference an array by appending <c>[]</c>.</summary>
    public TypeRef Array() => new($"{Value}[]");

    /// <summary>Makes this type reference a multi-dimensional array.</summary>
    /// <param name="rank">The number of dimensions (must be at least 1).</param>
    public TypeRef Array(int rank)
    {
        if (rank < 1)
            throw new ArgumentOutOfRangeException(nameof(rank), "Array rank must be at least 1.");

        if (rank == 1)
            return Array();

        var commas = new string(',', rank - 1);
        return new TypeRef($"{Value}[{commas}]");
    }

    /// <summary>Returns the string representation of this type reference.</summary>
    public override string ToString() => Value;

    /// <summary>Implicitly converts a <see cref="TypeRef"/> to a <see cref="string"/>.</summary>
    public static implicit operator string(TypeRef typeRef) => typeRef.Value;

    /// <summary>Implicitly converts a <see cref="string"/> to a <see cref="TypeRef"/>.</summary>
    public static implicit operator TypeRef(string typeName) => From(typeName);
}
