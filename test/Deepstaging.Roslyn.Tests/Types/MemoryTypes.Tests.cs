// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class MemoryTypesTests
{
    [Test]
    public async Task Span_creates_globally_qualified_type()
    {
        await Assert.That((string)MemoryTypes.Span("byte"))
            .IsEqualTo("global::System.Span<byte>");
    }

    [Test]
    public async Task ReadOnlySpan_creates_globally_qualified_type()
    {
        await Assert.That((string)MemoryTypes.ReadOnlySpan("char"))
            .IsEqualTo("global::System.ReadOnlySpan<char>");
    }

    [Test]
    public async Task Memory_creates_globally_qualified_type()
    {
        await Assert.That((string)MemoryTypes.Memory("byte"))
            .IsEqualTo("global::System.Memory<byte>");
    }

    [Test]
    public async Task ReadOnlyMemory_creates_globally_qualified_type()
    {
        await Assert.That((string)MemoryTypes.ReadOnlyMemory("int"))
            .IsEqualTo("global::System.ReadOnlyMemory<int>");
    }
}
