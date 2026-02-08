// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Helper methods for adding preprocessor directive trivia to syntax nodes.
/// </summary>
internal static class DirectiveHelper
{
    /// <summary>
    /// Wraps a member declaration syntax node in #if/#endif directives.
    /// </summary>
    /// <typeparam name="T">The type of member declaration syntax.</typeparam>
    /// <param name="member">The member to wrap.</param>
    /// <param name="directive">The directive condition.</param>
    /// <returns>The member wrapped in directive trivia.</returns>
    public static T WrapInDirective<T>(T member, Directive directive) where T : MemberDeclarationSyntax
    {
        var condition = directive.Condition;
        var newLine = SyntaxFactory.EndOfLine(Environment.NewLine);

        var ifDirective = SyntaxFactory.Trivia(
            SyntaxFactory.IfDirectiveTrivia(
                SyntaxFactory.ParseExpression(condition),
                isActive: true,
                branchTaken: true,
                conditionValue: true));

        var endIfDirective = SyntaxFactory.Trivia(
            SyntaxFactory.EndIfDirectiveTrivia(isActive: true));

        // Preserve existing leading trivia (like XML docs) and prepend the #if
        var existingLeading = member.GetLeadingTrivia();
        var newLeading = SyntaxFactory.TriviaList(ifDirective, newLine).AddRange(existingLeading);

        // Add #endif after the member with newlines
        var existingTrailing = member.GetTrailingTrivia();
        var newTrailing = existingTrailing.Add(newLine).Add(endIfDirective).Add(newLine);

        return member
            .WithLeadingTrivia(newLeading)
            .WithTrailingTrivia(newTrailing);
    }

    /// <summary>
    /// Wraps statements in #if/#endif directives within a block.
    /// </summary>
    /// <param name="statements">The statements to wrap.</param>
    /// <param name="directive">The directive condition.</param>
    /// <returns>The statements with directive trivia.</returns>
    public static ImmutableArray<StatementSyntax> WrapStatementsInDirective(
        ImmutableArray<StatementSyntax> statements,
        Directive directive)
    {
        if (statements.IsDefaultOrEmpty)
            return statements;

        var condition = directive.Condition;
        var newLine = SyntaxFactory.EndOfLine(Environment.NewLine);

        var ifDirective = SyntaxFactory.Trivia(
            SyntaxFactory.IfDirectiveTrivia(
                SyntaxFactory.ParseExpression(condition),
                isActive: true,
                branchTaken: true,
                conditionValue: true));

        var endIfDirective = SyntaxFactory.Trivia(
            SyntaxFactory.EndIfDirectiveTrivia(isActive: true));

        var result = ImmutableArray.CreateBuilder<StatementSyntax>();

        for (var i = 0; i < statements.Length; i++)
        {
            var statement = statements[i];

            if (i == 0)
            {
                // Add #if before the first statement
                var leading = statement.GetLeadingTrivia();
                var newLeading = SyntaxFactory.TriviaList(ifDirective, newLine).AddRange(leading);
                statement = statement.WithLeadingTrivia(newLeading);
            }

            if (i == statements.Length - 1)
            {
                // Add #endif after the last statement
                var trailing = statement.GetTrailingTrivia();
                var newTrailing = trailing.Add(newLine).Add(endIfDirective);
                statement = statement.WithTrailingTrivia(newTrailing);
            }

            result.Add(statement);
        }

        return result.ToImmutable();
    }
}
