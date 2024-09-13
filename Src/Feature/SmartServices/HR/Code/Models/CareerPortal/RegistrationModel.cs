using DEWAXP.Foundation.DataAnnotations;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DEWAXP.Feature.HR.Models.CareerPortal
{
    [Serializable]
    public class RegistrationModel
    {
        //[Required(AllowEmptyStrings = false, ValidationMessageKey = "registration type validation message")]
        //public string SelectedRegistrationType { get; set; }
        //public IEnumerable<System.Web.Mvc.SelectListItem> RegistrationTypes { get; set; }
     
        public IEnumerable<System.Web.Mvc.SelectListItem> ExperienceTypes { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> NatureOfWorkTypes { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> NationalityList { get; set; }
        //[Required(AllowEmptyStrings = false, ValidationMessageKey = "experiencetype validation message")]
        public string SelectedExperienceType { get; set; }
        //[Required(AllowEmptyStrings = false, ValidationMessageKey = "natureofworktype validation message")]
        public string SelectedNatureOfWorkType { get; set; }
        public string POD { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        [MinLength(6, ValidationMessageKey = "username length")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string FirstName { get; set; }

        //[Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        //public string MiddleName { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string LastName { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,}", ValidationMessageKey = "password validation message")]
        public string Password { get; set; }
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Compare("Password", ValidationMessageKey = "password and confirmation mismatch error")]
        public string ConfirmPassword { get; set; }
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string EmailAddress { get; set; }
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string repeatEmailAddress { get; set; }
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Nationality { get; set; }
        //[Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        //public string EmiratesID { get; set; }
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string PassportNumber { get; set; }
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public bool TermsCondition { get; set; }

    }
    public class CandidateEmailVerification
    {
        public string param { get; set; }
        public string applicationvisibility { get; set; }

        public string errorcode { get; set; }

        public string errormessage { get; set; }

        public string newverificationlinkenable { get; set; }

        public string success { get; set; }

        public string verificationstatus { get; set; }
    }
}