// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Xml.Linq;

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for MSBuild-aware manipulation of <c>.props</c> XML documents.
/// </summary>
public static class PropsXmlExtensions
{
    /// <summary>
    /// Sets an MSBuild property in the first (unlabeled) <c>PropertyGroup</c>.
    /// No-op if the property already exists. Creates the <c>PropertyGroup</c> if needed.
    /// </summary>
    /// <param name="document">The props XML document.</param>
    /// <param name="name">The property element name (e.g., <c>UserSecretsId</c>).</param>
    /// <param name="value">The property value.</param>
    /// <param name="comment">Optional XML comment placed before the property element.</param>
    public static void SetProperty(this XDocument document, string name, string value, string? comment = null)
        => SetProperty(document, name, value, label: null, comment: comment);

    /// <summary>
    /// Sets an MSBuild property in the <c>PropertyGroup</c> with the given label.
    /// No-op if the property already exists. Creates the <c>PropertyGroup</c> if needed.
    /// </summary>
    /// <param name="document">The props XML document.</param>
    /// <param name="name">The property element name.</param>
    /// <param name="value">The property value.</param>
    /// <param name="label">The <c>Label</c> attribute to match (or <c>null</c> for the first unlabeled group).</param>
    /// <param name="comment">Optional XML comment placed before the property element.</param>
    public static void SetProperty(this XDocument document, string name, string value, string? label, string? comment)
    {
        var root = document.Root;
        if (root is null) return;

        var propertyGroup = FindOrCreateGroup(root, "PropertyGroup", label);

        if (propertyGroup.Element(name) is not null) return;

        if (comment is not null)
            propertyGroup.Add(Indent(2), new XComment($" {comment} "));

        propertyGroup.Add(Indent(2), new XElement(name, value), Indent(1));
    }

    /// <summary>
    /// Replaces (or creates) a labeled <c>ItemGroup</c> with the given items.
    /// If an <c>ItemGroup</c> with the same label already exists, it is removed and rebuilt.
    /// </summary>
    /// <param name="document">The props XML document.</param>
    /// <param name="label">The <c>Label</c> attribute for the <c>ItemGroup</c>.</param>
    /// <param name="configure">Action to populate items via a fluent builder.</param>
    public static void SetItemGroup(this XDocument document, string label, Action<ItemGroupXmlBuilder> configure)
    {
        var root = document.Root;
        if (root is null) return;

        // Remove existing group with this label
        root.Elements("ItemGroup")
            .FirstOrDefault(ig => (string?)ig.Attribute("Label") == label)
            ?.Remove();

        var builder = new ItemGroupXmlBuilder();
        configure(builder);

        var itemGroup = new XElement("ItemGroup", new XAttribute("Label", label));

        foreach (var item in builder.Build())
            itemGroup.Add(Indent(2), item);

        itemGroup.Add(Indent(1));

        // Insert after last group
        var insertAfter = root.Elements("ItemGroup").LastOrDefault()
                          ?? root.Elements("PropertyGroup").LastOrDefault();

        if (insertAfter is not null)
            insertAfter.AddAfterSelf(Indent(1), itemGroup);
        else
            root.Add(Indent(1), itemGroup);
    }

    private static XElement FindOrCreateGroup(XElement root, string groupName, string? label)
    {
        var group = root.Elements(groupName)
            .FirstOrDefault(g => (string?)g.Attribute("Label") == label);

        if (group is not null) return group;

        group = new XElement(groupName);

        if (label is not null)
            group.Add(new XAttribute("Label", label));

        group.Add(Indent(1));

        var insertAfter = root.Elements("PropertyGroup").LastOrDefault()
                          ?? root.Elements("ItemGroup").LastOrDefault();

        if (insertAfter is not null)
            insertAfter.AddAfterSelf(Indent(1), group);
        else
            root.AddFirst(Indent(1), group);

        return group;
    }

    internal static XText Indent(int level) => new("\n" + new string(' ', level * 4));
}

/// <summary>
/// Fluent builder for constructing MSBuild item elements within an <c>ItemGroup</c>.
/// </summary>
public sealed class ItemGroupXmlBuilder
{
    private readonly List<XElement> _items = [];

    /// <summary>
    /// Adds an item with the specified action attribute (e.g., <c>Include</c>, <c>Update</c>, <c>Remove</c>).
    /// </summary>
    /// <param name="itemType">The item type (e.g., <c>None</c>, <c>AdditionalFiles</c>, <c>Compile</c>).</param>
    /// <param name="action">The action attribute name (e.g., <c>Update</c>).</param>
    /// <param name="pattern">The pattern value for the action attribute.</param>
    /// <param name="metadata">Optional child metadata elements (e.g., <c>DependentUpon</c>).</param>
    public ItemGroupXmlBuilder Item(string itemType, string action, string pattern,
        Action<ItemMetadataXmlBuilder>? metadata = null)
    {
        var element = new XElement(itemType, new XAttribute(action, pattern));

        if (metadata is not null)
        {
            var metadataBuilder = new ItemMetadataXmlBuilder();
            metadata(metadataBuilder);

            foreach (var (name, value) in metadataBuilder.Build())
                element.Add(PropsXmlExtensions.Indent(3), new XElement(name, value));

            element.Add(PropsXmlExtensions.Indent(2));
        }

        _items.Add(element);
        return this;
    }

    internal List<XElement> Build() => _items;
}

/// <summary>
/// Fluent builder for item metadata child elements.
/// </summary>
public sealed class ItemMetadataXmlBuilder
{
    private readonly List<(string Name, string Value)> _metadata = [];

    /// <summary>
    /// Adds a metadata child element.
    /// </summary>
    public ItemMetadataXmlBuilder Set(string name, string value)
    {
        _metadata.Add((name, value));
        return this;
    }

    internal List<(string Name, string Value)> Build() => _metadata;
}
