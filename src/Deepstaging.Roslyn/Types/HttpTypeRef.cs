// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// Convenience <see cref="TypeRef"/> constants for <c>System.Net.Http</c> types.
/// </summary>
public static class HttpTypes
{
    /// <summary>Gets the <c>System.Net.Http</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.Net.Http");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>HttpClient</c>.</summary>
    public static TypeRef Client => Namespace.GlobalType("HttpClient");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>HttpRequestMessage</c>.</summary>
    public static TypeRef RequestMessage => Namespace.GlobalType("HttpRequestMessage");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>HttpResponseMessage</c>.</summary>
    public static TypeRef ResponseMessage => Namespace.GlobalType("HttpResponseMessage");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>HttpMethod</c>.</summary>
    public static TypeRef Method => Namespace.GlobalType("HttpMethod");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>HttpContent</c>.</summary>
    public static TypeRef Content => Namespace.GlobalType("HttpContent");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>StringContent</c>.</summary>
    public static TypeRef StringContent => Namespace.GlobalType("StringContent");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>ByteArrayContent</c>.</summary>
    public static TypeRef ByteArrayContent => Namespace.GlobalType("ByteArrayContent");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>StreamContent</c>.</summary>
    public static TypeRef StreamContent => Namespace.GlobalType("StreamContent");
}
