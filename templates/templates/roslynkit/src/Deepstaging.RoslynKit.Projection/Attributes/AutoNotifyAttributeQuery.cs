// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Attr = Deepstaging.RoslynKit.AutoNotifyAttribute;

namespace Deepstaging.RoslynKit.Projection.Attributes;

/// <summary>
/// A queryable wrapper over <see cref="AutoNotifyAttribute"/> data.
/// Provides strongly-typed access to attribute properties with sensible defaults.
/// </summary>
/// <param name="AttributeData">The underlying Roslyn attribute data.</param>
public sealed record AutoNotifyAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// Gets whether to generate the base INotifyPropertyChanged implementation.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool GenerateBaseImplementation => NamedArg<bool>(nameof(Attr.GenerateBaseImplementation))
        .OrDefault(true);
}