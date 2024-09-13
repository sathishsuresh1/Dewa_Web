using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers;

namespace DEWAXP.Foundation.Integration.Responses
{
    [Serializable]
    [XmlRoot(ElementName = "GetBillEnquiryResponse")]
    public class BillEnquiryResponse
    {
	    private string _accountNumber;

	    public BillEnquiryResponse()
        {
            Bills = new List<Bill>();
        }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

	    [XmlElement(ElementName = "AccountNumber")]
	    public string AccountNumber
	    {
            get { return DewaResponseFormatter.Trimmer(_accountNumber); }
			set { _accountNumber = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "NickName")]
        public string Nickname { get; set; }

        [XmlElement(ElementName = "Water")]
        public decimal Water { get; set; }

        [XmlElement(ElementName = "Electricity")]
        public decimal Electricity { get; set; }

        [XmlElement(ElementName = "Sewerage")]
        public decimal Sewerage { get; set; }

        [XmlElement(ElementName = "Housing")]
        public decimal Housing { get; set; }
        
        [XmlElement(ElementName = "Cooling")]
        public decimal Cooling { get; set; }
        [XmlElement(ElementName = "Irrigation")]
        public decimal DM { get; set; }

        [XmlElement(ElementName = "Others")]
        public decimal Other { get; set; }

        [XmlElement(ElementName = "Total")]
        public decimal Balance { get; set; }

        [XmlElement(ElementName = "FinalBill")]
        public string FinalBillCode { get; set; }
        
        public bool PartialPaymentPermitted
        {
            get { return !"X".Equals(FinalBillCode); }
        }

        [XmlElement("ContractAccount")]
        public List<Bill> Bills { get; set; }

        public bool ContainsMultipleAccounts
        {
            get { return Bills.Any(); }
        }
    }

    [Serializable]
    [XmlRoot("ContractAccount")]
    public class Bill
    {
	    private string _accountNumber;

	    [XmlElement(ElementName = "AccountNumber")]
	    public string AccountNumber
	    {
            get { return DewaResponseFormatter.Trimmer(_accountNumber); }
			set { _accountNumber = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "NickName")]
        public string Nickname { get; set; }

        [XmlElement(ElementName = "Water")]
        public decimal Water { get; set; }

        [XmlElement(ElementName = "Electricity")]
        public decimal Electricity { get; set; }

        [XmlElement(ElementName = "Sewerage")]
        public decimal Sewerage { get; set; }

        [XmlElement(ElementName = "Housing")]
        public decimal Housing { get; set; }

        [XmlElement(ElementName = "Cooling")]
        public decimal Cooling { get; set; }
        [XmlElement(ElementName = "Irrigation")]
        public decimal DM { get; set; }

        [XmlElement(ElementName = "Others")]
        public decimal Other { get; set; }

        [XmlElement(ElementName = "Total")]
        public decimal Total { get; set; }

        [XmlElement(ElementName = "FinalBill")]
        public string FinalBillCode { get; set; }

        public bool PartialPaymentPermitted
        {
            get { return !"X".Equals(FinalBillCode); }
        }
    }
}
