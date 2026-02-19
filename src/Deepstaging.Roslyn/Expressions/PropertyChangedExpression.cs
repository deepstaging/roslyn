// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

/// <summary>
/// Builds <c>INotifyPropertyChanged</c> related expressions for use in generated code.
/// </summary>
/// <example>
/// <code>
/// PropertyChangedExpression.NewEventArgs("Name")
///     // → "new global::System.ComponentModel.PropertyChangedEventArgs(\"Name\")"
///
/// PropertyChangedExpression.Raise("PropertyChanged", "this", "nameArgs")
///     // → "PropertyChanged?.Invoke(this, nameArgs)"
/// </code>
/// </example>
public static class PropertyChangedExpression
{
    private static readonly TypeRef EventArgsType =
        TypeRef.Global("System.ComponentModel.PropertyChangedEventArgs");

    private static readonly TypeRef ChangingEventArgsType =
        TypeRef.Global("System.ComponentModel.PropertyChangingEventArgs");

    // ── PropertyChangedEventArgs ────────────────────────────────────────

    /// <summary>
    /// <c>new PropertyChangedEventArgs("propertyName")</c> — creates event args for a named property.
    /// </summary>
    /// <param name="propertyName">The property name (will be quoted in the output).</param>
    public static ExpressionRef NewEventArgs(string propertyName) =>
        EventArgsType.New(ExpressionRef.From($"\"{propertyName}\""));

    /// <summary>
    /// <c>new PropertyChangedEventArgs(nameofExpr)</c> — creates event args from an expression (e.g., <c>nameof(Name)</c>).
    /// </summary>
    /// <param name="nameExpression">The name expression (e.g., <c>nameof(Name)</c> or a <c>propertyName</c> parameter).</param>
    public static ExpressionRef NewEventArgs(ExpressionRef nameExpression) =>
        EventArgsType.New(nameExpression);

    // ── PropertyChangingEventArgs ───────────────────────────────────────

    /// <summary>
    /// <c>new PropertyChangingEventArgs("propertyName")</c> — creates changing event args for a named property.
    /// </summary>
    /// <param name="propertyName">The property name (will be quoted in the output).</param>
    public static ExpressionRef NewChangingEventArgs(string propertyName) =>
        ChangingEventArgsType.New(ExpressionRef.From($"\"{propertyName}\""));

    /// <summary>
    /// <c>new PropertyChangingEventArgs(nameofExpr)</c> — creates changing event args from an expression.
    /// </summary>
    /// <param name="nameExpression">The name expression.</param>
    public static ExpressionRef NewChangingEventArgs(ExpressionRef nameExpression) =>
        ChangingEventArgsType.New(nameExpression);

    // ── Event Raising ───────────────────────────────────────────────────

    /// <summary>
    /// <c>handler?.Invoke(sender, args)</c> — null-safe invocation of a property changed handler.
    /// </summary>
    /// <param name="handler">The event handler expression (e.g., <c>"PropertyChanged"</c>).</param>
    /// <param name="sender">The sender expression (typically <c>"this"</c>).</param>
    /// <param name="args">The event args expression.</param>
    public static ExpressionRef Raise(ExpressionRef handler, ExpressionRef sender, ExpressionRef args) =>
        handler.Invoke(sender, args);
}