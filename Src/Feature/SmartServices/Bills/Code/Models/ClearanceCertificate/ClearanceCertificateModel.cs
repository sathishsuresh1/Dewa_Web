using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.ClearanceCertificate
{
    [Serializable]
    public class AuthenticatedClearanceCertificateModel : ClearanceCertificateModel
    {
        public string NotificationNumber { get; set; }
        public string BusinessPartnerNumber { get; set; }

        public decimal OutstandingBill { get; set; }

        public string TradeLicenseNumber { get; set; }
        public string Amounts { get; set; }

        public decimal TotalPayable
        {
            get { return CertificateCost + VATamount + OutstandingBill; }
        }

        public PaymentMethod paymentMethod { get; set; }

        public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }

    public abstract class ClearanceCertificateModel
    {
        [Required]
        public string ContractAccountNumber { get; set; }

        public string ReceivedContractAccountNumber { get; set; }

        //[Required]
        public string FirstName { get; set; }

        //[Required(AllowEmptyStrings = true)]
        public string LastName { get; set; }

        public string Organization { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        [Required]
        public string Purpose { get; set; }

        [Required]
        public string PoBox { get; set; }

        [Required]
        public decimal CertificateCost { get; set; }

        public string Remarks { get; set; }

        public string City { get; set; }

        public string Branch { get; set; }

        public IEnumerable<SelectListItem> Purposes { get; set; }
        public List<DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster> NonDEWAPurposes { get; set; }

        public DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster CA_Setting { get; set; }

        public decimal TotalClearanceCharges { get; set; }

        public decimal VATamount { get; set; }

        public string TaxRate { get; set; }
        public decimal _OutstandingBill { get; set; }

        public string CourtReference { get; set; }

        public string MoveOutDate { get; set; }

        public double Amount { get; set; }
        public string Languague { get; set; }
    }

    
}