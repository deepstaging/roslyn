// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Fluent builder for composing type reference strings.
/// Supports generics, delegates, tuples, arrays, and nullability.
/// Immutable - each method returns a new instance.
/// Has implicit conversions to/from <see cref="string"/> for seamless integration with existing builders.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="TypeRef"/> is the foundational primitive. On top of it, a three-layer pattern
/// provides typed, composable wrappers for common .NET types:
/// </para>
/// <list type="number">
/// <item><b>TypeRef wrappers</b> (<c>TypeRefs/</c>) — <c>readonly record struct</c> types that carry constituent
/// generic arguments (e.g., <see cref="TaskTypeRef"/> carries <c>ResultType</c>).
/// Implicit conversions to <see cref="TypeRef"/> and <see cref="string"/> make them drop-in replacements.</item>
/// <item><b>Expression factories</b> (<c>Expressions/</c>) — static classes that produce <see cref="ExpressionRef"/>
/// for common code patterns (e.g., <see cref="EqualityComparerExpression.DefaultEquals"/>).</item>
/// <item><b>Builder extensions</b> (<c>Expressions/</c>) — extension methods on builders that wire
/// TypeRefs and Expressions into code generation workflows
/// (e.g., <see cref="PropertyBuilderNotifyingSetterExtensions.WithNotifyingSetter"/>).</item>
/// </list>
/// <para>
/// See also <see cref="AttributeRef"/> for type-safe attribute references that bridge to <see cref="Emit.AttributeBuilder"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Primitive composition
/// TypeRef.From("List").Of("string")                       // "List&lt;string&gt;"
/// TypeRef.Tuple(("string", "Name"), ("int", "Age"))       // "(string Name, int Age)"
///
/// // Typed wrappers carry generic arguments
/// var task = new TaskTypeRef("int");                       // global::System.Threading.Tasks.Task&lt;int&gt;
/// task.ResultType                                          // "int" — constituent type preserved
///
/// // Expression factories produce code snippets
/// EqualityComparerExpression.DefaultEquals("string", "_name", "value")
///     // "global::System.Collections.Generic.EqualityComparer&lt;string&gt;.Default.Equals(_name, value)"
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
    /// <remarks>Semantic alias for <see cref="From(string)"/> that reads naturally when <c>using static TypeRef</c> is in scope.</remarks>
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

    // ── Expression Gateways ────────────────────────────────────────────
    // These methods cross from the type domain into the expression domain.
    // All return ExpressionRef — the one-way gate.

    /// <summary>Produces a constructor call expression: <c>new Type(args)</c>.</summary>
    /// <param name="arguments">The constructor arguments.</param>
    public ExpressionRef New(params ExpressionRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return ExpressionRef.From($"new {Value}({args})");
    }

    /// <summary>Produces a static method call expression: <c>Type.method(args)</c>.</summary>
    /// <param name="method">The method name.</param>
    /// <param name="arguments">The method arguments.</param>
    public ExpressionRef Call(string method, params ExpressionRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return ExpressionRef.From($"{Value}.{method}({args})");
    }

    /// <summary>Produces a member access expression: <c>Type.member</c>.</summary>
    /// <param name="name">The member name (property, field, constant, or nested type).</param>
    public ExpressionRef Member(string name) => ExpressionRef.From($"{Value}.{name}");

    /// <summary>Produces a <c>typeof(Type)</c> expression.</summary>
    public ExpressionRef TypeOf() => ExpressionRef.From($"typeof({Value})");

    /// <summary>Produces a <c>default(Type)</c> expression.</summary>
    public ExpressionRef Default() => ExpressionRef.From($"default({Value})");

    /// <summary>Produces a <c>nameof(Type)</c> expression.</summary>
    public ExpressionRef NameOf() => ExpressionRef.From($"nameof({Value})");

    /// <summary>Produces a null-conditional delegate invocation expression: <c>value?.Invoke(args)</c>.</summary>
    /// <param name="arguments">The arguments to pass to the delegate.</param>
    public ExpressionRef Invoke(params ExpressionRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return ExpressionRef.From($"{Value}?.Invoke({args})");
    }

    /// <summary>Produces a safe cast expression: <c>value as Type</c>.</summary>
    /// <param name="target">The target type to cast to.</param>
    public ExpressionRef As(TypeRef target) => ExpressionRef.From($"{Value} as {target.Value}");

    /// <summary>Produces a direct cast expression: <c>(Type)value</c>.</summary>
    /// <param name="target">The target type to cast to.</param>
    public ExpressionRef Cast(TypeRef target) => ExpressionRef.From($"({target.Value}){Value}");

    /// <summary>Appends a null-coalescing fallback: <c>value ?? fallback</c>.</summary>
    /// <param name="fallback">The fallback expression when the value is null.</param>
    public ExpressionRef OrDefault(ExpressionRef fallback) => ExpressionRef.From($"{Value} ?? {fallback.Value}");

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