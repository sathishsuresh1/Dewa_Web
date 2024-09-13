using System;
using DEWAXP.Foundation.Content.Models.Payment;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
	[Serializable]
	public class EstimatePaymentCompletionModel : PaymentCompletionModel
	{
		public EstimatePaymentCompletionModel(PaymentCompletionModel model)
			: base(PaymentContext.PayFriendsEstimate, model.Succeeded)
		{
			this.DegTransactionId = model.DegTransactionId;
			this.DewaTransactionId = model.DewaTransactionId;
			this.Message = model.Message;
			this.Receipts = model.Receipts;
			this.PaymentDate = model.PaymentDate;
		}

		public DEWAXP.Foundation.Integration.Responses.EstimateDetailItem Estimate { get; set; }

		public string PayAnotherEstimateLink { get; set; }
	}
}