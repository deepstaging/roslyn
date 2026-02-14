// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for composing type reference strings.
/// Supports generics, delegates, tuples, arrays, and nullability.
/// Immutable - each method returns a new instance.
/// Has implicit conversions to/from <see cref="string"/> for seamless integration with existing builders.
/// </summary>
/// <example>
/// <code>
/// TypeRef.From("List").Of("string")                    // "List&lt;string&gt;"
/// TypeRef.Func("int", "string").Nullable()             // "Func&lt;int, string&gt;?"
/// TypeRef.Tuple(("string", "Name"), ("int", "Age"))    // "(string Name, int Age)"
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
            throw new ArgumentException("Fully qualified name cannot be null or whitespace.", nameof(fullyQualifiedName));

        return new TypeRef($"global::{fullyQualifiedName}");
    }

    // ── Collections (System.Collections.Generic) ──────────────────────────

    /// <summary>Creates a <c>List&lt;T&gt;</c> type reference.</summary>
    public static TypeRef List(TypeRef elementType) =>
        new($"global::System.Collections.Generic.List<{elementType.Value}>");

    /// <summary>Creates a <c>Dictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static TypeRef Dictionary(TypeRef keyType, TypeRef valueType) =>
        new($"global::System.Collections.Generic.Dictionary<{keyType.Value}, {valueType.Value}>");

    /// <summary>Creates a <c>HashSet&lt;T&gt;</c> type reference.</summary>
    public static TypeRef HashSet(TypeRef elementType) =>
        new($"global::System.Collections.Generic.HashSet<{elementType.Value}>");

    /// <summary>Creates a <c>KeyValuePair&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static TypeRef KeyValuePair(TypeRef keyType, TypeRef valueType) =>
        new($"global::System.Collections.Generic.KeyValuePair<{keyType.Value}, {valueType.Value}>");

    /// <summary>Creates an <c>IEnumerable&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IEnumerable(TypeRef elementType) =>
        new($"global::System.Collections.Generic.IEnumerable<{elementType.Value}>");

    /// <summary>Creates an <c>ICollection&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ICollection(TypeRef elementType) =>
        new($"global::System.Collections.Generic.ICollection<{elementType.Value}>");

    /// <summary>Creates an <c>IList&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IList(TypeRef elementType) =>
        new($"global::System.Collections.Generic.IList<{elementType.Value}>");

    /// <summary>Creates an <c>IDictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static TypeRef IDictionary(TypeRef keyType, TypeRef valueType) =>
        new($"global::System.Collections.Generic.IDictionary<{keyType.Value}, {valueType.Value}>");

    /// <summary>Creates an <c>ISet&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ISet(TypeRef elementType) =>
        new($"global::System.Collections.Generic.ISet<{elementType.Value}>");

    /// <summary>Creates an <c>IReadOnlyList&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IReadOnlyList(TypeRef elementType) =>
        new($"global::System.Collections.Generic.IReadOnlyList<{elementType.Value}>");

    /// <summary>Creates an <c>IReadOnlyCollection&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IReadOnlyCollection(TypeRef elementType) =>
        new($"global::System.Collections.Generic.IReadOnlyCollection<{elementType.Value}>");

    /// <summary>Creates an <c>IReadOnlyDictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static TypeRef IReadOnlyDictionary(TypeRef keyType, TypeRef valueType) =>
        new($"global::System.Collections.Generic.IReadOnlyDictionary<{keyType.Value}, {valueType.Value}>");

    // ── Immutable Collections (System.Collections.Immutable) ────────────

    /// <summary>Creates an <c>ImmutableArray&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ImmutableArray(TypeRef elementType) =>
        new($"global::System.Collections.Immutable.ImmutableArray<{elementType.Value}>");

    /// <summary>Creates an <c>ImmutableList&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ImmutableList(TypeRef elementType) =>
        new($"global::System.Collections.Immutable.ImmutableList<{elementType.Value}>");

    /// <summary>Creates an <c>ImmutableDictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
    public static TypeRef ImmutableDictionary(TypeRef keyType, TypeRef valueType) =>
        new($"global::System.Collections.Immutable.ImmutableDictionary<{keyType.Value}, {valueType.Value}>");

    // ── Tasks (System.Threading.Tasks) ──────────────────────────────────

    /// <summary>Creates a <c>Task</c> type reference (non-generic).</summary>
    public static TypeRef Task() => new("global::System.Threading.Tasks.Task");

    /// <summary>Creates a <c>Task&lt;T&gt;</c> type reference.</summary>
    public static TypeRef Task(TypeRef resultType) =>
        new($"global::System.Threading.Tasks.Task<{resultType.Value}>");

    /// <summary>Creates a <c>ValueTask</c> type reference (non-generic).</summary>
    public static TypeRef ValueTask() => new("global::System.Threading.Tasks.ValueTask");

    /// <summary>Creates a <c>ValueTask&lt;T&gt;</c> type reference.</summary>
    public static TypeRef ValueTask(TypeRef resultType) =>
        new($"global::System.Threading.Tasks.ValueTask<{resultType.Value}>");

    /// <summary>Gets a <c>Task.CompletedTask</c> expression for use in return statements.</summary>
    public static TypeRef CompletedTask => new("global::System.Threading.Tasks.Task.CompletedTask");

    /// <summary>Gets a <c>ValueTask.CompletedTask</c> expression for use in return statements.</summary>
    public static TypeRef CompletedValueTask => new("global::System.Threading.Tasks.ValueTask.CompletedTask");

    // ── Other Common Types ──────────────────────────────────────────────

    /// <summary>Gets a <c>CancellationToken</c> type reference.</summary>
    public static TypeRef CancellationToken => new("global::System.Threading.CancellationToken");

    // ── LINQ (System.Linq) ──────────────────────────────────────────────

    /// <summary>Creates an <c>IQueryable&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IQueryable(TypeRef elementType) =>
        new($"global::System.Linq.IQueryable<{elementType.Value}>");

    /// <summary>Creates an <c>IOrderedQueryable&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IOrderedQueryable(TypeRef elementType) =>
        new($"global::System.Linq.IOrderedQueryable<{elementType.Value}>");

    // ── LINQ Expressions (System.Linq.Expressions) ──────────────────────

    /// <summary>Creates an <c>Expression&lt;TDelegate&gt;</c> type reference.</summary>
    public static TypeRef Expression(TypeRef delegateType) =>
        new($"global::System.Linq.Expressions.Expression<{delegateType.Value}>");

    // ── Delegates ───────────────────────────────────────────────────────

    /// <summary>Creates a <c>Func&lt;...&gt;</c> delegate type reference. The last type argument is the return type.</summary>
    /// <param name="typeArguments">The type arguments. Must contain at least one (the return type).</param>
    public static TypeRef Func(params TypeRef[] typeArguments)
    {
        if (typeArguments.Length == 0)
            throw new ArgumentException("Func requires at least one type argument (the return type).", nameof(typeArguments));

        var args = string.Join(", ", typeArguments.Select(t => t.Value));
        return new TypeRef($"global::System.Func<{args}>");
    }

    /// <summary>Creates an <c>Action</c> or <c>Action&lt;...&gt;</c> delegate type reference.</summary>
    /// <param name="typeArguments">The type arguments. If empty, produces <c>Action</c>.</param>
    public static TypeRef Action(params TypeRef[] typeArguments)
    {
        if (typeArguments.Length == 0)
            return new TypeRef("global::System.Action");

        var args = string.Join(", ", typeArguments.Select(t => t.Value));
        return new TypeRef($"global::System.Action<{args}>");
    }

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
    /// <example>
    /// <code>
    /// TypeRef.From("OnSave").Invoke("id", "name")   // "OnSave?.Invoke(id, name)"
    /// </code>
    /// </example>
    public TypeRef Invoke(params TypeRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return new($"{Value}?.Invoke({args})");
    }

    /// <summary>Appends a null-coalescing fallback: <c>value ?? fallback</c>.</summary>
    /// <param name="fallback">The fallback expression when the value is null.</param>
    /// <example>
    /// <code>
    /// TypeRef.From("OnSave").Invoke("id").OrDefault(TypeRef.CompletedTask)
    /// // "OnSave?.Invoke(id) ?? global::System.Threading.Tasks.Task.CompletedTask"
    /// </code>
    /// </example>
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
        return new($"{Value}[{commas}]");
    }

    /// <summary>Returns the string representation of this type reference.</summary>
    public override string ToString() => Value;

    /// <summary>Implicitly converts a <see cref="TypeRef"/> to a <see cref="string"/>.</summary>
    public static implicit operator string(TypeRef typeRef) => typeRef.Value;

    /// <summary>Implicitly converts a <see cref="string"/> to a <see cref="TypeRef"/>.</summary>
    public static implicit operator TypeRef(string typeName) => From(typeName);
}
