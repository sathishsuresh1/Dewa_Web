using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        //[MinLength(8, ValidationMessageKey = "login password validation message")]
        public string Password { get; set; }

        public string LoginType { get; set; }

        public string AfterLoginURL { get; set; }

        public List<SelectListItem> MiscellaneousServices { get; set; }

        public string MiscellaneousServicesKey { get; set; }
    }
}