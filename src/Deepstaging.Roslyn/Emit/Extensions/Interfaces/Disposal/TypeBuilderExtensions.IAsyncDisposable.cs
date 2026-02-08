// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Disposal;

/// <summary>
/// TypeBuilder extensions for implementing IAsyncDisposable (NET Core 3.0+ / NET Standard 2.1+).
/// </summary>
public static class TypeBuilderAsyncDisposableExtensions
{
    /// <summary>
    /// Implements IAsyncDisposable with the async dispose pattern.
    /// Generates: DisposeAsync(), DisposeAsyncCore(), disposed field.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="disposeStatements">Async statements to execute when disposing.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIAsyncDisposable(
        this TypeBuilder builder,
        params string[] disposeStatements)
    {
        return builder
            .Implements("global::System.IAsyncDisposable", Directives.NetCoreApp30OrGreater)
            .AddField(FieldBuilder.Parse("private bool _disposed").When(Directives.NetCoreApp30OrGreater))
            .AddMethod(BuildDisposeAsyncMethod())
            .AddMethod(BuildDisposeAsyncCoreMethod(disposeStatements));
    }

    /// <summary>
    /// Implements IAsyncDisposable by disposing a single async-disposable field.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="fieldName">The async-disposable field to dispose.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIAsyncDisposableForField(
        this TypeBuilder builder,
        string fieldName) =>
        builder.ImplementsIAsyncDisposable($"if ({fieldName} is not null) await {fieldName}.DisposeAsync().ConfigureAwait(false);");

    /// <summary>
    /// Implements both IDisposable and IAsyncDisposable with proper coordination.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="syncDisposeStatements">Statements for synchronous dispose.</param>
    /// <param name="asyncDisposeStatements">Statements for async dispose.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIDisposableAndIAsyncDisposable(
        this TypeBuilder builder,
        string[] syncDisposeStatements,
        string[] asyncDisposeStatements) =>
        builder
            .ImplementsIDisposable(syncDisposeStatements)
            .ImplementsIAsyncDisposable(asyncDisposeStatements);

    private static MethodBuilder BuildDisposeAsyncMethod() =>
        MethodBuilder
            .Parse("public async global::System.Threading.Tasks.ValueTask DisposeAsync()")
            .When(Directives.NetCoreApp30OrGreater)
            .WithBody(b => b
                .AddStatement("await DisposeAsyncCore().ConfigureAwait(false);")
                .AddStatement("Dispose(false);")
                .AddStatement("global::System.GC.SuppressFinalize(this);"));

    private static MethodBuilder BuildDisposeAsyncCoreMethod(string[] userStatements)
    {
        return MethodBuilder
            .Parse("protected virtual async global::System.Threading.Tasks.ValueTask DisposeAsyncCore()")
            .When(Directives.NetCoreApp30OrGreater)
            .WithBody(b =>
            {
                b = b.AddStatement("if (_disposed) return;");
                foreach (var statement in userStatements)
                {
                    b = b.AddStatement(statement);
                }
                b = b.AddStatement("");
                b = b.AddStatement("_disposed = true;");
                return b;
            });
    }
}
