// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit.Generators;

using Projection;
using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Generators;
using Writers;

/// <summary>Generates INotifyPropertyChanged implementations for classes marked with [AutoNotify].</summary>
[Generator]
public sealed class AutoNotifyGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.ForAttribute<AutoNotifyAttribute>()
            .Map((ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryAutoNotify());

        context.RegisterSourceOutput(models, (ctx, model) =>
        {
            model.WriteAutoNotifyClass()
                .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName));
        });
    }
}