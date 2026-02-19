// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class QueryableTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new QueryableTypeRef("Customer");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Linq.IQueryable<Customer>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new QueryableTypeRef("Order");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("Order");
    }

    [Test]
    public async Task Implicitly_converts_to_TypeRef()
    {
        TypeRef typeRef = new QueryableTypeRef("Product");

        await Assert.That(typeRef.Value)
            .IsEqualTo("global::System.Linq.IQueryable<Product>");
    }
}

public class OrderedQueryableTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new OrderedQueryableTypeRef("Customer");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Linq.IOrderedQueryable<Customer>");
    }

    [Test]
    public async Task Carries_element_type()
    {
        var typeRef = new OrderedQueryableTypeRef("Order");

        await Assert.That((string)typeRef.ElementType).IsEqualTo("Order");
    }
}

public class LinqExpressionTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new LinqExpressionTypeRef("Func<Customer, bool>");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Linq.Expressions.Expression<Func<Customer, bool>>");
    }

    [Test]
    public async Task Carries_delegate_type()
    {
        var typeRef = new LinqExpressionTypeRef("Action<string>");

        await Assert.That((string)typeRef.DelegateType).IsEqualTo("Action<string>");
    }

    [Test]
    public async Task Composes_with_FuncTypeRef()
    {
        var func = new FuncTypeRef(
            System.Collections.Immutable.ImmutableArray.Create<TypeRef>("Customer"),
            "bool");
        var typeRef = new LinqExpressionTypeRef(func);

        await Assert.That((string)typeRef)
            .IsEqualTo("global::System.Linq.Expressions.Expression<global::System.Func<Customer, bool>>");
    }
}
