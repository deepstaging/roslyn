// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit;

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Extension methods for <see cref="OptionalEmit"/> to support user-customizable template definitions.
/// </summary>
public static class CustomizableEmitExtensions
{
    extension(OptionalEmit emit)
    {
        /// <summary>
        /// Defines a user-customizable template for this emit result.
        /// When resolved against <see cref="UserTemplates"/>, a matching user-provided Scriban template
        /// will be rendered with the given model. If no user template exists, the default emit is used.
        /// </summary>
        /// <param name="templateName">
        /// Namespaced template name matching the filesystem convention
        /// (e.g., "Deepstaging.Ids/StrongId" maps to Templates/Deepstaging.Ids/StrongId.scriban-cs).
        /// </param>
        /// <param name="model">
        /// The model object passed to the Scriban template. Should be the same model
        /// used to produce this emit, ensuring template and emit stay in sync.
        /// </param>
        public CustomizableEmit DefineUserTemplate(string templateName, object? model)
        {
            return new CustomizableEmit(emit, templateName, model);
        }
    }
}
