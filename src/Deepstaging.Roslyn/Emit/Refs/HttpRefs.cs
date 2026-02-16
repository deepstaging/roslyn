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
    public static TypeRef Client => Namespace.Type("HttpClient");

    /// <summary>Gets an <c>HttpRequestMessage</c> type reference.</summary>
    public static TypeRef RequestMessage => Namespace.Type("HttpRequestMessage");

    /// <summary>Gets an <c>HttpResponseMessage</c> type reference.</summary>
    public static TypeRef ResponseMessage => Namespace.Type("HttpResponseMessage");

    /// <summary>Gets an <c>HttpMethod</c> type reference.</summary>
    public static TypeRef Method => Namespace.Type("HttpMethod");

    private static readonly HashSet<string> KnownVerbs =
        ["Get", "Post", "Put", "Patch", "Delete", "Head", "Options", "Trace"];

    /// <summary>Creates an <c>HttpMethod.{verb}</c> expression (e.g., <c>HttpMethod.Get</c>).</summary>
    /// <param name="verb">The HTTP verb name (e.g., "Get", "Post"). Must be a known HTTP method.</param>
    /// <exception cref="ArgumentException">Thrown when the verb is not a recognized HTTP method.</exception>
    public static TypeRef Verb(string verb)
    {
        if (!KnownVerbs.Contains(verb))
            throw new ArgumentException(
                $"Unknown HTTP verb '{verb}'. Known verbs: {string.Join(", ", KnownVerbs)}.",
                nameof(verb));

        return TypeRef.From($"global::System.Net.Http.HttpMethod.{verb}");
    }

    /// <summary>Gets an <c>HttpMethod.Get</c> expression.</summary>
    public static TypeRef Get => TypeRef.From("global::System.Net.Http.HttpMethod.Get");

    /// <summary>Gets an <c>HttpMethod.Post</c> expression.</summary>
    public static TypeRef Post => TypeRef.From("global::System.Net.Http.HttpMethod.Post");

    /// <summary>Gets an <c>HttpMethod.Put</c> expression.</summary>
    public static TypeRef Put => TypeRef.From("global::System.Net.Http.HttpMethod.Put");

    /// <summary>Gets an <c>HttpMethod.Patch</c> expression.</summary>
    public static TypeRef Patch => TypeRef.From("global::System.Net.Http.HttpMethod.Patch");

    /// <summary>Gets an <c>HttpMethod.Delete</c> expression.</summary>
    public static TypeRef Delete => TypeRef.From("global::System.Net.Http.HttpMethod.Delete");

    /// <summary>Gets an <c>HttpContent</c> type reference.</summary>
    public static TypeRef Content => Namespace.Type("HttpContent");

    /// <summary>Gets a <c>StringContent</c> type reference.</summary>
    public static TypeRef StringContent => Namespace.Type("StringContent");

    /// <summary>Gets a <c>ByteArrayContent</c> type reference.</summary>
    public static TypeRef ByteArrayContent => Namespace.Type("ByteArrayContent");

    /// <summary>Gets a <c>StreamContent</c> type reference.</summary>
    public static TypeRef StreamContent => Namespace.Type("StreamContent");
}
