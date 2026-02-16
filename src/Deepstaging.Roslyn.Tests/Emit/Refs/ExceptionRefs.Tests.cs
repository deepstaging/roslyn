// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class ExceptionRefsTests
{
    [Test]
    public async Task ArgumentNull_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.ArgumentNull;

        await Assert.That((string)typeRef).IsEqualTo("global::System.ArgumentNullException");
    }

    [Test]
    public async Task Argument_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.Argument;

        await Assert.That((string)typeRef).IsEqualTo("global::System.ArgumentException");
    }

    [Test]
    public async Task ArgumentOutOfRange_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.ArgumentOutOfRange;

        await Assert.That((string)typeRef).IsEqualTo("global::System.ArgumentOutOfRangeException");
    }

    [Test]
    public async Task InvalidOperation_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.InvalidOperation;

        await Assert.That((string)typeRef).IsEqualTo("global::System.InvalidOperationException");
    }

    [Test]
    public async Task InvalidCast_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.InvalidCast;

        await Assert.That((string)typeRef).IsEqualTo("global::System.InvalidCastException");
    }

    [Test]
    public async Task Format_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.Format;

        await Assert.That((string)typeRef).IsEqualTo("global::System.FormatException");
    }

    [Test]
    public async Task NotSupported_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.NotSupported;

        await Assert.That((string)typeRef).IsEqualTo("global::System.NotSupportedException");
    }

    [Test]
    public async Task NotImplemented_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.NotImplemented;

        await Assert.That((string)typeRef).IsEqualTo("global::System.NotImplementedException");
    }
}
