// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Extension methods for combining validated emit results.
/// </summary>
public static class ValidEmitExtensions
{
    extension(ValidEmit emit)
    {
        /// <summary>
        /// Combines this emit with another into a single compilation unit.
        /// Deduplicates using directives and supports multiple namespaces.
        /// Preserves leading trivia (header comments, nullable directive) from this emit.
        /// Chain calls for more: <c>a.Combine(b).Combine(c)</c>.
        /// </summary>
        /// <param name="other">The other emit to combine with.</param>
        public ValidEmit Combine(ValidEmit other) => CombineAll([emit, other]);
    }

    internal static ValidEmit CombineAll(List<ValidEmit> emitList)
    {
        if (emitList.Count == 1)
            return emitList[0];

        var options = EmitOptions.NoValidation;
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Collect and deduplicate usings from all emits, preserving static modifiers
        var seenUsings = new HashSet<string>(StringComparer.Ordinal);
        var usingDirectives = new List<UsingDirectiveSyntax>();

        foreach (var emit in emitList)
        foreach (var usingDirective in emit.Usings)
        {
            var name = usingDirective.Name?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(name))
                continue;

            // Create a unique key that includes whether it's static
            var isStatic = usingDirective.StaticKeyword != default;
            var key = isStatic ? $"static {name}" : name;

            if (seenUsings.Add(key))
            {
                // Recreate the using directive to ensure clean syntax
                var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(name));

                if (isStatic)
                    newUsing = newUsing.WithStaticKeyword(
                        SyntaxFactory.Token(SyntaxKind.StaticKeyword));

                usingDirectives.Add(newUsing);
            }
        }

        // Sort usings: regular first (alphabetically), then static (alphabetically)
        var sortedUsings = usingDirectives
            .OrderBy(u => u.StaticKeyword != default ? 1 : 0)
            .ThenBy(u => u.Name?.ToString() ?? string.Empty, StringComparer.Ordinal);

        foreach (var usingDirective in sortedUsings) compilationUnit = compilationUnit.AddUsings(usingDirective);

        // Group types by namespace (using empty string for global scope)
        var typesByNamespace = new Dictionary<string, List<MemberDeclarationSyntax>>();

        foreach (var emit in emitList)
        {
            var ns = emit.Namespace ?? string.Empty;

            if (!typesByNamespace.TryGetValue(ns, out var list))
            {
                list = [];
                typesByNamespace[ns] = list;
            }

            list.AddRange(emit.Types);
        }

        // Add types grouped by namespace
        foreach (var kvp in typesByNamespace.OrderBy(k => k.Key))
        {
            var ns = kvp.Key;
            var types = kvp.Value;

            if (!string.IsNullOrEmpty(ns))
            {
                var nsDecl = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(ns))
                    .WithMembers(SyntaxFactory.List(types));

                compilationUnit = compilationUnit.AddMembers(nsDecl);
            }
            else
            {
                compilationUnit = compilationUnit.AddMembers([.. types]);
            }
        }

        // Preserve leading trivia from the first emit
        var firstTrivia = emitList[0].LeadingTrivia;

        if (firstTrivia.Count > 0)
        {
            var firstToken = compilationUnit.GetFirstToken();
            var newFirstToken = firstToken.WithLeadingTrivia(firstTrivia);
            compilationUnit = compilationUnit.ReplaceToken(firstToken, newFirstToken);
        }

        // Format the combined code
        var normalized = compilationUnit.NormalizeWhitespace(options.Indentation, options.EndOfLine);
        var code = normalized.ToFullString();

        return ValidEmit.From(normalized, code);
    }
}