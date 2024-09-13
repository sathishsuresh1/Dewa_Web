using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
	[XmlRoot(ElementName = "getrateCategortyChangeResponse")]
	public class SetRateCategoryResponse
	{
		[XmlElement(ElementName = "ResponseCode")]
		public int ResponseCode { get; set; }
		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "NotifNumber")]
		public string NotificationNumber { get; set; }
	}
}
