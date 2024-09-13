using System;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
	[Serializable]
	[XmlRoot(ElementName = "CustomerRegistration")]
	public class RegisterCustomerResponse
	{
		[XmlElement(ElementName = "ResponseCode")]
		public int ResponseCode { get; set; }
		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "Message")]
		public string Message { get; set; }
	}
}
