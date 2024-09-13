using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
    public class EstimatePaymentConfirmationModel
    {
        public DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateCustomerItemResponse Estimate { get; set; }

        public EstimateHistory.Account EstimateHistory { get; set; }
        public decimal SuqiaAmount { get; set; }
    }

    public class FriendsEstimatePaymentConfirmationModel
    {
        public DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateDetailitemResponse Estimate { get; set; }

        public EstimateHistory.Account EstimateHistory { get; set; }
        public decimal SuqiaAmount { get; set; }
    }
}