// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for composing type reference strings.
/// Supports generics, delegates, tuples, arrays, and nullability.
/// Immutable - each method returns a new instance.
/// Has implicit conversions to/from <see cref="string"/> for seamless integration with existing builders.
/// </summary>
/// <remarks>
/// Namespace-specific factories are organized into nested static classes:
/// <list type="bullet">
/// <item><see cref="Collections"/> — <c>System.Collections.Generic</c></item>
/// <item><see cref="Immutable"/> — <c>System.Collections.Immutable</c></item>
/// <item><see cref="Tasks"/> — <c>System.Threading.Tasks</c> and <c>System.Threading</c></item>
/// <item><see cref="Http"/> — <c>System.Net.Http</c></item>
/// <item><see cref="DependencyInjection"/> — <c>Microsoft.Extensions.DependencyInjection</c></item>
/// <item><see cref="Logging"/> — <c>Microsoft.Extensions.Logging</c></item>
/// <item><see cref="Linq"/> — <c>System.Linq</c> and <c>System.Linq.Expressions</c></item>
/// <item><see cref="Delegates"/> — <c>System.Func</c> and <c>System.Action</c></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// TypeRef.From("List").Of("string")                       // "List&lt;string&gt;"
/// TypeRef.Delegates.Func("int", "string").Nullable()      // "Func&lt;int, string&gt;?"
/// TypeRef.Tuple(("string", "Name"), ("int", "Age"))       // "(string Name, int Age)"
/// TypeRef.Tasks.Task(TypeRef.Collections.List("string"))  // "Task&lt;List&lt;string&gt;&gt;"
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

    // ── Exceptions (System) ────────────────────────────────────────────

    /// <summary>
    /// Factory members for common <c>System</c> exception types.
    /// </summary>
    public static class Exceptions
    {
        /// <summary>Gets an <c>ArgumentNullException</c> type reference.</summary>
        public static TypeRef ArgumentNull => new("global::System.ArgumentNullException");

        /// <summary>Gets an <c>ArgumentException</c> type reference.</summary>
        public static TypeRef Argument => new("global::System.ArgumentException");

        /// <summary>Gets an <c>ArgumentOutOfRangeException</c> type reference.</summary>
        public static TypeRef ArgumentOutOfRange => new("global::System.ArgumentOutOfRangeException");

        /// <summary>Gets an <c>InvalidOperationException</c> type reference.</summary>
        public static TypeRef InvalidOperation => new("global::System.InvalidOperationException");

        /// <summary>Gets an <c>InvalidCastException</c> type reference.</summary>
        public static TypeRef InvalidCast => new("global::System.InvalidCastException");

        /// <summary>Gets a <c>FormatException</c> type reference.</summary>
        public static TypeRef Format => new("global::System.FormatException");

        /// <summary>Gets a <c>NotSupportedException</c> type reference.</summary>
        public static TypeRef NotSupported => new("global::System.NotSupportedException");

        /// <summary>Gets a <c>NotImplementedException</c> type reference.</summary>
        public static TypeRef NotImplemented => new("global::System.NotImplementedException");
    }

    // ── Collections (System.Collections.Generic) ──────────────────────────

    /// <summary>
    /// Factory methods for <c>System.Collections.Generic</c> types.
    /// </summary>
    public static class Collections
    {
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
    }

    // ── Immutable Collections (System.Collections.Immutable) ────────────

    /// <summary>
    /// Factory methods for <c>System.Collections.Immutable</c> types.
    /// </summary>
    public static class Immutable
    {
        /// <summary>Creates an <c>ImmutableArray&lt;T&gt;</c> type reference.</summary>
        public static TypeRef ImmutableArray(TypeRef elementType) =>
            new($"global::System.Collections.Immutable.ImmutableArray<{elementType.Value}>");

        /// <summary>Creates an <c>ImmutableList&lt;T&gt;</c> type reference.</summary>
        public static TypeRef ImmutableList(TypeRef elementType) =>
            new($"global::System.Collections.Immutable.ImmutableList<{elementType.Value}>");

        /// <summary>Creates an <c>ImmutableDictionary&lt;TKey, TValue&gt;</c> type reference.</summary>
        public static TypeRef ImmutableDictionary(TypeRef keyType, TypeRef valueType) =>
            new($"global::System.Collections.Immutable.ImmutableDictionary<{keyType.Value}, {valueType.Value}>");
    }

    // ── Tasks & Threading (System.Threading.Tasks, System.Threading) ────

    /// <summary>
    /// Factory methods for <c>System.Threading.Tasks</c> and <c>System.Threading</c> types.
    /// </summary>
    public static class Tasks
    {
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

        /// <summary>Gets a <c>CancellationToken</c> type reference.</summary>
        public static TypeRef CancellationToken => new("global::System.Threading.CancellationToken");
    }

    // ── JSON (System.Text.Json) ─────────────────────────────────────────

    /// <summary>
    /// Factory methods for <c>System.Text.Json</c> and <c>System.Text.Json.Serialization</c> types.
    /// </summary>
    public static class Json
    {
        /// <summary>Gets a <c>JsonSerializer</c> type reference.</summary>
        public static TypeRef Serializer => new("global::System.Text.Json.JsonSerializer");

        /// <summary>Gets a <c>JsonSerializerOptions</c> type reference.</summary>
        public static TypeRef SerializerOptions => new("global::System.Text.Json.JsonSerializerOptions");

        /// <summary>Gets a <c>Utf8JsonReader</c> type reference.</summary>
        public static TypeRef Reader => new("global::System.Text.Json.Utf8JsonReader");

        /// <summary>Gets a <c>Utf8JsonWriter</c> type reference.</summary>
        public static TypeRef Writer => new("global::System.Text.Json.Utf8JsonWriter");

        /// <summary>Creates a <c>JsonConverter&lt;T&gt;</c> type reference.</summary>
        public static TypeRef Converter(TypeRef valueType) =>
            new($"global::System.Text.Json.Serialization.JsonConverter<{valueType.Value}>");

        /// <summary>Gets a <c>JsonConverterAttribute</c> type reference.</summary>
        public static TypeRef ConverterAttribute => new("global::System.Text.Json.Serialization.JsonConverter");
    }

    // ── Encoding (System.Text) ──────────────────────────────────────────

    /// <summary>
    /// Factory methods for <c>System.Text.Encoding</c> types.
    /// </summary>
    public static class Encoding
    {
        /// <summary>Gets an <c>Encoding.UTF8</c> expression.</summary>
        public static TypeRef UTF8 => new("global::System.Text.Encoding.UTF8");

        /// <summary>Gets an <c>Encoding.ASCII</c> expression.</summary>
        public static TypeRef ASCII => new("global::System.Text.Encoding.ASCII");

        /// <summary>Gets an <c>Encoding.Unicode</c> (UTF-16) expression.</summary>
        public static TypeRef Unicode => new("global::System.Text.Encoding.Unicode");
    }

    // ── HTTP (System.Net.Http) ──────────────────────────────────────────

    /// <summary>
    /// Factory methods for <c>System.Net.Http</c> types.
    /// </summary>
    public static class Http
    {
        /// <summary>Gets an <c>HttpClient</c> type reference.</summary>
        public static TypeRef Client => new("global::System.Net.Http.HttpClient");

        /// <summary>Gets an <c>HttpRequestMessage</c> type reference.</summary>
        public static TypeRef RequestMessage => new("global::System.Net.Http.HttpRequestMessage");

        /// <summary>Gets an <c>HttpResponseMessage</c> type reference.</summary>
        public static TypeRef ResponseMessage => new("global::System.Net.Http.HttpResponseMessage");

        /// <summary>Gets an <c>HttpMethod</c> type reference.</summary>
        public static TypeRef Method => new("global::System.Net.Http.HttpMethod");

        private static readonly HashSet<string> KnownVerbs = ["Get", "Post", "Put", "Patch", "Delete", "Head", "Options", "Trace"];

        /// <summary>Creates an <c>HttpMethod.{verb}</c> expression (e.g., <c>HttpMethod.Get</c>).</summary>
        /// <param name="verb">The HTTP verb name (e.g., "Get", "Post"). Must be a known HTTP method.</param>
        /// <exception cref="ArgumentException">Thrown when the verb is not a recognized HTTP method.</exception>
        public static TypeRef Verb(string verb)
        {
            if (!KnownVerbs.Contains(verb))
                throw new ArgumentException($"Unknown HTTP verb '{verb}'. Known verbs: {string.Join(", ", KnownVerbs)}.", nameof(verb));

            return new($"global::System.Net.Http.HttpMethod.{verb}");
        }

        /// <summary>Gets an <c>HttpMethod.Get</c> expression.</summary>
        public static TypeRef Get => new("global::System.Net.Http.HttpMethod.Get");

        /// <summary>Gets an <c>HttpMethod.Post</c> expression.</summary>
        public static TypeRef Post => new("global::System.Net.Http.HttpMethod.Post");

        /// <summary>Gets an <c>HttpMethod.Put</c> expression.</summary>
        public static TypeRef Put => new("global::System.Net.Http.HttpMethod.Put");

        /// <summary>Gets an <c>HttpMethod.Patch</c> expression.</summary>
        public static TypeRef Patch => new("global::System.Net.Http.HttpMethod.Patch");

        /// <summary>Gets an <c>HttpMethod.Delete</c> expression.</summary>
        public static TypeRef Delete => new("global::System.Net.Http.HttpMethod.Delete");

        /// <summary>Gets an <c>HttpContent</c> type reference.</summary>
        public static TypeRef Content => new("global::System.Net.Http.HttpContent");

        /// <summary>Gets a <c>StringContent</c> type reference.</summary>
        public static TypeRef StringContent => new("global::System.Net.Http.StringContent");

        /// <summary>Gets a <c>ByteArrayContent</c> type reference.</summary>
        public static TypeRef ByteArrayContent => new("global::System.Net.Http.ByteArrayContent");

        /// <summary>Gets a <c>StreamContent</c> type reference.</summary>
        public static TypeRef StreamContent => new("global::System.Net.Http.StreamContent");
    }

    // ── Dependency Injection (Microsoft.Extensions.DependencyInjection) ──

    /// <summary>
    /// Factory methods for <c>Microsoft.Extensions.DependencyInjection</c> types.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>Gets an <c>IServiceCollection</c> type reference.</summary>
        public static TypeRef IServiceCollection => new("global::Microsoft.Extensions.DependencyInjection.IServiceCollection");

        /// <summary>Gets an <c>IServiceProvider</c> type reference.</summary>
        public static TypeRef IServiceProvider => new("global::System.IServiceProvider");

        /// <summary>Gets an <c>IServiceScopeFactory</c> type reference.</summary>
        public static TypeRef IServiceScopeFactory => new("global::Microsoft.Extensions.DependencyInjection.IServiceScopeFactory");

        /// <summary>Gets an <c>IServiceScope</c> type reference.</summary>
        public static TypeRef IServiceScope => new("global::Microsoft.Extensions.DependencyInjection.IServiceScope");

        /// <summary>Gets a <c>ServiceDescriptor</c> type reference.</summary>
        public static TypeRef ServiceDescriptor => new("global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor");
    }

    // ── Logging (Microsoft.Extensions.Logging) ────────────────────────

    /// <summary>
    /// Factory methods for <c>Microsoft.Extensions.Logging</c> types.
    /// </summary>
    public static class Logging
    {
        /// <summary>Gets an <c>ILogger</c> type reference.</summary>
        public static TypeRef ILogger => new("global::Microsoft.Extensions.Logging.ILogger");

        /// <summary>Creates an <c>ILogger&lt;T&gt;</c> type reference.</summary>
        public static TypeRef ILoggerOf(TypeRef categoryType) =>
            new($"global::Microsoft.Extensions.Logging.ILogger<{categoryType.Value}>");

        /// <summary>Gets an <c>ILoggerFactory</c> type reference.</summary>
        public static TypeRef ILoggerFactory => new("global::Microsoft.Extensions.Logging.ILoggerFactory");

        /// <summary>Gets a <c>LogLevel</c> type reference.</summary>
        public static TypeRef LogLevel => new("global::Microsoft.Extensions.Logging.LogLevel");
    }

    // ── LINQ (System.Linq, System.Linq.Expressions) ────────────────────

    /// <summary>
    /// Factory methods for <c>System.Linq</c> and <c>System.Linq.Expressions</c> types.
    /// </summary>
    public static class Linq
    {
        /// <summary>Creates an <c>IQueryable&lt;T&gt;</c> type reference.</summary>
        public static TypeRef IQueryable(TypeRef elementType) =>
            new($"global::System.Linq.IQueryable<{elementType.Value}>");

        /// <summary>Creates an <c>IOrderedQueryable&lt;T&gt;</c> type reference.</summary>
        public static TypeRef IOrderedQueryable(TypeRef elementType) =>
            new($"global::System.Linq.IOrderedQueryable<{elementType.Value}>");

        /// <summary>Creates an <c>Expression&lt;TDelegate&gt;</c> type reference.</summary>
        public static TypeRef Expression(TypeRef delegateType) =>
            new($"global::System.Linq.Expressions.Expression<{delegateType.Value}>");
    }

    // ── Delegates (System) ──────────────────────────────────────────────

    /// <summary>
    /// Factory methods for <c>System.Func</c> and <c>System.Action</c> delegate types.
    /// </summary>
    public static class Delegates
    {
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

    /// <summary>Produces a safe cast expression: <c>value as Type</c>.</summary>
    /// <param name="target">The target type to cast to.</param>
    /// <example>
    /// <code>
    /// TypeRef.From("field").As("MyRecord")   // "field as MyRecord"
    /// </code>
    /// </example>
    public TypeRef As(TypeRef target) => new($"{Value} as {target.Value}");

    /// <summary>Produces a direct cast expression: <c>(Type)value</c>.</summary>
    /// <param name="target">The target type to cast to.</param>
    /// <example>
    /// <code>
    /// TypeRef.From("field").Cast("MyRecord")   // "(MyRecord)field"
    /// </code>
    /// </example>
    public TypeRef Cast(TypeRef target) => new($"({target.Value}){Value}");

    /// <summary>Appends a null-coalescing fallback: <c>value ?? fallback</c>.</summary>
    /// <param name="fallback">The fallback expression when the value is null.</param>
    /// <example>
    /// <code>
    /// TypeRef.From("OnSave").Invoke("id").OrDefault(TypeRef.Tasks.CompletedTask)
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
