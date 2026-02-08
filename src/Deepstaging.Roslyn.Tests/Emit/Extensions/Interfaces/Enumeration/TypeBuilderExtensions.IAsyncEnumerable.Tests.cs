// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Enumeration;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Enumeration;

/// <summary>
/// Tests for TypeBuilder IAsyncEnumerable interface implementation extensions.
/// </summary>
public class TypeBuilderIAsyncEnumerableExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task IAsyncEnumerable_delegates_to_inner_collection()
    {
        var result = TypeBuilder
            .Class("AsyncItemStream")
            .InNamespace("Test")
            .ImplementsIAsyncEnumerable("Item", "_asyncItems")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#if NETCOREAPP3_0_OR_GREATER");
        await Assert.That(result.Code).Contains("IAsyncEnumerable<Item>");
        await Assert.That(result.Code).Contains("IAsyncEnumerator<Item> GetAsyncEnumerator");
        await Assert.That(result.Code).Contains("CancellationToken cancellationToken = default");
        await Assert.That(result.Code).Contains("_asyncItems.GetAsyncEnumerator(cancellationToken)");
    }

    [Test]
    public async Task IAsyncEnumerable_with_custom_expression()
    {
        var result = TypeBuilder
            .Class("FilteredAsyncStream")
            .InNamespace("Test")
            .ImplementsIAsyncEnumerableWith("Item", "_source.WhereAsync(x => x.IsValid, cancellationToken)")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("_source.WhereAsync(x => x.IsValid, cancellationToken)");
    }

    [Test]
    public async Task IAsyncEnumerable_with_iterator_body()
    {
        var result = TypeBuilder
            .Class("GeneratedAsyncStream")
            .InNamespace("Test")
            .ImplementsIAsyncEnumerableWithIterator("int", b => b
                .AddStatement("for (int i = 0; i < 10; i++)")
                .AddStatement("{")
                .AddStatement("    await global::System.Threading.Tasks.Task.Delay(100, cancellationToken);")
                .AddStatement("    yield return i;")
                .AddStatement("}"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("async global::System.Collections.Generic.IAsyncEnumerator<int>");
        await Assert.That(result.Code).Contains("yield return i;");
    }
}
