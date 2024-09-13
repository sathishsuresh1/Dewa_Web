using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
    public class EsitimateRefundViewModel
    {
        public string ApplicationNumber {get;set; }
        public string CustomerName { get; set; }
        public string RegisteredMobileNumber { get; set; }
        public string RegisteredEmail { get; set; }
        public string ReasonForRefund { get; set; }
        public string IBANNo { get; set; }       
        public string ConfirmIBANno { get; set; }
        public string IBANNoMasked { get; set; }
        public string RefundTo { get; set; }
        public string OkCheque { get; set; }
        public string OkIBAN { get; set; }
        public string ChequeNO { get; set; }
        public string MaskedRefundTo { get; set; }
        public string ReferenceNo { get; set; }
        public string RefundMode { get; set; }
        public List<NewConnectionRefundIbandetail> IbanDetails { get; set; }
        public List<NewConnectionRefundConnectiondetail> ConnectionDetails { get; set; }
        public HttpPostedFileBase FTADeclarationFormFile { get; set; }
        public List<SelectListItem> RefundModeList { get; set; }
        public List<SelectListItem> ReasonCodeList { get; set; }
        public string AttachFlag { get; set; }
        public bool IsServiceFailure { get; set; }
        public string EstimateNo { get; set; }
    }

    public class GetAppilcationDetails
    {
        /// <summary>
        /// Application ID
        /// </summary>
        public string AppID { get; set; }
        /// <summary>
        /// Estimate no
        /// </summary>
        public string EstNo { get; set; }
        /// <summary>
        /// Reason
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// Refund Mode
        /// </summary>
        public string rmode { get; set; }

    }
}