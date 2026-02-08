// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Disposal;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Disposal;

/// <summary>
/// Tests for TypeBuilder IDisposable interface implementation extensions.
/// </summary>
public class TypeBuilderIDisposableExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task IDisposable_generates_full_dispose_pattern()
    {
        var result = TypeBuilder
            .Class("ResourceManager")
            .InNamespace("Test")
            .ImplementsIDisposable("_connection?.Close();", "_stream?.Dispose();")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IDisposable");
        await Assert.That(result.Code).Contains("bool _disposed");
        await Assert.That(result.Code).Contains("void Dispose()");
        await Assert.That(result.Code).Contains("Dispose(true)");
        await Assert.That(result.Code).Contains("SuppressFinalize");
        await Assert.That(result.Code).Contains("Dispose(bool disposing)");
        await Assert.That(result.Code).Contains("if (_disposed)");
        await Assert.That(result.Code).Contains("if (disposing)");
        await Assert.That(result.Code).Contains("_connection?.Close();");
        await Assert.That(result.Code).Contains("_stream?.Dispose();");
    }

    [Test]
    public async Task IDisposable_for_single_field()
    {
        var result = TypeBuilder
            .Class("FileWrapper")
            .InNamespace("Test")
            .AddField(FieldBuilder.Parse("private readonly global::System.IO.Stream _stream;"))
            .ImplementsIDisposableForField("_stream")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("_stream?.Dispose();");
    }

    [Test]
    public async Task IDisposable_for_multiple_fields()
    {
        var result = TypeBuilder
            .Class("MultiResourceManager")
            .InNamespace("Test")
            .ImplementsIDisposableForFields("_reader", "_writer", "_connection")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("_reader?.Dispose();");
        await Assert.That(result.Code).Contains("_writer?.Dispose();");
        await Assert.That(result.Code).Contains("_connection?.Dispose();");
    }
}
