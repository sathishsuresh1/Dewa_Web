namespace DEWAXP.Foundation.Content.Models.Payment
{
	public class TemporaryConnectionPaymentRequestModel : PaymentRequestModel
	{
		public string ContractAccount { get; set; }

		public decimal Amount { get; set; }

		public string BusinessPartnerNumber { get; set; }

		public string EmailAddress { get; set; }

		public string NotificationReference { get; set; }

		public string MobileNumber { get; set; }

		public override PaymentContext PaymentContext
		{
			get { return PaymentContext.TemporaryConnection; }
		}
        public PaymentMethod paymentMethod { get; set; }

		public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }
}