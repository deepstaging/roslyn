// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Diagnostic IDs for the Scriban scaffold analyzer and code fix.
/// </summary>
public static class ScaffoldDiagnostics
{
    /// <summary>
    /// DSRK005: A customizable template is available for a type with a trigger attribute.
    /// </summary>
    public const string ScaffoldAvailable = "DSRK005";
}
