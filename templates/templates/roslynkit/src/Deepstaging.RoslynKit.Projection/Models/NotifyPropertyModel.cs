// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit.Projection.Models;

/// <summary>
/// Model representing a property generated from a backing field.
/// </summary>
public sealed record NotifyPropertyModel
{
    /// <summary>
    /// The generated property name (e.g., "FirstName" from "_firstName").
    /// </summary>
    public required string PropertyName { get; init; }

    /// <summary>
    /// The backing field name (e.g., "_firstName").
    /// </summary>
    public required string FieldName { get; init; }

    /// <summary>
    /// The type of the property as a display string.
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    /// Additional properties to notify when this property changes.
    /// Derived from [AlsoNotify] attributes on the field.
    /// </summary>
    public required ImmutableArray<string> AlsoNotify { get; init; }

    /// <summary>
    /// Whether the backing field has the correct accessibility (private).
    /// </summary>
    public required bool IsFieldPrivate { get; init; }
}