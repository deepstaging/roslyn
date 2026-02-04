// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for method and constructor bodies.
/// Supports string-based statement composition with escape hatches for complex scenarios.
/// Immutable - each method returns a new instance.
/// </summary>
public readonly struct BodyBuilder
{
    private readonly ImmutableArray<StatementSyntax> _statements;

    private BodyBuilder(ImmutableArray<StatementSyntax> statements)
    {
        _statements = statements.IsDefault ? ImmutableArray<StatementSyntax>.Empty : statements;
    }

    #region Factory Methods

    /// <summary>
    /// Creates an empty body builder.
    /// </summary>
    public static BodyBuilder Empty() => new(ImmutableArray<StatementSyntax>.Empty);

    #endregion

    #region Statement Building

    /// <summary>
    /// Adds a statement from a C# string.
    /// Automatically appends semicolon if not present (unless it's a block statement).
    /// </summary>
    /// <param name="statement">The C# statement as a string (e.g., "var x = 5").</param>
    public BodyBuilder AddStatement(string statement)
    {
        if (string.IsNullOrWhiteSpace(statement))
            return this;

        var trimmed = statement.Trim();
        
        // Don't add semicolon for block statements (if, for, while, etc.) or statements that already have one
        var needsSemicolon = !trimmed.EndsWith(";") &&
                            !trimmed.EndsWith("}") &&
                            !trimmed.EndsWith("{");

        var code = needsSemicolon ? trimmed + ";" : trimmed;
        var statementSyntax = SyntaxFactory.ParseStatement(code);

        return new BodyBuilder(_statements.Add(statementSyntax));
    }

    /// <summary>
    /// Adds multiple statements from a multi-line C# string.
    /// Each non-empty line is treated as a separate statement.
    /// Preserves structure for control flow (if, for, while, etc.).
    /// </summary>
    /// <param name="statements">Multi-line C# code as a string.</param>
    public BodyBuilder AddStatements(string statements)
    {
        if (string.IsNullOrWhiteSpace(statements))
            return this;

        // Parse as a block to preserve structure
        var blockCode = statements.Trim();
        if (!blockCode.StartsWith("{"))
            blockCode = "{\n" + blockCode + "\n}";

        var block = SyntaxFactory.ParseStatement(blockCode) as BlockSyntax;
        if (block == null)
            return this;

        return new BodyBuilder(_statements.AddRange(block.Statements));
    }

    /// <summary>
    /// Adds a return statement.
    /// </summary>
    /// <param name="expression">The expression to return (e.g., "x * 2", "null", "true").</param>
    public BodyBuilder AddReturn(string expression)
    {
        var returnStatement = SyntaxFactory.ReturnStatement(
            SyntaxFactory.ParseExpression(expression));
        return new BodyBuilder(_statements.Add(returnStatement));
    }

    /// <summary>
    /// Adds a return statement with no value (for void methods).
    /// </summary>
    public BodyBuilder AddReturn()
    {
        var returnStatement = SyntaxFactory.ReturnStatement();
        return new BodyBuilder(_statements.Add(returnStatement));
    }

    /// <summary>
    /// Adds a throw statement.
    /// </summary>
    /// <param name="expression">The exception expression (e.g., "new ArgumentException(\"message\")").</param>
    public BodyBuilder AddThrow(string expression)
    {
        var throwStatement = SyntaxFactory.ThrowStatement(
            SyntaxFactory.ParseExpression(expression));
        return new BodyBuilder(_statements.Add(throwStatement));
    }

    /// <summary>
    /// Adds a custom statement syntax directly.
    /// Use this as an escape hatch for complex scenarios not covered by string-based methods.
    /// </summary>
    /// <param name="statement">The statement syntax to add.</param>
    public BodyBuilder AddCustom(StatementSyntax statement)
    {
        return new BodyBuilder(_statements.Add(statement));
    }

    #endregion

    #region Building

    /// <summary>
    /// Builds the body as a block syntax.
    /// Returns an empty block if no statements were added.
    /// </summary>
    internal BlockSyntax Build()
    {
        return SyntaxFactory.Block(_statements);
    }

    /// <summary>
    /// Gets a value indicating whether the body is empty (no statements).
    /// </summary>
    public bool IsEmpty => _statements.IsEmpty;

    #endregion
}
