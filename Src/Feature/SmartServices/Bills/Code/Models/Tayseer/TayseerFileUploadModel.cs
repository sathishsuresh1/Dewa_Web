using Sitecore.Web.UI.HtmlControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Models.Tayseer
{
    public class TayseerFileUploadModel
    {
        public int SrNo { get; set; }
        public string ContractAccountNo { get; set; }
        public string OutstandingAmount { get; set; }
        public string AmounttoPay { get; set; }
        public string Remarks { get; set; }
        public bool isSelected { get; set; }
        public string Indicator { get; set; }

    }
    public class TayseerFileUploadDetail
    {
        public List<TayseerFileUploadModel> TayseerFileUploadDetails { get; set; }
        public List<SelectListItem> _emaildropdown { get; set; }
    }
}