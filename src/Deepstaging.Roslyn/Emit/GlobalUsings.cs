// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Emits a file containing <c>global using</c> directives with proper headers.
/// Returns <see cref="OptionalEmit"/> for integration with the standard emit pipeline.
/// </summary>
/// <example>
/// <code>
/// var result = GlobalUsings.Emit(
///     JsonRefs.Namespace,
///     LoggingRefs.Namespace,
///     CollectionRefs.Namespace);
///
/// if (result.IsValid(out var valid))
///     context.AddSource("MyGenerator.GlobalUsings.g.cs", valid.Code);
/// </code>
/// </example>
public static class GlobalUsings
{
    /// <summary>
    /// Emits <c>global using</c> directives for the specified namespaces using default options.
    /// Supports static usings via the <c>"static Namespace"</c> convention (see <see cref="NamespaceRef.AsStatic"/>).
    /// </summary>
    /// <param name="namespaces">The namespaces to include as global usings.</param>
    public static OptionalEmit Emit(params string[] namespaces) => Emit(EmitOptions.Default, namespaces);

    /// <summary>
    /// Emits <c>global using</c> directives for the specified namespaces with the given options.
    /// Supports static usings via the <c>"static Namespace"</c> convention (see <see cref="NamespaceRef.AsStatic"/>).
    /// </summary>
    /// <param name="options">Emit options controlling headers and formatting.</param>
    /// <param name="namespaces">The namespaces to include as global usings.</param>
    public static OptionalEmit Emit(EmitOptions options, params string[] namespaces)
    {
        try
        {
            var compilationUnit = SyntaxFactory.CompilationUnit();

            foreach (var ns in namespaces)
            {
                if (string.IsNullOrWhiteSpace(ns))
                    continue;

                UsingDirectiveSyntax directive;

                if (ns.StartsWith("static ", StringComparison.Ordinal))
                {
                    directive = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(ns.Substring(7)))
                        .WithStaticKeyword(SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                        .WithGlobalKeyword(SyntaxFactory.Token(SyntaxKind.GlobalKeyword));
                }
                else
                {
                    directive = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(ns))
                        .WithGlobalKeyword(SyntaxFactory.Token(SyntaxKind.GlobalKeyword));
                }

                compilationUnit = compilationUnit.AddUsings(directive);
            }

            compilationUnit = AddHeaders(compilationUnit, options);

            var formatted = compilationUnit
                .NormalizeWhitespace(options.Indentation, options.EndOfLine)
                .ToFullString();

            return OptionalEmit.FromSuccess(compilationUnit, formatted);
        }
        catch (Exception ex)
        {
            return OptionalEmit.FromFailure([
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "EMIT003",
                        "GlobalUsings emit failed",
                        "GlobalUsings.Emit failed: {0}",
                        "Deepstaging.Emit",
                        DiagnosticSeverity.Error,
                        true),
                    Location.None,
                    ex.Message)
            ]);
        }
    }

    /// <summary>
    /// Emits <c>global using</c> directives for the specified namespace references using default options.
    /// </summary>
    /// <param name="namespaces">The namespace references to include as global usings.</param>
    public static OptionalEmit Emit(params NamespaceRef[] namespaces) => Emit(EmitOptions.Default, namespaces);

    /// <summary>
    /// Emits <c>global using</c> directives for the specified namespace references with the given options.
    /// </summary>
    /// <param name="options">Emit options controlling headers and formatting.</param>
    /// <param name="namespaces">The namespace references to include as global usings.</param>
    public static OptionalEmit Emit(EmitOptions options, params NamespaceRef[] namespaces)
    {
        var strings = new string[namespaces.Length];

        for (var i = 0; i < namespaces.Length; i++)
            strings[i] = namespaces[i].Value;

        return Emit(options, strings);
    }

    private static CompilationUnitSyntax AddHeaders(CompilationUnitSyntax compilationUnit, EmitOptions options)
    {
        var headerComment = SyntaxFactory.Comment(options.HeaderComment);
        var newLine = SyntaxFactory.EndOfLine(Environment.NewLine);
        var firstToken = compilationUnit.GetFirstToken();

        var triviaList = new List<SyntaxTrivia> { headerComment, newLine };

        if (!string.IsNullOrWhiteSpace(options.LicenseHeader))
            foreach (var line in options.LicenseHeader!.Split('\n'))
            {
                var trimmedLine = line.TrimStart();

                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    triviaList.Add(SyntaxFactory.Comment(trimmedLine));
                    triviaList.Add(newLine);
                }
            }

        var newFirstToken = firstToken.WithLeadingTrivia(
            SyntaxFactory.TriviaList(triviaList)
                .AddRange(firstToken.LeadingTrivia));

        return compilationUnit.ReplaceToken(firstToken, newFirstToken);
    }
}