using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class ConsumptionAccount
    {
        [XmlElement(ElementName = "Item")]
        public List<ConsumptionDataPoint> DataPoints { get; set; }

        public string AccountNumber
        {
            get
            {
                if (DataPoints.Any())
                {
                    return DataPoints.First().AccountNumber;
                }
                return string.Empty;
            }
        }
    }
    
    [XmlRoot(ElementName = "LastYearlyConsumption")]
    public class ComparativeConsumptionResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement("AccountNo")]
        public List<ConsumptionAccount> Accounts { get; set; }
    }
}
