namespace DEWAXP.Foundation.Content.Models.ClearanceCertificate
{
	public class ClearanceCertificatePaymentModel
	{
		public string BusinessPartnerNumber { get; set; }

		public string ContractAccountNumber { get; set; }

		public decimal Amount { get; set; }
		public decimal SuqiaAmount { get; set; }
        public string TransactionNumber { get; set; }
	}
}