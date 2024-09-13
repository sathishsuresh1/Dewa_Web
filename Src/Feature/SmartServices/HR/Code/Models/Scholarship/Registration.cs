
using DEWAXP.Foundation.DataAnnotations;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.HR.Models.Scholarship
{
    public class Registration
    {
        //internal const string FIELD_REQUIRED_MESSAGE_KEY = "Scholarship Field Required";
        internal const string FIELD_LENGTH_25_MESSAGE_KEY = "Max length is 25 chars";

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string Program { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        [MaxLength(25, ValidationMessageKey = FIELD_LENGTH_25_MESSAGE_KEY)]
        public string FirstName { get; set; }


        [MaxLength(25, ValidationMessageKey = FIELD_LENGTH_25_MESSAGE_KEY)]
        public string MiddleName { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]

        [MaxLength(25, ValidationMessageKey = FIELD_LENGTH_25_MESSAGE_KEY)]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        [RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Scholarship Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }
        /*[MaxLength(15, ValidationMessageKey = "Max length is 15 chars")]
        [Required()]
        public string PassportNumber { get; set; }*/

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]

        [MaxLength(25, ValidationMessageKey = FIELD_LENGTH_25_MESSAGE_KEY)]
        [MinLength(6, ValidationMessageKey = "Min length is 6 chars")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.USERNAME_FORMAT_VALIDATION)]
        public string UserName { get; set; }

        [EmiratesID(ValidationMessageKey = "Scholarship Please enter a valid Emirates ID")]
        public string EmiratesID { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]

        //[MaxLength(40, ValidationMessageKey = "Max length is 40 chars")]
        //[MinLength(8, ValidationMessageKey = "Min length is 8 chars")]
        [RegularExpression(@"^(?=(.*\d){1})(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\d]).{8,40}$",
            AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.PASSWORD_FORMAT_VALIDATION)]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]

        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]

        //[MaxLength(40, ValidationMessageKey = "Max length is 40 chars")]
        //[MinLength(8, ValidationMessageKey = "Min length is 8 chars")]
        [RegularExpression(@"^(?=(.*\d){1})(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\d]).{8,40}$",
            AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.PASSWORD_FORMAT_VALIDATION)]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password1 { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        [EmailAddress(AllowEmptyStrings = false, ValidationMessageKey = "Scholarship Please Enter Valid Email Address")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        [EmailAddress(AllowEmptyStrings = false, ValidationMessageKey = "Scholarship Please enter a valid Email Address")]
        public string Email1 { get; set; }



        [MaxLength(1, ValidationMessageKey = "Max length is 1 chars")]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string HaveScored80Plus { get; set; }


        /*public bool TermsAccepted { get; set; }

        [SitecoreField("Terms and Conditions Title")]        
        public virtual string TermsAndConditionsTitle { get; set; }

        [SitecoreField("Terms and Conditions Details",FieldType = Glass.Mapper.Sc.Configuration.SitecoreFieldType.RichText)]
        
        public virtual System.Text.StringBuilder TermsAndConditions { get; set; }*/

        public List<SelectListItem> Programs { get; set; }
    }
}