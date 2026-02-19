// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

using Emit;

/// <summary>
/// PropertyBuilder extensions for generating notifying property setters that use
/// <c>EqualityComparer&lt;T&gt;.Default.Equals</c> for change detection and
/// raise <c>OnPropertyChanged</c> (and optionally dependent property notifications).
/// </summary>
/// <remarks>
/// This extension is designed to pair with
/// <see cref="Emit.Interfaces.Observable.TypeBuilderNotifyPropertyChangedExtensions"/>
/// which adds the <c>PropertyChanged</c> event and <c>OnPropertyChanged</c> method to the type.
/// </remarks>
/// <example>
/// <code>
/// // Produces a setter like:
/// // set
/// // {
/// //     if (!global::System.Collections.Generic.EqualityComparer&lt;string&gt;.Default.Equals(_name, value))
/// //     {
/// //         _name = value;
/// //         OnPropertyChanged(nameof(Name));
/// //         OnPropertyChanged(nameof(FullName));
/// //     }
/// // }
/// property.WithNotifyingSetter("_name", "OnPropertyChanged", "FullName");
/// </code>
/// </example>
public static class PropertyBuilderNotifyingSetterExtensions
{
    /// <summary>
    /// Configures the property setter to check for equality before assigning the backing field,
    /// then raises <c>OnPropertyChanged</c> for this property and any dependent properties.
    /// </summary>
    /// <param name="property">The property builder.</param>
    /// <param name="fieldName">The backing field name (e.g., <c>"_name"</c>).</param>
    /// <param name="onPropertyChangedMethod">The name of the change notification method (default: <c>"OnPropertyChanged"</c>).</param>
    /// <param name="alsoNotify">Additional property names to raise change notifications for.</param>
    /// <returns>The property builder with a notifying setter configured.</returns>
    public static PropertyBuilder WithNotifyingSetter(
        this PropertyBuilder property,
        string fieldName,
        string onPropertyChangedMethod = "OnPropertyChanged",
        params string[] alsoNotify)
    {
        var equalityCheck = EqualityComparerExpression
            .DefaultEquals(property.Type, ExpressionRef.From(fieldName), ExpressionRef.From("value"));

        return property.WithSetter(b => b
            .AddIf($"!{equalityCheck.Value}", ifBody =>
            {
                ifBody = ifBody
                    .AddStatement($"{fieldName} = value")
                    .AddStatement($"{onPropertyChangedMethod}(nameof({property.Name}))");

                foreach (var dependentProperty in alsoNotify)
                    ifBody = ifBody.AddStatement($"{onPropertyChangedMethod}(nameof({dependentProperty}))");

                return ifBody;
            }));
    }
}