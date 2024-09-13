namespace DEWAXP.Foundation.Integration.Requests
{
	public class LandlordDetailsParameters
	{
		public string UserId { get; set; }

		public string BusinessPartnerNumber { get; set; }

		public string SessionId { get; set; }

		public string PremiseNumber { get; set; }
		
		public string Mobile { get; set; }

		public string Remarks { get; set; }
		
		public byte[] Attachment1 { get; set; }

		public string Filename1 { get; set; }

		public string FileType1 { get; set; }

		public byte[] Attachment2 { get; set; }

		public string Filename2 { get; set; }

		public string FileType2 { get; set; }
	}
}
