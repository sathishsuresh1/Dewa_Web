using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DEWAXP.Foundation.Content.Models.Payment;

namespace DEWAXP.Feature.EV.Models.EVCharger
{
    public class EVAccountSetup : EVPreRegistrationModel
    {
        public string mulkiyanum { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Foundation.DataAnnotations.Compare("Password", ValidationMessageKey = "Password mismatch error")]
        public string ConfirmPassword { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public string Emirate { get; set; }
        public string POBox { get; set; }
        public string userCreateFlag { get; set; }
        public string IdType { get; set; }
        public string IdNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string DateOfBirth { get; set; }
        public string processtFlag { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase AttachedDocument { get; set; }

        public byte[] AttachmentFileBinary { get; set; }

        public string AttachmentFileType { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase AttachedDocument2 { get; set; }

        public byte[] AttachmentFileBinary2 { get; set; }

        public string AttachmentFileType2 { get; set; }

        //public EVPreRegistrationModel PreRegistrationModel { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string bankkey { get; set; }

        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }

        public void FromPreRegistrationModel(EVPreRegistrationModel model)
        {
            this.AccountType = model.AccountType;
            this.CarPlateNumber = model.CarPlateNumber;
            this.CarRegisteredIn = model.CarRegisteredIn;
            this.EmirateOrCountry = model.EmirateOrCountry;
            this.TrafficCodeNumber = model.TrafficCodeNumber;
        }

        public string IssuingAuthority { get; set; }
        public List<SelectListItem> IssuingAuthorities { get; set; }

        public string CarPlateNumberList { get; set; }
        public string CarPlateCodeList { get; set; }
        public string CarCategoryList { get; set; }
        public List<SelectListItem> SupportingDocTypes { get; internal set; }
        public decimal PayingAmount { get; internal set; }

        public bool IsUserExited { get; internal set; }
        public ActionResult RedirectUrl { get; set; }

        public string SubmittedCarPlateNumbers { get; set; }
        public string SubmittedCarPlateCodes { get; set; }
        public string SubmittedCarCategories { get; set; }
        public string SubmittedTradeLicenceAuthorityName { get; set; }
        public string SubmittedTradeLicenceAuthorityCode { get; set; }

        public string CarDetailJs { get; set; }
        public bool IsIdReadonly { get; set; }
        public bool AgreedToPayment { get; set; }
        public bool Isbackfromsd { get; set; }
    }

    public class EVPreRegistrationModel
    {
        public string CarRegisteredIn { get; set; }
        public string EmirateOrCountry { get; set; }
        public string TrafficCodeNumber { get; set; }
        public string CarPlateNumber { get; set; }
        public string AccountType { get; set; }

        public Stage RegistrationStage { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Emirates { get; set; }

        public string GetCarRegisteredRegion()
        {

            return this.CarRegisteredIn == "1" ? this.EmirateOrCountry : "";
        }
        public string GetCarRegisteredCountry()
        {
            return this.CarRegisteredIn == "1" ? "AE" : this.EmirateOrCountry;
        }

        public string mulkiya { get; set; }

        public string CategoryCode { get; set; }
        public string PlateCode { get; set; }

        public List<EvCardPaymentDetail> EvCardPaymentDetail { get; set; }
        public string bpNumber { get; set; }
    }
    public enum Stage
    {
        NOT_DEFINED = 0, /*unknown*/
        STAGE_ONE = 1, /*Car Detail screen*/
        STAGE_TWO = 2, /*Customer Detail Screen*/
        STAGE_THREE = 3,/*Customer Creation Screen*/
        STAGE_FOUR = 4, /*Request and Payment Review Screen*/
        STAGE_FIVE = 5 /*Request and Payment Confirmation Screen*/
    }
}