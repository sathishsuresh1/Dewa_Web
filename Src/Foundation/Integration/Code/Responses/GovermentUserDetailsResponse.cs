using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "GetGovtUserDetails")]
    public class GovermentUserDetailsResponse
	{
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

		[XmlElement(ElementName = "ZMOBUSR")]
		public string Username { get; set; }

		[XmlElement(ElementName = "ZEMAIL")]
        public string Email { get; set; }

		[XmlElement(ElementName = "ZMOBILE")]
		public string Mobile { get; set; }

		[XmlElement(ElementName = "ZMANAGER_MAIL")]
		public string ManagerEmail { get; set; }
	}
}