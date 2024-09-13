using DEWAXP.Foundation.DataAnnotations;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DEWAXP.Feature.HR.Models.Scholarship
{
    public class ContactDetail
    {
        //private const string FIELD_REQUIRED_MESSAGE = "Scholarship Field Required";
        [EmailAddress]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string EmailAddress { get; set; }

        [EmailAddress]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string FatherEamilAddress { get; set; }

        [EmailAddress]
        public string MotherEmailAddress { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string CandidateMobileNo { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string FatherMobileNo { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string MotherMobileNo { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string Emirate { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string AreaOfEmirate { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string StreetAddress { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string POBox { get; set; }

        public string PostalCode { get; set; }

        public List<SelectListItem> Emirates { get; set; }

        public List<SelectListItem> Areas
        {
            get
            {
                return _areas;
            }

            set
            {
                _areas = value;
            }
        }

        private List<SelectListItem> _areas = new List<SelectListItem>();
    }
}