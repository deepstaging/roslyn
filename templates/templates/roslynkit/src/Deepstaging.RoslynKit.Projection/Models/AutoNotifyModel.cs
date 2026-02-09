// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit.Projection.Models;

/// <summary>
/// Model representing a class decorated with [AutoNotify].
/// Contains all information needed to generate INotifyPropertyChanged implementation.
/// </summary>
public sealed record AutoNotifyModel
{
    /// <summary>
    /// The namespace containing the type.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The simple name of the type.
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    /// The full name including namespace.
    /// </summary>
    public string FullTypeName => string.IsNullOrEmpty(Namespace) ? TypeName : $"{Namespace}.{TypeName}";

    /// <summary>
    /// The accessibility modifier (e.g., "public", "internal").
    /// </summary>
    public required Accessibility Accessibility { get; init; }

    /// <summary>
    /// Whether to generate the base INotifyPropertyChanged implementation.
    /// </summary>
    public required bool GenerateBaseImplementation { get; init; }

    /// <summary>
    /// The properties to generate with change notification.
    /// </summary>
    public required ImmutableArray<NotifyPropertyModel> Properties { get; init; }
}