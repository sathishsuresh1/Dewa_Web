using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "HighLowComplaints")]
    public class BillingComplaintResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "NotifNumber")]
        public string Reference { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }
}