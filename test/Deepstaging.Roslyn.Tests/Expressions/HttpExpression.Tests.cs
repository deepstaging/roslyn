// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class HttpExpressionTests
{
    [Test]
    public async Task Get_produces_correct_expression()
    {
        var expr = HttpExpression.Get;

        await Assert.That(expr.Value).IsEqualTo("global::System.Net.Http.HttpMethod.Get");
    }

    [Test]
    public async Task Post_produces_correct_expression()
    {
        var expr = HttpExpression.Post;

        await Assert.That(expr.Value).IsEqualTo("global::System.Net.Http.HttpMethod.Post");
    }

    [Test]
    public async Task Put_produces_correct_expression()
    {
        var expr = HttpExpression.Put;

        await Assert.That(expr.Value).IsEqualTo("global::System.Net.Http.HttpMethod.Put");
    }

    [Test]
    public async Task Patch_produces_correct_expression()
    {
        var expr = HttpExpression.Patch;

        await Assert.That(expr.Value).IsEqualTo("global::System.Net.Http.HttpMethod.Patch");
    }

    [Test]
    public async Task Delete_produces_correct_expression()
    {
        var expr = HttpExpression.Delete;

        await Assert.That(expr.Value).IsEqualTo("global::System.Net.Http.HttpMethod.Delete");
    }

    [Test]
    public async Task Verb_produces_correct_expression()
    {
        var expr = HttpExpression.Verb("Options");

        await Assert.That(expr.Value).IsEqualTo("global::System.Net.Http.HttpMethod.Options");
    }

    [Test]
    public async Task GetAsync_produces_correct_expression()
    {
        var expr = HttpExpression.GetAsync("client", "url");

        await Assert.That(expr.Value).IsEqualTo("client.GetAsync(url)");
    }

    [Test]
    public async Task PostAsync_produces_correct_expression()
    {
        var expr = HttpExpression.PostAsync("client", "url", "content");

        await Assert.That(expr.Value).IsEqualTo("client.PostAsync(url, content)");
    }

    [Test]
    public async Task PutAsync_produces_correct_expression()
    {
        var expr = HttpExpression.PutAsync("client", "url", "content");

        await Assert.That(expr.Value).IsEqualTo("client.PutAsync(url, content)");
    }

    [Test]
    public async Task DeleteAsync_produces_correct_expression()
    {
        var expr = HttpExpression.DeleteAsync("client", "url");

        await Assert.That(expr.Value).IsEqualTo("client.DeleteAsync(url)");
    }

    [Test]
    public async Task SendAsync_produces_correct_expression()
    {
        var expr = HttpExpression.SendAsync("client", "request");

        await Assert.That(expr.Value).IsEqualTo("client.SendAsync(request)");
    }

    [Test]
    public async Task SendAsync_with_cancellation_produces_correct_expression()
    {
        var expr = HttpExpression.SendAsync("client", "request", "ct");

        await Assert.That(expr.Value).IsEqualTo("client.SendAsync(request, ct)");
    }

    [Test]
    public async Task EnsureSuccessStatusCode_produces_correct_expression()
    {
        var expr = HttpExpression.EnsureSuccessStatusCode("response");

        await Assert.That(expr.Value).IsEqualTo("response.EnsureSuccessStatusCode()");
    }

    [Test]
    public async Task ReadAsStringAsync_produces_correct_expression()
    {
        var expr = HttpExpression.ReadAsStringAsync("response");

        await Assert.That(expr.Value).IsEqualTo("response.Content.ReadAsStringAsync()");
    }
}
