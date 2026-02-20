// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class LinqTypesTests
{
    [Test]
    public async Task Namespace_returns_system_linq()
    {
        await Assert.That((string)LinqTypes.Namespace)
            .IsEqualTo("System.Linq");
    }

    [Test]
    public async Task ExpressionsNamespace_returns_system_linq_expressions()
    {
        await Assert.That((string)LinqTypes.ExpressionsNamespace)
            .IsEqualTo("System.Linq.Expressions");
    }

    [Test]
    public async Task Queryable_creates_globally_qualified_type()
    {
        await Assert.That((string)LinqTypes.Queryable("Customer"))
            .IsEqualTo("global::System.Linq.IQueryable<Customer>");
    }

    [Test]
    public async Task OrderedQueryable_creates_globally_qualified_type()
    {
        await Assert.That((string)LinqTypes.OrderedQueryable("Customer"))
            .IsEqualTo("global::System.Linq.IOrderedQueryable<Customer>");
    }

    [Test]
    public async Task Expression_creates_globally_qualified_type()
    {
        await Assert.That((string)LinqTypes.Expression("Func<Customer, bool>"))
            .IsEqualTo("global::System.Linq.Expressions.Expression<Func<Customer, bool>>");
    }
}
