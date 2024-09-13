using DEWAXP.Foundation.Integration.Enums;

namespace DEWAXP.Feature.Bills.Models.BusinessPartnerSelectionDetails
{
	public class BusinessPartnerDetails
	{
		public string BusinessPartnerNumber { get; set; }
	
		public string BillingClass { get; set; }

		public static BusinessPartnerDetails From(DEWAXP.Foundation.Integration.Responses.AccountDetails accountDetails)
		{
			return new BusinessPartnerDetails
			{
				BusinessPartnerNumber = accountDetails.BusinessPartnerNumber,
				BillingClass = (accountDetails.BillingClass == BillingClassification.Residential) ? "Residential" : "Non-residential"
			};
		}

	}
}