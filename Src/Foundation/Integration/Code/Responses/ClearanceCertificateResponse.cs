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
    [XmlRoot(ElementName = "ClearanceCertificate")]
    public class ClearanceCertificateDetails
    {
        private string _contractAccountNumber;

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Amount")]
        public decimal Amount { get; set; }

        [XmlElement(ElementName = "FirstName")]
        public string FirstName { get; set; }

        [XmlElement(ElementName = "MiddleName")]
        public string MiddleName { get; set; }

        [XmlElement(ElementName = "LastName")]
        public string LastName { get; set; }

        [XmlElement(ElementName = "Email")]
        public string Email { get; set; }

        [XmlElement(ElementName = "Mobile")]
        public string Mobile { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "TradeLicenseNum")]
        public string TradeLicenseNumber { get; set; }

        [XmlElement(ElementName = "PoBox")]
        public string PoBox { get; set; }

        [XmlElement(ElementName = "Region")]
        public string Region { get; set; }

        [XmlElement(ElementName = "Data")]
        public string Data { get; set; }

        [XmlElement(ElementName = "ContractAccount")]
        public string ContractAccountNumber
        {
            get { return DewaResponseFormatter.Trimmer(_contractAccountNumber); }
            set { _contractAccountNumber = value ?? string.Empty; }
        }

        [XmlElement(ElementName = "ClearanceCharge")]
        public decimal ClearanceCharge { get; set; }

        [XmlElement(ElementName = "ClearanceTax")]
        public decimal ClearanceTax { get; set; }

        [XmlElement(ElementName = "ClearanceTotal")]
        public decimal ClearanceTotal { get; set; }

        [XmlElement(ElementName = "TaxRate")]
        public string TaxRate { get; set; }
    }
}
