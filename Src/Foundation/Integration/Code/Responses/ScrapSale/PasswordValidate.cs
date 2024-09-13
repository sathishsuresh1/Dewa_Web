
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.ScrapSale
{
    [XmlRoot(ElementName = "PasswordValidate")]
    public class PasswordValidate
    {
        [XmlElement(ElementName = "ResponseCode")]
        public string ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }
}
