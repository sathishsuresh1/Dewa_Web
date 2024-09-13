using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Extensions;

namespace DEWAXP.Foundation.Integration.Responses
{
    [Serializable]
    [XmlRoot(ElementName = "CustomerDetails")]
    public class GetCustomerDetailsServiceResponse
    {
	    public GetCustomerDetailsServiceResponse()
        {
            BusinessPartners = new List<BusinessPartner>();
        }

        [XmlElement(ElementName = "ResponseCode")]
        public string ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }


        [XmlElement("BusinessPartnerDetail")]
        public List<BusinessPartner> BusinessPartners { get; set; }

    }

    [Serializable]
    [XmlRoot("BusinessPartnerDetail")]
    public class BusinessPartner
    {
	    [XmlElement(ElementName = "businesspartnernumber")]
        public string businesspartnernumber
	    {
            get;
            set;
		}

        [XmlElement(ElementName = "bpname")]
        public string bpname { get; set; }

        [XmlElement(ElementName = "mobilenumber")]
        public string mobilenumber { get; set; }

        [XmlElement(ElementName = "email")]
        public string email { get; set; }

        [XmlElement(ElementName = "Type")]
        public string CustomerType { get; set; }
        [XmlElement(ElementName = "EmiratesID")]
        public string EmiratesID { get; set; }

        [XmlElement(ElementName = "EmiratesIDExpiry")]
        public string EmiratesIDExpiry { get; set; }
        [XmlElement(ElementName = "TradeLicensenumber")]
        public string TradeLicensenumber { get; set; }
        [XmlElement(ElementName = "TradeLicenseExpiry")]
        public string TradeLicenseExpiry { get; set; }
        [XmlElement(ElementName = "FirstName")]
        public string FirstName { get; set; }
        [XmlElement(ElementName = "LastName")]
        public string LastName { get; set; }
        [XmlElement(ElementName = "POBox")]
        public string POBox { get; set; }
        [XmlElement(ElementName = "Nationality")]
        public string Nationality { get; set; }

        [XmlElement(ElementName = "KindOfBP")]
        public string BPType { get; set; }

    }
}
