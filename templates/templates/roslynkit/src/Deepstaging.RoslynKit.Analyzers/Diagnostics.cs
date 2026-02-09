// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit.Analyzers;

/// <summary>
/// Centralized diagnostic IDs for all RoslynKit analyzers.
/// </summary>
public static class Diagnostics
{
    /// <summary>
    /// RK1001: Type with [GenerateWith] must be partial.
    /// </summary>
    public const string GenerateWithMustBePartial = "RK1001";

    /// <summary>
    /// RK1002: Type with [AutoNotify] must be partial.
    /// </summary>
    public const string AutoNotifyMustBePartial = "RK1002";

    /// <summary>
    /// RK1003: AutoNotify backing field should be private.
    /// </summary>
    public const string AutoNotifyFieldMustBePrivate = "RK1003";
}
