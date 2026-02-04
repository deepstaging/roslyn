// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel;
using Deepstaging.Roslyn.Emit;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.Testing.Assertions;

/// <summary>
/// TUnit assertions for OptionalEmit.
/// These assertions provide fluent API for testing emit results.
/// </summary>
public static partial class OptionalEmitAssertions
{
    /// <summary>
    /// Asserts that the emit was successful (has syntax and no errors).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be successful")]
    public static bool IsSuccessful(this OptionalEmit emit)
    {
        return emit.Success;
    }

    /// <summary>
    /// Asserts that the emit failed (has error diagnostics).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have failed")]
    public static bool HasFailed(this OptionalEmit emit)
    {
        return !emit.Success;
    }

    /// <summary>
    /// Asserts that the emit has a value (syntax is present).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have a value")]
    public static bool HasValue(this OptionalEmit emit)
    {
        return emit.HasValue;
    }

    /// <summary>
    /// Asserts that the emit is empty (no syntax).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be empty")]
    public static bool IsEmpty(this OptionalEmit emit)
    {
        return emit.IsEmpty;
    }

    /// <summary>
    /// Asserts that the emit has diagnostics (any severity).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have diagnostics")]
    public static bool HasDiagnostics(this OptionalEmit emit)
    {
        return emit.Diagnostics.Length > 0;
    }

    /// <summary>
    /// Asserts that the emit has no diagnostics.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have no diagnostics")]
    public static bool HasNoDiagnostics(this OptionalEmit emit)
    {
        return emit.Diagnostics.Length == 0;
    }

    /// <summary>
    /// Asserts that the emit has error diagnostics.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have errors")]
    public static bool HasErrors(this OptionalEmit emit)
    {
        return emit.Errors.Any();
    }

    /// <summary>
    /// Asserts that the emit has warning diagnostics.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to have warnings")]
    public static bool HasWarnings(this OptionalEmit emit)
    {
        return emit.Warnings.Any();
    }

    /// <summary>
    /// Asserts that the emitted code contains the specified substring.
    /// Returns false if emit failed.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "code to contain the specified text")]
    public static bool CodeContains(this OptionalEmit emit, string text)
    {
        return emit.Code?.Contains(text) ?? false;
    }

    /// <summary>
    /// Asserts that the emitted code does not contain the specified substring.
    /// Returns true if emit failed (no code to contain it).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "code to not contain the specified text")]
    public static bool CodeDoesNotContain(this OptionalEmit emit, string text)
    {
        return emit.Code == null || !emit.Code.Contains(text);
    }
}
