// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CodeActions;

namespace Deepstaging.Roslyn.Tests.CodeFixes;

// Reuses NeedsTypeFixAttribute / NeedsTypeFixAnalyzer from TypeCodeFixActions.Fixtures.cs

// ── Project-level code fixes ─────────────────────────────────────────────────

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class AddProjectPropertyFix : ProjectCodeFix
{
    protected override CodeAction? CreateFix(Project project, Diagnostic diagnostic) =>
        project.AddProjectPropertyAction("UserSecretsId", "test-secrets-id", "User secrets for testing");
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class WriteFileFix : ProjectCodeFix
{
    protected override CodeAction? CreateFix(Project project, Diagnostic diagnostic) =>
        project.WriteFileAction("appsettings.json", """{"Key": "Value"}""");
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class WriteFilesFix : ProjectCodeFix
{
    protected override CodeAction? CreateFix(Project project, Diagnostic diagnostic) =>
        project.WriteFilesAction("Generate config files",
            [("app.json", """{"a": 1}"""), ("secrets.json", """{"b": 2}""")]);
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class FileActionsFix : ProjectCodeFix
{
    protected override CodeAction? CreateFix(Project project, Diagnostic diagnostic) =>
        project.FileActions("Setup project files")
            .Write("config.json", """{"enabled": true}""")
            .ToCodeAction();
}

[CodeFix(NeedsTypeFixAnalyzer.DiagnosticId)]
public sealed class ModifyXmlFileFix : ProjectCodeFix
{
    protected override CodeAction? CreateFix(Project project, Diagnostic diagnostic) =>
        project.ModifyXmlFileAction("Add build property", "Directory.Build.props",
            doc => doc.Root?.Add(new XElement("PropertyGroup")));
}
