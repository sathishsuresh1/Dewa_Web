using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "PrimaryContractAccountUX")]
    public class PrimaryAccountResponse
    {
        private string _accountNumber;
        private string _businessPartner;

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "PrimaryContractAccount")]
        public string AccountNumber
        {
            get { return DewaResponseFormatter.Trimmer(_accountNumber); }
            set { _accountNumber = value ?? string.Empty; }
        }

        [XmlElement(ElementName = "BusinessPartner")]
        public string BusinessPartner
        {
            get { return DewaResponseFormatter.Trimmer(_businessPartner); }
            set { _businessPartner = value ?? string.Empty; }
        }
        [XmlElement(ElementName = "Mobile")]
        public string Mobile { get; set; }
        [XmlElement(ElementName = "Email")]
        public string Email
        {
            get; set;
        }
        [XmlElement(ElementName = "FullName")]
        public string FullName { get; set; }

        [XmlElement(ElementName = "TermsandCondition")]
        public string AcceptedTerms { get; set; }

        [XmlElement(ElementName = "UpdateEmailMobile")]
        public string UpdateContactDetailsCode { get; set; }

        public bool IsUpdateContact
        {
            get { return !"X".Equals(UpdateContactDetailsCode); }
        }
    }
}
