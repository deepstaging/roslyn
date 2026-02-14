// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Extension for discovering user templates from the incremental generator pipeline.
/// </summary>
public static class UserTemplatesExtensions
{
    extension(IncrementalGeneratorInitializationContext context)
    {
        /// <summary>
        /// Discovers user-provided Scriban C# templates from AdditionalTexts.
        /// Filters for <c>.scriban-cs</c> files and creates a pipeline-safe
        /// <see cref="UserTemplates"/> provider.
        /// </summary>
        public IncrementalValueProvider<UserTemplates> UserTemplatesProvider =>
            context.AdditionalTextsProvider
                .Where(static t => t.Path.EndsWith(ScribanExtension.CSharp.Value))
                .Collect()
                .Select(static (texts, _) => UserTemplates.From(texts));
    }
}
