using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.ScrapSale
{
    [XmlRoot(ElementName = "ResetCustomerPortalPassword")]
    public class ResetCustomerPortalPassword
    {
        [XmlElement(ElementName = "ResponseCode")]
        public string ResponseCode { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "Email")]
        public string Email { get; set; }
    }
}
