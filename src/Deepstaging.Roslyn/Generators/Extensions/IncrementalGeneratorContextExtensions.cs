// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Generators;

/// <summary>
/// Extension methods for simplified symbol discovery and mapping in source generators.
/// </summary>
public static class IncrementalGeneratorContextExtensions
{
    extension(IncrementalGeneratorInitializationContext context)
    {
        /// <summary>
        /// Starts a fluent mapping chain for an attribute. The model type is inferred from the builder.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type to map</typeparam>
        /// <returns>A fluent mapping builder</returns>
        public AttributeMapper ForAttribute<TAttribute>() where TAttribute : Attribute
        {
            return new AttributeMapper(context, typeof(TAttribute).FullName!);
        }

        /// <summary>
        /// Starts a fluent mapping chain for an attribute by name. The model type is inferred from the builder.
        /// </summary>
        /// <param name="fullyQualifiedAttributeName">Fully qualified name of the attribute (e.g., "MyApp.MyAttribute")</param>
        /// <returns>A fluent mapping builder</returns>
        public AttributeMapper ForAttribute(string fullyQualifiedAttributeName)
        {
            return new AttributeMapper(context, fullyQualifiedAttributeName);
        }

        /// <summary>
        /// Maps types from the compilation using a query builder pattern.
        /// Useful for discovering types without requiring attributes.
        /// </summary>
        /// <typeparam name="TModel">The model type to transform symbols into</typeparam>
        /// <param name="selector">Function to select and transform types from compilation</param>
        /// <returns>Incremental values provider of models</returns>
        public IncrementalValuesProvider<TModel> MapTypes<TModel>(
            Func<Compilation, CancellationToken, IEnumerable<TModel>> selector)
        {
            return context.CompilationProvider.SelectMany(selector);
        }

        /// <summary>
        /// Registers code generation for each model in the provided values provider.
        /// </summary>
        /// <param name="models">The incremental values provider containing models to process</param>
        /// <param name="produce">Action to generate code for each model</param>
        /// <param name="onError">Optional error handler for generation failures</param>
        public void ForEach<TModel>(
            IncrementalValuesProvider<TModel> models,
            Action<SourceProductionContext, TModel> produce,
            Action<SourceProductionContext, TModel, Exception>? onError = null)
        {
            context.RegisterImplementationSourceOutput(models.Collect(), (ctx, array) =>
            {
                foreach (var model in array)
                    try
                    {
                        produce(ctx, model);
                    }
                    catch (Exception ex)
                    {
                        if (onError != null)
                            onError(ctx, model, ex);
                        else
                            ReportDefaultError(ctx, model, ex);
                    }
            });
        }
    }

    /// <summary>
    /// Fluent builder for attribute mapping. Model type is inferred from the builder function.
    /// </summary>
    public readonly struct AttributeMapper
    {
        private readonly IncrementalGeneratorInitializationContext _context;
        private readonly string _attributeName;

        internal AttributeMapper(IncrementalGeneratorInitializationContext context, string attributeName)
        {
            _context = context;
            _attributeName = attributeName;
        }

        /// <summary>
        /// Maps to models using the builder function. Model type is inferred automatically.
        /// </summary>
        public IncrementalValuesProvider<TModel> Map<TModel>(
            Func<GeneratorAttributeSyntaxContext, CancellationToken, TModel?> builder)
        {
            return MapAttribute(_attributeName, builder);
        }

        /// <summary>
        /// Maps to models using the builder function, with a custom syntax predicate.
        /// </summary>
        public IncrementalValuesProvider<TModel> Where<TModel>(
            Func<SyntaxNode, CancellationToken, bool> syntaxPredicate,
            Func<GeneratorAttributeSyntaxContext, CancellationToken, TModel?> builder)
        {
            return MapAttribute(_attributeName, builder, syntaxPredicate);
        }

        private IncrementalValuesProvider<TModel> MapAttribute<TModel>(string fullyQualifiedAttributeName,
            Func<GeneratorAttributeSyntaxContext, CancellationToken, TModel?> builder,
            Func<SyntaxNode, CancellationToken, bool>? syntaxPredicate = null)
        {
            return _context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    fullyQualifiedAttributeName,
                    syntaxPredicate ?? (static (_, _) => true),
                    builder)
                .Where(static model => model is not null)
                .Select(static (model, _) => model!);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="model"></param>
    /// <param name="exception"></param>
    /// <typeparam name="TModel"></typeparam>
    public static void ReportDefaultError<TModel>(this SourceProductionContext context, TModel model,
        Exception exception)
    {
        var descriptor = new DiagnosticDescriptor(
            "DEEPGEN001",
            "Code generation error",
            "Error generating code for {0}: {1}",
            "Deepstaging.CodeGeneration",
            DiagnosticSeverity.Error,
            true);

        context.ReportDiagnostic(Diagnostic.Create(
            descriptor,
            Location.None,
            model?.GetType().Name ?? "UnknownModel",
            exception.Message));
    }
}