// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.ComponentModel;
using Deepstaging.Roslyn.TypeScript.Emit;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.TypeScript.Testing.Assertions;

/// <summary>
/// TUnit fluent assertions for <see cref="TsOptionalEmit"/>.
/// Use with <c>await Assert.That(result).IsSuccessful()</c> and similar patterns.
/// </summary>
public static partial class TsOptionalEmitAssertions
{
    /// <summary>
    /// Asserts that the emit was successful (code was produced and no error diagnostics).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be successful")]
    public static bool IsSuccessful(this TsOptionalEmit emit) => emit.Success;

    /// <summary>
    /// Asserts that the emit failed (no code or error diagnostics present).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have failed")]
    public static bool HasFailed(this TsOptionalEmit emit) => !emit.Success;

    /// <summary>
    /// Asserts that the emit produced diagnostics (any severity).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have diagnostics")]
    public static bool HasDiagnostics(this TsOptionalEmit emit) => emit.Diagnostics.Length > 0;

    /// <summary>
    /// Asserts that the emit produced no diagnostics.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have no diagnostics")]
    public static bool HasNoDiagnostics(this TsOptionalEmit emit) => emit.Diagnostics.Length == 0;

    /// <summary>
    /// Asserts that the emitted code contains the specified substring.
    /// Returns <c>false</c> if the emit produced no code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "code to contain the specified text")]
    public static bool CodeContains(this TsOptionalEmit emit, string text) =>
        emit.Code?.Contains(text) ?? false;

    /// <summary>
    /// Asserts that the emitted code does not contain the specified substring.
    /// Returns <c>true</c> if the emit produced no code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "code to not contain the specified text")]
    public static bool CodeDoesNotContain(this TsOptionalEmit emit, string text) =>
        emit.Code == null || !emit.Code.Contains(text);
}
