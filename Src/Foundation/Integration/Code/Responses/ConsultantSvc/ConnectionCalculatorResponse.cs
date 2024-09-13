using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.ConsultantSvc
{
    [Serializable]
    [XmlRoot(ElementName = "GetConnectionServChargesCalc")]
    public class ConnectionCalculatorResponse : BaseResponse
    {
        [XmlElement(ElementName = "LoadDetails")]
        public List<CostDetailsResponse> LoadDetails { get; set; }

        [XmlElement(ElementName = "calc")]
        public string TotalCalculation { get; set; }

        [XmlElement(ElementName = "rate")]
        public string Rate { get; set; }
    }
}
