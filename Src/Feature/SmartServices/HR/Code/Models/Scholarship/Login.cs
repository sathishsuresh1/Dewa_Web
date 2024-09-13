using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace DEWAXP.Feature.HR.Models.Scholarship
{
    [Serializable]
    public class Login
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string UserName { get; set; }
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string UserPassword { get; set; }

        public bool IsValidationError { get; set; }

        public string WebServiceCredentials { get; set; }

        public bool InitialLogin
        {
            get
            {
                return initialLogin;
            }

            set
            {
                initialLogin = value;
            }
        }

        private bool initialLogin = false;
    }
}