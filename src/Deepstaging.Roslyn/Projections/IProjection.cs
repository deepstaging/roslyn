// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Base interface for all projection types.
/// Provides common validation and access patterns for wrapped Roslyn data.
/// </summary>
/// <typeparam name="T">The type of value being projected.</typeparam>
public interface IProjection<out T>
{
    /// <summary>
    /// Checks if the projection contains a valid value.
    /// </summary>
    bool HasValue { get; }

    /// <summary>
    /// Checks if the projection is empty (no value).
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Returns the value or throws if not present.
    /// </summary>
    /// <param name="message">Optional error message.</param>
    /// <returns>The non-null value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when value is not present.</exception>
    T OrThrow(string? message = null);

    /// <summary>
    /// Returns the value or null if not present.
    /// </summary>
    /// <returns>The value or null.</returns>
    T? OrNull();
}