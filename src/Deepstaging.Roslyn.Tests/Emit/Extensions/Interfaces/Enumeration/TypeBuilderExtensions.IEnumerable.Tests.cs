// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Enumeration;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Enumeration;

/// <summary>
/// Tests for TypeBuilder IEnumerable interface implementation extensions.
/// </summary>
public class TypeBuilderIEnumerableExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task IEnumerable_delegates_to_inner_collection()
    {
        var result = TypeBuilder
            .Class("ItemCollection")
            .InNamespace("Test")
            .AddField(FieldBuilder.Parse("private readonly global::System.Collections.Generic.List<Item> _items;"))
            .ImplementsIEnumerable("Item", "_items")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IEnumerable<Item>");
        await Assert.That(result.Code).Contains("IEnumerator<Item> GetEnumerator()");
        await Assert.That(result.Code).Contains("_items.GetEnumerator()");
        // Non-generic GetEnumerator is also generated
        await Assert.That(result.Code).Contains("IEnumerator GetEnumerator()");
    }

    [Test]
    public async Task IEnumerable_with_custom_enumerator_expression()
    {
        var result = TypeBuilder
            .Class("FilteredCollection")
            .InNamespace("Test")
            .ImplementsIEnumerableWith("Item", "_items.Where(x => x.IsActive).GetEnumerator()")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("_items.Where(x => x.IsActive).GetEnumerator()");
    }

    [Test]
    public async Task IReadOnlyCollection_adds_count_property()
    {
        var result = TypeBuilder
            .Class("ReadOnlyItems")
            .InNamespace("Test")
            .ImplementsIReadOnlyCollection("Item", "_items")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IEnumerable<Item>");
        await Assert.That(result.Code).Contains("IReadOnlyCollection<Item>");
        await Assert.That(result.Code).Contains("int Count");
        await Assert.That(result.Code).Contains("_items.Count");
    }

    [Test]
    public async Task IReadOnlyList_adds_indexer()
    {
        var result = TypeBuilder
            .Class("ReadOnlyItemList")
            .InNamespace("Test")
            .ImplementsIReadOnlyList("Item", "_items")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IReadOnlyList<Item>");
        await Assert.That(result.Code).Contains("this[int index]");
        await Assert.That(result.Code).Contains("_items[index]");
    }
}