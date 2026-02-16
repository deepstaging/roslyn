// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Linq.Expressions;

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// A binding recorded by <see cref="TemplateMap{TModel}"/> that maps a model property path
/// to the string value it contributed to the emit output.
/// </summary>
/// <param name="PropertyPath">
/// Dot-separated property path (e.g., "Name" or "BackingType.CodeName").
/// Used as the Scriban placeholder: <c>{{ PropertyPath }}</c>.
/// </param>
/// <param name="Value">The string representation of the value used in the emit output.</param>
public readonly record struct TemplateBinding(string PropertyPath, string Value);

/// <summary>
/// Side-channel tracker that records which model property values were used during emit construction.
/// Pass to builder calls via <see cref="Bind{T}"/> to record mappings, then pass alongside the emit
/// to <see cref="CustomizableEmitExtensions"/> to produce a <see cref="CustomizableEmit"/>.
/// </summary>
/// <remarks>
/// <para>
/// <c>Bind</c> is a pass-through — it returns the value unchanged but records a mapping
/// from the property path (extracted from the expression) to the string representation of the value.
/// </para>
/// <para>
/// Only non-null, non-empty string representations are recorded. Null values and empty strings
/// are silently skipped (they would produce meaningless template placeholders).
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var map = new TemplateMap&lt;StrongIdModel&gt;();
///
/// TypeBuilder.Struct(map.Bind(model.TypeName, m => m.TypeName))
///     .InNamespace(map.Bind(model.Namespace, m => m.Namespace))
///     .AddProperty("Value", map.Bind(model.BackingType.CodeName, m => m.BackingType.CodeName))
///     .Emit()
///     .WithUserTemplate("Deepstaging.Ids/StrongId", model, map);
/// </code>
/// </example>
/// <typeparam name="TModel">The pipeline model type whose properties are being mapped.</typeparam>
public sealed class TemplateMap<TModel>
{
    private readonly List<TemplateBinding> _bindings = [];

    /// <summary>
    /// Gets the recorded bindings in the order they were added.
    /// </summary>
    public IReadOnlyList<TemplateBinding> Bindings => _bindings;

    /// <summary>
    /// Records a mapping from a model property to its value and returns the value unchanged.
    /// The property path is extracted from the expression and will become a Scriban placeholder.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to pass through (returned as-is).</param>
    /// <param name="selector">
    /// Expression selecting the model property (e.g., <c>m => m.Name</c> or <c>m => m.Type.CodeName</c>).
    /// </param>
    /// <returns>The <paramref name="value"/> unchanged.</returns>
    public T Bind<T>(T value, Expression<Func<TModel, T>> selector)
    {
        var path = ExtractPropertyPath(selector.Body);
        var stringValue = value?.ToString();

        if (!string.IsNullOrWhiteSpace(stringValue))
            _bindings.Add(new TemplateBinding(path, stringValue!));

        return value;
    }

    /// <summary>
    /// Extracts a dot-separated property path from a member access expression.
    /// Handles simple (<c>m => m.Name</c>) and nested (<c>m => m.Type.CodeName</c>) access.
    /// </summary>
    private static string ExtractPropertyPath(Expression expression) =>
        expression switch
        {
            MemberExpression { Expression: ParameterExpression } member
                => member.Member.Name,

            MemberExpression member
                => $"{ExtractPropertyPath(member.Expression!)}.{member.Member.Name}",

            UnaryExpression { NodeType: ExpressionType.Convert } unary
                => ExtractPropertyPath(unary.Operand),

            _ => throw new ArgumentException(
                $"Unsupported expression in TemplateMap.Bind: {expression.NodeType}. " +
                "Use a simple property access like m => m.Name or m => m.Nested.Prop.")
        };
}