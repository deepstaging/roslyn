// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit;

/// <summary>
/// Marks a class for automatic INotifyPropertyChanged implementation.
/// The class must be declared as partial. Fields decorated with this attribute
/// will be converted to properties with change notification.
/// </summary>
/// <example>
/// <code>
/// [AutoNotify]
/// public partial class PersonViewModel
/// {
///     [AlsoNotify(nameof(FullName))]
///     private string _firstName = "";
///     
///     [AlsoNotify(nameof(FullName))]
///     private string _lastName = "";
///     
///     public string FullName => $"{FirstName} {LastName}";
/// }
/// 
/// // Generated:
/// // public string FirstName
/// // {
/// //     get => _firstName;
/// //     set => SetField(ref _firstName, value, also: nameof(FullName));
/// // }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class AutoNotifyAttribute : Attribute
{
    /// <summary>
    /// When true, generates a base implementation of INotifyPropertyChanged.
    /// When false, assumes the class already inherits from a base that provides it.
    /// Default is true.
    /// </summary>
    public bool GenerateBaseImplementation { get; set; } = true;
}