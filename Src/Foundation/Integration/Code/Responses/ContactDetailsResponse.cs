using System;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    /*
    <?xml version="1.0" encoding="UTF-8" standalone="no"?><Getupdatecontactdetails><DateTimeStamp>20151014152703</DateTimeStamp><ResponseCode>000</ResponseCode><Description>Success</Description><ContractAccount><Account>2002261016</Account><PoBOX>97</PoBOX><Emirate>DXB</Emirate><Telephone>null</Telephone><Mobile>0559999999</Mobile><Email>dummygreendewa@gmail.com</Email><Fax/><PreferredLang>EN</PreferredLang><NickName/></ContractAccount></Getupdatecontactdetails>
    */

    [Serializable]
    [XmlRoot(ElementName = "Getupdatecontactdetails")]
    public class ContactDetailsResponse
    {
        [XmlElement(ElementName = "DateTimeStamp")]
        public string DateTimeStamp { get; set; }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "ContractAccount")]
        public AccountContactDetails ContactDetails { get; set; }
    }

    [Serializable]
    public class AccountContactDetails
    {
        [XmlElement(ElementName = "Account")]
        public string Account { get; set; }

        [XmlElement(ElementName = "PoBOX")]
        public string PoBox { get; set; }

        [XmlElement(ElementName = "Emirate")]
        public string Emirate { get; set; }

        [XmlElement(ElementName = "Telephone")]
        public string Telephone { get; set; }

        [XmlElement(ElementName = "Mobile")]
        public string Mobile { get; set; }

        [XmlElement(ElementName = "Email")]
        public string Email { get; set; }

        [XmlElement(ElementName = "Fax")]
        public string Fax { get; set; }

        [XmlElement(ElementName = "PreferredLang")]
        public string PreferredLang { get; set; }

        [XmlElement(ElementName = "NickName")]
        public string NickName { get; set; }
    }
}
