// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.RoslynKit.Projection.Attributes;

using Deepstaging.Roslyn;

/// <summary>
/// 
/// </summary>
/// <param name="AttributeData"></param>
public record AlsoNotifyAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// An array of property names to also raise PropertyChanged for when the attributed field changes.
    /// </summary>
    public EquatableArray<string> AlsoNotify => AttributeData
        .GetConstructorArgument<string[]>(0)
        .Map(values => values.ToEquatableArray())
        .OrDefault([]);
}