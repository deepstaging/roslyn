// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for <see cref="ManagedPropsFile"/> integration with code fix actions.
/// </summary>
public static class ManagedPropsFileExtensions
{
    /// <summary>
    /// Creates a code action that modifies a <see cref="ManagedPropsFile"/>,
    /// ensuring defaults exist before applying the modification.
    /// </summary>
    /// <typeparam name="TProps">The managed props file type.</typeparam>
    /// <param name="project">The project to modify.</param>
    /// <param name="title">The title for the code action.</param>
    /// <param name="modify">Action to modify the XML document after defaults are ensured.</param>
    public static CodeAction ModifyPropsFileAction<TProps>(this Project project, string title, Action<XDocument> modify)
        where TProps : ManagedPropsFile, new() =>
        project.ModifyPropsFileAction<TProps>(title, null, modify);

    /// <summary>
    /// Creates a code action that modifies a <see cref="ManagedPropsFile"/> in a subdirectory,
    /// ensuring defaults exist before applying the modification.
    /// </summary>
    /// <typeparam name="TProps">The managed props file type.</typeparam>
    /// <param name="project">The project to modify.</param>
    /// <param name="title">The title for the code action.</param>
    /// <param name="directory">Optional directory relative to the project root (e.g., <c>.config</c>).</param>
    /// <param name="modify">Action to modify the XML document after defaults are ensured.</param>
    public static CodeAction ModifyPropsFileAction<TProps>(
        this Project project,
        string title,
        string? directory,
        Action<XDocument> modify)
        where TProps : ManagedPropsFile, new()
    {
        var props = new TProps();

        return project.ModifyXmlFileAction(title, ResolvePath(props, directory), doc =>
        {
            props.EnsureDefaults(doc);
            modify(doc);
        });
    }

    /// <summary>
    /// Modifies a <see cref="ManagedPropsFile"/>, ensuring defaults exist before
    /// applying the modification.
    /// </summary>
    /// <typeparam name="TProps">The managed props file type.</typeparam>
    /// <param name="builder">The file actions builder.</param>
    /// <param name="modify">Action to modify the XML document after defaults are ensured.</param>
    public static ProjectFileActionsBuilder ModifyPropsFile<TProps>(
        this ProjectFileActionsBuilder builder,
        Action<XDocument> modify)
        where TProps : ManagedPropsFile, new() =>
        builder.ModifyPropsFile<TProps>(null, modify);

    /// <summary>
    /// Modifies a <see cref="ManagedPropsFile"/> in a subdirectory, ensuring defaults exist before
    /// applying the modification.
    /// </summary>
    /// <typeparam name="TProps">The managed props file type.</typeparam>
    /// <param name="builder">The file actions builder.</param>
    /// <param name="directory">Optional directory relative to the project root (e.g., <c>.config</c>).</param>
    /// <param name="modify">Action to modify the XML document after defaults are ensured.</param>
    public static ProjectFileActionsBuilder ModifyPropsFile<TProps>(
        this ProjectFileActionsBuilder builder,
        string? directory,
        Action<XDocument> modify)
        where TProps : ManagedPropsFile, new()
    {
        var props = new TProps();

        return builder.ModifyXmlFile(ResolvePath(props, directory), doc =>
        {
            props.EnsureDefaults(doc);
            modify(doc);
        });
    }

    private static string ResolvePath(ManagedPropsFile props, string? directory) =>
        directory is null ? props.FileName : $"{directory}/{props.FileName}";
}