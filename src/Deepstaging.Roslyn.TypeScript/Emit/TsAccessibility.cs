// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Specifies the accessibility level of a TypeScript class member.
/// </summary>
public enum TsAccessibility
{
    /// <summary>No accessibility modifier (default for interface members).</summary>
    None = 0,

    /// <summary>The <c>public</c> modifier.</summary>
    Public,

    /// <summary>The <c>private</c> modifier.</summary>
    Private,

    /// <summary>The <c>protected</c> modifier.</summary>
    Protected,
}
