// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.RoslynKit;

namespace RoslynKit.Sample;

/// <summary>
/// A sample ViewModel demonstrating the [AutoNotify] attribute.
/// This generates INotifyPropertyChanged implementation with change notification.
/// </summary>
[AutoNotify]
public partial class PersonViewModel
{
    /// <summary>
    /// Backing field for FirstName property.
    /// The [AlsoNotify] attribute ensures FullName is notified when this changes.
    /// </summary>
    [AlsoNotify(nameof(FullName))]
    private string _firstName = "";

    /// <summary>
    /// Backing field for LastName property.
    /// The [AlsoNotify] attribute ensures FullName is notified when this changes.
    /// </summary>
    [AlsoNotify(nameof(FullName))]
    private string _lastName = "";

    /// <summary>
    /// Backing field for Age property.
    /// </summary>
    private int _age;

    /// <summary>
    /// Backing field for Email property.
    /// </summary>
    private string? _email;

    /// <summary>
    /// Gets the full name computed from first and last name.
    /// This property is automatically notified when FirstName or LastName changes.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}

/// <summary>
/// A sample ViewModel for settings demonstrating [AutoNotify] with dependent properties.
/// </summary>
[AutoNotify]
public partial class SettingsViewModel
{
    [AlsoNotify(nameof(IsValid))]
    private string _serverUrl = "";

    [AlsoNotify(nameof(IsValid))]
    private int _port = 8080;

    private bool _useSsl = true;

    /// <summary>
    /// Gets whether the current settings are valid.
    /// </summary>
    public bool IsValid => !string.IsNullOrWhiteSpace(ServerUrl) && Port > 0 && Port < 65536;
}
