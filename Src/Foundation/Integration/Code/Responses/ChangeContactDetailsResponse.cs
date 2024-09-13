using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
	[XmlRoot(ElementName = "UpdateContactDetails")]
	public class ChangeContactDetailsResponse
	{
		[XmlElement(ElementName = "ResponseCode")]
		public int ResponseCode { get; set; }

		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }
	}
}