using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class TenderBidingStep3Model
    {
        public string TenderBidRefNumber { get; set; }
        public string TenderNumber { get; set; }
        public string EarnestDepositAmount { get; set; }
        public string SubmitType { get; set; }
        public bool IsOnline { get; set; }
        public string chkEarnestmoney { get; set; }
        public string chkTenderbond { get; set; }
        public string TenderEndDescription { get; set; }
        public string TenderARDescription { get; set; }
    }
}