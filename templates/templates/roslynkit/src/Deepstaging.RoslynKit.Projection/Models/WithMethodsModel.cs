// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit.Projection.Models;

/// <summary>
/// Model representing a type marked with [GenerateWith] attribute.
/// Contains all information needed to generate immutable With*() methods.
/// </summary>
public sealed record WithMethodsModel
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
    /// Whether this is a struct (true) or class (false).
    /// </summary>
    public required bool IsStruct { get; init; }

    /// <summary>
    /// The properties to generate With methods for.
    /// </summary>
    public required ImmutableArray<WithPropertyModel> Properties { get; init; }
}
