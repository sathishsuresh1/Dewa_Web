using DEWAXP.Foundation.DataAnnotations;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.HR.Models.Scholarship
{
    public class PersonalDetail
    {
        #region
        internal const string FIELD_REQUIRED_MESSAGE_KEY = "Scholarship Field Required";
        internal const string FIELD_LENGTH_25_MESSAGE_KEY = "Max length is 25 chars";

        private const string ATTACHMENT_MAX_SIZE_MESSAGE = "The file may not be bigger than 2MB";
        public const string CANDIDATE_PHOTO_FILE_NAME = "Photo";
        public const string CANDIDATE_PASSPORT_FILE_NAME = "Passport";
        public const string CANDIDATE_EMIRATES_ID_FILE_NAME = "EmiratesID";
        public const string CANDIDATE_FAMILYBOOK_FILE_NAME = "FamilyBook";

        #endregion

        [MaxLength(25, ValidationMessageKey = FIELD_LENGTH_25_MESSAGE_KEY)]
        public string FirstName { get; set; }

        [MaxLength(25, ValidationMessageKey = FIELD_LENGTH_25_MESSAGE_KEY)]
        public string MiddleName { get; set; }

        [Required]
        [MaxLength(25, ValidationMessageKey = FIELD_LENGTH_25_MESSAGE_KEY)]
        public string LastName { get; set; }

        [MaxLength(40, ValidationMessageKey = "Max length is 40 chars")]
        [MinLength(6, ValidationMessageKey = "Min length is 6 chars")]
        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string FullNameInArabic { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string Nationality { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string DateOfBirth { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string Gender { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string PlaceOfBirth { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string PassportNumber { get; set; }

        public string PassportIssueDate { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string PassportExpiryDate { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string PassportPlaceOfIssue { get; set; }

        public string EmiratesID { get; set; }
        public string EIDIssueDate { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string EIDExpiryDate { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string FatherFirstName { get; set; }

        public string FatherMiddleName { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string FatherLastName { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string MotherFirstName { get; set; }

        public string MotherMiddleName { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string MotherLastName { get; set; }

        //[Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string FamilyBookNumber { get; set; }

        [Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        public string SourceOfApplication { get; set; }

        //public Dictionary<string, string> Attachements { get; set; }

        //[Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = ATTACHMENT_MAX_SIZE_MESSAGE)]
        public HttpPostedFileBase Photo { get; set; }

        //[Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = ATTACHMENT_MAX_SIZE_MESSAGE)]
        public HttpPostedFileBase EmirateIDUpload { get; set; }

        //[Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = ATTACHMENT_MAX_SIZE_MESSAGE)]
        public HttpPostedFileBase PassportUpload { get; set; }

        //[Required(ValidationMessageKey = Constants.DictionaryKeys.FIELD_REQUIRED)]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = ATTACHMENT_MAX_SIZE_MESSAGE)]
        public HttpPostedFileBase FamilyBookUpload { get; set; }

        public List<SelectListItem> Nationalities { get; set; }
        public List<SelectListItem> ApplicationSources { get; set; }

        public Attachment ExistingPhoto { get; set; }
        public Attachment ExistingPassport { get; set; }
        public Attachment ExistingEmiratesID { get; set; }
        public Attachment ExistingFamilyBook { get; set; }
    }

    public class Attachment
    {
        public string FileName { get; set; }
        public string DownloadUrl { get; set; }
    }
}