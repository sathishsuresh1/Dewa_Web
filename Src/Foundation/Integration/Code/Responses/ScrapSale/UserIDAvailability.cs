using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.ScrapSale
{
    [XmlRoot(ElementName = "UserIDAvailability")]
    public  class UserIDAvailability
    {
        [XmlElement(ElementName = "ResponseCode")]
        public string ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "UserID")]
        public string UserID { get; set; }
    }
}
