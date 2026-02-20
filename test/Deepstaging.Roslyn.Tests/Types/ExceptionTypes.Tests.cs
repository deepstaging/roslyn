// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class ExceptionTypesTests
{
    [Test]
    public async Task Exception_returns_globally_qualified_type()
    {
        await Assert.That(ExceptionTypes.Exception.Value)
            .IsEqualTo("global::System.Exception");
    }

    [Test]
    public async Task InvalidOperation_returns_globally_qualified_type()
    {
        await Assert.That(ExceptionTypes.InvalidOperation.Value)
            .IsEqualTo("global::System.InvalidOperationException");
    }

    [Test]
    public async Task Argument_returns_globally_qualified_type()
    {
        await Assert.That(ExceptionTypes.Argument.Value)
            .IsEqualTo("global::System.ArgumentException");
    }

    [Test]
    public async Task ArgumentNull_returns_globally_qualified_type()
    {
        await Assert.That(ExceptionTypes.ArgumentNull.Value)
            .IsEqualTo("global::System.ArgumentNullException");
    }

    [Test]
    public async Task ArgumentOutOfRange_returns_globally_qualified_type()
    {
        await Assert.That(ExceptionTypes.ArgumentOutOfRange.Value)
            .IsEqualTo("global::System.ArgumentOutOfRangeException");
    }

    [Test]
    public async Task NotSupported_returns_globally_qualified_type()
    {
        await Assert.That(ExceptionTypes.NotSupported.Value)
            .IsEqualTo("global::System.NotSupportedException");
    }

    [Test]
    public async Task NotImplemented_returns_globally_qualified_type()
    {
        await Assert.That(ExceptionTypes.NotImplemented.Value)
            .IsEqualTo("global::System.NotImplementedException");
    }

    [Test]
    public async Task KeyNotFound_returns_globally_qualified_type()
    {
        await Assert.That(ExceptionTypes.KeyNotFound.Value)
            .IsEqualTo("global::System.Collections.Generic.KeyNotFoundException");
    }

    [Test]
    public async Task ObjectDisposed_returns_globally_qualified_type()
    {
        await Assert.That(ExceptionTypes.ObjectDisposed.Value)
            .IsEqualTo("global::System.ObjectDisposedException");
    }

    [Test]
    public async Task OperationCanceled_returns_globally_qualified_type()
    {
        await Assert.That(ExceptionTypes.OperationCanceled.Value)
            .IsEqualTo("global::System.OperationCanceledException");
    }

    [Test]
    public async Task Format_returns_globally_qualified_type()
    {
        await Assert.That(ExceptionTypes.Format.Value)
            .IsEqualTo("global::System.FormatException");
    }
}
