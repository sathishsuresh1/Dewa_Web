using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Models.MoveOut.v3
{
    public class MoveOutAccount
    {
        public SharedAccount SelectedAccount { get; set; }

        public string AccountNumber { get; set; }

        [DataAnnotations.Required]
        public string DisconnectDate { get; set; }

        public DateTime? DisconnectDateAsDateTime
        {
            get
            {
                DateTime dateTime;
                if (DateTime.TryParse(DisconnectDate, out dateTime))
                {
                    return dateTime;
                }
                return null;
            }
        }

        [DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [DataAnnotations.RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }

        [DataAnnotations.IbanNumber(ValidationMessageKey = "IBAN number is required.")]
        public string IbanAccountNumber { get; set; }

        [DataAnnotations.Compare("IbanAccountNumber", ValidationMessageKey = "IBAN number is required.")]
        public string ConfirmIbanAccountNumber { get; set; }

        [DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string SelectedBusinessPartnerNumber { get; set; }

        public bool TransferToIbanAccount { get; set; }

        public decimal OutStandingAmount { get; set; }
        public string SelectedPurpose { get; set; }
        public IEnumerable<SelectListItem> Purposes { get; set; }

        public bool IsChequeAllowed { get; set; }

        public bool IsIbanAllowed { get; set; }

        public bool IsCollectPayment { get; set; }

        public bool Clearance { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ClearanceAmount { get; set; }

        public string BusinessPartnerType { get; set; }

        public decimal ClearanceCharge { get; set; }
        public decimal ClearanceTax { get; set; }
        public decimal ClearanceTotalAmount { get; set; }
        public string TaxRate { get; set; }
    }
}