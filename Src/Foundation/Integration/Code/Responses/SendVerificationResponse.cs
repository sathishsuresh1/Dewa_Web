using System;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers;

namespace DEWAXP.Foundation.Integration.Responses
{
	[Serializable]
	[XmlRoot(ElementName = "SendVerification")]
	public class SendVerificationResponse
	{
		private string _businessPartnerNumber;

		[XmlElement(ElementName = "ResponseCode")]
		public int ResponseCode { get; set; }

		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }

		[XmlElement(ElementName = "BusinessPartnerNumber")]
		public string BusinessPartnerNumber
		{
            get { return DewaResponseFormatter.Trimmer(_businessPartnerNumber); }
			set { _businessPartnerNumber = value ?? string.Empty; }
		}

		[XmlElement(ElementName = "Mobile")]
		public string Mobile { get; set; }

		[XmlElement(ElementName = "Email")]
		public string Email { get; set; }

        [XmlElement(ElementName = "Maxattempt")]
        public string Maxattempt { get; set; }

        [XmlElement(ElementName = "RandomNumber")]
        public string RandomNumber { get; set; }

    }
}
