using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.EV.Models.EVDashboard
{
    public class EVTransactionModel
    {
        public EVTransactionModel()
        {
            MonthList = GenerateMonthList();
        }
        public string Selectedaccountnumber { get; set; }
        public string Selectedcardid { get; set; }
        public string Selectedmonth { get; set; }
        public string Selectedsortby { get; set; }
        public EVTransactionsConfig eVTransactionsConfig { get; set; }

        public IEnumerable<SelectListItem> MonthList { get; set; }

        public IEnumerable<SelectListItem> GenerateMonthList()
        {
            var start = DateTime.Now.AddMonths(-11);
            var end = DateTime.Now;
            end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));

            var diff = Enumerable.Range(0, Int32.MaxValue)
                                 .Select(e => start.AddMonths(e))
                                 .TakeWhile(e => e <= end).OrderByDescending(y=>y.Date)
                                 .Select((e, i) => new SelectListItem
                                 {
                                     Text = e.ToString("MMMM yyyy", global::Sitecore.Context.Culture),
                                     Value = e.ToString("MMyyyy", global::Sitecore.Context.Culture),
                                     Selected = i==0
                                 });
            return diff;
        }
        
    }
}