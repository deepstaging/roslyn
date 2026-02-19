// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class SpanTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new SpanTypeRef("byte");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Span<byte>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new SpanTypeRef("char");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("char");
    }

    [Test]
    public async Task Implicitly_converts_to_TypeRef()
    {
        TypeRef typeRef = new SpanTypeRef("int");

        await Assert.That(typeRef.Value)
            .IsEqualTo("global::System.Span<int>");
    }
}

public class ReadOnlySpanTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ReadOnlySpanTypeRef("byte");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.ReadOnlySpan<byte>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new ReadOnlySpanTypeRef("char");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("char");
    }
}

public class MemoryTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new MemoryTypeRef("byte");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Memory<byte>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new MemoryTypeRef("int");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("int");
    }
}

public class ReadOnlyMemoryTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ReadOnlyMemoryTypeRef("byte");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.ReadOnlyMemory<byte>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new ReadOnlyMemoryTypeRef("char");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("char");
    }
}

public class ImmutableListTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new ImmutableListTypeRef("string");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Collections.Immutable.ImmutableList<string>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new ImmutableListTypeRef("Order");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("Order");
    }
}
