using System;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
	[Serializable]
	[XmlRoot(ElementName = "PremiseTypeChangeResponse")]
	public class PremiseTypeChangeResponse
	{
		[XmlElement(ElementName = "ResponseCode")]
		public int ResponseCode { get; set; }

		[XmlElement(ElementName = "DateTimeStamp")]
		public string DateTimeStamp { get; set; }

		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }

		[XmlElement(ElementName = "EQMNUM")]
		public string NotificationNumber { get; set; }

		[XmlElement(ElementName = "ESUBRC")]
		public string ESUBRC { get; set; }
	}
}
