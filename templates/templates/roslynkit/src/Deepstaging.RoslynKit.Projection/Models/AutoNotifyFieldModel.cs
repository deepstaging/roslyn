// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit.Projection.Models;

using Deepstaging.Roslyn;

/// <summary>Pipeline model for a single field to generate a property for.</summary>
[PipelineModel]
public sealed record AutoNotifyFieldModel
{
    /// <summary>
    /// 
    /// </summary>
    public required string FieldName { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required string PropertyName { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required EquatableArray<string> AlsoNotify { get; init; }
}