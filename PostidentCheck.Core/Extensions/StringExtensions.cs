using System;
using System.Text;

namespace Postident.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Trims provided <paramref name="longString"/> and removes any multiplicated white spaces found.
        /// </summary>
        /// <param name="longString"><see cref="string"/> to be cleaned</param>
        /// <returns>A trimmed string with single spaces in place of multiple spaces</returns>
        public static string RemoveMultiplicatedWhitespaces(this string longString)
        {
            _ = longString ?? throw new ArgumentNullException(nameof(longString));
            var sb = new StringBuilder();
            var lastWasSpace = true; // True to eliminate leading spaces

            for (var i = 0; i < longString.Length; i++)
            {
                if (char.IsWhiteSpace(longString[i]) && lastWasSpace)
                {
                    continue;
                }

                lastWasSpace = char.IsWhiteSpace(longString[i]);

                sb.Append(longString[i]);
            }

            // The last character might be a space
            if (char.IsWhiteSpace(sb[^1]))
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }
    }
}