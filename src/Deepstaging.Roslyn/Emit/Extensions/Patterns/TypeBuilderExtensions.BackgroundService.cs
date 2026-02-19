// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Patterns;

using Types;

/// <summary>
/// TypeBuilder extensions for generating <c>BackgroundService</c> subclasses.
/// Provides a fluent API to scaffold the <c>ExecuteAsync</c> override, constructor injection,
/// and dispose pattern commonly needed for hosted services.
/// </summary>
public static class TypeBuilderBackgroundServiceExtensions
{
    /// <summary>
    /// Configures the type as a <c>BackgroundService</c> subclass with an <c>ExecuteAsync</c> override.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="executeBody">
    /// A function that builds the body of the <c>ExecuteAsync</c> method.
    /// The <c>CancellationToken</c> parameter is named <c>stoppingToken</c>.
    /// </param>
    /// <returns>The modified type builder inheriting <c>BackgroundService</c> with the override method.</returns>
    /// <example>
    /// <code>
    /// TypeBuilder.Class("MyWorker")
    ///     .AsSealed()
    ///     .AsBackgroundService(body => body
    ///         .AddStatement("await foreach (var item in _channel.Reader.ReadAllAsync(stoppingToken))")
    ///         .AddStatement("{")
    ///         .AddStatement("    await ProcessItemAsync(item);")
    ///         .AddStatement("}"));
    /// </code>
    /// </example>
    public static TypeBuilder AsBackgroundService(
        this TypeBuilder builder,
        Func<BodyBuilder, BodyBuilder> executeBody) => builder
        .Implements(HostingTypes.BackgroundService)
        .AddMethod("ExecuteAsync", m => m
            .WithAccessibility(Accessibility.Protected)
            .AsOverride()
            .Async()
            .WithReturnType("global::System.Threading.Tasks.Task")
            .AddParameter("stoppingToken", "global::System.Threading.CancellationToken")
            .WithBody(executeBody));

    /// <summary>
    /// Configures the type as a <c>BackgroundService</c> subclass with an expression-bodied
    /// <c>ExecuteAsync</c> override.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="expression">The expression body for <c>ExecuteAsync</c>.</param>
    /// <returns>The modified type builder.</returns>
    /// <example>
    /// <code>
    /// TypeBuilder.Class("PingWorker")
    ///     .AsSealed()
    ///     .AsBackgroundService("RunLoopAsync(stoppingToken)");
    /// </code>
    /// </example>
    public static TypeBuilder AsBackgroundService(
        this TypeBuilder builder,
        string expression) => builder
        .Implements(HostingTypes.BackgroundService)
        .AddMethod("ExecuteAsync", m => m
            .WithAccessibility(Accessibility.Protected)
            .AsOverride()
            .Async()
            .WithReturnType("global::System.Threading.Tasks.Task")
            .AddParameter("stoppingToken", "global::System.Threading.CancellationToken")
            .WithExpressionBody(expression));

    /// <summary>
    /// Overrides <c>Dispose(bool disposing)</c> on the <c>BackgroundService</c>.
    /// Calls <c>base.Dispose(disposing)</c> after the user-provided statements.
    /// </summary>
    /// <param name="builder">The type builder (must already be a BackgroundService).</param>
    /// <param name="disposeStatements">Statements to execute before calling base.Dispose.</param>
    /// <returns>The modified type builder with a Dispose override.</returns>
    public static TypeBuilder WithDisposeOverride(
        this TypeBuilder builder,
        params string[] disposeStatements) => builder
        .AddMethod("Dispose", m =>
        {
            var method = m
                .WithAccessibility(Accessibility.Public)
                .AsOverride()
                .WithReturnType("void")
                .AddParameter("disposing", "bool");

            return method.WithBody(body =>
            {
                foreach (var statement in disposeStatements)
                    body = body.AddStatement(statement);

                return body.AddStatement("base.Dispose(disposing);");
            });
        });
}
