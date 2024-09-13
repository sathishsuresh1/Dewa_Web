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
    [XmlRoot(ElementName = "ClearanceCertificateGen")]
    public class ClearanceCertificateApplicationResponse
    {
	    private string _contractAccountNumber;

	    [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "TxnNo")]
        public string TransactionNumber { get; set; }

        [XmlElement(ElementName = "Amount")]
        public decimal Amount { get; set; }

	    [XmlElement(ElementName = "PostingContractAccount")]
	    public string ContractAccountNumber
	    {
            get { return DewaResponseFormatter.Trimmer(_contractAccountNumber); }
			set { _contractAccountNumber = value ?? string.Empty; }
		}
    }
}
