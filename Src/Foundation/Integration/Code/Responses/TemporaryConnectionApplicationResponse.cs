using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "tempConnectionResponse")]
    public class TemporaryConnectionApplicationResponse
    {
        [XmlElement(ElementName = "DateTimeStamp")]
        public string DateTimeStamp { get; set; }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "EQMNUM")]
        public string RequestNumber { get; set; }

        [XmlElement(ElementName = "ESUBRC")]
        public string InnerResponse { get; set; }
    }
}
