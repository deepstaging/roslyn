// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Xml.Linq;

namespace Deepstaging.Roslyn;

/// <summary>
/// Base class for a managed MSBuild <c>.props</c> file that generators and code fixes can
/// read from and write to. Subclasses declare a file name and default contents; the framework
/// ensures those defaults exist whenever the file is modified.
/// </summary>
/// <remarks>
/// <para>
/// Usage pattern:
/// <list type="number">
///   <item>Subclass <see cref="ManagedPropsFile"/> and override <see cref="FileName"/>
///         and <see cref="ConfigureDefaults"/>.</item>
///   <item>In NuGet <c>build/*.props</c>, auto-import the local file:
///         <c>&lt;Import Project="$(MSBuildProjectDirectory)/myfile.props"
///              Condition="Exists('$(MSBuildProjectDirectory)/myfile.props')"/&gt;</c></item>
///   <item>In code fixes, use <c>project.ModifyPropsFile&lt;T&gt;(doc =&gt; ...)</c> or
///         <c>builder.ModifyPropsFile&lt;T&gt;(doc =&gt; ...)</c>.</item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public sealed class MyGeneratorProps : ManagedPropsFile
/// {
///     public override string FileName => "mygenerator.props";
///
///     protected override void ConfigureDefaults(PropsBuilder builder) =>
///         builder
///             .Property("CompilerGeneratedFilesOutputPath", "!generated")
///             .ItemGroup(items =>
///             {
///                 items.Remove("Compile", "!generated/**");
///                 items.Include("None", "!generated/**");
///             });
/// }
/// </code>
/// </example>
public abstract class ManagedPropsFile
{
    private PropsBuilder? _builder;

    /// <summary>
    /// The file name for the props file, relative to the project directory (e.g., "deepstaging.props").
    /// </summary>
    public abstract string FileName { get; }

    /// <summary>
    /// Declares the default properties and item groups for this props file.
    /// </summary>
    protected abstract void ConfigureDefaults(PropsBuilder builder);

    /// <summary>
    /// Ensures all declared defaults exist in the given XML document.
    /// Called automatically before any modification via <c>ModifyPropsFile</c>.
    /// </summary>
    public void EnsureDefaults(XDocument document)
    {
        var root = document.Root;
        if (root is null) return;

        var builder = GetBuilder();

        foreach (var propertyGroup in builder.PropertyGroups)
            EnsurePropertyGroup(root, propertyGroup);

        foreach (var itemGroup in builder.ItemGroups)
            EnsureItemGroup(root, itemGroup);
    }

    private PropsBuilder GetBuilder()
    {
        if (_builder is not null)
            return _builder;

        _builder = new PropsBuilder();
        ConfigureDefaults(_builder);
        return _builder;
    }

    private static void EnsurePropertyGroup(XElement root, PropsPropertyGroup group)
    {
        if (group.Properties.Count == 0) return;

        var propertyGroup = FindOrCreateGroup(root, "PropertyGroup", group.Label);

        foreach (var (name, value) in group.Properties)
        {
            if (propertyGroup.Element(name) is null)
                propertyGroup.Add(new XElement(name, value));
        }
    }

    private static void EnsureItemGroup(XElement root, PropsItemGroup itemGroup)
    {
        var identity = itemGroup.Identity;

        // Check if an ItemGroup with matching label and identity already exists
        var candidates = root.Elements("ItemGroup")
            .Where(ig => (string?)ig.Attribute("Label") == itemGroup.Label);

        var exists = candidates
            .Any(ig => ig.Elements(identity.ItemType)
                .Any(el => (string?)el.Attribute(identity.Action) == identity.Pattern));

        if (exists) return;

        var element = FindOrCreateGroup(root, "ItemGroup", itemGroup.Label);

        foreach (var item in itemGroup.Items)
        {
            var itemElement = new XElement(item.ItemType, new XAttribute(item.Action, item.Pattern));

            if (item.Metadata is { Count: > 0 })
            {
                foreach (var (name, value) in item.Metadata)
                    itemElement.Add(new XElement(name, value));
            }

            element.Add(itemElement);
        }
    }

    private static XElement FindOrCreateGroup(XElement root, string groupName, string? label)
    {
        // Find existing group with matching label
        var group = root.Elements(groupName)
            .FirstOrDefault(g => (string?)g.Attribute("Label") == label);

        if (group is not null) return group;

        // Create new group
        group = new XElement(groupName);

        if (label is not null)
            group.Add(new XAttribute("Label", label));

        root.Add(group);

        return group;
    }
}
