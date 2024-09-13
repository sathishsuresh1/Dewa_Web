using System;

namespace DEWAXP.Foundation.Content.Models.Bills
{
	[Serializable]
	public class RequestCollectiveAccountWorkflowState
	{
		public string BusinessPartnerNumber { get; set; }
		
		public string EmailAddress { get; set; }
		
		public string ContactName { get; set; }
		
		public string Mobile { get; set; }
		
		public string SelectedCategoryValue { get; set; }
	
		public byte[] OfficialLetterUploader { get; set; }

		public byte[] TradeLicenceUploader { get; set; }

		public string OfficialLetterFileName { get; set; }

		public string TradeLicenceFileName { get; set; }

		public string NotificationNumber { get; set; }
	}
}