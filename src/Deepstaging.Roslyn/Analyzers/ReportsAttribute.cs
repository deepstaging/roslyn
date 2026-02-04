// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Declares the diagnostic that an analyzer reports.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ReportsAttribute : Attribute
{
    /// <summary>
    /// Gets the diagnostic ID (e.g., "DS0001").
    /// </summary>
    public string DiagnosticId { get; }

    /// <summary>
    /// Gets the diagnostic title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets or sets the message format with placeholders (default: "{0}" = symbol name).
    /// </summary>
    public string Message { get; set; } = "{0}";

    /// <summary>
    /// Gets or sets the detailed description.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Gets or sets the diagnostic category (default: "Usage").
    /// </summary>
    public string Category { get; set; } = "Usage";

    /// <summary>
    /// Gets or sets the diagnostic severity (default: Error).
    /// </summary>
    public DiagnosticSeverity Severity { get; set; } = DiagnosticSeverity.Error;

    /// <summary>
    /// Gets or sets whether the diagnostic is enabled by default (default: true).
    /// </summary>
    public bool IsEnabledByDefault { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportsAttribute"/> class.
    /// </summary>
    /// <param name="diagnosticId">The diagnostic ID.</param>
    /// <param name="title">The diagnostic title.</param>
    public ReportsAttribute(string diagnosticId, string title)
    {
        DiagnosticId = diagnosticId;
        Title = title;
    }

    /// <summary>
    /// Creates a <see cref="DiagnosticDescriptor"/> from this attribute.
    /// </summary>
    internal DiagnosticDescriptor ToDescriptor() => new(
        DiagnosticId,
        Title,
        Message,
        Category,
        Severity,
        IsEnabledByDefault,
        Description);
}