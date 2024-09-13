using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DEWAXP.Foundation.Content.Utils
{
    public static class RegexUtil
    {
        private static bool IsMatch(string input, string wildcardPattern)
        {
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(wildcardPattern))
            {
                return false;
            }

            //string regexPattern = WildcardToRegex(wildcardPattern);
            return Regex.IsMatch(input, wildcardPattern, RegexOptions.IgnoreCase);
        }

        public static bool IsWildcardMatch(string subject, string wildcardPattern)
        {
            if (string.IsNullOrWhiteSpace(wildcardPattern))
            {
                return false;
            }

            string regexPattern = string.Concat("^", Regex.Escape(wildcardPattern).Replace("\\*", ".*"), "$");

            int wildcardCount = wildcardPattern.Count(x => x.Equals('*'));
            if (wildcardCount <= 0)
            {
                return subject.Equals(wildcardPattern, StringComparison.CurrentCultureIgnoreCase);
            }
            else if (wildcardCount == 1)
            {
                string newWildcardPattern = wildcardPattern.Replace("*", "");

                if (wildcardPattern.StartsWith("*"))
                {
                    return subject.EndsWith(newWildcardPattern, StringComparison.CurrentCultureIgnoreCase);
                }
                else if (wildcardPattern.EndsWith("*"))
                {
                    return subject.StartsWith(newWildcardPattern, StringComparison.CurrentCultureIgnoreCase);
                }
                else
                {
                    try
                    {
                        return Regex.IsMatch(subject, regexPattern);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            else
            {
                try
                {
                    return Regex.IsMatch(subject, regexPattern);
                }
                catch
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Password Regex With Small Letter, Symbol And Number.
        /// </summary>
        public const string PasswordRegexWithSmallLetterSymbolAndNumber = @"(?=^.{6,25}$)(?=(?:.*?\d){2})(?=.*[a-z]{1})(?=(?:.*?[a-z]){1})(?=(?:.*?[!@#$%*()_+^&}{:;?.]){1})(?!.*\s)[0-9a-zA-Z!@#$%*()_+^&]*";

    }
}