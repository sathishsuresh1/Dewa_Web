using DEWAXP.Foundation.Integration.DewaSvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Linq;
using SitecoreX = Sitecore.Context;
using Sitecore.Globalization;
using DEWAXP.Foundation.Content;

namespace DEWAXP.Feature.Bills.Models.Refund
{
    public class RefundModel
    {
        public DateTime Date { get; set; }

        public string NonFormattedDate { get; set; }

        public string FormattedDate => Date.ToString("dd MMM yyyy", SitecoreX.Culture);

        public string FormattedDateTime => Date.ToString("dd MMM yyyy | HH:mm:ss", SitecoreX.Culture);

        public string mtcnnumber { get; set; }
        public string mtcnnumbervalidity { get; set; }

        public string notificationnumber { get; set; }

        public string refundapproved { get; set; }

        public string refundinitiated { get; set; }

        public string refundcode { get; set; }

        public string refundstatusdescription { get; set; }

        public string refundrejectedreason { get; set; }

        public string refundstatus { get; set; }
        public bool refundrejected { get; set; }

        public string refundstatustext { get; set; }
        public bool IsIBANNotExit { get; set; }
        public string IbanMessage { get; set; }
        public string refundsteps { get; set; }
        public int refundstepcount { get; set; }

        public string refundtype { get; set; }

        public string refundtypetext { get; set; }

        public string secretcode { get; set; }

        public string receivingcountry { get; set; }

        public string payoutamount { get; set; }

        public string payoutcurrency { get; set; }
        //unused : can be used in future
        public static object books_1 { get; private set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NameChangeAllow { get; set; }
        public string NameChangeAllowDescription { get; set; }

        public static RefundModel From(refundHistoryDetails refund, List<SelectListItem> lstds, List<SelectListItem> lstacc
            , List<SelectListItem> lstch, List<SelectListItem> lstiban, List<SelectListItem> lstwu)
        {
            var lststatus = Enumerable.Empty<SelectListItem>();
            var refundsteps = string.Empty;
            int refundcount = 0;
            int[] a = null;
            bool isIbanNoAvailble = false;
            if (!String.IsNullOrEmpty(refund.deletestatus))
            {
                a = Array.ConvertAll(refund.deletestatus.Split(','), int.Parse).OrderByDescending(x => x).ToArray();
            }

            switch (refund.refundtype)
            {
                case "IBRF":
                    List<SelectListItem> _lstiban = new List<SelectListItem>(lstiban.ToArray());
                    if (a != null)
                    {
                        foreach (int item in a)
                        {
                            _lstiban.RemoveAt(item - 1);

                        }
                        lststatus = _lstiban;
                    }

                    isIbanNoAvailble = string.IsNullOrWhiteSpace(refund.ibanNumber);
                    break;
                case "EWRF":
                    List<SelectListItem> _lstwu = new List<SelectListItem>(lstwu.ToArray());

                    if (a != null)
                    {
                        foreach (int item in a)
                        {
                            _lstwu.RemoveAt(item - 1);

                        }
                        lststatus = _lstwu;
                    }

                    break;
                case "CQRF":
                    List<SelectListItem> _lstch = new List<SelectListItem>(lstch.ToArray());
                    if (a != null)
                    {
                        foreach (int item in a)
                        {
                            _lstch.RemoveAt(item - 1);

                        }
                        lststatus = _lstch;
                    }
                    break;
                case "ATRF":
                    List<SelectListItem> _lstacc = new List<SelectListItem>(lstacc.ToArray());
                    if (a != null)
                    {
                        foreach (int item in a)
                        {
                            _lstacc.RemoveAt(item - 1);

                        }
                        lststatus = _lstacc;
                    }
                    break;
            }
            var lststatuscount = lststatus.Select((item, index) => new { index, item });
            refundcount = lststatuscount.Where(x => x.item.Value.Equals(refund.refundstatus)).FirstOrDefault().index;
            var model = new RefundModel
            {
                mtcnnumber = refund.mtcnnumber,
                mtcnnumbervalidity = !string.IsNullOrWhiteSpace(refund.mtcnvaliditydate) && !refund.mtcnvaliditydate.Equals("0000-00-00") ?
               (SitecoreX.Culture.Name.Equals("ar-AE") ?
                string.Format(Translate.Text("updateiban.Mtcn_number_validity"), DateTime.ParseExact(refund.mtcnvaliditydate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("MMM dd، yyyy", SitecoreX.Culture)) :
                string.Format(Translate.Text("updateiban.Mtcn_number_validity"), DateTime.ParseExact(refund.mtcnvaliditydate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("MMM dd, yyyy", SitecoreX.Culture))) :
                string.Empty,
                Date = DateTime.ParseExact(refund.notificationdate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                NonFormattedDate = refund.notificationdate,
                notificationnumber = refund.notificationnumber,
                refundapproved = refund.refundapproved,
                refundinitiated = refund.refundinitiated,
                refundrejectedreason = refund.refundrejectedreason,
                refundstatus = refund.refundstatus,
                refundtype = refund.refundtype,
                secretcode = refund.secretcode,
                receivingcountry = refund.receivecountryname,
                payoutamount = refund.receivetransactioncurrencyamount,
                payoutcurrency = refund.receivetransactioncurrency,
                refundstatusdescription = refund.refundstatusdescription,
                refundcode = refund.refundcode,
                refundtypetext = lstds.Where(x => x != null && x.Value.Equals(refund.refundtype)).FirstOrDefault()?.Text ?? "",
                refundstatustext = lststatus.Where(x => x != null && x.Value.Equals(refund.refundstatus)).FirstOrDefault()?.Text ?? "",
                refundsteps = lststatus.Select(i => i.Text).Aggregate((i, j) => i + "," + j),
                refundstepcount = refundcount + 1,
                refundrejected = refund.refundstatus.Equals(DataSources.REFUNDREJECTED) || refund.refundstatus.Equals(DataSources.REFUNDMTCNCANCELLED),
                IsIBANNotExit = isIbanNoAvailble,
                IbanMessage = refund.ibanMessage,
                FirstName = refund.firstname,
                LastName = refund.lastname ,
                NameChangeAllow = refund.namechangeallowed,
                NameChangeAllowDescription = refund.namechangestatus,
                
            };

            return model;
        }

    }
}