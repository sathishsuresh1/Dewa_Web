using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers;

namespace DEWAXP.Foundation.Integration.Responses
{
    [Serializable]
    [XmlRoot(ElementName = "Iteam")]
    public class Receipt
    {
	    private string _accountNumber;
	    private string _businessPartnerNumber;

	    [XmlElement(ElementName = "DEWATransactionID")]
        public string DewaTransactionReference { get; set; }

	    [XmlElement(ElementName = "ContractAccountNumber")]
	    public string AccountNumber
	    {
            get { return DewaResponseFormatter.Trimmer(_accountNumber); }
			set { _accountNumber = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "BusinessPartner")]
	    public string BusinessPartnerNumber
	    {
            get { return DewaResponseFormatter.Trimmer(_businessPartnerNumber); }
			set { _businessPartnerNumber = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "NewStatus")]
        public string NewStatus { get; set; }

        [XmlElement(ElementName = "OldStatus")]
        public string OldStatus { get; set; }

        [XmlElement(ElementName = "SPCODE")]
        public string SPCODE { get; set; }

        [XmlElement(ElementName = "SERVCODE")]
        public string ServiceCode { get; set; }

        [XmlElement(ElementName = "PMTCHNL")]
        public string PaymentDate { get; set; }

        [XmlElement(ElementName = "ReceiptID")]
        public string ReceiptId { get; set; }

        [XmlElement(ElementName = "PMTMODE")]
        public string PaymentMode { get; set; }

        [XmlElement(ElementName = "PMTMETHOD")]
        public string PaymentMethod { get; set; }

        [XmlElement(ElementName = "TRNAMOUNT")]
        public decimal Amount { get; set; }

        [XmlElement(ElementName = "PMTGATENAME")]
        public string PaymentGateway { get; set; }

        [XmlElement(ElementName = "PMTGATETXNNO")]
        public string PaymentGatewayTransactionReference { get; set; }

        [XmlElement(ElementName = "CARDTYPE")]
        public string Cardtype { get; set; }

        [XmlElement(ElementName = "ReconStatus")]
        public string ReconStatus { get; set; }

        [XmlElement(ElementName = "PMTGATERECSTAT")]
        public string PaymentGatewayReconStatus { get; set; }

        [XmlElement(ElementName = "BankReconStatus")]
        public string BankReconStatus { get; set; }

        [XmlElement(ElementName = "REFUNDID")]
        public string Refundid { get; set; }

        [XmlElement(ElementName = "REFUNDSTAT")]
        public string Refundstat { get; set; }

        [XmlElement(ElementName = "DISPUTEID")]
        public string Disputeid { get; set; }

        [XmlElement(ElementName = "MESSAGECODE")]
        public string Messagecode { get; set; }

        [XmlElement(ElementName = "MESSAGE")]
        public string Message { get; set; }
	}

    [XmlRoot(ElementName = "GetBillReceipt")]
    public class BillReceiptResponse
    {
        [XmlElement(ElementName = "DateTimeStamp")]
        public string DateTimeStamp { get; set; }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "Iteam")]
        public List<Receipt> Receipts { get; set; }
    }
}
