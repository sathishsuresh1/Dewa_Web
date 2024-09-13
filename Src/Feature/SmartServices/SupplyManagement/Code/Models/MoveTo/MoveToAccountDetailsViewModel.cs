using DEWAXP.Foundation.Content.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Models.MoveTo
{
    public class MoveToAccountDetailsViewModel
    {
        public string BusinessPartner { get; set; }

        public string MovoutAccountnumber { get; set; }

        public DateTime? MoveoutAccountDate { get; set; }

        public bool loggedinuser { get; set; }

        public bool owner { get; set; }

        public bool MoveoutPayment { get; set; }
        public string UserId { get; set; }

        public bool Moveinindicator { get; set; }

        public string SessionToken { get; set; }

        public string PremiseAccount { get; set; }

        public bool personbusinesspartner { get; set; }

        public bool organisationbusinesspartner { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string CustomerCategory { get; set; }

        public string AccountType { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string IdType { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string CustomerType { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string PremiseNo { get; set; }

        public string SelectedIDType { get; set; }
        public string IdNumber { get; set; }

        public string SecurityDeposit { get; set; }

        public string ReconnectionAddressRegistrationFee { get; set; }

        public string OutstandingBalance { get; set; }

        public DateTime? MoveinStartDate { get; set; }

        public DateTime? DocumentExpiryDate { get; set; }

        public string Reference { get; set; }
        public string KnowledgeFee { get; set; }
        public string InnovationFee { get; set; }

        public string ContractAccont { get; set; }

        public int NumberOfRooms { get; set; }

        public string PassportorEmiratesLabel { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase PassportDocument { get; set; }

        public string IdentityDocumentLabel1 { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase OwnerDocument { get; set; }

        public string OwnerDocumentLabel { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase PurchaseAgreement { get; set; }

        public string PurchaseAgreementLabel { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase TradeLicense { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase VatDocument { get; set; }

        public string VatNumber { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string bankkey { get;set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }
}
