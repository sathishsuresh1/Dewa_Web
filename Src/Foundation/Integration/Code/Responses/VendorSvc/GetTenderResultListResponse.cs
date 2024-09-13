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
    [XmlRoot(ElementName = "TenderResult")]
    public class TenderResultListResponse
    {

        [XmlElement(ElementName = "LineNumber")]
        public string LineNumber { get; set; }

        [XmlElement(ElementName = "TenderNumber")]
        public string TenderNumber { get; set; }

        [XmlElement(ElementName = "TenderDescription")]
        public string TenderDescription { get; set; }

        [XmlElement(ElementName = "FloatDate")]
        public string FloatDate { get; set; }

        [XmlElement(ElementName = "ClosingDate")]
        public string ClosingDate { get; set; }

        [XmlElement(ElementName = "TenderType")]
        public string TenderType { get; set; }
    }

    [XmlRoot(ElementName = "TenderResultList")]
    public class TenderResultDisplayList
    {
        [XmlElement(ElementName = "TenderResult")]
        public List<TenderResultListResponse> TenderResult { get; set; }

    }

    [Serializable]
    [XmlRoot(ElementName = "GetTenderResultListResponse")]
    public class GetTenderResultListDataResponse
    {  
        [XmlElement(ElementName = "TenderResultList")]
        public TenderResultDisplayList TenderResultList { get; set; }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }


}
