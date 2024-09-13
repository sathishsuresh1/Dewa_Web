// <copyright file="StringExtensions.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Helpers.Extensions
{
    using DEWAXP.Foundation.Helpers.Models;
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Defines the <see cref="StringExtensions" />
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Defines the NonAlphaNumericCharacters
        /// </summary>
        public const string NonAlphaNumericCharacters = "[a-zA-Z0-9]";

        /// <summary>
        /// The Truncate
        /// </summary>
        /// <param name="value">The value<see cref="string"/></param>
        /// <param name="maxLength">The maxLength<see cref="int"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// The AssertAccountNumberPrefix
        /// </summary>
        /// <param name="accountNumber">The accountNumber<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string AssertAccountNumberPrefix(this string accountNumber)
        {
            if (!string.IsNullOrWhiteSpace(accountNumber) && !accountNumber.StartsWith("00"))
            {
                return string.Concat("00", accountNumber);
            }
            return accountNumber;
        }

        /// <summary>
        /// The AddMobileNumberZeroPrefix
        /// </summary>
        /// <param name="value">The value<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string AddMobileNumberZeroPrefix(this string value)
        {
            if (string.IsNullOrEmpty(value) || value.StartsWith("0"))
            {
                return value;
            }

            if (value.StartsWith("+971"))
            {
                value = value.Replace("+971", string.Empty).Trim();
            }

            return value.Insert(0, "0").Trim();
        }

        /// <summary>
        /// The RemoveMobileNumberZeroPrefix
        /// </summary>
        /// <param name="value">The value<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string RemoveMobileNumberZeroPrefix(this string value)
        {
            if (string.IsNullOrEmpty(value) || !value.StartsWith("0"))
            {
                return value;
            }

            if (value.StartsWith("+971"))
            {
                value = value.Replace("+971", string.Empty).Trim();
            }
            if (value.StartsWith("0"))
            {
                value = value.Substring(1);
            }
            return value;
        }

        /// <summary>
        /// The FormatDate
        /// </summary>
        /// <param name="value">The value<see cref="string"/></param>
        /// <param name="formatter">The formatter<see cref="string"/></param>
        /// <returns>The <see cref="DateTime?"/></returns>
        public static DateTime? FormatDate(this string value, string formatter)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (DateTime.TryParseExact(value.Trim(), formatter, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime @return))
            {
                return @return;
            }

            return null;
        }

        /// <summary>
        /// The ConvertBytesToMegabytes
        /// </summary>
        /// <param name="bytes">The bytes<see cref="long"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double ConvertBytesToMegabytes(this long bytes)
        {
            return Math.Round((bytes / 1024f) / 1024f, 2);
        }

        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        /// <param name="value">The value<see cref="string"/></param>
        /// <param name="a">The a<see cref="string"/></param>
        /// <param name="b">The b<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string Between(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        /// <param name="value">The value<see cref="string"/></param>
        /// <param name="a">The a<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string Before(this string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        /// <param name="value">The value<see cref="string"/></param>
        /// <param name="a">The a<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string After(this string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }

        public static string GetSanitizePlainText(string dirtyString, SanitizerOptions opt = null)
        {
            if (string.IsNullOrWhiteSpace(dirtyString))
            {
                return "";
            }

            if (opt == null)
            {
                opt = new SanitizerOptions();
            }
            string regEx_style = "<style[^>]*?>[\\s\\S]*?<\\/style>"; //Regular expression defining style 
            string regEx_script = "<script[^>]*?>[\\s\\S]*?<\\/script>"; //Define script regular expression 
            string regEx_html = "<[^>]+>"; //Regular expression defining HTML tags 
            if (opt.IsUrlEncode)
            {
                dirtyString = Uri.EscapeUriString(dirtyString);
            }
            if (!opt.AllowStyle)
            {
                dirtyString = Regex.Replace(dirtyString, regEx_style, "");//Delete css
            }

            if (!opt.AllowScript)
            {
                dirtyString = Regex.Replace(dirtyString, regEx_script, "");//Delete js
            }

            if (!opt.AllowHtml)
            {
                dirtyString = Regex.Replace(dirtyString, regEx_html, "");//Remove html tags
            }
            if (!opt.AllowNewlineTab)
            {
                dirtyString = Regex.Replace(dirtyString, "\\s*|\t|\r|\n", "");//remove tabs, spaces, and blank lines
            }



            if (!opt.AllowSpace)
            {
                dirtyString = dirtyString.Replace(" ", "");
            }
            if (!opt.AllowAbnormalQuote)
            {
                dirtyString = dirtyString.Replace("\"", "");//Remove abnormal quotes "" "
            }
            return dirtyString.Trim();
        }

        public static string HtmlEncodeAndSanitize(string val, SanitizerOptions opt = null)
        {
            if(!string.IsNullOrWhiteSpace(val))
            {
                HttpUtility.HtmlEncode(GetSanitizePlainText(val));
            }
            return val;
        }
    }
}
