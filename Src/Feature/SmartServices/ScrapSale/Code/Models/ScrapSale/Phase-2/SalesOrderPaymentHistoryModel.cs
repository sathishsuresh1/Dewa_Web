using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DEWAXP.Foundation.Integration.CustomerSmartSalesSvc;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class SalesOrderPaymentHistoryModel
    {
        public emdList emdList { get; set; }
        public scrapOrdersDetailsList scrapOrdersDetailsList { get; set; }

        // Download Dcoument Params
        public string fiscalYear { get; set; }
        public string companyCode { get; set; }
        public string accDocNumber { get; set; }
        public string salesDocNumber { get; set; }
        public string emdAccDocNumber { get; set; }
        public string tenderRefNumber { get; set; }
    }
}