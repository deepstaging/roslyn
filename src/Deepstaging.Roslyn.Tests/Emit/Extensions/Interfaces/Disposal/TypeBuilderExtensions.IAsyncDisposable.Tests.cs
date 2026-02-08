// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Disposal;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Disposal;

/// <summary>
/// Tests for TypeBuilder IAsyncDisposable interface implementation extensions.
/// </summary>
public class TypeBuilderIAsyncDisposableExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task IAsyncDisposable_generates_async_dispose_pattern()
    {
        var result = TypeBuilder
            .Class("AsyncResourceManager")
            .InNamespace("Test")
            .ImplementsIAsyncDisposable("await _connection.CloseAsync().ConfigureAwait(false);")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if NETCOREAPP3_0_OR_GREATER");
        await Assert.That(result.Code).Contains("IAsyncDisposable");
        await Assert.That(result.Code).Contains("ValueTask DisposeAsync()");
        await Assert.That(result.Code).Contains("await DisposeAsyncCore().ConfigureAwait(false)");
        await Assert.That(result.Code).Contains("SuppressFinalize");
        await Assert.That(result.Code).Contains("ValueTask DisposeAsyncCore()");
        await Assert.That(result.Code).Contains("await _connection.CloseAsync()");
    }

    [Test]
    public async Task IAsyncDisposable_for_single_field()
    {
        var result = TypeBuilder
            .Class("AsyncFileWrapper")
            .InNamespace("Test")
            .ImplementsIAsyncDisposableForField("_asyncStream")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("await _asyncStream.DisposeAsync().ConfigureAwait(false)");
    }

    [Test]
    public async Task IDisposable_and_IAsyncDisposable_together()
    {
        var result = TypeBuilder
            .Class("DualDisposableResource")
            .InNamespace("Test")
            .ImplementsIDisposableAndIAsyncDisposable(
                ["_stream?.Dispose();"],
                ["await _stream.DisposeAsync().ConfigureAwait(false);"])
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IDisposable");
        await Assert.That(result.Code).Contains("IAsyncDisposable");
        await Assert.That(result.Code).Contains("public void Dispose()");
        await Assert.That(result.Code).Contains("DisposeAsync()");
    }
}
