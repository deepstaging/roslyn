// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class HttpTypesTests
{
    [Test]
    public async Task Client_produces_correct_type()
    {
        await Assert.That(HttpTypes.Client.Value)
            .IsEqualTo("global::System.Net.Http.HttpClient");
    }

    [Test]
    public async Task RequestMessage_produces_correct_type()
    {
        await Assert.That(HttpTypes.RequestMessage.Value)
            .IsEqualTo("global::System.Net.Http.HttpRequestMessage");
    }

    [Test]
    public async Task ResponseMessage_produces_correct_type()
    {
        await Assert.That(HttpTypes.ResponseMessage.Value)
            .IsEqualTo("global::System.Net.Http.HttpResponseMessage");
    }

    [Test]
    public async Task Method_produces_correct_type()
    {
        await Assert.That(HttpTypes.Method.Value)
            .IsEqualTo("global::System.Net.Http.HttpMethod");
    }

    [Test]
    public async Task StringContent_produces_correct_type()
    {
        await Assert.That(HttpTypes.StringContent.Value)
            .IsEqualTo("global::System.Net.Http.StringContent");
    }
}
