using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "CustomerPortalLoginAuth")]
    public class SmartCustAuthenticationResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "SessionNumber")]
        public string SessionNumber { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "LastLogin")]
        public string LastLogin { get; set; }

        [XmlElement(ElementName = "PrimaryContractAccount")]
        public string PrimaryContractAccount { get; set; }

        [XmlElement(ElementName = "BusinessPartner")]
        public string BusinessPartner { get; set; }

        [XmlElement(ElementName = "Mobile")]
        public string Mobile { get; set; }

        [XmlElement(ElementName = "Email")]
        public string Email { get; set; }

        [XmlElement(ElementName = "TermsandCondition")]
        public string TermsandCondition { get; set; }

        [XmlElement(ElementName = "UpdateEmailMobile")]
        public string UpdateEmailMobile { get; set; }
    }
}