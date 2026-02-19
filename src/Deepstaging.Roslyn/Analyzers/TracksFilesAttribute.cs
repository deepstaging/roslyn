// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Declares the "missing" and "stale" diagnostics that a <see cref="TrackedFileTypeAnalyzer"/> reports.
/// Both diagnostics share the same <see cref="DiagnosticId"/> so a single code fix can handle either case.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class TracksFilesAttribute : Attribute
{
    /// <summary>
    /// Gets the diagnostic ID shared by both the missing and stale rules.
    /// </summary>
    public string DiagnosticId { get; }

    /// <summary>
    /// Gets or sets the title for the "missing files" diagnostic (default: "Files should be generated").
    /// </summary>
    public string MissingTitle { get; set; } = "Files should be generated";

    /// <summary>
    /// Gets or sets the message format for the "missing files" diagnostic (default: "{0}").
    /// Use <c>{0}</c> for the symbol name.
    /// </summary>
    public string MissingMessage { get; set; } = "{0}";

    /// <summary>
    /// Gets or sets the description for the "missing files" diagnostic.
    /// </summary>
    public string MissingDescription { get; set; } = "";

    /// <summary>
    /// Gets or sets the severity for the "missing files" diagnostic (default: Info).
    /// </summary>
    public DiagnosticSeverity MissingSeverity { get; set; } = DiagnosticSeverity.Info;

    /// <summary>
    /// Gets or sets the title for the "stale files" diagnostic (default: "Files are out of date").
    /// </summary>
    public string StaleTitle { get; set; } = "Files are out of date";

    /// <summary>
    /// Gets or sets the message format for the "stale files" diagnostic (default: "{0}").
    /// Use <c>{0}</c> for the symbol name.
    /// </summary>
    public string StaleMessage { get; set; } = "{0}";

    /// <summary>
    /// Gets or sets the description for the "stale files" diagnostic.
    /// </summary>
    public string StaleDescription { get; set; } = "";

    /// <summary>
    /// Gets or sets the severity for the "stale files" diagnostic (default: Warning).
    /// </summary>
    public DiagnosticSeverity StaleSeverity { get; set; } = DiagnosticSeverity.Warning;

    /// <summary>
    /// Gets or sets the diagnostic category (default: "Usage").
    /// </summary>
    public string Category { get; set; } = "Usage";

    /// <summary>
    /// Gets or sets whether both diagnostics are enabled by default (default: true).
    /// </summary>
    public bool IsEnabledByDefault { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="TracksFilesAttribute"/> class.
    /// </summary>
    /// <param name="diagnosticId">The diagnostic ID shared by the missing and stale rules.</param>
    public TracksFilesAttribute(string diagnosticId) => DiagnosticId = diagnosticId;

    /// <summary>
    /// Creates the <see cref="DiagnosticDescriptor"/> for the "missing files" rule.
    /// </summary>
    internal DiagnosticDescriptor ToMissingDescriptor() => new(
        DiagnosticId,
        MissingTitle,
        MissingMessage,
        Category,
        MissingSeverity,
        IsEnabledByDefault,
        MissingDescription);

    /// <summary>
    /// Creates the <see cref="DiagnosticDescriptor"/> for the "stale files" rule.
    /// </summary>
    internal DiagnosticDescriptor ToStaleDescriptor() => new(
        DiagnosticId,
        StaleTitle,
        StaleMessage,
        Category,
        StaleSeverity,
        IsEnabledByDefault,
        StaleDescription);
}