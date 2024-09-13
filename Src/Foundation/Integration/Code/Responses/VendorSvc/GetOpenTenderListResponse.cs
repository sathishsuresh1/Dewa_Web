using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers;

namespace DEWAXP.Foundation.Integration.Responses.VendorSvc
{
    [Serializable]
    [XmlRoot(ElementName = "Tender")]
    public class Tender
    {

        [XmlElement(ElementName = "LineNumber")]
        public string LineNumber { get; set; }

        [XmlElement(ElementName = "TenderNumber")]
        public string TenderNumber { get; set; }

        [XmlElement(ElementName = "TenderDescripton")]
        public string TenderDescription { get; set; }

        [XmlElement(ElementName = "FloatingDate")]
        public string FloatingDate { get; set; }

        [XmlElement(ElementName = "ClosingDate")]
        public string ClosingDate { get; set; }

        [XmlElement(ElementName = "TenderFee")]
        public string TenderFee { get; set; }

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }
    }

    [XmlRoot(ElementName = "TenderResultList")]
    public class OpenTendersList
    {
        [XmlElement(ElementName = "Tender")]
        public List<Tender> Tender { get; set; }

    }

    [Serializable]
    [XmlRoot(ElementName = "GetOpenTenderListResponse")]
    public class GetOpenTenderListDataResponse
    {
        [XmlElement(ElementName = "OpenTendersList")]
        public OpenTendersList OpenTendersList { get; set; }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }


}
