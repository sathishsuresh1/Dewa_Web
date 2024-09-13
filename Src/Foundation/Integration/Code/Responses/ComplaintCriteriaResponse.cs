using System.Collections.Generic;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "CityList")]
    public class CityList
    {
        [XmlElement(ElementName = "City")]
        public List<string> Cities { get; set; }
    }

    [XmlRoot(ElementName = "CodeGrouping")]
    public class ComplaintCodes
    {
        [XmlElement(ElementName = "GroupCode")]
        public string GroupCode { get; set; }

        [XmlElement(ElementName = "GroupDesc")]
        public string GroupDescription { get; set; }

        [XmlElement(ElementName = "Code")]
        public string Code { get; set; }

        [XmlElement(ElementName = "CodeDesc")]
        public string CodeDescription { get; set; }
    }

    [XmlRoot(ElementName = "CodeGroupList")]
    public class ComplaintCodeList
    {
        [XmlElement(ElementName = "CodeGrouping")]
        public List<ComplaintCodes> Codes { get; set; }
    }

    [XmlRoot(ElementName = "ComplaintsMasterResponse")]
    public class ComplaintCriteriaResponse
    {
        [XmlElement(ElementName = "CityList")]
        public CityList CityList { get; set; }

        [XmlElement(ElementName = "CodeGroupList")]
        public ComplaintCodeList ComplaintCodeList { get; set; }
    }
}
