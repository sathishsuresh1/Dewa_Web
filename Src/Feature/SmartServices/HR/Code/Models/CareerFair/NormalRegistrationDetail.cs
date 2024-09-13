using DEWAXP.Foundation.DataAnnotations;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.HR.Models.CareerFair
{
    public class NormalRegistrationDetail
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a first name.")]
        public string ApplicantFirstName { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a last name.")]
        public string ApplicantLastName { get; set; }

        //[Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid email address")]
        public string ApplicantMiddleName { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a date Of birth.")]
        public string DateOfBirth { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid email address")]
        public string Emailaddress { get; set; }

        [EmiratesID(ValidationMessageKey = "Please enter a valid Emirates ID")]
        public string EmiratesID { get; set; }

        public string Gender { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a qualification")]
        public string Qualification { get; set; }

        public List<SelectListItem> QualificationList { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a specialisation")]
        public string Specialisation { get; set; }

        public List<SelectListItem> SpecialisationList { get; set; }

        //[Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase UploadedResume { get; set; }

        public string Other { get; set; }
        public string University { get; set; }

        public string YearsOfExperience { get; set; }
    }
}