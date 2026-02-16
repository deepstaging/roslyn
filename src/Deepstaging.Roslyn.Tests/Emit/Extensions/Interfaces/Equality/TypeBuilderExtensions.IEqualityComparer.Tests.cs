// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Equality;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Equality;

/// <summary>
/// Tests for TypeBuilder IEqualityComparer interface implementation extensions.
/// </summary>
public class TypeBuilderIEqualityComparerExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task IEqualityComparer_generates_equals_and_gethashcode()
    {
        var result = TypeBuilder
            .Class("PersonEqualityComparer")
            .InNamespace("Test")
            .ImplementsIEqualityComparer("Person", "Id")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IEqualityComparer<Person>");
        await Assert.That(result.Code).Contains("bool Equals(Person? x, Person? y)");
        await Assert.That(result.Code).Contains("int GetHashCode(Person obj)");
        await Assert.That(result.Code).Contains("ReferenceEquals(x, y)");
        await Assert.That(result.Code).Contains("x is null || y is null");
    }

    [Test]
    public async Task IEqualityComparer_for_value_type_generates_simple_equals()
    {
        var result = TypeBuilder
            .Class("MyIdEqualityComparer")
            .InNamespace("Test")
            .ImplementsIEqualityComparerForValueType("MyId", "Value", "global::System.Guid")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IEqualityComparer<MyId>");
        await Assert.That(result.Code).Contains("bool Equals(MyId x, MyId y)");
        await Assert.That(result.Code).Contains("EqualityComparer<global::System.Guid>.Default.Equals(x.Value, y.Value)");
    }

    [Test]
    public async Task IEqualityComparer_with_multiple_properties()
    {
        var result = TypeBuilder
            .Class("CompositeKeyComparer")
            .InNamespace("Test")
            .ImplementsIEqualityComparer("Order", "CustomerId", "OrderDate", "OrderNumber")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IEqualityComparer<Order>");
        await Assert.That(result.Code).Contains("x.CustomerId");
        await Assert.That(result.Code).Contains("x.OrderDate");
        await Assert.That(result.Code).Contains("x.OrderNumber");
        await Assert.That(result.Code).Contains("HashCode.Combine");
    }
}