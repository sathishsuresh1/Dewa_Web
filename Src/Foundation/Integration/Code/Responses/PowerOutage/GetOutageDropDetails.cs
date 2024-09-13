using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.PowerOutage
{
    //site: https://json2csharp.com/xml-to-csharp
    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(GetOutageDropDetails));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (GetOutageDropDetails)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "OutageItem")]
    public class GetOutageDropDetails_OutageItem
    {

        [XmlElement(ElementName = "Outage_Code")]
        public string OutageCode { get; set; }

        [XmlElement(ElementName = "Outage_Text")]
        public string OutageText { get; set; }
    }

    [XmlRoot(ElementName = "Outage")]
    public class GetOutageDropDetails_Outage
    {

        [XmlElement(ElementName = "OutageItem")]
        public List<GetOutageDropDetails_OutageItem> OutageItem { get; set; }
    }

    [XmlRoot(ElementName = "InterruptionItem")]
    public class GetOutageDropDetails_InterruptionItem
    {

        [XmlElement(ElementName = "InterruptionCode")]
        public string InterruptionCode { get; set; }

        [XmlElement(ElementName = "InterruptionText")]
        public string InterruptionText { get; set; }
    }

    [XmlRoot(ElementName = "Interruption")]
    public class GetOutageDropDetails_Interruption
    {

        [XmlElement(ElementName = "InterruptionItem")]
        public List<GetOutageDropDetails_InterruptionItem> InterruptionItem { get; set; }
    }

    [XmlRoot(ElementName = "WorkItem")]
    public class GetOutageDropDetails_WorkItem
    {

        [XmlElement(ElementName = "OutageCode")]
        public string OutageCode { get; set; }

        [XmlElement(ElementName = "InterruptionCode")]
        public string InterruptionCode { get; set; }

        [XmlElement(ElementName = "WorkCode")]
        public string WorkCode { get; set; }

        [XmlElement(ElementName = "WorkDescription")]
        public string WorkDescription { get; set; }
    }

    [XmlRoot(ElementName = "Work")]
    public class GetOutageDropDetails_Work
    {

        [XmlElement(ElementName = "WorkItem")]
        public List<GetOutageDropDetails_WorkItem> WorkItem { get; set; }
    }

    [XmlRoot(ElementName = "GetOutageDropDetails")]
    public class GetOutageDropDetailsXMLResponse
    {

        [XmlElement(ElementName = "ResponseCode")]
        public string ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "Outage")]
        public GetOutageDropDetails_Outage Outage { get; set; }

        [XmlElement(ElementName = "Interruption")]
        public GetOutageDropDetails_Interruption Interruption { get; set; }

        [XmlElement(ElementName = "Work")]
        public GetOutageDropDetails_Work Work { get; set; }
    }



}
