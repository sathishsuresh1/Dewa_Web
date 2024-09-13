using DEWAXP.Foundation.Integration.CustomerSmartSalesSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class TenderBidingConfirm
    {
        public string confirmReferenceNumber { get; set; }
        public string confirmBidReferenceNumber { get; set; }
        public string confirmTenderNumber { get; set; }
        public bool confirmSuccess { get; set; }
    }
}