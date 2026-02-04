// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// A validated emit result where emission is guaranteed successful.
/// Similar to ValidSymbol&lt;T&gt; - no null checks needed.
/// Contains both the generated syntax tree and formatted code string.
/// </summary>
public readonly struct ValidEmit : IProjection<CompilationUnitSyntax>
{
    private readonly CompilationUnitSyntax _syntax;
    private readonly string _code;

    private ValidEmit(CompilationUnitSyntax syntax, string code)
    {
        _syntax = syntax ?? throw new ArgumentNullException(nameof(syntax));
        _code = code ?? throw new ArgumentNullException(nameof(code));
    }

    #region Factory Methods

    /// <summary>
    /// Creates a validated emit result from syntax and code.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if syntax or code is null.</exception>
    internal static ValidEmit From(CompilationUnitSyntax syntax, string code)
        => new(syntax, code);

    #endregion

    #region Core Properties

    /// <summary>
    /// Gets the generated compilation unit syntax tree.
    /// Guaranteed non-null.
    /// </summary>
    public CompilationUnitSyntax Syntax => _syntax;

    /// <summary>
    /// Gets the formatted C# code as a string.
    /// Guaranteed non-null.
    /// </summary>
    public string Code => _code;

    #endregion

    #region IProjection Implementation

    /// <summary>
    /// Always returns true for validated emits.
    /// </summary>
    public bool HasValue => true;

    /// <summary>
    /// Always returns false for validated emits.
    /// </summary>
    public bool IsEmpty => false;

    /// <summary>
    /// Returns the guaranteed non-null syntax.
    /// </summary>
    public CompilationUnitSyntax OrThrow(string? message = null) => _syntax;

    /// <summary>
    /// Returns the guaranteed non-null syntax.
    /// </summary>
    public CompilationUnitSyntax? OrNull() => _syntax;

    #endregion

    #region Part Extraction

    /// <summary>
    /// Gets the using directives from this emit.
    /// </summary>
    public ImmutableArray<UsingDirectiveSyntax> Usings =>
        _syntax.Usings.ToImmutableArray();

    /// <summary>
    /// Gets the type declarations from this emit.
    /// Unwraps types from namespace declarations if present.
    /// </summary>
    public ImmutableArray<MemberDeclarationSyntax> Types
    {
        get
        {
            var types = ImmutableArray.CreateBuilder<MemberDeclarationSyntax>();
            foreach (var member in _syntax.Members)
            {
                if (member is BaseNamespaceDeclarationSyntax ns)
                {
                    types.AddRange(ns.Members);
                }
                else
                {
                    types.Add(member);
                }
            }
            return types.ToImmutable();
        }
    }

    /// <summary>
    /// Gets the leading trivia (header comments, nullable directive, etc.) from the first token.
    /// </summary>
    public SyntaxTriviaList LeadingTrivia =>
        _syntax.GetFirstToken().LeadingTrivia;

    /// <summary>
    /// Gets the namespace name if this emit uses a namespace, or null if types are at global scope.
    /// </summary>
    public string? Namespace
    {
        get
        {
            var nsDecl = _syntax.Members.OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
            return nsDecl?.Name.ToString();
        }
    }

    #endregion

    #region Combination

    /// <summary>
    /// Combines multiple emits into a single compilation unit.
    /// Deduplicates using directives and supports multiple namespaces.
    /// Preserves leading trivia (header comments, nullable directive) from the first emit.
    /// </summary>
    /// <param name="emits">The emits to combine.</param>
    /// <param name="options">Optional emit options for formatting. Defaults to no validation.</param>
    public static ValidEmit Combine(IEnumerable<ValidEmit> emits, EmitOptions? options = null)
    {
        var emitList = emits.ToList();
        if (emitList.Count == 0)
            throw new ArgumentException("At least one emit is required.", nameof(emits));

        if (emitList.Count == 1)
            return emitList[0];

        options ??= EmitOptions.NoValidation;
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Collect and deduplicate usings from all emits, preserving static modifiers
        var seenUsings = new HashSet<string>(StringComparer.Ordinal);
        var usingDirectives = new List<UsingDirectiveSyntax>();

        foreach (var emit in emitList)
        {
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
                    {
                        newUsing = newUsing.WithStaticKeyword(
                            SyntaxFactory.Token(SyntaxKind.StaticKeyword));
                    }
                    usingDirectives.Add(newUsing);
                }
            }
        }

        // Sort usings: regular first (alphabetically), then static (alphabetically)
        var sortedUsings = usingDirectives
            .OrderBy(u => u.StaticKeyword != default ? 1 : 0)
            .ThenBy(u => u.Name?.ToString() ?? string.Empty, StringComparer.Ordinal);

        foreach (var usingDirective in sortedUsings)
        {
            compilationUnit = compilationUnit.AddUsings(usingDirective);
        }

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

        return new ValidEmit(normalized, code);
    }

    /// <summary>
    /// Combines multiple emits into a single compilation unit.
    /// Deduplicates using directives and supports multiple namespaces.
    /// Preserves leading trivia (header comments, nullable directive) from the first emit.
    /// </summary>
    /// <param name="emits">The emits to combine.</param>
    public static ValidEmit Combine(params ValidEmit[] emits)
        => Combine((IEnumerable<ValidEmit>)emits);

    #endregion

    #region Convenience Methods

    /// <summary>
    /// Returns the formatted code string.
    /// </summary>
    public override string ToString() => _code;

    #endregion
}
