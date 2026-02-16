// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Fluent builder for composing C# expression strings.
/// Represents a value-position expression — anything valid on the right side of an assignment.
/// Immutable — each method returns a new instance.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ExpressionRef"/> is the expression-domain counterpart to <see cref="TypeRef"/>.
/// A <see cref="TypeRef"/> crosses into expression domain via gateway methods like
/// <see cref="TypeRef.New(ExpressionRef[])"/>, <see cref="TypeRef.Call(string, ExpressionRef[])"/>,
/// and <see cref="TypeRef.Member(string)"/>. Once in expression domain, chaining continues
/// through <see cref="ExpressionRef"/> methods.
/// </para>
/// <para>
/// Both types convert implicitly to <see cref="string"/>. <see cref="TypeRef"/> converts
/// implicitly to <see cref="ExpressionRef"/> (a type name is a valid expression), but
/// never the reverse.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Instantiation
/// ExceptionRefs.ArgumentNull.New("nameof(value)")
///     // "new global::System.ArgumentNullException(nameof(value))"
///
/// // Static method call
/// TypeRef.From("Guid").Call("Parse", "input")
///     // "Guid.Parse(input)"
///
/// // Chained member access + method call
/// ExpressionRef.From("value").Member("Name").Call("ToUpper")
///     // "value.Name.ToUpper()"
///
/// // Delegate invocation with fallback
/// TypeRef.From("OnSave").Invoke("id").OrDefault(TaskRefs.CompletedTask)
///     // "OnSave?.Invoke(id) ?? global::System.Threading.Tasks.Task.CompletedTask"
///
/// // Await with ConfigureAwait
/// ExpressionRef.From("stream").Call("FlushAsync").Await().ConfigureAwait(false)
///     // "await stream.FlushAsync().ConfigureAwait(false)"
/// </code>
/// </example>
public readonly record struct ExpressionRef
{
    /// <summary>Gets the string representation of the expression.</summary>
    public string Value { get; }

    private ExpressionRef(string value) => Value = value;

    /// <summary>Creates an expression reference from a raw expression string.</summary>
    /// <param name="expression">The expression string (e.g., "value", "x + y", "handler?.Invoke()").</param>
    public static ExpressionRef From(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new ArgumentException("Expression cannot be null or whitespace.", nameof(expression));

        return new ExpressionRef(expression);
    }

    // ── Method Calls ────────────────────────────────────────────────────

    /// <summary>Produces a method call expression: <c>expr.method(args)</c>.</summary>
    /// <param name="method">The method name.</param>
    /// <param name="arguments">The arguments to pass.</param>
    public ExpressionRef Call(string method, params ExpressionRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return new ExpressionRef($"{Value}.{method}({args})");
    }

    // ── Member Access ───────────────────────────────────────────────────

    /// <summary>Produces a member access expression: <c>expr.member</c>.</summary>
    /// <param name="name">The member name (property, field, or nested type).</param>
    public ExpressionRef Member(string name) => new($"{Value}.{name}");

    // ── Delegate Invocation ─────────────────────────────────────────────

    /// <summary>Produces a null-conditional delegate invocation: <c>expr?.Invoke(args)</c>.</summary>
    /// <param name="arguments">The arguments to pass to the delegate.</param>
    public ExpressionRef Invoke(params ExpressionRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return new ExpressionRef($"{Value}?.Invoke({args})");
    }

    // ── Type Operations ─────────────────────────────────────────────────

    /// <summary>Produces a safe cast expression: <c>expr as Type</c>.</summary>
    /// <param name="target">The target type to cast to.</param>
    public ExpressionRef As(TypeRef target) => new($"{Value} as {target.Value}");

    /// <summary>Produces a direct cast expression: <c>(Type)expr</c>.</summary>
    /// <param name="target">The target type to cast to.</param>
    public ExpressionRef Cast(TypeRef target) => new($"({target.Value}){Value}");

    /// <summary>Produces a type check expression: <c>expr is Type</c>.</summary>
    /// <param name="target">The type to check against.</param>
    public ExpressionRef Is(TypeRef target) => new($"{Value} is {target.Value}");

    /// <summary>Produces a type check with pattern variable: <c>expr is Type name</c>.</summary>
    /// <param name="target">The type to check against.</param>
    /// <param name="variableName">The pattern variable name.</param>
    public ExpressionRef Is(TypeRef target, string variableName) =>
        new($"{Value} is {target.Value} {variableName}");

    // ── Null Handling ───────────────────────────────────────────────────

    /// <summary>Appends a null-coalescing fallback: <c>expr ?? fallback</c>.</summary>
    /// <param name="fallback">The fallback expression when the value is null.</param>
    public ExpressionRef OrDefault(ExpressionRef fallback) => new($"{Value} ?? {fallback.Value}");

    /// <summary>Appends the null-forgiving operator: <c>expr!</c>.</summary>
    public ExpressionRef NullForgiving() => new($"{Value}!");

    /// <summary>Appends null-conditional access: <c>expr?.member</c>.</summary>
    /// <param name="name">The member name to access conditionally.</param>
    public ExpressionRef NullConditionalMember(string name) => new($"{Value}?.{name}");

    /// <summary>Produces a null-conditional method call: <c>expr?.method(args)</c>.</summary>
    /// <param name="method">The method name.</param>
    /// <param name="arguments">The arguments to pass.</param>
    public ExpressionRef NullConditionalCall(string method, params ExpressionRef[] arguments)
    {
        var args = string.Join(", ", arguments.Select(a => a.Value));
        return new ExpressionRef($"{Value}?.{method}({args})");
    }

    // ── Async ───────────────────────────────────────────────────────────

    /// <summary>Produces an await expression: <c>await expr</c>.</summary>
    public ExpressionRef Await() => new($"await {Value}");

    /// <summary>Appends <c>.ConfigureAwait(continueOnCapturedContext)</c>.</summary>
    /// <param name="continueOnCapturedContext">Whether to continue on the captured context.</param>
    public ExpressionRef ConfigureAwait(bool continueOnCapturedContext = false) =>
        new($"{Value}.ConfigureAwait({(continueOnCapturedContext ? "true" : "false")})");

    // ── Parenthesization ────────────────────────────────────────────────

    /// <summary>Wraps the expression in parentheses: <c>(expr)</c>.</summary>
    public ExpressionRef Parenthesize() => new($"({Value})");

    // ── Conversions ─────────────────────────────────────────────────────

    /// <summary>Returns the string representation of this expression.</summary>
    public override string ToString() => Value;

    /// <summary>Implicitly converts an <see cref="ExpressionRef"/> to a <see cref="string"/>.</summary>
    public static implicit operator string(ExpressionRef expr) => expr.Value;

    /// <summary>Implicitly converts a <see cref="string"/> to an <see cref="ExpressionRef"/>.</summary>
    public static implicit operator ExpressionRef(string expression) => From(expression);

    /// <summary>Implicitly converts a <see cref="TypeRef"/> to an <see cref="ExpressionRef"/>. A type name is a valid expression.</summary>
    public static implicit operator ExpressionRef(TypeRef typeRef) => new(typeRef.Value);
}
