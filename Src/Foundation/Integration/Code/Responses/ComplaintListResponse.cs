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
    [XmlRoot(ElementName = "Iteam")]
    public class Complaint
    {
	    private string _accountNumber;
	    private string _businessPartnerNumber;

	    [XmlElement(ElementName = "Request_NO")]
        public string Reference { get; set; }

	    [XmlElement(ElementName = "ContractAccount")]
	    public string AccountNumber
	    {
            get { return DewaResponseFormatter.Trimmer(_accountNumber); }
			set { _accountNumber = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "AttachFlag")]
        public string AttachFlag { get; set; }

        [XmlElement(ElementName = "MeterNo")]
        public string MeterNumber { get; set; }

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "CurrentDate")]
        public string CurrentDate { get; set; }

        [XmlElement(ElementName = "CompletedDate")]
        public string CompletedDate { get; set; }

        [XmlElement(ElementName = "CompletedTime")]
        public string CompletedTime { get; set; }

        [XmlElement(ElementName = "RequestType")]
        public string RequestType { get; set; }

        [XmlElement(ElementName = "RequestDate")]
        public string RequestDate { get; set; }

        [XmlElement(ElementName = "RequestTime")]
        public string RequestTime { get; set; }

	    [XmlElement(ElementName = "BPNum")]
	    public string BusinessPartnerNumber
	    {
            get { return DewaResponseFormatter.Trimmer(_businessPartnerNumber); }
			set { _businessPartnerNumber = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "CodeGroup")]
        public string CodeGroup { get; set; }

        [XmlElement(ElementName = "ComplaintRequestType")]
        public ComplaintType Type { get; set; }
	}

    [XmlRoot(ElementName = "RequestComplaints")]
    public class ComplaintList
    {
        [XmlElement(ElementName = "Iteam")]
        public List<Complaint> Complaints { get; set; }
    }

    [XmlRoot(ElementName = "GetComplaintsList")]
    public class ComplaintListResponse
    {
        [XmlElement(ElementName = "DateTimeStamp")]
        public string DateTimeStamp { get; set; }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "RequestComplaints")]
        public ComplaintList ComplaintList { get; set; }
    }
}
