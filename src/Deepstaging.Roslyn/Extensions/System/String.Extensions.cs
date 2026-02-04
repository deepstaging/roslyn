// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.Globalization;
using System.Text;

namespace Deepstaging.Roslyn;

/// <summary>
/// Provides string case conversion extensions for code generation.
/// </summary>
public static class StringExtensions
{
    extension(string text)
    {
        /// <summary>
        /// Converts the string to snake_case (e.g., "MyPropertyName" → "my_property_name").
        /// </summary>
        /// <returns>The snake_case version of the string.</returns>
        public string ToSnakeCase()
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var builder = new StringBuilder(text.Length + Math.Min(2, text.Length / 5));
            UnicodeCategory? previousCategory = null;

            for (var i = 0; i < text.Length; i++)
            {
                var current = text[i];

                if (current == '_')
                {
                    builder.Append('_');
                    previousCategory = null;
                    continue;
                }

                var category = char.GetUnicodeCategory(current);

                switch (category)
                {
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.TitlecaseLetter:
                        if (ShouldInsertUnderscore(previousCategory, i, text))
                            builder.Append('_');

                        builder.Append(char.ToLowerInvariant(current));
                        break;

                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (previousCategory == UnicodeCategory.SpaceSeparator)
                            builder.Append('_');

                        builder.Append(current);
                        break;

                    default:
                        if (previousCategory != null)
                            previousCategory = UnicodeCategory.SpaceSeparator;
                        continue;
                }

                previousCategory = category;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts the string to camelCase (e.g., "MyPropertyName" → "myPropertyName").
        /// </summary>
        /// <returns>The camelCase version of the string.</returns>
        public string ToCamelCase()
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalized = text.IsAllUppercase() ? text.ToLowerInvariant() : text;
            var hasLeadingUnderscore = normalized.StartsWith("_");
            var builder = new StringBuilder(normalized.Length);
            var capitalizeNext = false;

            foreach (var c in normalized)
                if (IsSeparator(c))
                {
                    capitalizeNext = true;
                }
                else
                {
                    builder.Append(capitalizeNext ? char.ToUpperInvariant(c) : c);
                    capitalizeNext = false;
                }

            if (builder.Length > 0)
                builder[0] = char.ToLowerInvariant(builder[0]);

            if (hasLeadingUnderscore)
                builder.Insert(0, '_');

            return builder.ToString();
        }

        /// <summary>
        /// Converts the string to PascalCase (e.g., "my_property_name" → "MyPropertyName").
        /// </summary>
        /// <returns>The PascalCase version of the string.</returns>
        public string ToPascalCase()
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var builder = new StringBuilder(text.Length);
            var startOfWord = true;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (char.IsLetterOrDigit(c))
                {
                    if (startOfWord)
                    {
                        builder.Append(char.ToUpperInvariant(c));
                        startOfWord = false;
                    }
                    else
                    {
                        var nextIsLower = i < text.Length - 1 && char.IsLower(text[i + 1]);
                        builder.Append(char.IsUpper(c) && nextIsLower ? c : char.ToLowerInvariant(c));
                    }
                }
                else
                {
                    startOfWord = true;
                }

                if (i < text.Length - 1 && char.IsLower(c) && char.IsUpper(text[i + 1]))
                    startOfWord = true;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts the string to kebab-case (e.g., "MyPropertyName" → "my-property-name").
        /// </summary>
        /// <returns>The kebab-case version of the string.</returns>
        public string ToKebabCase()
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var builder = new StringBuilder(text.Length);
            var previousWasSeparator = true;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (char.IsUpper(c) || char.IsDigit(c))
                {
                    if (ShouldInsertHyphen(previousWasSeparator, i, text))
                        builder.Append('-');

                    builder.Append(char.ToLowerInvariant(c));
                    previousWasSeparator = false;
                }
                else if (char.IsLower(c))
                {
                    builder.Append(c);
                    previousWasSeparator = false;
                }
                else if (IsSeparator(c) || char.IsWhiteSpace(c))
                {
                    if (!previousWasSeparator)
                        builder.Append('-');

                    previousWasSeparator = true;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts the string to Train-Case (e.g., "myPropertyName" → "My-Property-Name").
        /// </summary>
        /// <returns>The Train-Case version of the string.</returns>
        public string ToTrainCase()
        {
            return text.ToPascalCase()
                .InsertHyphenBeforeUpperCase()
                .WithFirstCharUpperCase()
                .Replace("--", "-");
        }

        /// <summary>
        /// Converts the string to Title Case (e.g., "my_property_name" → "My Property Name").
        /// </summary>
        /// <returns>The Title Case version of the string.</returns>
        public string ToTitleCase()
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var builder = new StringBuilder(text.Length);
            var startOfWord = true;

            foreach (var c in text)
                if (char.IsWhiteSpace(c) || c is '-' or '_')
                {
                    builder.Append(c);
                    startOfWord = true;
                }
                else
                {
                    builder.Append(startOfWord ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c));
                    startOfWord = false;
                }

            return builder.ToString();
        }

        private string InsertSpaceBeforeUpperCase() => text.InsertCharacterBeforeUpperCase(' ');
        private string InsertHyphenBeforeUpperCase() => text.InsertCharacterBeforeUpperCase('-');

        private string WithFirstCharLowerCase() =>
            string.IsNullOrEmpty(text) || char.IsLower(text[0])
                ? text
                : char.ToLowerInvariant(text[0]) + text.Substring(1);

        private string WithFirstCharUpperCase() =>
            string.IsNullOrEmpty(text) || char.IsUpper(text[0])
                ? text
                : char.ToUpperInvariant(text[0]) + text.Substring(1);

        private bool IsAllUppercase() =>
            !string.IsNullOrEmpty(text) && text.All(c => !char.IsLetter(c) || char.IsUpper(c));
    }

    private static bool ShouldInsertUnderscore(UnicodeCategory? previousCategory, int currentIndex, string text)
    {
        switch (previousCategory)
        {
            case UnicodeCategory.SpaceSeparator:
            case UnicodeCategory.LowercaseLetter:
                return true;
        }

        return previousCategory != UnicodeCategory.DecimalDigitNumber &&
               previousCategory != null &&
               currentIndex > 0 &&
               currentIndex + 1 < text.Length &&
               char.IsLower(text[currentIndex + 1]);
    }

    private static bool ShouldInsertHyphen(bool previousWasSeparator, int currentIndex, string text)
    {
        if (previousWasSeparator)
            return false;

        if (currentIndex == 0)
            return false;

        var isAfterLower = char.IsLower(text[currentIndex - 1]);
        var isBeforeLower = currentIndex < text.Length - 1 && char.IsLower(text[currentIndex + 1]);

        return isAfterLower || isBeforeLower;
    }

    private static bool IsSeparator(char c) => c is '-' or '_';

    private static string InsertCharacterBeforeUpperCase(this string text, char separator)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var builder = new StringBuilder(text.Length * 2);
        var previous = '\0';

        foreach (var c in text)
        {
            if (char.IsUpper(c) && builder.Length > 0 && previous != ' ')
                builder.Append(separator);

            builder.Append(c);
            previous = c;
        }

        return builder.ToString();
    }
}