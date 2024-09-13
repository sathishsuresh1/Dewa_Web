// <copyright file="CommonUtility.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Content.Utils
{
    using DEWAXP.Foundation.Logger;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines the <see cref="CommonUtility" />
    /// </summary>
    public class CommonUtility
    {
        /// <summary>
        /// Defines the DF_yyyyMMddHHmmss
        /// </summary>
        public const string DF_yyyyMMddHHmmss = "yyyyMMddHHmmss";

        /// <summary>
        /// Defines the DF_yyyyMMddhhmmss
        /// </summary>
        public const string DF_yyyyMMddhhmmss = "yyyyMMddhhmmss";

        /// <summary>
        /// Defines the DF_yyyy_MM_dd_HHmmss
        /// </summary>
        public const string DF_yyyy_MM_dd_HHmmss = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Defines the DF_dd_MMM_yyyy
        /// </summary>
        public const string DF_dd_MMM_yyyy = "dd MMM yyyy";

        /// <summary>
        /// Defines the DF_dd_MMM_yyyy_HHmmss
        /// </summary>
        public const string DF_dd_MMM_yyyy_HHmmss = "dd MMM yyyy | HH:mm:ss";

        /// <summary>
        /// Defines the DF_dd_MM_yyyy_hhmmtt
        /// </summary>
        public const string DF_dd_MM_yyyy_hhmmtt = "dd/MM/yyyy hh:mm tt";

        /// <summary>
        /// The DateTimeFormatParse
        /// </summary>
        /// <param name="date">The date<see cref="string"/></param>
        /// <param name="dateFormate">The dateFormate<see cref="string"/></param>
        /// <returns>The <see cref="DateTime"/></returns>
        public static DateTime DateTimeFormatParse(string date, string dateFormate)
        {
            DateTime formattedDate = DateTime.MinValue;

            try
            {
                formattedDate = DateTime.ParseExact(date, dateFormate, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, new object());
            }
            return formattedDate;
        }

        /// <summary>
        /// The DateDiffrence
        /// </summary>
        /// <param name="fromDate">The fromDate<see cref="DateTime"/></param>
        /// <param name="toDate">The toDate<see cref="DateTime"/></param>
        /// <returns>The <see cref="long"/></returns>
        public static long DateDiffrence(DateTime fromDate, DateTime toDate)
        {

            long _fromDate = Convert.ToInt64(fromDate.ToString("yyyyMMdd"));
            long _toDate = Convert.ToInt64(toDate.ToString("yyyyMMdd"));
            return (_fromDate - _toDate);
        }

        /// <summary>
        /// The GetPaginationRange
        /// </summary>
        /// <param name="currentPage">The currentPage<see cref="int"/></param>
        /// <param name="totalPages">The totalPages<see cref="int"/></param>
        /// <returns>The <see cref="IEnumerable{int}"/></returns>
        public static IEnumerable<int> GetPaginationRange(int currentPage, int totalPages)
        {
            const int desiredCount = 5;
            List<int> returnint = new List<int>();

            int start = currentPage - 1;
            int projectedEnd = start + desiredCount;
            if (projectedEnd > totalPages)
            {
                start = start - (projectedEnd - totalPages);
                projectedEnd = totalPages;
            }

            int p = start;
            while (p++ < projectedEnd)
            {
                if (p > 0)
                {
                    returnint.Add(p);
                }
            }
            return returnint;
        }

        public static string ConvertDateEnToAr(string date)
        {
            if (!string.IsNullOrWhiteSpace(date))
            {
                date = date.Replace("January", "يناير").Replace("February", "فبراير").Replace("March", "مارس").Replace("April", "أبريل").Replace("May", "مايو").Replace("June", "يونيو").Replace("July", "يوليو").Replace("August", "أغسطس").Replace("September", "سبتمبر").Replace("October", "أكتوبر").Replace("November", "نوفمبر").Replace("December", "ديسمبر");
            }
            return date;
        }

        public static string ConvertDateArToEn(string date)
        {
            if (!string.IsNullOrWhiteSpace(date))
            {
                date = date.Replace("يناير", "January").Replace("يناير", "January")
                                     .Replace("فبراير", "February").Replace("فبراير", "February")
                                     .Replace("مارس", "March").Replace("مارس", "March")
                                     .Replace("أبريل", "April").Replace("ابريل", "April")
                                     .Replace("مايو", "May").Replace("مايو", "May")
                                     .Replace("يونيو", "June").Replace("يونيو", "June")
                                     .Replace("يوليو", "July").Replace("يوليو", "July")
                                     .Replace("أغسطس", "August").Replace("اغسطس", "August")
                                     .Replace("سبتمبر", "September").Replace("سبتمبر", "September")
                                     .Replace("أكتوبر", "October").Replace("اكتوبر", "October")
                                     .Replace("نوفمبر", "November").Replace("نوفمبر", "November")
                                     .Replace("ديسمبر", "December").Replace("ديسمبر", "December");
            }
            return date;
        }
        

        public static string GetMaskedIBAN(string plainIban)
        {
            string maskIban = null;
            if (!string.IsNullOrWhiteSpace(plainIban) && plainIban.Length > 20)
            {
                maskIban = string.Format("{0}*****00*****{1}", plainIban.Substring(0, 5), plainIban.Substring(17, plainIban.Length - 17));
            }
            return maskIban;
        }

        public static string GetSanitizePlainText(string dirtyString, Foundation.Helpers.Models.SanitizerOptions opt = null)
        {
            if (string.IsNullOrWhiteSpace(dirtyString))
            {
                return "";
            }

            if (opt == null)
            {
                opt = new Foundation.Helpers.Models.SanitizerOptions();
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
    }
}
