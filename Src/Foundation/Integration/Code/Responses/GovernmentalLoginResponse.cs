using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "LoginSessionGov")]
    public class GovernmentalLoginResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "SessionNumber")]
        public string SessionNumber { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }
}