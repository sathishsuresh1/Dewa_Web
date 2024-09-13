using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers;

namespace DEWAXP.Foundation.Integration.Responses
{
    [Serializable]
    [XmlRoot(ElementName = "Invoice")]
    public class Invoice
    {
        [XmlElement(ElementName = "Invoice_NO")]
        public string InvoiceNumber { get; set; }

        [XmlElement(ElementName = "Invoice_Date")]
        public string InvoiceDate { get; set; }

        [XmlElement(ElementName = "NetAmount")]
        public decimal NetAmount { get; set; }

        [XmlElement(ElementName = "BillMonth")]
        public string BillingMonth { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "Invoices")]
    public class InvoiceList
    {
        public InvoiceList()
        {
            Invoices = new List<Invoice>();
        }

        [XmlElement(ElementName = "Invoice")]
        public List<Invoice> Invoices { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "Payment")]
    public class Payment
    {
	    private string _contractAccountNumber;

	    [XmlElement(ElementName = "ContractAccount")]
	    public string ContractAccountNumber
	    {
            get { return DewaResponseFormatter.Trimmer(_contractAccountNumber); }
			set { _contractAccountNumber = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "PaymentAmount")]
        public decimal PaymentAmount { get; set; }

        [XmlElement(ElementName = "PaymentDate")]
        public string PaymentDate { get; set; }

        [XmlElement(ElementName = "DEGTRN")]
        public string DegTransactionReference { get; set; }

        [XmlElement(ElementName = "VendorID")]
        public string VendorId { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "Payments")]
    public class PaymentList
    {
        public PaymentList()
        {
            Payments = new List<Payment>();
        }

        [XmlElement(ElementName = "Payment")]
        public List<Payment> Payments { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "GetCapayments")]
    public class ContractAccountPaymentListResponse
    {
        public ContractAccountPaymentListResponse()
        {
            Invoices = new InvoiceList();
            Payments = new PaymentList();
        }

        [XmlElement(ElementName = "DateTimeStamp")]
        public string DateTimeStamp { get; set; }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "Invoices")]
        public InvoiceList Invoices { get; set; }

        [XmlElement(ElementName = "Payments")]
        public PaymentList Payments { get; set; }
    }
}
