// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Represents a validated TypeScript emit result that is guaranteed to contain valid source code.
/// Obtain via <see cref="TsOptionalEmit.ValidateOrThrow"/> or <see cref="TsOptionalEmit.TryValidate"/>.
/// </summary>
public readonly struct TsValidEmit
{
    /// <summary>Gets the emitted TypeScript source code.</summary>
    public string Code { get; }

    /// <summary>Initializes a new <see cref="TsValidEmit"/> with the given code.</summary>
    /// <param name="code">The validated TypeScript source code.</param>
    internal TsValidEmit(string code) => Code = code;

    /// <summary>Returns the emitted TypeScript source code.</summary>
    public override string ToString() => Code;
}
