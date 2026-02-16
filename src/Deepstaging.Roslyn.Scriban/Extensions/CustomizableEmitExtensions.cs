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
        /// Defines a user-customizable template for this emit result using explicit property bindings.
        /// When resolved against <see cref="UserTemplates"/>, a matching user-provided Scriban template
        /// will be rendered with the given model. If no user template exists, the default emit is used.
        /// </summary>
        /// <typeparam name="TModel">The pipeline model type.</typeparam>
        /// <param name="templateName">
        /// Namespaced template name matching the filesystem convention
        /// (e.g., "Deepstaging.Ids/StrongId" maps to Templates/Deepstaging.Ids/StrongId.scriban-cs).
        /// </param>
        /// <param name="model">
        /// The model object passed to the Scriban template. Should be the same model
        /// used to produce this emit, ensuring template and emit stay in sync.
        /// </param>
        /// <param name="map">
        /// The template map containing explicit property bindings recorded during emit construction.
        /// These bindings define which model values become template placeholders when scaffolding.
        /// </param>
        public CustomizableEmit WithUserTemplate<TModel>(
            string templateName,
            TModel model,
            TemplateMap<TModel> map) =>
            new(emit, templateName, model, map.Bindings);

        /// <summary>
        /// Defines a user-customizable template for this emit result without explicit bindings.
        /// Use this overload when template scaffolding is not needed — only user template override.
        /// </summary>
        /// <param name="templateName">
        /// Namespaced template name matching the filesystem convention
        /// (e.g., "Deepstaging.Ids/StrongId" maps to Templates/Deepstaging.Ids/StrongId.scriban-cs).
        /// </param>
        /// <param name="model">
        /// The model object passed to the Scriban template.
        /// </param>
        public CustomizableEmit WithUserTemplate(string templateName, object? model) => new(emit, templateName, model);
    }
}