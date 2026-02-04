// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System;

namespace Deepstaging.Roslyn;

/// <summary>
/// Declares the diagnostic ID that a code fix provider handles.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class CodeFixAttribute : Attribute
{
    /// <summary>
    /// Gets the diagnostic ID this code fix handles (e.g., "DS0001").
    /// </summary>
    public string DiagnosticId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeFixAttribute"/> class.
    /// </summary>
    /// <param name="diagnosticId">The diagnostic ID to fix.</param>
    public CodeFixAttribute(string diagnosticId)
    {
        DiagnosticId = diagnosticId;
    }
}
