using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
    [Serializable]
    public class EstimateTaxInvoicePDFRequest
    {
        /// <summary>
        /// Estimate No
        /// </summary>
        public string EstNo { get; set; }
        public string Sdtype { get; set; }
        /// <summary>
        /// document No.
        /// </summary>
        public string DocNo { get; set; }
    }
}