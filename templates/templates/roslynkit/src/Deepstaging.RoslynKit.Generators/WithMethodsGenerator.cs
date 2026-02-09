// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Generators;
using Deepstaging.RoslynKit.Generators.Writers;
using Deepstaging.RoslynKit.Projection;
using Microsoft.CodeAnalysis;

namespace Deepstaging.RoslynKit.Generators;

/// <summary>
/// Incremental source generator that generates immutable With*() methods
/// for classes marked with <see cref="GenerateWithAttribute"/>.
/// </summary>
[Generator]
public sealed class WithMethodsGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find all types with [GenerateWith] attribute and map to model
        var models = context.ForAttribute<GenerateWithAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryWithMethods());

        // Register source output for each model
        context.RegisterSourceOutput(models, static (ctx, model) => model
            .WriteWithMethods()
            .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName)));
    }
}