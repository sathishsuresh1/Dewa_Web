using System;

namespace DEWAXP.Foundation.Integration.Requests
{
	[Serializable]
	public class PremiseTypeChangeRequest
	{
		public string ContractAccountNumber { get; set; }

		public string MobileNumber { get; set; }

		public string PremiseNumber { get; set; }

		public byte[] Attachment { get; set; }

		public string AttachmentType { get; set; }

		public string Remarks { get; set; }
	}
}
