// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Deepstaging.Roslyn;

/// <summary>
/// Base class for code fix providers that operate on the project level rather than on syntax nodes.
/// Uses declarative configuration via <see cref="CodeFixAttribute"/>.
/// </summary>
/// <remarks>
/// Unlike <see cref="SyntaxCodeFix{TSyntax}"/> which resolves a syntax node at the diagnostic location,
/// this base class passes the <see cref="Project"/> and <see cref="Diagnostic"/> directly.
/// Useful for fixes that modify the project file (e.g., adding MSBuild properties) rather than source code.
/// </remarks>
public abstract class ProjectCodeFix : CodeFixProvider
{
    /// <inheritdoc />
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; }

    /// <inheritdoc />
    public override FixAllProvider? GetFixAllProvider() => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectCodeFix"/> class.
    /// </summary>
    protected ProjectCodeFix()
    {
        var codeFixAttrs = GetType()
            .GetCustomAttributes<CodeFixAttribute>()
            .ToArray();

        if (codeFixAttrs.Length == 0)
            throw new InvalidOperationException(
                $"CodeFix {GetType().Name} must have at least one [CodeFix] attribute.");

        FixableDiagnosticIds = [..codeFixAttrs.Select(a => a.DiagnosticId)];
    }

    /// <inheritdoc />
    public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var project = context.Document.Project;
        var diagnostic = context.Diagnostics[0];

        var codeAction = CreateFix(project, diagnostic);

        if (codeAction is not null)
            context.RegisterCodeFix(codeAction, diagnostic);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates the code action that fixes the diagnostic.
    /// </summary>
    /// <param name="project">The project containing the diagnostic.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <returns>A code action to fix the diagnostic, or null to skip.</returns>
    protected abstract CodeAction? CreateFix(Project project, Diagnostic diagnostic);
}

/// <summary>
/// Base class for project-level code fix providers with automatic symbol resolution
/// from the diagnostic location.
/// </summary>
/// <typeparam name="TSymbol">The expected symbol type at the diagnostic location.</typeparam>
/// <remarks>
/// Combines <see cref="ProjectCodeFix"/> with automatic symbol resolution.
/// The diagnostic location is used to find and validate a symbol of type <typeparamref name="TSymbol"/>,
/// which is then passed to <see cref="CreateFix(Project, ValidSymbol{TSymbol}, Diagnostic)"/>.
/// <para>
/// Diagnostic properties (e.g., MSBuild build properties forwarded by <c>TrackedFileTypeAnalyzer</c>)
/// are available via <c>diagnostic.Properties</c>.
/// </para>
/// </remarks>
public abstract class ProjectCodeFix<TSymbol> : ProjectCodeFix
    where TSymbol : class, ISymbol
{
    /// <inheritdoc />
    protected sealed override CodeAction? CreateFix(Project project, Diagnostic diagnostic)
    {
        var compilation = project.GetCompilationAsync().GetAwaiter().GetResult();

        if (compilation is null)
            return null;

        if (compilation.GetSymbolAtDiagnostic(diagnostic).OfType<TSymbol>().IsNotValid(out var symbol))
            return null;

        return CreateFix(project, symbol, diagnostic);
    }

    /// <summary>
    /// Creates the code action that fixes the diagnostic.
    /// </summary>
    /// <param name="project">The project containing the diagnostic.</param>
    /// <param name="symbol">The validated symbol at the diagnostic location.</param>
    /// <param name="diagnostic">The diagnostic being fixed. Use <c>diagnostic.Properties</c> to
    /// access build properties forwarded by the analyzer.</param>
    /// <returns>A code action to fix the diagnostic, or null to skip.</returns>
    protected abstract CodeAction? CreateFix(Project project, ValidSymbol<TSymbol> symbol, Diagnostic diagnostic);
}