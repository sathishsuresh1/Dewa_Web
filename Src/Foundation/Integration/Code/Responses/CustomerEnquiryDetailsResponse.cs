using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Enums;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "RequestComplaints")]
    public class CustomerEnquiryDetails
    {
        [XmlElement(ElementName = "Iteam")]
        public CustomerEnquiry Enquiry { get; set; }
    }

    [XmlRoot(ElementName = "GetComplaintsDetails")]
    public class CustomerEnquiryDetailsResponse
    {
        [XmlElement(ElementName = "DateTimeStamp")]
        public string Datestamp { get; set; }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "RequestComplaints")]
        public CustomerEnquiryDetails CustomerEnquiryDetails { get; set; }
    }
}