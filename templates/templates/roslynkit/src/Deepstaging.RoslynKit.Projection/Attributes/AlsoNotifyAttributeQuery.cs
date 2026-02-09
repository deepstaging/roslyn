// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit.Projection.Attributes;

/// <summary>
/// A queryable wrapper over <see cref="AlsoNotifyAttribute"/> data.
/// Provides access to the property names that should also be notified.
/// </summary>
/// <param name="AttributeData">The underlying Roslyn attribute data.</param>
public sealed record AlsoNotifyAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// Gets the property names that should also be notified.
    /// </summary>
    public ImmutableArray<string> PropertyNames => ConstructorArg<string[]>(0)
        .Map(ImmutableArray.Create)
        .OrDefault([]);
}