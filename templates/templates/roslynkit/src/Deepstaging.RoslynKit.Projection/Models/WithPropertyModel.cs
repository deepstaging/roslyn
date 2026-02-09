// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit.Projection.Models;

/// <summary>
/// Model representing a property for which a With method will be generated.
/// </summary>
public sealed record WithPropertyModel
{
    /// <summary>
    /// The property name (e.g., "FirstName").
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The property type as a display string (e.g., "string", "int?").
    /// </summary>
    public required string TypeName { get; init; }
}
