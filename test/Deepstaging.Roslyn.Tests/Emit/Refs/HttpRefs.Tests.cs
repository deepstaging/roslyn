// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class HttpRefsTests
{
    [Test]
    public async Task Client_creates_globally_qualified_type()
    {
        var typeRef = HttpRefs.Client;

        await Assert.That(typeRef).IsEqualTo("global::System.Net.Http.HttpClient");
    }

    [Test]
    public async Task RequestMessage_creates_globally_qualified_type()
    {
        var typeRef = HttpRefs.RequestMessage;

        await Assert.That(typeRef).IsEqualTo("global::System.Net.Http.HttpRequestMessage");
    }

    [Test]
    public async Task ResponseMessage_creates_globally_qualified_type()
    {
        var typeRef = HttpRefs.ResponseMessage;

        await Assert.That(typeRef).IsEqualTo("global::System.Net.Http.HttpResponseMessage");
    }

    [Test]
    public async Task Method_creates_globally_qualified_type()
    {
        var typeRef = HttpRefs.Method;

        await Assert.That(typeRef).IsEqualTo("global::System.Net.Http.HttpMethod");
    }

    [Test]
    public async Task Verb_creates_method_expression()
    {
        var expr = HttpRefs.Verb("Get");

        await Assert.That(expr).IsEqualTo("global::System.Net.Http.HttpMethod.Get");
    }

    [Test]
    public void Verb_throws_on_unknown_verb() => Assert.Throws<ArgumentException>(() => HttpRefs.Verb("Yeet"));

    [Test]
    public async Task Get_creates_method_expression()
    {
        var expr = HttpRefs.Get;

        await Assert.That(expr).IsEqualTo("global::System.Net.Http.HttpMethod.Get");
    }

    [Test]
    public async Task Post_creates_method_expression()
    {
        var expr = HttpRefs.Post;

        await Assert.That(expr).IsEqualTo("global::System.Net.Http.HttpMethod.Post");
    }

    [Test]
    public async Task Content_creates_globally_qualified_type()
    {
        var typeRef = HttpRefs.Content;

        await Assert.That(typeRef).IsEqualTo("global::System.Net.Http.HttpContent");
    }

    [Test]
    public async Task StringContent_creates_globally_qualified_type()
    {
        var typeRef = HttpRefs.StringContent;

        await Assert.That(typeRef).IsEqualTo("global::System.Net.Http.StringContent");
    }

    [Test]
    public async Task ByteArrayContent_creates_globally_qualified_type()
    {
        var typeRef = HttpRefs.ByteArrayContent;

        await Assert.That(typeRef).IsEqualTo("global::System.Net.Http.ByteArrayContent");
    }

    [Test]
    public async Task StreamContent_creates_globally_qualified_type()
    {
        var typeRef = HttpRefs.StreamContent;

        await Assert.That(typeRef).IsEqualTo("global::System.Net.Http.StreamContent");
    }
}