using DEWAXP.Foundation.Integration.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
    [Serializable]
    public class UpdateContactInfoModel
    {
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string AccountNumberSelected { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string SelectedAccountName { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string SelectedBusinessPartnerNumber { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string SelectedPremiseNumber { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string SelectedCategory { get; set; }

        public string NickName { get; set; }

        [Foundation.DataAnnotations.RegularExpression(@"^(?:0)?(?:2|3|4|6|7|9)\d{7}$", ValidationMessageKey = "Please enter a valid UAE telephone number")]
        public string TelephoneNumber { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [Foundation.DataAnnotations.RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }

        [Foundation.DataAnnotations.RegularExpression(@"^(?:0)?(?:2|3|4|6|7|9)\d{7}$", ValidationMessageKey = "Please enter a valid UAE telephone number")]
        public string FaxNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "email validation message")]
        [Foundation.DataAnnotations.EmailAddress(ValidationMessageKey = "email validation message")]
        public string EmailAddress { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string PoBox { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string SelectedEmirateKey { get; set; }

        public string SelectedEmirateValue { get; set; }

        public Dictionary<string, string> Emirates { get; set; }

        public SupportedLanguage PreferredLanguage { get; set; }

        public HttpPostedFileBase ProfilePictureUploader { get; set; }

        public bool IsAccountActive { get; set; }

        public UpdateContactInfoSuccessModel UpdateContactInfoSuccessModel { get; set; }

        public string TabId { get; set; }
    }

    [Serializable]
    public class UpdateContactInfoSuccessModel
    {
        public string SelectedAccountName { get; set; }

        public string AccountNumberSelected { get; set; }

        public string MultipleAccountNumberSelected { get; set; }

        public string SelectedCategory { get; set; }

        public string SelectedPremiseNumber { get; set; }

        public string SelectedBusinessPartnerNumber { get; set; }

        public string NickName { get; set; }

        public string TelephoneNumber { get; set; }

        public string MobileNumber { get; set; }

        public string FaxNumber { get; set; }

        public string EmailAddress { get; set; }

        public string PoBox { get; set; }

        public string SelectedEmirateValue { get; set; }

        public SupportedLanguage PreferredLanguage { get; set; }

        public bool IsAccountActive { get; set; }
    }
}