using DEWAXP.Foundation.Content.ModelBinders;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.MoveOut;
using DEWAXP.Foundation.Integration.DewaSvc;
using Glass.Mapper.Sc.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Models.MoveOut
{
    [ModelBinder(typeof(MoveOutModelBinder))]
    public class DetailsView
    {
        public SharedAccount SelectedAccount { get; set; }

        public ibanListchild[] IbanValues { get; set; }

        [DataAnnotations.Required]
        public string DisconnectDate { get; set; }

        public bool norefund { get; set; }

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

        public bool TransferToIbanAccount { get; set; }

        public bool IsIbanAvailable { get; set; }

        public bool Attachmentrequired { get; set; }

        public bool TransferToChequeAccount { get; set; }

        [DataAnnotations.MaxLength(255, ValidationMessageKey = "Address field should be less than 255")]
        public string Comments { get; set; }

        [DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IbanRefundDocument { get; set; }

        public HttpPostedFileBase Attachment { get; set; }

        public string SelectedRefundOption { get; set; }

        public IEnumerable<SelectListItem> RefundOptions { get; set; }
    }

    public class LstMoveoutAccount
    {
        [DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [DataAnnotations.RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }

        public List<MoveoutDetailsv3> lstdetails { get; set; }
        public List<MoveOutTransferAccountsResponse> lsttranferaccount { get; set; }
        public bool SameDisconnectDate { get; set; }
        public bool SameDisconnectTime { get; set; }
        public string DisconnectionDateTimeNotes { get; set; }
        public List<SelectListItem> DisconnectionCurrentTimeOptions { get; set; }
        public List<SelectListItem> DisconnectionTimeOptions { get; set; }
        public List<string> HolidayList { get; set; }
        public DateTime ShiftStartTime { get; set; }
        public DateTime ShiftEndTime { get; set; }
        public string TimeInterval { get; set; }
        public string TimeFormat { get; set; }
        public bool SameTransferIban { get; set; }
        public bool SameTransferAcccount { get; set; }
        public bool SameTransferCheque { get; set; }
        public bool SameTransferWestern { get; set; }
        public bool SameNoRefund { get; set; }

        [DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase RefundDocument { get; set; }

        public byte[] Attachment { get; set; }
        public bool IsAttachmentMandatory { get; set; }
        public bool IsChequeRefundForAll { get; set; }
        public bool IsIbanRefundForAll { get; set; }
        public bool IsTransferRefundForAll { get; set; }
        public bool IsWesternUnionForAll { get; set; }
        public bool IsNoRefundForAll { get; set; }
        public List<SelectListItem> MobileList { get; set; }
        public List<SelectListItem> EmailList { get; set; }

        public string SelectedMobile { get; set; }
        public string SelectedEmail { get; set; }

    }

    //[ModelBinder(typeof(MoveOutModelBinder))]
    public class MoveoutDetailsv3
    {
        public bool refund { get; set; }

        [DataAnnotations.Required]
        public string DisconnectDate { get; set; }

        public DateTime? DisconnectDateTime { get; set; }
        public string DisconnectionTime { get; set; }
        public string contractaccountnumber { get; set; }
        public string contractaccountname { get; set; }
        public string customerfirstname { get; set; }
        public string customerlastname { get; set; }
        public string customeremailid { get; set; }
        public string businesspartnernumber { get; set; }
        public string businesspartnercategory { get; set; }
        public IEnumerable<SelectListItem> RefundOptions { get; set; }
        public string SelectedRefundOption { get; set; }

        [DataAnnotations.MaxLength(255, ValidationMessageKey = "Address field should be less than 255")]
        public string Comments { get; set; }

        [DataAnnotations.IbanNumber(ValidationMessageKey = "IBAN number is required.")]
        public string IbanAccountNumber { get; set; }

        [DataAnnotations.Compare("IbanAccountNumber", ValidationMessageKey = "IBAN number is required.")]
        public string ConfirmIbanAccountNumber { get; set; }

        public IEnumerable<SelectListItem> IbanList { get; set; }
        public string SelectedIban { get; set; }
        public Tuple<string, string> ValidBankCodes { get; set; }

        [DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IbanRefundDocument { get; set; }

        public string transferaccount { get; set; }
        public string IsWestenCountrylist { get; set; }
        public string CountrylistText { get; set; }
        public string IsWestenStatelist { get; set; }
        public string StatelistText { get; set; }
        public string IsWestenCitylist { get; set; }
        public string IsWestenCurrencylist { get; set; }
        public string Mobile { get; set; }
        public string MaskedMobile { get; set; }
        public string Email { get; set; }
        public string MaskedEmail { get; set; }

    }

    public class MoveOutDemolish
    {
        public string contractaccountnumber { get; set; }

        [DataAnnotations.Required]
        public string DisconnectDate { get; set; }

        public DateTime? DisconnectDateTime { get; set; }
        public string DisconnectionTime { get; set; }
        public string DisconnectionDateTimeNotes { get; set; }
        public List<SelectListItem> DisconnectionCurrentTimeOptions { get; set; }
        public List<SelectListItem> DisconnectionTimeOptions { get; set; }
        public List<string> HolidayList { get; set; }
        public string Emailaddress { get; set; }

        [DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [DataAnnotations.RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }

        public string plotnumber { get; set; }
        public string premisenumber { get; set; }

        [DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase RequestLetter { get; set; }

        [DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase SitePlan { get; set; }

        [DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase EmiratesID { get; set; }

        [DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase TradeLicense { get; set; }

        public bool isCustomer { get; set; }
    }

    public class MoveoutReviewV3
    {
        public string CustomerAccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string RefundThrough { get; set; }
        public string RefundFlag { get; set; }
        public string IBANNumber { get; set; }
        public string DisconnectionDate { get; set; }
        public string DisconnectionTime { get; set; }
        public string AccountNumber { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingState { get; set; }
        public string ReceivingCity { get; set; }
        public string ReceivingCurrency { get; set; }
        public string CardNumber { get; set; }
        public string Mobile { get; set; }    
        public string MaskedMobile { get; set; }
        public string Email { get; set; }
        public string MaskedEmail { get; set; }
    }

    public class MoveoutDisconnectionTime
    {
        [SitecoreField(FieldName = "TimeFormat")]
        public virtual string TimeFormat { get; set; }

        [SitecoreField(FieldName = "TimeInterval")]
        public virtual string TimeInterval { get; set; }

        [SitecoreField(FieldName = "Notes")]
        public virtual string Notes { get; set; }

        [SitecoreField(FieldName = "FromTime")]
        public virtual DateTime FromTime { get; set; }

        [SitecoreField(FieldName = "ToTime")]
        public virtual DateTime ToTime { get; set; }
    }

    public class MoveOutMobileEmailList
    {
        public List<SelectListItem> MobileItems { get; set; }
        public List<SelectListItem> EMailItems { get; set; }
        public string SelectedMobile { get; set; }
        public string SelectedEmail { get; set; }
        public string SelectedBusinessPartnerNumber { get; set; }
        public string OtpRequestId { get; set; }
    }
}