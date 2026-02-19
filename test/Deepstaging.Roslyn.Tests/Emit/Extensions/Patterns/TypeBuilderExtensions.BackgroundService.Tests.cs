// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Patterns;

namespace Deepstaging.Roslyn.Tests.Emit.Patterns;

public class TypeBuilderBackgroundServiceTests : RoslynTestBase
{
    [Test]
    public async Task AsBackgroundService_inherits_and_overrides_ExecuteAsync()
    {
        var result = TypeBuilder
            .Class("MyWorker")
            .InNamespace("Test")
            .AsSealed()
            .AsBackgroundService(body => body
                .AddStatement("await Task.Delay(1000, stoppingToken)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("BackgroundService");
        await Assert.That(result.Code).Contains("async override");
        await Assert.That(result.Code).Contains("Task ExecuteAsync");
        await Assert.That(result.Code).Contains("CancellationToken stoppingToken");
        await Assert.That(result.Code).Contains("await Task.Delay(1000, stoppingToken)");
    }

    [Test]
    public async Task AsBackgroundService_expression_body()
    {
        var result = TypeBuilder
            .Class("PingWorker")
            .InNamespace("Test")
            .AsSealed()
            .AsBackgroundService("RunLoopAsync(stoppingToken)")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("BackgroundService");
        await Assert.That(result.Code).Contains("RunLoopAsync(stoppingToken)");
    }

    [Test]
    public async Task AsBackgroundService_with_constructor_and_fields()
    {
        var result = TypeBuilder
            .Class("QueueWorker")
            .InNamespace("Test")
            .AsSealed()
            .AddField("_logger", "global::Microsoft.Extensions.Logging.ILogger<QueueWorker>", f => f.AsReadonly())
            .AddConstructor(c => c
                .WithAccessibility(Accessibility.Public)
                .AddParameter("logger", "global::Microsoft.Extensions.Logging.ILogger<QueueWorker>")
                .WithBody(b => b.AddStatement("_logger = logger")))
            .AsBackgroundService(body => body
                .AddStatement("_logger.LogInformation(\"Starting...\")"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("BackgroundService");
        await Assert.That(result.Code).Contains("readonly");
        await Assert.That(result.Code).Contains("_logger = logger");
    }

    [Test]
    public async Task WithDisposeOverride_adds_dispose_method()
    {
        var result = TypeBuilder
            .Class("ChannelWorker")
            .InNamespace("Test")
            .AsSealed()
            .AsBackgroundService(body => body
                .AddStatement("await Task.CompletedTask"))
            .WithDisposeOverride(
                "_channel.Writer.Complete();",
                "_cts.Cancel();",
                "_cts.Dispose();")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("override void Dispose(bool disposing)");
        await Assert.That(result.Code).Contains("_channel.Writer.Complete()");
        await Assert.That(result.Code).Contains("_cts.Cancel()");
        await Assert.That(result.Code).Contains("base.Dispose(disposing)");
    }
}
