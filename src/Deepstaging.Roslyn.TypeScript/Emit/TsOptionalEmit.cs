// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Collections.Immutable;
using System.Linq;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Represents the result of a TypeScript emit operation that may or may not have succeeded.
/// Use <see cref="ValidateOrThrow"/> or <see cref="TryValidate"/> to obtain a <see cref="TsValidEmit"/>.
/// </summary>
public readonly struct TsOptionalEmit
{
    /// <summary>Gets a value indicating whether the emit succeeded without errors.</summary>
    public bool Success { get; }

    /// <summary>Gets the emitted TypeScript source code, or <c>null</c> if the emit failed.</summary>
    public string? Code { get; }

    /// <summary>Gets the diagnostics produced during emit.</summary>
    public ImmutableArray<string> Diagnostics { get; }

    /// <summary>Initializes a new <see cref="TsOptionalEmit"/> with the given code and diagnostics.</summary>
    /// <param name="code">The emitted source code, or <c>null</c> on failure.</param>
    /// <param name="diagnostics">The diagnostics produced during emit.</param>
    internal TsOptionalEmit(string? code, ImmutableArray<string> diagnostics)
    {
        Code = code;
        Success = code != null && !diagnostics.Any(d => d.StartsWith("error:", StringComparison.Ordinal));
        Diagnostics = diagnostics;
    }

    /// <summary>
    /// Validates the emit result and returns a <see cref="TsValidEmit"/>, or throws if the emit failed.
    /// </summary>
    /// <param name="message">An optional message to include in the exception.</param>
    /// <returns>A validated emit result containing the emitted code.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the emit failed or produced no code.</exception>
    public TsValidEmit ValidateOrThrow(string? message = null)
    {
        if (!Success || Code == null)
            throw new InvalidOperationException(message ?? $"Emit failed: {string.Join(", ", Diagnostics)}");
        return new TsValidEmit(Code);
    }

    /// <summary>
    /// Attempts to validate the emit result, returning <c>true</c> if the emit succeeded.
    /// </summary>
    /// <param name="validated">When this method returns <c>true</c>, contains the validated emit result.</param>
    /// <returns><c>true</c> if the emit succeeded; otherwise, <c>false</c>.</returns>
    public bool TryValidate(out TsValidEmit validated)
    {
        if (Success && Code != null)
        {
            validated = new TsValidEmit(Code);
            return true;
        }

        validated = default;
        return false;
    }
}
