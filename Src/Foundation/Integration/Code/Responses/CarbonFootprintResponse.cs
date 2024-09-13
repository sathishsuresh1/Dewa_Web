using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "CO2Footprint")]
    public class CarbonFootprintResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Co2FootPrint")]
        public decimal Footprint { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }
}