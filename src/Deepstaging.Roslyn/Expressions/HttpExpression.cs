// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Expressions;

using Deepstaging.Roslyn.Types;

/// <summary>
/// Expression factory for <c>System.Net.Http</c> operations.
/// </summary>
public static class HttpExpression
{
    /// <summary>Gets an expression for <c>HttpMethod.Get</c>.</summary>
    public static ExpressionRef Get => HttpTypes.Method.Member("Get");

    /// <summary>Gets an expression for <c>HttpMethod.Post</c>.</summary>
    public static ExpressionRef Post => HttpTypes.Method.Member("Post");

    /// <summary>Gets an expression for <c>HttpMethod.Put</c>.</summary>
    public static ExpressionRef Put => HttpTypes.Method.Member("Put");

    /// <summary>Gets an expression for <c>HttpMethod.Patch</c>.</summary>
    public static ExpressionRef Patch => HttpTypes.Method.Member("Patch");

    /// <summary>Gets an expression for <c>HttpMethod.Delete</c>.</summary>
    public static ExpressionRef Delete => HttpTypes.Method.Member("Delete");

    /// <summary>Produces <c>HttpMethod.{verb}</c> for the given HTTP verb.</summary>
    public static ExpressionRef Verb(string verb) => HttpTypes.Method.Member(verb);

    /// <summary>Produces <c>client.GetAsync(url)</c>.</summary>
    public static ExpressionRef GetAsync(ExpressionRef client, ExpressionRef url) =>
        client.Call("GetAsync", url);

    /// <summary>Produces <c>client.PostAsync(url, content)</c>.</summary>
    public static ExpressionRef PostAsync(ExpressionRef client, ExpressionRef url, ExpressionRef content) =>
        client.Call("PostAsync", url, content);

    /// <summary>Produces <c>client.PutAsync(url, content)</c>.</summary>
    public static ExpressionRef PutAsync(ExpressionRef client, ExpressionRef url, ExpressionRef content) =>
        client.Call("PutAsync", url, content);

    /// <summary>Produces <c>client.DeleteAsync(url)</c>.</summary>
    public static ExpressionRef DeleteAsync(ExpressionRef client, ExpressionRef url) =>
        client.Call("DeleteAsync", url);

    /// <summary>Produces <c>client.SendAsync(request)</c>.</summary>
    public static ExpressionRef SendAsync(ExpressionRef client, ExpressionRef request) =>
        client.Call("SendAsync", request);

    /// <summary>Produces <c>client.SendAsync(request, cancellationToken)</c>.</summary>
    public static ExpressionRef SendAsync(ExpressionRef client, ExpressionRef request, ExpressionRef cancellationToken) =>
        client.Call("SendAsync", request, cancellationToken);

    /// <summary>Produces <c>response.EnsureSuccessStatusCode()</c>.</summary>
    public static ExpressionRef EnsureSuccessStatusCode(ExpressionRef response) =>
        response.Call("EnsureSuccessStatusCode");

    /// <summary>Produces <c>response.Content.ReadAsStringAsync()</c>.</summary>
    public static ExpressionRef ReadAsStringAsync(ExpressionRef response) =>
        response.Member("Content").Call("ReadAsStringAsync");
}
