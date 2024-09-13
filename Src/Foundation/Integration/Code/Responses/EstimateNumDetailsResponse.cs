using System;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers;

namespace DEWAXP.Foundation.Integration.Responses
{
    [Serializable]
    [XmlRoot(ElementName = "GetEstimateNumDetails")]
    public class EstimateNumDetailsResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "EstimateDetailItem")]
        public EstimateDetailItem EstimateDetail { get; set; }
    }

    [Serializable]
    public class EstimateDetailItem
    {
	    private string _ownerNum;
	    private string _consultantNo;
	    private string _caNumber;
	    private string _soldToParty;
        private decimal PartialPaymentField;

        [XmlElement(ElementName = "Sales_Distribution_Doc")]
        public string SalesDistributionDoc { get; set; }

        [XmlElement(ElementName = "Sales_Document_Type")]
        public string SalesDocumentType { get; set; }

	    [XmlElement(ElementName = "Sold_To_Party")]
	    public string SoldToParty
	    {
            get { return !string.IsNullOrEmpty(_soldToParty) ? _soldToParty.TrimStart('0') : string.Empty; }
			set { _soldToParty = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "Customer_PO_Number")]
        public string CustomerPONumber { get; set; }

	    [XmlElement(ElementName = "CA_Number")]
	    public string CaNumber
	    {
            get { return DewaResponseFormatter.Trimmer(_caNumber); }
			set { _caNumber = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "City")]
        public string City { get; set; }

        [XmlElement(ElementName = "NetValue1")]
        public decimal NetValue1 { get; set; }

        [XmlElement(ElementName = "NetValue2")]
        public decimal NetValue2 { get; set; }

        [XmlElement(ElementName = "NetValue3")]
        public decimal NetValue3 { get; set; }
        public decimal PartialPayment
        {
            get
            {
                return this.PartialPaymentField;
            }
            set
            {
                this.PartialPaymentField = value;
            }
        }
        [XmlElement(ElementName = "OwnerNum")]
	    public string OwnerNum
	    {
            get { return DewaResponseFormatter.Trimmer(_ownerNum); }
			set { _ownerNum = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "OwnerName")]
        public string OwnerName { get; set; }

        [XmlElement(ElementName = "EstimateNo")]
        public string EstimateNo { get; set; }

        [XmlElement(ElementName = "EstimateValidFromDate")]
        public DateTime EstimateValidFromDate { get; set; }

        [XmlElement(ElementName = "EstimateValidToDate")]
        public DateTime EstimateValidToDate { get; set; }

        [XmlElement(ElementName = "Comment")]
        public string Comment { get; set; }

        [XmlElement(ElementName = "Plot")]
        public string Plot { get; set; }

        [XmlElement(ElementName = "Area")]
        public string Area { get; set; }

	    [XmlElement(ElementName = "ConsultantNo")]
	    public string ConsultantNo
	    {
            get { return DewaResponseFormatter.Trimmer(_consultantNo); }
			set { _consultantNo = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "Con_Name")]
        public string Con_Name { get; set; }

        [XmlElement(ElementName = "Con_EMail")]
        public string Con_EMail { get; set; }
    }
}
