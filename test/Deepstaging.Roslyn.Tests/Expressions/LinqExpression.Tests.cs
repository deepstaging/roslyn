// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class LinqExpressionTests
{
    [Test]
    public async Task Simple_select()
    {
        var expr = LinqExpression
            .From("x", "source")
            .Select("x.Name");

        await Assert.That(expr.Value)
            .IsEqualTo("from x in source\nselect x.Name");
    }

    [Test]
    public async Task Where_and_select()
    {
        var expr = LinqExpression
            .From("x", "source")
            .Where("x.IsActive")
            .Select("x.Name");

        await Assert.That(expr.Value)
            .IsEqualTo("from x in source\nwhere x.IsActive\nselect x.Name");
    }

    [Test]
    public async Task Let_where_orderby_select()
    {
        var expr = LinqExpression
            .From("x", "source")
            .Let("y", "x.Transform()")
            .Where("y.IsValid")
            .OrderBy("y.Name")
            .Select("y.Result");

        await Assert.That(expr.Value)
            .IsEqualTo("from x in source\nlet y = x.Transform()\nwhere y.IsValid\norderby y.Name\nselect y.Result");
    }

    [Test]
    public async Task Nested_from_selectmany()
    {
        var expr = LinqExpression
            .From("customer", "customers")
            .ThenFrom("order", "customer.Orders")
            .Where("order.Total > 100")
            .Select("new { customer.Name, order.Id }");

        await Assert.That(expr.Value)
            .IsEqualTo("from customer in customers\nfrom order in customer.Orders\nwhere order.Total > 100\nselect new { customer.Name, order.Id }");
    }

    [Test]
    public async Task GroupBy_terminal()
    {
        var expr = LinqExpression
            .From("x", "source")
            .GroupBy("x", "x.Category");

        await Assert.That(expr.Value)
            .IsEqualTo("from x in source\ngroup x by x.Category");
    }

    [Test]
    public async Task GroupBy_with_into_continuation()
    {
        var expr = LinqExpression
            .From("x", "source")
            .GroupBy("x", "x.Category", into: "g")
            .ThenFrom("item", "g")
            .OrderByDescending("item.Name")
            .Select("new { Group = g.Key, Item = item.Name }");

        await Assert.That(expr.Value)
            .IsEqualTo("from x in source\ngroup x by x.Category into g\nfrom item in g\norderby item.Name descending\nselect new { Group = g.Key, Item = item.Name }");
    }

    [Test]
    public async Task Join_clause()
    {
        var expr = LinqExpression
            .From("o", "orders")
            .Join("c", "customers", "o.CustomerId", "c.Id")
            .Select("new { o.Id, c.Name }");

        await Assert.That(expr.Value)
            .IsEqualTo("from o in orders\njoin c in customers on o.CustomerId equals c.Id\nselect new { o.Id, c.Name }");
    }

    [Test]
    public async Task Join_into_group_join()
    {
        var expr = LinqExpression
            .From("c", "customers")
            .Join("o", "orders", "c.Id", "o.CustomerId", into: "customerOrders")
            .Select("new { c.Name, Orders = customerOrders }");

        await Assert.That(expr.Value)
            .IsEqualTo("from c in customers\njoin o in orders on c.Id equals o.CustomerId into customerOrders\nselect new { c.Name, Orders = customerOrders }");
    }

    [Test]
    public async Task OrderByDescending()
    {
        var expr = LinqExpression
            .From("x", "source")
            .OrderByDescending("x.Date")
            .Select("x");

        await Assert.That(expr.Value)
            .IsEqualTo("from x in source\norderby x.Date descending\nselect x");
    }
}
