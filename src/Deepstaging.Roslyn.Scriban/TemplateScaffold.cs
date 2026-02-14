// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Generates a Scriban template scaffold from a <see cref="CustomizableEmit"/>.
/// Replaces explicitly-bound model property values in the default emit output
/// with Scriban template placeholders (e.g., <c>CustomerId</c> → <c>{{ TypeName }}</c>).
/// The result is a starting-point template that reproduces the default output.
/// </summary>
/// <remarks>
/// Only values recorded via <see cref="TemplateMap{TModel}.Bind{T}"/> become placeholders.
/// This gives generator authors precise control over which parts of the output are customizable.
/// Replacements are applied longest-value-first to prevent partial matches
/// (e.g., "TestApp.OrderId" is replaced before "OrderId").
/// </remarks>
public static class TemplateScaffold
{
    /// <summary>
    /// Generates a Scriban template that would reproduce the default emit output
    /// when rendered with the given model.
    /// </summary>
    /// <param name="emit">The customizable emit containing the default output and bindings.</param>
    /// <returns>
    /// The scaffold template text, or <c>null</c> if the default emit is not valid.
    /// </returns>
    public static string? Generate(CustomizableEmit emit)
    {
        if (emit.DefaultEmit.IsNotValid(out var valid))
            return null;

        if (emit.Bindings.Count == 0)
            return valid.Code;

        // Sort by value length descending to avoid partial replacements
        // (e.g., "TestApp.OrderId" before "OrderId")
        var sorted = emit.Bindings
            .OrderByDescending(b => b.Value.Length)
            .ToList();

        var result = valid.Code;
        foreach (var binding in sorted)
            result = result.Replace(binding.Value, $"{{{{ {binding.PropertyPath} }}}}");

        return result;
    }
}
