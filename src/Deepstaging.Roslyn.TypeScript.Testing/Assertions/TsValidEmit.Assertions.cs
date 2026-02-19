// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.ComponentModel;
using Deepstaging.Roslyn.TypeScript.Emit;
using TUnit.Assertions.Attributes;

namespace Deepstaging.Roslyn.TypeScript.Testing.Assertions;

/// <summary>
/// TUnit fluent assertions for <see cref="TsValidEmit"/>.
/// Use with <c>await Assert.That(validEmit).CodeContains("export")</c> and similar patterns.
/// </summary>
public static partial class TsValidEmitAssertions
{
    /// <summary>
    /// Asserts that the validated emitted code contains the specified substring.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "code to contain the specified text")]
    public static bool CodeContains(this TsValidEmit emit, string text) => emit.Code.Contains(text);

    /// <summary>
    /// Asserts that the validated emitted code does not contain the specified substring.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "code to not contain the specified text")]
    public static bool CodeDoesNotContain(this TsValidEmit emit, string text) => !emit.Code.Contains(text);
}
