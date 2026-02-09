// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit;

/// <summary>
/// Specifies additional properties that should be notified when this field changes.
/// Used in conjunction with <see cref="AutoNotifyAttribute"/>.
/// </summary>
/// <example>
/// <code>
/// [AutoNotify]
/// public partial class PersonViewModel
/// {
///     [AlsoNotify(nameof(FullName), nameof(DisplayName))]
///     private string _firstName = "";
///     
///     public string FullName => $"{FirstName} {LastName}";
///     public string DisplayName => $"{LastName}, {FirstName}";
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public sealed class AlsoNotifyAttribute : Attribute
{
    /// <summary>
    /// The names of properties that should also be notified when this field changes.
    /// </summary>
    public string[] PropertyNames { get; }

    /// <summary>
    /// Creates a new AlsoNotifyAttribute with the specified property names.
    /// </summary>
    /// <param name="propertyNames">The names of properties to notify.</param>
    public AlsoNotifyAttribute(params string[] propertyNames)
    {
        PropertyNames = propertyNames;
    }
}