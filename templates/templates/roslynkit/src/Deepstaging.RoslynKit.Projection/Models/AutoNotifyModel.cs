// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit.Projection.Models;

using Deepstaging.Roslyn;

/// <summary>Pipeline model for an AutoNotify class.</summary>
[PipelineModel]
public sealed record AutoNotifyModel
{
    /// <summary>
    /// 
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required string TypeName { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public required string Accessibility { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required EquatableArray<AutoNotifyFieldModel> Fields { get; init; }
}