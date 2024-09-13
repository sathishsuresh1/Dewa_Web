using DEWAXP.Foundation.Integration.DewaSvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SitecoreX = Sitecore.Context;
using DEWAXP.Foundation.Content.Models.Payment;

namespace DEWAXP.Foundation.Content.Models.MoveOut.v3
{
    public class MoveOutAnonymous : LstMoveoutAccount
    {
        public string AccountNumber { get; set; }
        public string PremiseNumber { get; set; }

        public string SelectedOptions { get; set; }

        public string EmailAddress { get; set; }

        public string MaskedMobileNumber { get; set; }
        public string MaskedEmailAddress { get; set; }

        public string OTPNumber { get; set; }

        public string OTPflag { get; set; }

        public string Message { get; set; }
        public string AdditionalInformation { get; set; }
        public string Ibannumber { get; set; }
        public string MaskedDisplayValue { get; set; }

        public string PayAmount { get; set; }

        public string BusinessPartnerName { get; set; }
        public string BusinessPartnerCatg { get; set; }
        public string BusinessPartnerNumber { get; set; }

        public string okiban { get; set; }
        public string okcheque { get; set; }
        public string oknorefund { get; set; }

        public IEnumerable<SelectListItem> RefundOptions { get; set; }
        public bool refund { get; set; }

        [DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase RequestLetter { get; set; }

        public string SelectedRefundOption { get; set; }
        public bool attachmentflag { get; set; }

        public string DisconnectionDate { get; set; }
        public string DisconnectionTime { get; set; }
        public string DisconnectionDateReview { get; set; }

        public DateTime? DiconnectionDateTime
        {
            get
            {
                CultureInfo culture;
                DateTimeStyles styles;
                culture = SitecoreX.Culture;
                if (!string.IsNullOrWhiteSpace(DisconnectionDate))
                {
                    DisconnectionDate = DisconnectionDate.Replace("يناير", "Jan").Replace("فبراير", "Feb").Replace("مارس", "Mar").Replace("أبريل", "Apr").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "Aug").Replace("سبتمبر", "Sept").Replace("أكتوبر", "Oct").Replace("نوفمبر", "Nov").Replace("ديسمبر", "Dec");
                    DateTime dateTime;
                    styles = DateTimeStyles.None;
                    if (DateTime.TryParse(DisconnectionDate, culture, styles, out dateTime))
                    {
                        return dateTime;
                    }
                }
                return null;
            }
            set { }
        }

        public string Refundthrough { get; set; }

        public string NotificationNumber { get; set; }
        public string ErrorMessage { get; set; }

        [DataAnnotations.IbanNumber(ValidationMessageKey = "IBAN number is required.")]
        public string IbanAccountNumber { get; set; }

        [DataAnnotations.Compare("IbanAccountNumber", ValidationMessageKey = "IBAN number is required.")]
        public string ConfirmIbanAccountNumber { get; set; }

        [DataAnnotations.Compare("IbanAccountNumber", ValidationMessageKey = "IBAN number is required.")]
        public bool Ibanavailable { get; set; }

        public bool IsSuccess { get; set; }
        public IEnumerable<SelectListItem> IbanList { get; set; }
        public string SelectedIban { get; set; }
        public Tuple<string, string> ValidBankCodes { get; set; }
        public byte[] RefundDocumentbyte { get; set; }
        public string RefundFilename { get; set; }
        public byte[] RequestDocumentbyte { get; set; }
        public string RequestFilename { get; set; }
        public outstandingRecovery[] recoverylist { get; set; }
        public string TotalPendingBalance { get; set; }
        public string OutstandingBalance { get; set; }
        public string PaymentAmountList { get; set; }
        public string PaymentAccountList { get; set; }
        public string PaymentBP_List { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }
}