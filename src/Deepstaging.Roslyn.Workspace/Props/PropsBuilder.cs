// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Fluent builder for declaring default MSBuild properties and item groups
/// in a <see cref="ManagedPropsFile"/>.
/// </summary>
public sealed class PropsBuilder
{
    internal List<PropsPropertyGroup> PropertyGroups { get; } = [];
    internal List<PropsItemGroup> ItemGroups { get; } = [];

    /// <summary>
    /// Declares a default MSBuild property in the first (unlabeled) PropertyGroup.
    /// Added only if it does not already exist.
    /// </summary>
    public PropsBuilder Property(string name, string value)
    {
        var group = PropertyGroups.FirstOrDefault(g => g.Label is null);
        if (group is null)
        {
            group = new PropsPropertyGroup(null);
            PropertyGroups.Insert(0, group);
        }

        group.Properties.Add((name, value));
        return this;
    }

    /// <summary>
    /// Declares a labeled PropertyGroup with properties.
    /// </summary>
    public PropsBuilder PropertyGroup(string label, Action<PropsPropertyGroupBuilder> configure)
    {
        var builder = new PropsPropertyGroupBuilder();
        configure(builder);
        PropertyGroups.Add(new PropsPropertyGroup(label) { Properties = builder.Properties });
        return this;
    }

    /// <summary>
    /// Declares a default ItemGroup using a fluent builder.
    /// </summary>
    /// <param name="configure">Action to configure items in the group.</param>
    /// <param name="label">Optional label attribute for the ItemGroup.</param>
    public PropsBuilder ItemGroup(Action<PropsItemGroupBuilder> configure, string? label = null)
    {
        var builder = new PropsItemGroupBuilder();
        configure(builder);
        ItemGroups.Add(builder.Build(label));
        return this;
    }
}

/// <summary>
/// Fluent builder for declaring items within an ItemGroup.
/// </summary>
public sealed class PropsItemGroupBuilder
{
    private readonly List<PropsItem> _items = [];

    /// <summary>
    /// Adds an <c>Include</c> item.
    /// </summary>
    public PropsItemGroupBuilder Include(string itemType, string pattern)
    {
        _items.Add(new PropsItem(itemType, "Include", pattern));
        return this;
    }

    /// <summary>
    /// Adds a <c>Remove</c> item.
    /// </summary>
    public PropsItemGroupBuilder Remove(string itemType, string pattern)
    {
        _items.Add(new PropsItem(itemType, "Remove", pattern));
        return this;
    }

    /// <summary>
    /// Adds an <c>Update</c> item with child elements.
    /// </summary>
    public PropsItemGroupBuilder Update(string itemType, string pattern, Action<PropsItemMetadataBuilder>? metadata = null)
    {
        var metadataBuilder = new PropsItemMetadataBuilder();
        metadata?.Invoke(metadataBuilder);
        _items.Add(new PropsItem(itemType, "Update", pattern, metadataBuilder.Build()));
        return this;
    }

    internal PropsItemGroup Build(string? label = null) => new(_items, label);
}

/// <summary>
/// Fluent builder for item metadata (child elements like DependentUpon).
/// </summary>
public sealed class PropsItemMetadataBuilder
{
    private readonly List<(string Name, string Value)> _metadata = [];

    /// <summary>
    /// Adds a metadata element to the item.
    /// </summary>
    public PropsItemMetadataBuilder Add(string name, string value)
    {
        _metadata.Add((name, value));
        return this;
    }

    internal List<(string Name, string Value)> Build() => _metadata;
}

/// <summary>
/// Represents a declared item in a props ItemGroup.
/// </summary>
internal sealed class PropsItem(
    string itemType,
    string action,
    string pattern,
    List<(string Name, string Value)>? metadata = null)
{
    internal string ItemType => itemType;
    internal string Action => action;
    internal string Pattern => pattern;
    internal List<(string Name, string Value)>? Metadata => metadata;
}

/// <summary>
/// Represents a declared ItemGroup in a props file.
/// </summary>
internal sealed class PropsItemGroup(List<PropsItem> items, string? label)
{
    internal List<PropsItem> Items => items;
    internal string? Label => label;

    /// <summary>
    /// Returns the identifying item for matching against existing ItemGroups.
    /// Uses the first item's type, action, and pattern.
    /// </summary>
    internal PropsItem Identity => items[0];
}

/// <summary>
/// Represents a declared PropertyGroup in a props file.
/// </summary>
internal sealed class PropsPropertyGroup(string? label)
{
    internal string? Label => label;
    internal List<(string Name, string Value)> Properties { get; set; } = [];
}

/// <summary>
/// Fluent builder for declaring properties within a labeled PropertyGroup.
/// </summary>
public sealed class PropsPropertyGroupBuilder
{
    internal List<(string Name, string Value)> Properties { get; } = [];

    /// <summary>
    /// Declares a property within the group.
    /// </summary>
    public PropsPropertyGroupBuilder Property(string name, string value)
    {
        Properties.Add((name, value));
        return this;
    }
}
