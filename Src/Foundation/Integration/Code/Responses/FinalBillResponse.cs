using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "FinalBill")]
    public class FinalBillResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "NotifNumber")]
        public string Reference { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }
}