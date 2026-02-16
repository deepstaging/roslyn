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
    internal static ValidEmit From(CompilationUnitSyntax syntax, string code) => new(syntax, code);

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
                if (member is BaseNamespaceDeclarationSyntax ns)
                    types.AddRange(ns.Members);
                else
                    types.Add(member);

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

    #region Convenience Methods

    /// <summary>
    /// Returns the formatted code string.
    /// </summary>
    public override string ToString() => _code;

    #endregion
}