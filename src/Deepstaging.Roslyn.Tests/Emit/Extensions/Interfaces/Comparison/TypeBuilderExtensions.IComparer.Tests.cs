// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Comparison;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Comparison;

/// <summary>
/// Tests for TypeBuilder IComparer interface implementation extensions.
/// </summary>
public class TypeBuilderIComparerExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task IComparer_generates_compare_method_for_reference_types()
    {
        var result = TypeBuilder
            .Class("PersonComparer")
            .InNamespace("Test")
            .ImplementsIComparer("Person", "Name")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IComparer<Person>");
        await Assert.That(result.Code).Contains("int Compare(Person? x, Person? y)");
        await Assert.That(result.Code).Contains("ReferenceEquals(x, y)");
        await Assert.That(result.Code).Contains("x is null");
        await Assert.That(result.Code).Contains("y is null");
    }

    [Test]
    public async Task IComparer_for_value_type_generates_simple_compare()
    {
        var result = TypeBuilder
            .Class("MyIdComparer")
            .InNamespace("Test")
            .ImplementsIComparerForValueType("MyId", "Value", "global::System.Guid")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IComparer<MyId>");
        await Assert.That(result.Code).Contains("int Compare(MyId x, MyId y)");
        await Assert.That(result.Code).Contains("Comparer<global::System.Guid>.Default.Compare(x.Value, y.Value)");
    }

    [Test]
    public async Task IComparer_with_custom_body()
    {
        var result = TypeBuilder
            .Class("CustomComparer")
            .InNamespace("Test")
            .ImplementsIComparer("Item", b => b
                .AddStatement("if (x is null && y is null) return 0;")
                .AddStatement("return x?.Priority.CompareTo(y?.Priority) ?? -1;"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("x?.Priority.CompareTo(y?.Priority)");
    }
}
