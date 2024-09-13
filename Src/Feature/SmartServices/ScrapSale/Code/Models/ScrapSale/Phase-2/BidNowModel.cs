using DEWAXP.Foundation.Integration.CustomerSmartSalesSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class BidNowModel
    {
        public TenderBidingStep1Model bidStep1 { get; set; }
        public TenderBidingStep2Model bidStep2 { get; set; }
        public TenderBidingStep3Model bidStep3 { get; set; }
        public TenderBidingStep5Model bidStep5 { get; set; }
        public TenderBidingStep6Model bidStep6 { get; set; }
        public TenderBidingStep7Model bidStep7 { get; set; }

        public string bidMode { get; set; }
        public List<bidAttachment> bidtenderAttachments { get; set; }
        public string successReferenceNumber { get; set; }
    }
}