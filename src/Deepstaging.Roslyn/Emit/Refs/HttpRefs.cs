// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory methods for <c>System.Net.Http</c> types.
/// </summary>
public static class HttpRefs
{
    /// <summary>Gets the <c>System.Net.Http</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Net.Http");

    /// <summary>Gets an <c>HttpClient</c> type reference.</summary>
    public static TypeRef Client => Namespace.GlobalType("HttpClient");

    /// <summary>Gets an <c>HttpRequestMessage</c> type reference.</summary>
    public static TypeRef RequestMessage => Namespace.GlobalType("HttpRequestMessage");

    /// <summary>Gets an <c>HttpResponseMessage</c> type reference.</summary>
    public static TypeRef ResponseMessage => Namespace.GlobalType("HttpResponseMessage");

    /// <summary>Gets an <c>HttpMethod</c> type reference.</summary>
    public static TypeRef Method => Namespace.GlobalType("HttpMethod");

    private static readonly HashSet<string> KnownVerbs =
        ["Get", "Post", "Put", "Patch", "Delete", "Head", "Options", "Trace"];

    /// <summary>Creates an <c>HttpMethod.{verb}</c> expression (e.g., <c>HttpMethod.Get</c>).</summary>
    /// <param name="verb">The HTTP verb name (e.g., "Get", "Post"). Must be a known HTTP method.</param>
    /// <exception cref="ArgumentException">Thrown when the verb is not a recognized HTTP method.</exception>
    public static ExpressionRef Verb(string verb)
    {
        if (!KnownVerbs.Contains(verb))
            throw new ArgumentException(
                $"Unknown HTTP verb '{verb}'. Known verbs: {string.Join(", ", KnownVerbs)}.",
                nameof(verb));

        return ExpressionRef.From($"global::System.Net.Http.HttpMethod.{verb}");
    }

    /// <summary>Gets an <c>HttpMethod.Get</c> expression.</summary>
    public static ExpressionRef Get => ExpressionRef.From("global::System.Net.Http.HttpMethod.Get");

    /// <summary>Gets an <c>HttpMethod.Post</c> expression.</summary>
    public static ExpressionRef Post => ExpressionRef.From("global::System.Net.Http.HttpMethod.Post");

    /// <summary>Gets an <c>HttpMethod.Put</c> expression.</summary>
    public static ExpressionRef Put => ExpressionRef.From("global::System.Net.Http.HttpMethod.Put");

    /// <summary>Gets an <c>HttpMethod.Patch</c> expression.</summary>
    public static ExpressionRef Patch => ExpressionRef.From("global::System.Net.Http.HttpMethod.Patch");

    /// <summary>Gets an <c>HttpMethod.Delete</c> expression.</summary>
    public static ExpressionRef Delete => ExpressionRef.From("global::System.Net.Http.HttpMethod.Delete");

    /// <summary>Gets an <c>HttpContent</c> type reference.</summary>
    public static TypeRef Content => Namespace.GlobalType("HttpContent");

    /// <summary>Gets a <c>StringContent</c> type reference.</summary>
    public static TypeRef StringContent => Namespace.GlobalType("StringContent");

    /// <summary>Gets a <c>ByteArrayContent</c> type reference.</summary>
    public static TypeRef ByteArrayContent => Namespace.GlobalType("ByteArrayContent");

    /// <summary>Gets a <c>StreamContent</c> type reference.</summary>
    public static TypeRef StreamContent => Namespace.GlobalType("StreamContent");

    // ── Well-Known API Calls ────────────────────────────────────────────

    /// <summary>Produces a <c>client.GetAsync(url)</c> expression.</summary>
    public static ExpressionRef GetAsync(ExpressionRef client, ExpressionRef url) =>
        ExpressionRef.From(client).Call("GetAsync", url);

    /// <summary>Produces a <c>client.PostAsync(url, content)</c> expression.</summary>
    public static ExpressionRef PostAsync(ExpressionRef client, ExpressionRef url, ExpressionRef content) =>
        ExpressionRef.From(client).Call("PostAsync", url, content);

    /// <summary>Produces a <c>client.PutAsync(url, content)</c> expression.</summary>
    public static ExpressionRef PutAsync(ExpressionRef client, ExpressionRef url, ExpressionRef content) =>
        ExpressionRef.From(client).Call("PutAsync", url, content);

    /// <summary>Produces a <c>client.DeleteAsync(url)</c> expression.</summary>
    public static ExpressionRef DeleteAsync(ExpressionRef client, ExpressionRef url) =>
        ExpressionRef.From(client).Call("DeleteAsync", url);

    /// <summary>Produces a <c>client.SendAsync(request)</c> expression.</summary>
    public static ExpressionRef SendAsync(ExpressionRef client, ExpressionRef request) =>
        ExpressionRef.From(client).Call("SendAsync", request);

    /// <summary>Produces a <c>client.SendAsync(request, cancellationToken)</c> expression.</summary>
    public static ExpressionRef SendAsync(ExpressionRef client, ExpressionRef request, ExpressionRef cancellationToken) =>
        ExpressionRef.From(client).Call("SendAsync", request, cancellationToken);

    /// <summary>Produces a <c>response.EnsureSuccessStatusCode()</c> expression.</summary>
    public static ExpressionRef EnsureSuccessStatusCode(ExpressionRef response) =>
        ExpressionRef.From(response).Call("EnsureSuccessStatusCode");

    /// <summary>Produces a <c>response.Content.ReadAsStringAsync()</c> expression.</summary>
    public static ExpressionRef ReadAsStringAsync(ExpressionRef response) =>
        ExpressionRef.From(response).Member("Content").Call("ReadAsStringAsync");
}