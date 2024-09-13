using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Logger;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration.Fluent;
using Glass.Mapper.Sc.Web;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Helpers
{
    public static partial class DateHelper
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));

        public static IEnumerable<SelectListItem> GetMonthsList()
        {
            var months = _contentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(DataSources.MONTHS_LIST)).Items;
            var convertedItems = months.Select(m => new SelectListItem { Text = m.Text, Value = m.Value });
            return convertedItems;
        }

        public static IEnumerable<SelectListItem> GetStaticMonthList()
        {
            var months = _contentRepository.GetItem<ListDataSources>(new GetItemByIdOptions(Guid.Parse(DataSources.SHORT_WORD_MONTHS_LIST))).Items;
            var convertedItems = months.Select(m => new SelectListItem { Text = m.Text, Value = m.Value });
            return convertedItems;
        }
        public static string GetFormatedDate(string date)
        {
            var _formatteddate = date.ToString().FormatDate("dd.MM.yyyy");
            return _formatteddate.Value.ToString("dd-MMM-yyyy", Sitecore.Context.Culture);
        }
        public static string CustomDateFormate(string date, string currentFormat, string requiredFormat)
        {
            string _date = "";

            try
            {
                if (!string.IsNullOrWhiteSpace(date))
                {
                    _date = DateTime.ParseExact(date, currentFormat, new CultureInfo("en-GB")).ToString(requiredFormat);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, new object());
            }

            return _date;
        }

        public static string ConvertLocalDateFormate(string date, string currentFormat, string requiredFormat)
        {
            string d = CustomDateFormate(date, currentFormat, requiredFormat);
            if (!string.IsNullOrWhiteSpace(date) && !string.IsNullOrWhiteSpace(d) && Translate.Text("lang") == "ar")
            {
                //"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
                date = d?.ToLower().Replace("jan", "يناير").Replace("january", "يناير")
                                    .Replace("feb", "فبراير").Replace("february", "فبراير")
                                    .Replace("mar", "مارس").Replace("march", "مارس")
                                    .Replace("apr", "أبريل").Replace("april", "أبريل")
                                    .Replace("may", "مايو")
                                     .Replace("jun", "يونيو").Replace("june", "يونيو")
                                    .Replace("jul", "يوليو").Replace("july", "يوليو")
                                    .Replace("aug", "أغسطس").Replace("august", "أغسطس")
                                    .Replace("sep", "سبتمبر").Replace("september", "سبتمبر")
                                    .Replace("oct", "أكتوبر").Replace("october", "أكتوبر")
                                    .Replace("nov", "نوفمبر").Replace("november", "نوفمبر")
                                    .Replace("dec", "ديسمبر").Replace("december", "ديسمبر");
            }
            return date;
        }
    }
}