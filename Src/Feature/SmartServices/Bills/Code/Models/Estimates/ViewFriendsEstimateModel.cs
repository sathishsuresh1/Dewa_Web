using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DEWAXP.Foundation.Integration.Responses;
using Sitecore.Mvc.Presentation;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
    [Serializable]
    public class ViewFriendsEstimateModel
    {
        public DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateDetailitemResponse Estimate { get; set; }

        public string LoginLink { get; set; }
        public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }
}