// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Controls the level of validation applied to emitted TypeScript code.
/// </summary>
public enum TsValidationLevel
{
    /// <summary>No external validation — fastest, pure string emit. Default.</summary>
    None = 0,

    /// <summary>Validates emitted code via <c>tsc --noEmit</c>. Requires TypeScript to be installed.</summary>
    Syntax = 1,
}
