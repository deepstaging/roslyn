// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

using System.Collections.Generic;

/// <summary>
/// Simple English pluralization and singularization.
/// Handles common patterns and irregular forms without external dependencies.
/// </summary>
internal static class EnglishInflector
{
    private static readonly Dictionary<string, string> Irregulars = new(StringComparer.OrdinalIgnoreCase)
    {
        ["person"] = "people",
        ["man"] = "men",
        ["woman"] = "women",
        ["child"] = "children",
        ["mouse"] = "mice",
        ["goose"] = "geese",
        ["tooth"] = "teeth",
        ["foot"] = "feet",
        ["ox"] = "oxen",
        ["datum"] = "data",
        ["index"] = "indices",
        ["matrix"] = "matrices",
        ["vertex"] = "vertices",
        ["status"] = "statuses",
        ["alias"] = "aliases"
    };

    private static readonly Dictionary<string, string> IrregularsReverse;

    private static readonly string[] Uncountable =
    [
        "equipment", "information", "rice", "money", "species", "series", "fish", "sheep", "deer", "aircraft", "data"
    ];

    static EnglishInflector()
    {
        IrregularsReverse = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in Irregulars)
            IrregularsReverse[kvp.Value] = kvp.Key;
    }

    public static string Pluralize(string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;

        if (IsUncountable(word))
            return word;

        if (Irregulars.TryGetValue(word, out var irregular))
            return PreserveCase(word, irregular);

        if (word.EndsWith("ss", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("sh", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("z", StringComparison.OrdinalIgnoreCase))
            return word + Cased("es", word);

        if (word.EndsWith("f", StringComparison.OrdinalIgnoreCase))
            return word.Substring(0, word.Length - 1) + Cased("ves", word);

        if (word.EndsWith("fe", StringComparison.OrdinalIgnoreCase))
            return word.Substring(0, word.Length - 2) + Cased("ves", word);

        if (word.EndsWith("y", StringComparison.OrdinalIgnoreCase) &&
            word.Length > 1 && !IsVowel(word[word.Length - 2]))
            return word.Substring(0, word.Length - 1) + Cased("ies", word);

        if (word.EndsWith("o", StringComparison.OrdinalIgnoreCase) &&
            word.Length > 1 && !IsVowel(word[word.Length - 2]))
            return word + Cased("es", word);

        if (word.EndsWith("is", StringComparison.OrdinalIgnoreCase))
            return word.Substring(0, word.Length - 2) + Cased("es", word);

        if (word.EndsWith("us", StringComparison.OrdinalIgnoreCase))
            return word.Substring(0, word.Length - 2) + Cased("i", word);

        return word + Cased("s", word);
    }

    public static string Singularize(string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;

        if (IsUncountable(word))
            return word;

        if (IrregularsReverse.TryGetValue(word, out var irregular))
            return PreserveCase(word, irregular);

        if (word.EndsWith("ies", StringComparison.OrdinalIgnoreCase) && word.Length > 3)
            return word.Substring(0, word.Length - 3) + Cased("y", word);

        if (word.EndsWith("ves", StringComparison.OrdinalIgnoreCase) && word.Length > 3)
            return word.Substring(0, word.Length - 3) + Cased("f", word);

        if (word.EndsWith("sses", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("shes", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("ches", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("xes", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("zes", StringComparison.OrdinalIgnoreCase))
            return word.Substring(0, word.Length - 2);

        if (word.EndsWith("ses", StringComparison.OrdinalIgnoreCase) && word.Length > 3 &&
            !word.EndsWith("ases", StringComparison.OrdinalIgnoreCase))
            return word.Substring(0, word.Length - 1);

        if (word.EndsWith("i", StringComparison.OrdinalIgnoreCase) && word.Length > 1)
            return word.Substring(0, word.Length - 1) + Cased("us", word);

        if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) &&
            !word.EndsWith("ss", StringComparison.OrdinalIgnoreCase) &&
            !word.EndsWith("us", StringComparison.OrdinalIgnoreCase))
            return word.Substring(0, word.Length - 1);

        return word;
    }

    private static bool IsUncountable(string word) =>
        Array.Exists(Uncountable, u => u.Equals(word, StringComparison.OrdinalIgnoreCase));

    private static bool IsVowel(char c) =>
        "aeiouAEIOU".IndexOf(c) >= 0;

    private static string Cased(string suffix, string reference) =>
        char.IsUpper(reference[reference.Length - 1]) ? suffix.ToUpperInvariant() : suffix;

    private static string PreserveCase(string source, string target) =>
        char.IsUpper(source[0]) ? char.ToUpperInvariant(target[0]) + target.Substring(1) : target;
}
