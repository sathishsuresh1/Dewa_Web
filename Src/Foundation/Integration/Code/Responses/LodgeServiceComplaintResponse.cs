using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "SetLogComplaints")]
    public class LodgeServiceComplaintResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "QmNum")]
        public string Reference { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }
}