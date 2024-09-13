using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web.Mvc;
using Sitecore.Data.Items;
using DEWAXP.Foundation.Logger;
using Glass.Mapper.Sc.Configuration.Fluent;
using System.Globalization;

namespace DEWAXP.Foundation.Helpers
{
    public static partial class DateHelper
    {
        private static ISitecoreService _service;
        private static ISitecoreService SCService
        {
            get { return _service ?? (_service = new SitecoreService("web")); }
        }

        public static DateTime Today()
        {
            var timeZone = System.TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == "Arabian Standard Time");
            if (timeZone == null)
            {
                return DateTime.Today;
            }
            return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, timeZone).Date;
        }

        public static DateTime DubaiNow()
        {
            var dubai = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
            var dateAndOffset = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, dubai);
            return dateAndOffset.DateTime;
        }

        public static string Ago(DateTime timestamp)
        {
            var difference = DubaiNow() - timestamp;
            if (difference.Days > 0) return difference.Days + " days ago";
            if (difference.Hours > 0) return difference.Hours + "h ago";
            if (difference.Minutes > 0) return difference.Minutes + "min ago";
            if (difference.Seconds > 0) return difference.Seconds + "s ago";
            return "Just now";
        }


        //TO DO ::
        //public static IEnumerable<SelectListItem> GetMonthsList()
        //{
        //    var listing = SCService.GetItem<Item>(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE));
        //    var children = listing.Axes.GetDescendants()
        //        .Select(c => _service.Cast<Article>(c)).Where(c => c != null
        //        && (c.TemplateId == Guid.Parse("960E0516-38BC-495D-A49B-57A9EB0CE1CA")
        //        || c.TemplateId == Guid.Parse("A72A1FCC-CDB9-4949-842F-A8001075A7EA")))
        //        .ToList().OrderByDescending(c => c.PublishDate).FirstOrDefault();

        //    var months = SCService.GetItem<ListDataSources>(DataSources.MONTHS_LIST, false).Items;
        //    var convertedItems = months.Select(m => new SelectListItem { Text = m.Text, Value = m.Value, Selected = m.Value == children.PublishDate.Month.ToString() ? true : false });
        //    return convertedItems;
        //}

        //public static IEnumerable<SelectListItem> GetStaticMonthList()
        //{
        //    var months = SCService.GetItem<ListDataSources>(DataSources.SHORT_WORD_MONTHS_LIST, false).Items;
        //    var convertedItems = months.Select(m => new SelectListItem { Text = m.Text, Value = m.Value });
        //    return convertedItems;
        //}

        public static bool InRange(this DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck >= startDate && dateToCheck < endDate;
        }

        public static string CustomDateFormate(string date, string currentFormat, string requiredFormat)
        {
            string _date = "";


            try
            {
                if (!string.IsNullOrWhiteSpace(date))
                {
                    _date = DateTime.ParseExact(date, currentFormat, System.Globalization.CultureInfo.InvariantCulture).ToString(requiredFormat);
                }
            }
            catch (Exception ex)
            {

                LogService.Error(ex, typeof(DateHelper));
            }

            return _date;

        }
        public static DateTime getCultureDate(string strDate)
        {
            CultureInfo culture;
            DateTimeStyles styles;


            culture = Sitecore.Context.Culture;
            if (culture.ToString().Equals("ar-AE"))
            {
                strDate = strDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            }
            styles = DateTimeStyles.None;
            DateTime dateResult;
            DateTime.TryParse(strDate, culture, styles, out dateResult);
            return dateResult;
        }
    }
}