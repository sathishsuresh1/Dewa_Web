using DEWAXP.Foundation.Content.Models.Payment;
using Sitecore.Web.UI.HtmlControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Models.Tayseer
{
    public class TayseerReferenceList
    {
        public int SrNo { get; set; }
        public string DewaReferenceNumber { get; set; }
        public string TotalAccounts { get; set; }
        public string TotalAmount { get; set; }
        public string CreatedDate { get; set; }
        public string PaymentStatus { get; set; }
        public string Status { get; set; }
        public string ReferenceNumberPaidAccount { get; set; }
        public decimal ReferenceNumberPaidAmount { get; set; }
        public string ContractAccountNumber { get; set; }
        public string CheckNumber { get; set; }
        public string NoCheckPay { get; set; }

        public string OkPay { get; set; }

    }
    public class TayseerReferenceListDetail
    {
        public List<TayseerReferenceList> TayseerReferenceListDetails { get; set; }
        public string TotalAmountPay { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string PaymentDewaReferenceNumber { get; set; }
        public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }
}