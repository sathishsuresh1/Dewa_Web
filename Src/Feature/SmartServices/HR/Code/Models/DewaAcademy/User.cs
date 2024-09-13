using DEWAXP.Foundation.DataAnnotations;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.HR.Models.DewaAcademy
{
    public class User
    {
        //Personal details
        [Required]
        public string ApplicantFirstName { get; set; }

        //[Required]
        //public string FullNamear { get; set; }

        [Required]
        public string ApplicantLastName { get; set; }

        [Required]
        public string ApplicantMiddleName { get; set; }

        public IEnumerable<SelectListItem> NationalityList { get; set; }

        [Required]
        public string Nationality { get; set; }

        [Required]
        public string PassportNumber { get; set; }

        [Required]
        public string EmiratesIdNumber { get; set; }

        [Required]
        public string DateOfBirth { get; set; }

        public string Birthplace { get; set; }

        //uploads
        [Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase StudentPhotoUpload { get; set; }

        [Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase PassportUpload { get; set; }

        [Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase EmiratesIdUpload { get; set; }

        [Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase FamilyBookUpload { get; set; }

        [Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase BirthcertificateUpload { get; set; }

        [Required]
        public string Password { get; set; }

        // Family Details
        [Required]
        public string familybookissueddate { get; set; }

        [Required]
        public string familybooknumber { get; set; }

        [Required]
        public string familynumber { get; set; }

        public string fatherfirstname { get; set; }

        public string fatherlastname { get; set; }

        public string fathermiddlename { get; set; }

        public string fathermobilenumber { get; set; }

        public string motherfirstname { get; set; }

        public string motherlastname { get; set; }

        public string mothermiddlename { get; set; }

        public string mothermobilenumber { get; set; }

        [Required]
        public string postalcode { get; set; }

        [Required]
        public string EmiratesIDexpiredate { get; set; }

        [Required]
        public string EmiratesIDissueddate { get; set; }

        [Required]
        public string Idbaranumber { get; set; }

        [Required]
        public string Passportexpirydate { get; set; }

        [Required]
        public string Passportissueddate { get; set; }

        [Required]
        public string Placeofidentificationissued { get; set; }

        public string Secondarymobilenumber { get; set; }
        public string Telephonenumber { get; set; }

        [Required]
        public string Unifiednumber { get; set; }

        // Contact Details
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid email address")]
        public string EmailAddress { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }

        [Required]
        public string AddressRoad { get; set; }

        [Required]
        public string Landmark { get; set; }

        [Required]
        public string City { get; set; }

        public IEnumerable<SelectListItem> EmirateList { get; set; }
        public IEnumerable<SelectListItem> CityList { get; set; }

        [Required]
        public int NumberOfSiblings { get; set; }

        //Academic Information
        [Required]
        public string SchoolName { get; set; }

        [Required]
        public string FinalPercentageGrade { get; set; }

        [Required]
        public string AcademicYear { get; set; }

        public IEnumerable<SelectListItem> AcademicYearList { get; set; }

        [Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase AcademicCertificateUpload { get; set; }

        public HttpPostedFileBase CourcesTrainingCertificateUpload { get; set; }

        public HttpPostedFileBase CertificateIssuedAbroadUpload { get; set; }

        //Status
        public string Status { get; set; }

        public string SuccessMessage { get; set; }
    }
}