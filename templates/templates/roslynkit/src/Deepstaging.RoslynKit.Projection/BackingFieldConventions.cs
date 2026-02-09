// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit.Projection;

/// <summary>
/// Constants for backing field naming conventions used in AutoNotify.
/// </summary>
public static class BackingFieldConventions
{
    /// <summary>
    /// Standard underscore prefix for backing fields (e.g., _fieldName).
    /// </summary>
    public const string UnderscorePrefix = "_";

    /// <summary>
    /// Hungarian notation prefix for backing fields (e.g., m_fieldName).
    /// </summary>
    public const string HungarianPrefix = "m_";

    /// <summary>
    /// All supported backing field prefixes.
    /// </summary>
    public static readonly string[] SupportedPrefixes = [UnderscorePrefix, HungarianPrefix];

    /// <summary>
    /// Determines if a field name follows the backing field naming convention.
    /// Supports: _fieldName, m_fieldName
    /// </summary>
    public static bool IsBackingFieldName(string name)
    {
        if (name.StartsWith(HungarianPrefix) && name.Length > HungarianPrefix.Length)
            return true;

        return name.StartsWith(UnderscorePrefix) && name.Length > 1 && char.IsLower(name[1]);
    }
}
