// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Generators;
using Deepstaging.RoslynKit.Generators.Writers;
using Deepstaging.RoslynKit.Projection;
using Microsoft.CodeAnalysis;

namespace Deepstaging.RoslynKit.Generators;

/// <summary>
/// Incremental source generator that generates INotifyPropertyChanged implementation
/// for classes marked with <see cref="AutoNotifyAttribute"/>.
/// </summary>
[Generator]
public sealed class AutoNotifyGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.ForAttribute<AutoNotifyAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryAutoNotify());
        
        context.RegisterSourceOutput(models, static (ctx, model) => model
            .WriteAutoNotifyClass()
            .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName)));
    }
}