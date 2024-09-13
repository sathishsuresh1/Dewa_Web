using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers;

namespace DEWAXP.Foundation.Integration.Responses
{
    [Serializable]
    [XmlRoot(ElementName = "Iteam")]
    public class CustomerEnquiry
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
        //NOCEDIT

        [XmlElement(ElementName = "NOCEDIT")]
        public string NocEdit { get; set; }
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
        public CustomerEnquiryType Type { get; set; }

        public bool Completed
        {
            get { return !string.IsNullOrWhiteSpace(CompletedDate); }
        }

        [XmlElement(ElementName = "StatusDate")]
        public string StatusDate { get; set; }

        [XmlElement(ElementName = "StatusTime")]
        public string StatusTime { get; set; }

        [XmlElement(ElementName = "StatusDescription")]
        public string StatusDescription { get; set; }
        [XmlElement(ElementName = "StatusCode")]
        public string StatusCode { get; set; }
    }

    [XmlRoot(ElementName = "RequestComplaints")]
    public class CustomerEnquiryList
    {
        [XmlElement(ElementName = "Iteam")]
        public List<CustomerEnquiry> Enquiries { get; set; }
    }

    [XmlRoot(ElementName = "GetComplaintsList")]
    public class CustomerEnquiryListResponse
    {
        [XmlElement(ElementName = "DateTimeStamp")]
        public string DateTimeStamp { get; set; }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "RequestComplaints")]
        public CustomerEnquiryList EnquiryList { get; set; }

    }
}
