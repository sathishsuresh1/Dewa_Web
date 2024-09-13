using System.Collections.Generic;
using System.Linq;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
    public class EstimatePaymentHistoryModel
    {
        public EstimatePaymentHistoryModel()
        {
            HistoricalEstimates = new List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimatehistoryItemResponse>();
        }

        public List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimatehistoryItemResponse> HistoricalEstimates { get; set; }

        public bool ThirdPartyPayments { get; set; }

        public int ItemsPerPage
        {
            get { return 20; }
        }

        public int TotalPages
        {
            get
            {
                if (!HistoricalEstimates.Any())
                {
                    return 0;
                }
                return (HistoricalEstimates.Count + ItemsPerPage - 1) / ItemsPerPage;
            }
        }
    }
}