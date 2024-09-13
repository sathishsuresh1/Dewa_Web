using System;

namespace DEWAXP.Feature.Bills.Models.LandlordInformation
{
	[Serializable]
	public class ChangeLandlordDetailsWorkflowState
	{
		public string BusinessPartnerNumber { get; set; }

		public string PremiseNumber { get; set; }

		public string Remarks { get; set; }

		public string Mobile { get; set; }

		public byte[] OfficialLetter { get; set; }

		public byte[] TitleDeed { get; set; }

		public string OfficialLetterFileName { get; set; }

		public string TitleDeedFileName { get; set; }

		public string NotificationNumber { get; set; }

		public string OfficialLetterType { get; set; }

		public string TitleDeedType { get; set; }
	}
}