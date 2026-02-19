// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Expressions;

/// <summary>Factory methods that produce <see cref="TsExpressionRef"/> values for TypeScript <c>fetch</c> operations.</summary>
public static class TsFetchExpression
{
    private static readonly TsExpressionRef Fetch = TsExpressionRef.From("fetch");

    /// <summary>Produces <c>fetch(url)</c>.</summary>
    /// <param name="url">The URL to fetch.</param>
    public static TsExpressionRef Get(TsExpressionRef url) =>
        Fetch.Invoke(url);

    /// <summary>Produces <c>fetch(url, { method: 'POST', body: JSON.stringify(body), headers: { 'Content-Type': 'application/json' } })</c>.</summary>
    /// <param name="url">The URL to post to.</param>
    /// <param name="body">The request body.</param>
    public static TsExpressionRef Post(TsExpressionRef url, TsExpressionRef body) =>
        Fetch.Invoke(url, RequestOptions("POST", body));

    /// <summary>Produces <c>fetch(url, { method: 'PUT', body: JSON.stringify(body), headers: { 'Content-Type': 'application/json' } })</c>.</summary>
    /// <param name="url">The URL to put to.</param>
    /// <param name="body">The request body.</param>
    public static TsExpressionRef Put(TsExpressionRef url, TsExpressionRef body) =>
        Fetch.Invoke(url, RequestOptions("PUT", body));

    /// <summary>Produces <c>fetch(url, { method: 'DELETE' })</c>.</summary>
    /// <param name="url">The URL to delete.</param>
    public static TsExpressionRef Delete(TsExpressionRef url) =>
        Fetch.Invoke(url, TsExpressionRef.From("{ method: 'DELETE' }"));

    /// <summary>Produces <c>response.json()</c>.</summary>
    /// <param name="response">The response expression.</param>
    public static TsExpressionRef ResponseJson(TsExpressionRef response) =>
        response.Call("json");

    /// <summary>Produces <c>response.text()</c>.</summary>
    /// <param name="response">The response expression.</param>
    public static TsExpressionRef ResponseText(TsExpressionRef response) =>
        response.Call("text");

    private static TsExpressionRef RequestOptions(string method, TsExpressionRef body) =>
        TsExpressionRef.From(
            $"{{ method: '{method}', body: JSON.stringify({body.Value}), headers: {{ 'Content-Type': 'application/json' }} }}");
}
