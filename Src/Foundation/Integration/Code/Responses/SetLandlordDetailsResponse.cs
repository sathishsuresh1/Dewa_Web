using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
	[XmlRoot(ElementName = "ChangeLandLordResponse")]
	public class SetLandlordDetailsResponse
	{
		[XmlElement(ElementName = "NotificationNumber")]
		public string NotificationNumber { get; set; }

		[XmlElement(ElementName = "DateTimeStamp")]
		public string DateTimeStamp { get; set; }

		[XmlElement(ElementName = "ResponseCode")]
		public int ResponseCode { get; set; }

		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }
	}
}
