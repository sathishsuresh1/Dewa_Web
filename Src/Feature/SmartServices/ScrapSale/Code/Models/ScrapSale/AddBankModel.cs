using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Web;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class AddBankModel
    {
        public string BankName { get; set; }
        public string Country { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string Currency { get; set; }
        public string SwiftCode { get; set; }
        public string SwiftCodeCorespondent { get; set; }
        public string IFSCCode { get; set; }
        public string IBAN { get; set; }
        public string IsCorrespondent { get; set; }
        public string BankNameCorrespondent { get; set; }
        public string CountryCorrespondent { get; set; }
        public string CityCorrespondent { get; set; }
        public string Address { get; set; }
        public string AddressBank { get; set; }
        public string City { get; set; }
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase Attachment { get; set; }
    }
}