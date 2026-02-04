// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Represents the result of a Scriban template rendering operation.
/// Can be either a <see cref="Success"/> containing the rendered text, or a <see cref="Failure"/> containing diagnostic information.
/// </summary>
public abstract record RenderResult
{
    /// <summary>
    /// Gets or initializes the name of the template that was rendered.
    /// </summary>
    public required string TemplateName { get; init; }

    /// <summary>
    /// Represents a successful template render operation.
    /// </summary>
    /// <param name="Text">The rendered template text.</param>
    /// <param name="Context">The context object used during rendering.</param>
    public sealed record Success(string Text, object? Context) : RenderResult;

    /// <summary>
    /// Represents a failed template render operation.
    /// </summary>
    /// <param name="Diagnostic">The diagnostic information describing the failure.</param>
    public sealed record Failure(Diagnostic Diagnostic) : RenderResult;
}
