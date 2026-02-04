// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// Interface for projections that support validation to a non-nullable form.
/// Enables fast-exit patterns with IsNotValid(out var validated).
/// </summary>
/// <typeparam name="T">The type of value being validated.</typeparam>
/// <typeparam name="TValidated">The validated projection type.</typeparam>
public interface IValidatableProjection<out T, TValidated> : IProjection<T?>
    where T : notnull
    where TValidated : struct
{
    /// <summary>
    /// Attempts to validate the projection, returning a validated instance with non-nullable access.
    /// Returns null if validation fails (projection is empty).
    /// </summary>
    /// <returns>Validated projection if value is present, null otherwise.</returns>
    TValidated? Validate();

    /// <summary>
    /// Validates the projection or throws if invalid.
    /// </summary>
    /// <param name="message">Optional error message.</param>
    /// <returns>Validated projection with non-null value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when projection is empty.</exception>
    TValidated ValidateOrThrow(string? message = null);

    /// <summary>
    /// Attempts to validate the projection. Returns true if valid.
    /// Standard .NET pattern: returns true when successful.
    /// </summary>
    /// <param name="validated">The validated projection if successful, default if failed.</param>
    /// <returns>True if projection contains a value, false otherwise.</returns>
    bool TryValidate(out TValidated validated);

    /// <summary>
    /// Checks if projection is invalid. Returns true if INVALID (empty).
    /// Enables fast-exit pattern: if (projection.IsNotValid(out var valid)) return;
    /// 
    /// When this returns FALSE (projection is valid), the out parameter contains the validated value.
    /// When this returns TRUE (projection is invalid), the out parameter is default.
    /// </summary>
    /// <param name="validated">The validated projection if valid (when returns false), default otherwise.</param>
    /// <returns>True if projection is empty/invalid, false if projection is valid.</returns>
    bool IsNotValid(out TValidated validated);
}