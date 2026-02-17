// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class ExceptionRefsTests
{
    [Test]
    public async Task ArgumentNull_creates_globally_qualified_type()
    {
        var typeRef = ExceptionRefs.ArgumentNull;

        await Assert.That(typeRef).IsEqualTo("global::System.ArgumentNullException");
    }

    [Test]
    public async Task Argument_creates_globally_qualified_type()
    {
        var typeRef = ExceptionRefs.Argument;

        await Assert.That(typeRef).IsEqualTo("global::System.ArgumentException");
    }

    [Test]
    public async Task ArgumentOutOfRange_creates_globally_qualified_type()
    {
        var typeRef = ExceptionRefs.ArgumentOutOfRange;

        await Assert.That(typeRef).IsEqualTo("global::System.ArgumentOutOfRangeException");
    }

    [Test]
    public async Task InvalidOperation_creates_globally_qualified_type()
    {
        var typeRef = ExceptionRefs.InvalidOperation;

        await Assert.That(typeRef).IsEqualTo("global::System.InvalidOperationException");
    }

    [Test]
    public async Task InvalidCast_creates_globally_qualified_type()
    {
        var typeRef = ExceptionRefs.InvalidCast;

        await Assert.That(typeRef).IsEqualTo("global::System.InvalidCastException");
    }

    [Test]
    public async Task Format_creates_globally_qualified_type()
    {
        var typeRef = ExceptionRefs.Format;

        await Assert.That(typeRef).IsEqualTo("global::System.FormatException");
    }

    [Test]
    public async Task NotSupported_creates_globally_qualified_type()
    {
        var typeRef = ExceptionRefs.NotSupported;

        await Assert.That(typeRef).IsEqualTo("global::System.NotSupportedException");
    }

    [Test]
    public async Task NotImplemented_creates_globally_qualified_type()
    {
        var typeRef = ExceptionRefs.NotImplemented;

        await Assert.That(typeRef).IsEqualTo("global::System.NotImplementedException");
    }
}