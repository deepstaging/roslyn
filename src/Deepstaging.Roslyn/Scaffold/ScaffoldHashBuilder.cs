// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Scaffold;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Fluent builder for computing a deterministic SHA-256 hash from key-value pairs.
/// Used by satellite libraries to produce scaffold hashes from their projection models.
/// </summary>
/// <remarks>
/// <para>
/// The builder accumulates entries in insertion order. Each entry is written as
/// <c>key:value\n</c> to the hash input. The final hash is a lowercase hex string.
/// </para>
/// <example>
/// <code>
/// var hash = new ScaffoldHashBuilder()
///     .Add("route", "POST /api/orders")
///     .Add("prop", "CustomerId:string")
///     .Add("prop", "Amount:decimal")
///     .Build();
/// </code>
/// </example>
/// </remarks>
public sealed class ScaffoldHashBuilder
{
    private readonly StringBuilder _sb = new();

    /// <summary>
    /// Adds a key-value pair to the hash input.
    /// </summary>
    /// <param name="key">The entry key (e.g., "route", "prop", "entity").</param>
    /// <param name="value">The entry value.</param>
    /// <returns>This builder for chaining.</returns>
    public ScaffoldHashBuilder Add(string key, string value)
    {
        _sb.Append(key).Append(':').AppendLine(value);
        return this;
    }

    /// <summary>
    /// Computes the SHA-256 hash of all accumulated entries and returns it as a lowercase hex string.
    /// </summary>
    public string Build()
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(_sb.ToString()));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}
