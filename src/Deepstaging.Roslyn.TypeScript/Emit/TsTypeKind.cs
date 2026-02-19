// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Specifies the kind of TypeScript type declaration to emit.
/// </summary>
public enum TsTypeKind
{
    /// <summary>A TypeScript <c>class</c> declaration.</summary>
    Class,

    /// <summary>A TypeScript <c>interface</c> declaration.</summary>
    Interface,

    /// <summary>A TypeScript <c>type</c> alias declaration.</summary>
    TypeAlias,

    /// <summary>A TypeScript <c>enum</c> declaration.</summary>
    Enum,

    /// <summary>A TypeScript <c>const enum</c> declaration.</summary>
    ConstEnum,
}
