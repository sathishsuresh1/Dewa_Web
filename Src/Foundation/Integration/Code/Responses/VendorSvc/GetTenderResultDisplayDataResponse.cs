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
    [XmlRoot(ElementName = "TenderOffer")]
    public class TenderOffer
    {

        [XmlElement(ElementName = "OfferNumber")]
        public string OfferNumber { get; set; }

        [XmlElement(ElementName = "TendererName")]
        public string TendererName { get; set; }

        [XmlElement(ElementName = "AmountAED")]
        public string AmountAED { get; set; }

        [XmlElement(ElementName = "Currency1")]
        public string Currency1 { get; set; }

        [XmlElement(ElementName = "Currency2")]
        public string Currency2 { get; set; }

        [XmlElement(ElementName = "DeliveryDays")]
        public string DeliveryDays { get; set; }

        [XmlElement(ElementName = "BankGuarantee")]
        public string BankGuarantee { get; set; }

        [XmlElement(ElementName = "Remarks")]
        public string Remarks { get; set; }
    }

    [XmlRoot(ElementName = "OfferList")]
    public class OfferList
    {
        [XmlElement(ElementName = "TenderOffer")]
        public List<TenderOffer> TenderOffer { get; set; }

    }

    [Serializable]
    [XmlRoot(ElementName = "GetTenderResultDisplayResponse")]
    public class GetTenderResultDisplayDataResponse
    {   
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "TenderNumber")]
        public string TenderNumber { get; set; }
        
        [XmlElement(ElementName = "TenderDescription")]
        public string TenderDescription { get; set; }

        [XmlElement(ElementName = "FloatingDate")]
        public string FloatingDate { get; set; }

        [XmlElement(ElementName = "ClosingDate")]
        public string ClosingDate { get; set; }

        [XmlElement(ElementName = "TenderType")]
        public string TenderType { get; set; }

        [XmlElement(ElementName = "OfferList")]
        public OfferList OfferList { get; set; }


    }


}
