using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.EV.Models.EVSmartCharger
{
    public class EV_Green_Charger_GeneralDetails
    {
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string FullName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "email validation message")]
        [Foundation.DataAnnotations.EmailAddress(ValidationMessageKey = "email validation message")]
        public string EmailAddress { get; set; }
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [Foundation.DataAnnotations.RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string PlateNumber { get; set; }
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string CountryCode { get; set; }
        public string VatNumber { get; set; }
    }
}