// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Xml.Linq;

namespace Deepstaging.Roslyn;

/// <summary>
/// Specifies how a property or item is handled during a merge operation.
/// </summary>
public enum MergeAction
{
    /// <summary>Add the entry only if it does not already exist (default).</summary>
    Add,

    /// <summary>Remove the entry if it exists.</summary>
    Remove,

    /// <summary>Add the entry if missing, or overwrite its value if it already exists.</summary>
    Set,
}

/// <summary>
/// Extension methods for MSBuild-aware manipulation of <c>.props</c> XML documents.
/// </summary>
public static class PropsXmlExtensions
{
    /// <summary>
    /// Sets properties in a labeled <c>PropertyGroup</c>, preserving existing user content.
    /// Creates the <c>PropertyGroup</c> if it does not exist. By default, properties are added
    /// only if they do not already exist. Use <see cref="MergeAction"/> to control per-property behavior.
    /// </summary>
    /// <param name="document">The props XML document.</param>
    /// <param name="label">The <c>Label</c> attribute for the <c>PropertyGroup</c>.</param>
    /// <param name="configure">Action to declare properties via a fluent builder.</param>
    /// <returns>The same <paramref name="document"/> for fluent chaining.</returns>
    public static XDocument SetPropertyGroup(this XDocument document, string label, Action<PropertyGroupXmlBuilder> configure)
    {
        var root = document.Root;
        if (root is null) return document;

        var builder = new PropertyGroupXmlBuilder();
        configure(builder);

        var propertyGroup = FindOrCreateGroup(root, "PropertyGroup", label, builder.GroupComment);

        foreach (var (name, value, propertyComment, action) in builder.Build())
        {
            var existing = propertyGroup.Element(name);

            switch (action)
            {
                case MergeAction.Add:
                    if (existing is not null) break;
                    if (propertyComment is not null)
                        propertyGroup.Add(Indent(2), new XComment($" {propertyComment} "));
                    propertyGroup.Add(Indent(2), new XElement(name, value), Indent(1));
                    break;

                case MergeAction.Remove:
                    existing?.Remove();
                    break;

                case MergeAction.Set:
                    if (existing is not null)
                    {
                        existing.Value = value;
                    }
                    else
                    {
                        if (propertyComment is not null)
                            propertyGroup.Add(Indent(2), new XComment($" {propertyComment} "));
                        propertyGroup.Add(Indent(2), new XElement(name, value), Indent(1));
                    }
                    break;
            }
        }

        return document;
    }

    /// <summary>
    /// Sets items in a labeled <c>ItemGroup</c>, preserving existing user content.
    /// Creates the <c>ItemGroup</c> if it does not exist. By default, items are added
    /// only if a matching item does not already exist. Use <see cref="MergeAction"/> to control per-item behavior.
    /// Items are identified by the combination of element name, action attribute, and pattern value.
    /// </summary>
    /// <param name="document">The props XML document.</param>
    /// <param name="label">The <c>Label</c> attribute for the <c>ItemGroup</c>.</param>
    /// <param name="configure">Action to declare items via a fluent builder.</param>
    /// <returns>The same <paramref name="document"/> for fluent chaining.</returns>
    public static XDocument SetItemGroup(this XDocument document, string label, Action<ItemGroupXmlBuilder> configure)
    {
        var root = document.Root;
        if (root is null) return document;

        var builder = new ItemGroupXmlBuilder();
        configure(builder);

        var itemGroup = FindOrCreateGroup(root, "ItemGroup", label, builder.GroupComment);

        foreach (var entry in builder.Build())
        {
            var existing = itemGroup.Elements(entry.ItemType)
                .FirstOrDefault(el => (string?)el.Attribute(entry.Action) == entry.Pattern);

            switch (entry.MergeAction)
            {
                case MergeAction.Add:
                    if (existing is not null) break;
                    if (entry.Comment is not null)
                        itemGroup.Add(Indent(2), new XComment($" {entry.Comment} "));
                    itemGroup.Add(Indent(2), entry.Element, Indent(1));
                    break;

                case MergeAction.Remove:
                    existing?.Remove();
                    break;

                case MergeAction.Set:
                    if (existing is not null)
                    {
                        existing.ReplaceWith(entry.Element);
                    }
                    else
                    {
                        if (entry.Comment is not null)
                            itemGroup.Add(Indent(2), new XComment($" {entry.Comment} "));
                        itemGroup.Add(Indent(2), entry.Element, Indent(1));
                    }
                    break;
            }
        }

        return document;
    }

    /// <summary>
    /// Conditionally applies modifications to the document.
    /// The <paramref name="configure"/> callback is invoked only when <paramref name="condition"/> is <see langword="true"/>.
    /// </summary>
    /// <param name="document">The props XML document.</param>
    /// <param name="condition">When <see langword="true"/>, the callback is executed.</param>
    /// <param name="configure">A callback that modifies the document.</param>
    /// <returns>The same <paramref name="document"/> for fluent chaining.</returns>
    public static XDocument If(this XDocument document, bool condition, Action<XDocument> configure)
    {
        if (condition)
            configure(document);
        return document;
    }

    /// <summary>
    /// Conditionally applies one of two sets of modifications to the document.
    /// Invokes <paramref name="configure"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or <paramref name="otherwise"/> when it is <see langword="false"/>.
    /// </summary>
    /// <param name="document">The props XML document.</param>
    /// <param name="condition">Determines which callback is executed.</param>
    /// <param name="configure">A callback applied when the condition is <see langword="true"/>.</param>
    /// <param name="otherwise">A callback applied when the condition is <see langword="false"/>.</param>
    /// <returns>The same <paramref name="document"/> for fluent chaining.</returns>
    public static XDocument If(
        this XDocument document,
        bool condition,
        Action<XDocument> configure,
        Action<XDocument> otherwise)
    {
        if (condition)
            configure(document);
        else
            otherwise(document);
        return document;
    }

    /// <summary>
    /// Iterates over <paramref name="items"/> and invokes <paramref name="configure"/> for each element.
    /// If <paramref name="items"/> is <see langword="null"/>, this is a no-op.
    /// </summary>
    /// <typeparam name="T">The element type of the collection.</typeparam>
    /// <param name="document">The props XML document.</param>
    /// <param name="items">The collection to iterate. May be <see langword="null"/>.</param>
    /// <param name="configure">A callback that receives the document and the current item.</param>
    /// <returns>The same <paramref name="document"/> for fluent chaining.</returns>
    public static XDocument WithEach<T>(this XDocument document, IEnumerable<T>? items, Action<XDocument, T> configure)
    {
        if (items is null)
            return document;

        foreach (var item in items)
            configure(document, item);
        return document;
    }

    private static XElement FindOrCreateGroup(XElement root, string groupName, string label, string? comment)
    {
        var group = root.Elements(groupName)
            .FirstOrDefault(g => (string?)g.Attribute("Label") == label);

        if (group is not null) return group;

        group = new XElement(groupName, new XAttribute("Label", label));

        group.Add(Indent(1));

        var insertAfter = root.Elements("PropertyGroup").LastOrDefault()
                          ?? root.Elements("ItemGroup").LastOrDefault();

        if (insertAfter is not null)
        {
            if (comment is not null)
                insertAfter.AddAfterSelf(Indent(1), new XComment($" {comment} "), Indent(1), group);
            else
                insertAfter.AddAfterSelf(Indent(1), group);
        }
        else
        {
            if (comment is not null)
                root.AddFirst(Indent(1), new XComment($" {comment} "), Indent(1), group);
            else
                root.AddFirst(Indent(1), group);
        }

        return group;
    }

    internal static XText Indent(int level) => new("\n" + new string(' ', level * 4));
}

/// <summary>
/// Fluent builder for constructing MSBuild item elements within an <c>ItemGroup</c>.
/// Each item can optionally specify a <see cref="MergeAction"/> to control add, remove, or overwrite behavior.
/// </summary>
public sealed class ItemGroupXmlBuilder
{
    private readonly List<ItemEntry> _items = [];

    /// <summary>
    /// Gets the group-level comment configured via <see cref="Comment"/>.
    /// </summary>
    internal string? GroupComment { get; private set; }

    /// <summary>
    /// Sets an XML comment placed before the <c>ItemGroup</c> element when it is first created.
    /// </summary>
    /// <param name="comment">The comment text.</param>
    public ItemGroupXmlBuilder Comment(string comment)
    {
        GroupComment = comment;
        return this;
    }

    /// <summary>
    /// Adds an item with the specified action attribute (e.g., <c>Include</c>, <c>Update</c>, <c>Remove</c>).
    /// Defaults to <see cref="MergeAction.Add"/> (skip if a matching item already exists).
    /// </summary>
    /// <param name="itemType">The item type (e.g., <c>None</c>, <c>AdditionalFiles</c>, <c>Compile</c>).</param>
    /// <param name="action">The action attribute name (e.g., <c>Update</c>).</param>
    /// <param name="pattern">The pattern value for the action attribute.</param>
    /// <param name="metadata">Optional child metadata elements (e.g., <c>DependentUpon</c>).</param>
    /// <param name="comment">Optional XML comment placed before the item element when added.</param>
    public ItemGroupXmlBuilder Item(string itemType, string action, string pattern,
        Action<ItemMetadataXmlBuilder>? metadata = null, string? comment = null)
        => Item(itemType, action, pattern, MergeAction.Add, metadata, comment);

    /// <summary>
    /// Adds an item with the specified action attribute and <see cref="MergeAction"/>.
    /// </summary>
    /// <param name="itemType">The item type (e.g., <c>None</c>, <c>AdditionalFiles</c>, <c>Compile</c>).</param>
    /// <param name="action">The action attribute name (e.g., <c>Update</c>).</param>
    /// <param name="pattern">The pattern value for the action attribute.</param>
    /// <param name="mergeAction">The merge action to apply.</param>
    /// <param name="metadata">Optional child metadata elements (e.g., <c>DependentUpon</c>).</param>
    /// <param name="comment">Optional XML comment placed before the item element when added.</param>
    public ItemGroupXmlBuilder Item(string itemType, string action, string pattern,
        MergeAction mergeAction, Action<ItemMetadataXmlBuilder>? metadata = null, string? comment = null)
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

        _items.Add(new ItemEntry(itemType, action, pattern, element, mergeAction, comment));
        return this;
    }

    /// <summary>
    /// Conditionally adds items to the group.
    /// The <paramref name="configure"/> callback is invoked only when <paramref name="condition"/> is <see langword="true"/>.
    /// </summary>
    /// <param name="condition">When <see langword="true"/>, the callback is executed.</param>
    /// <param name="configure">A callback that adds items to this builder.</param>
    public ItemGroupXmlBuilder If(bool condition, Action<ItemGroupXmlBuilder> configure)
    {
        if (condition)
            configure(this);
        return this;
    }

    /// <summary>
    /// Conditionally applies one of two sets of items to the group.
    /// Invokes <paramref name="configure"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or <paramref name="otherwise"/> when it is <see langword="false"/>.
    /// </summary>
    /// <param name="condition">Determines which callback is executed.</param>
    /// <param name="configure">A callback applied when the condition is <see langword="true"/>.</param>
    /// <param name="otherwise">A callback applied when the condition is <see langword="false"/>.</param>
    public ItemGroupXmlBuilder If(
        bool condition,
        Action<ItemGroupXmlBuilder> configure,
        Action<ItemGroupXmlBuilder> otherwise)
    {
        if (condition)
            configure(this);
        else
            otherwise(this);
        return this;
    }

    /// <summary>
    /// Iterates over <paramref name="items"/> and invokes <paramref name="configure"/> for each element.
    /// If <paramref name="items"/> is <see langword="null"/>, this is a no-op.
    /// </summary>
    /// <typeparam name="T">The element type of the collection.</typeparam>
    /// <param name="items">The collection to iterate. May be <see langword="null"/>.</param>
    /// <param name="configure">A callback that receives the builder and the current item.</param>
    public ItemGroupXmlBuilder WithEach<T>(IEnumerable<T>? items, Action<ItemGroupXmlBuilder, T> configure)
    {
        if (items is null)
            return this;

        foreach (var item in items)
            configure(this, item);
        return this;
    }

    internal List<ItemEntry> Build() => _items;
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

    /// <summary>
    /// Conditionally adds metadata to the item.
    /// The <paramref name="configure"/> callback is invoked only when <paramref name="condition"/> is <see langword="true"/>.
    /// </summary>
    /// <param name="condition">When <see langword="true"/>, the callback is executed.</param>
    /// <param name="configure">A callback that adds metadata to this builder.</param>
    public ItemMetadataXmlBuilder If(bool condition, Action<ItemMetadataXmlBuilder> configure)
    {
        if (condition)
            configure(this);
        return this;
    }

    /// <summary>
    /// Conditionally applies one of two sets of metadata to the item.
    /// Invokes <paramref name="configure"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or <paramref name="otherwise"/> when it is <see langword="false"/>.
    /// </summary>
    /// <param name="condition">Determines which callback is executed.</param>
    /// <param name="configure">A callback applied when the condition is <see langword="true"/>.</param>
    /// <param name="otherwise">A callback applied when the condition is <see langword="false"/>.</param>
    public ItemMetadataXmlBuilder If(
        bool condition,
        Action<ItemMetadataXmlBuilder> configure,
        Action<ItemMetadataXmlBuilder> otherwise)
    {
        if (condition)
            configure(this);
        else
            otherwise(this);
        return this;
    }

    /// <summary>
    /// Iterates over <paramref name="items"/> and invokes <paramref name="configure"/> for each element.
    /// If <paramref name="items"/> is <see langword="null"/>, this is a no-op.
    /// </summary>
    /// <typeparam name="T">The element type of the collection.</typeparam>
    /// <param name="items">The collection to iterate. May be <see langword="null"/>.</param>
    /// <param name="configure">A callback that receives the builder and the current item.</param>
    public ItemMetadataXmlBuilder WithEach<T>(IEnumerable<T>? items, Action<ItemMetadataXmlBuilder, T> configure)
    {
        if (items is null)
            return this;

        foreach (var item in items)
            configure(this, item);
        return this;
    }

    internal List<(string Name, string Value)> Build() => _metadata;
}

/// <summary>
/// Fluent builder for constructing MSBuild property elements within a <c>PropertyGroup</c>.
/// Each property can optionally specify a <see cref="MergeAction"/> to control add, remove, or overwrite behavior.
/// </summary>
public sealed class PropertyGroupXmlBuilder
{
    private readonly List<(string Name, string Value, string? Comment, MergeAction Action)> _properties = [];

    /// <summary>
    /// Gets the group-level comment configured via <see cref="Comment"/>.
    /// </summary>
    internal string? GroupComment { get; private set; }

    /// <summary>
    /// Sets an XML comment placed before the <c>PropertyGroup</c> element when it is first created.
    /// </summary>
    /// <param name="comment">The comment text.</param>
    public PropertyGroupXmlBuilder Comment(string comment)
    {
        GroupComment = comment;
        return this;
    }

    /// <summary>
    /// Adds a property element. Defaults to <see cref="MergeAction.Add"/> (skip if the property already exists).
    /// </summary>
    /// <param name="name">The property element name (e.g., <c>OutputPath</c>).</param>
    /// <param name="value">The property value.</param>
    /// <param name="comment">Optional XML comment placed before the property element when added.</param>
    public PropertyGroupXmlBuilder Property(string name, string value, string? comment = null)
    {
        _properties.Add((name, value, comment, MergeAction.Add));
        return this;
    }

    /// <summary>
    /// Adds a property element with a specific <see cref="MergeAction"/>.
    /// </summary>
    /// <param name="name">The property element name.</param>
    /// <param name="value">The property value (ignored for <see cref="MergeAction.Remove"/>).</param>
    /// <param name="action">The merge action to apply.</param>
    /// <param name="comment">Optional XML comment placed before the property element when added.</param>
    public PropertyGroupXmlBuilder Property(string name, string value, MergeAction action, string? comment = null)
    {
        _properties.Add((name, value, comment, action));
        return this;
    }

    /// <summary>
    /// Adds a property with a <see cref="MergeAction"/> and no value.
    /// Typically used with <see cref="MergeAction.Remove"/>.
    /// </summary>
    /// <param name="name">The property element name.</param>
    /// <param name="action">The merge action to apply.</param>
    public PropertyGroupXmlBuilder Property(string name, MergeAction action)
    {
        _properties.Add((name, string.Empty, null, action));
        return this;
    }

    /// <summary>
    /// Conditionally adds properties to the group.
    /// The <paramref name="configure"/> callback is invoked only when <paramref name="condition"/> is <see langword="true"/>.
    /// </summary>
    /// <param name="condition">When <see langword="true"/>, the callback is executed.</param>
    /// <param name="configure">A callback that adds properties to this builder.</param>
    public PropertyGroupXmlBuilder If(bool condition, Action<PropertyGroupXmlBuilder> configure)
    {
        if (condition)
            configure(this);
        return this;
    }

    /// <summary>
    /// Conditionally applies one of two sets of properties to the group.
    /// Invokes <paramref name="configure"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or <paramref name="otherwise"/> when it is <see langword="false"/>.
    /// </summary>
    /// <param name="condition">Determines which callback is executed.</param>
    /// <param name="configure">A callback applied when the condition is <see langword="true"/>.</param>
    /// <param name="otherwise">A callback applied when the condition is <see langword="false"/>.</param>
    public PropertyGroupXmlBuilder If(
        bool condition,
        Action<PropertyGroupXmlBuilder> configure,
        Action<PropertyGroupXmlBuilder> otherwise)
    {
        if (condition)
            configure(this);
        else
            otherwise(this);
        return this;
    }

    /// <summary>
    /// Iterates over <paramref name="items"/> and invokes <paramref name="configure"/> for each element.
    /// If <paramref name="items"/> is <see langword="null"/>, this is a no-op.
    /// </summary>
    /// <typeparam name="T">The element type of the collection.</typeparam>
    /// <param name="items">The collection to iterate. May be <see langword="null"/>.</param>
    /// <param name="configure">A callback that receives the builder and the current item.</param>
    public PropertyGroupXmlBuilder WithEach<T>(IEnumerable<T>? items, Action<PropertyGroupXmlBuilder, T> configure)
    {
        if (items is null)
            return this;

        foreach (var item in items)
            configure(this, item);
        return this;
    }

    internal List<(string Name, string Value, string? Comment, MergeAction Action)> Build() => _properties;
}

/// <summary>
/// Represents a declared item entry, including its merge action.
/// </summary>
internal sealed class ItemEntry(
    string itemType,
    string action,
    string pattern,
    XElement element,
    MergeAction mergeAction,
    string? comment)
{
    internal string ItemType => itemType;
    internal string Action => action;
    internal string Pattern => pattern;
    internal XElement Element => element;
    internal MergeAction MergeAction => mergeAction;
    internal string? Comment => comment;
}
