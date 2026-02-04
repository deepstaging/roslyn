// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel;
using TUnit.Assertions.Attributes;
using TUnit.Assertions.Core;

namespace Deepstaging.Roslyn.Testing.Assertions;

public static partial class CompilationAssertions
{
    /// <summary>
    /// Asserts that the compilation was successful (has syntax and no errors).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be successful")]
    public static AssertionResult IsSuccessful(this Compilation compilation)
    {
        var errors = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(d => d.GetMessage())
            .ToArray();

        return AssertionResult.FailIf(errors.Any(), string.Join("\n", [
            ..errors,
            "Full source",
            "----------------",
            compilation.SyntaxTrees.FirstOrDefault()?.ToString() ?? "<no syntax tree>"
        ]));
    }
}