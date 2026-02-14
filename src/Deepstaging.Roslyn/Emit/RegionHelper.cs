// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Helper methods for adding #region/#endregion directive trivia to syntax nodes.
/// </summary>
internal static class RegionHelper
{
    /// <summary>
    /// Wraps a single member declaration in #region/#endregion directives.
    /// </summary>
    /// <typeparam name="T">The type of member declaration syntax.</typeparam>
    /// <param name="member">The member to wrap.</param>
    /// <param name="regionName">The region name.</param>
    /// <returns>The member wrapped in region directive trivia.</returns>
    public static T WrapInRegion<T>(T member, string regionName) where T : MemberDeclarationSyntax
    {
        var newLine = SyntaxFactory.EndOfLine(Environment.NewLine);

        var regionStart = CreateRegionDirective(regionName);
        var regionEnd = CreateEndRegionDirective();

        var existingLeading = member.GetLeadingTrivia();
        var newLeading = SyntaxFactory.TriviaList(regionStart, newLine).AddRange(existingLeading);

        var existingTrailing = member.GetTrailingTrivia();
        var newTrailing = existingTrailing.Add(newLine).Add(regionEnd).Add(newLine);

        return member
            .WithLeadingTrivia(newLeading)
            .WithTrailingTrivia(newTrailing);
    }

    /// <summary>
    /// Wraps an array of member declarations in #region/#endregion directives.
    /// Attaches #region to the leading trivia of the first member and
    /// #endregion to the trailing trivia of the last member.
    /// </summary>
    /// <param name="members">The members to wrap.</param>
    /// <param name="regionName">The region name.</param>
    /// <returns>The members with region directive trivia on first and last.</returns>
    public static MemberDeclarationSyntax[] WrapMembersInRegion(
        MemberDeclarationSyntax[] members,
        string regionName)
    {
        if (members.Length == 0)
            return members;

        if (members.Length == 1)
        {
            members[0] = WrapInRegion(members[0], regionName);
            return members;
        }

        var newLine = SyntaxFactory.EndOfLine(Environment.NewLine);

        var regionStart = CreateRegionDirective(regionName);
        var regionEnd = CreateEndRegionDirective();

        // Add #region before the first member
        var first = members[0];
        var existingLeading = first.GetLeadingTrivia();
        var newLeading = SyntaxFactory.TriviaList(regionStart, newLine).AddRange(existingLeading);
        members[0] = first.WithLeadingTrivia(newLeading);

        // Add #endregion after the last member
        var last = members[^1];
        var existingTrailing = last.GetTrailingTrivia();
        var newTrailing = existingTrailing.Add(newLine).Add(regionEnd).Add(newLine);
        members[^1] = last.WithTrailingTrivia(newTrailing);

        return members;
    }

    private static SyntaxTrivia CreateRegionDirective(string regionName)
    {
        var regionNameToken = SyntaxFactory.PreprocessingMessage(regionName);

        var endOfDirective = SyntaxFactory.Token(
            SyntaxFactory.TriviaList(regionNameToken),
            SyntaxKind.EndOfDirectiveToken,
            SyntaxFactory.TriviaList());

        return SyntaxFactory.Trivia(
            SyntaxFactory.RegionDirectiveTrivia(isActive: true)
                .WithEndOfDirectiveToken(endOfDirective));
    }

    private static SyntaxTrivia CreateEndRegionDirective()
    {
        return SyntaxFactory.Trivia(
            SyntaxFactory.EndRegionDirectiveTrivia(isActive: true));
    }
}
