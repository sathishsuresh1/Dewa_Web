using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Integration.DewaSvc;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Models.UpdateIBAN
{
    [Serializable]
    public class UpdateIBANModel
    {
        [DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [DataAnnotations.RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }

        [DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string AccountNumberSelected { get; set; }

        [DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string AccountSelected { get; set; }

        [DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string SelectedBusinessPartnerNumber { get; set; }

        public string SelectedPremiseNumber { get; set; }

        public ibanListchild[] IbanValues { get; set; }
        public IEnumerable<SelectListItem> IBANSList { get; set; }

        public string SelectedIBAN { get; set; }
        public string SelectedCountry { get; set; }
        public string SelectedState { get; set; }
        public string SelectedCountrytext { get; set; }
        public string SelectedStatetext { get; set; }
        public string SelectedCity { get; set; }
        public string SelectedCurrency { get; set; }
        public IEnumerable<DEWAXP.Foundation.Integration.DewaSvc.ibanListchild> IBANLISTENUM { get; set; }

        [DataAnnotations.Compare("ConfirmIBAN", ValidationMessageKey = "confirm IBAN should be same as IBAN")]
        public string ConfirmIBAN { get; set; }

        public bool IsUpdateChequeAllowed { get; set; }

        public bool IsUpdateIbanAllowed { get; set; }

        public bool IsUpdateTransferAllowed { get; set; }

        public bool IsRequestChequeAllowed { get; set; }

        public bool IsRequestIbanAllowed { get; set; }

        public bool IsRequestTransferAllowed { get; set; }

        public bool IsWesternUnionAllowed { get; set; }

        [DataAnnotations.IbanNumber(ValidationMessageKey = "IBAN number is required.")]
        public string IbanAccountNumber { get; set; }

        [DataAnnotations.Compare("IbanAccountNumber", ValidationMessageKey = "IBAN number is required.")]
        public string ConfirmIbanAccountNumber { get; set; }

        [DataAnnotations.Compare("IbanAccountNumber", ValidationMessageKey = "IBAN number is required.")]
        public string ConfirmIbanAccountNumber2 { get; set; }

        [DataAnnotations.MaxLength(255, ValidationMessageKey = "Address field should be less than 255")]
        public string Comments { get; set; }

        [DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IbanRefundDocument { get; set; }

        public bool Ibanavailable { get; set; }
        public string IbanNumber { get; set; }

        public bool Successful { get; set; }

        public string Notificationnumber { get; set; }

        public string Successfulmessage { get; set; }

        public string Errormessage { get; set; }

        public string BusinessPartnerType { get; set; }

        public string CustomerName { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }
        public string CustomerAccountnumber { get; set; }
        public string CreditBalance { get; set; }
        public string MaskedMobileNumber { get; set; }
        public string Emailid { get; set; }
        public string MaskedEmailid { get; set; }
        public string RefundThrough { get; set; }

        public string transferaccount { get; set; }
        public List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.MoveOut.MoveOutTransferAccountsResponse> lsttranferaccount { get; set; }
        public List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.MoveOut.MoveOutDivisionWiseCAResponse> divisionlist { get; set; }
        public Tuple<string, string> ValidBankCodes { get; set; }

        public byte[] Attachment { get; set; }

        public string TotalPendingBalance { get; set; }
        public string OutstandingBalance { get; set; }
        public string PaymentAmountList { get; set; }
        public string PaymentAccountList { get; set; }
        public string PaymentBP_List { get; set; }
        public string SumbitType { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
        public string nomoveoutpayflag { get; set; }
        public string OtpRequestId { get; set; }
    }
}