using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.ConsultantSvc
{
    [Serializable]
    [XmlRoot("LoadDetails")]
    public class CostDetailsResponse
    {
        [XmlElement(ElementName = "sno")]
        public int SerialNo { get; set; }

        [XmlElement(ElementName = "fromLoad")]
        public decimal FromLoad { get; set; }

        [XmlElement(ElementName = "toLoad")]
        public decimal ToLoad { get; set; }

        [XmlElement(ElementName = "totVal")]
        public decimal TotalValue { get; set; }

        [XmlElement(ElementName = "unitVal")]
        public decimal UnitValue { get; set; }

        [XmlElement(ElementName = "quantity")]
        public decimal Quantity { get; set; }
    }
}
