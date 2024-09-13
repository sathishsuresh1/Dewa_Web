using DEWAXP.Foundation.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.HR.Models.Scholarship
{
    public class AcademicDetail
    {
        private const string ATTACHMENT_VALIDATION_MESSAGE_KEY = "The file may not be bigger than 2MB";

        public const string ACADEMIC_CERTIFICATES_FILE_NAME = "Academic_Certificates";
        public const string ACADEMIC_GRADE12_FINAL_CERTIFICATES_FILE_NAME = "Grade12_Final_Certificate";
        public const string ACADEMIC_CERTIFICATE_OF_GOOD_CONDUCT_FILE_NAME = "Certificate_of_Good_Conduct";
        public const string ACADEMIC_POLICE_CLEARANCE_FILE_NAME = "Police_Clearance";
        public const string ACADEMIC_POD_CARD_FILE_NAME = "POD_Card";
        public const string ACADEMIC_UNIVERSITY_TRANSCRIPT_FILE_NAME = "UniversityTranscript";

        public List<Eduction> Eduction { get; set; }

        public string EducationJson { get; set; }
        public string Program { get; set; }
        public string University { get; set; }
        public string OtherUniversity { get; set; }
        public string Major { get; set; }
        public string OtherMajor { get; set; }

        //[Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = ATTACHMENT_VALIDATION_MESSAGE_KEY)]
        public HttpPostedFileBase AcademicCertificates { get; set; }

        //[Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = ATTACHMENT_VALIDATION_MESSAGE_KEY)]
        public HttpPostedFileBase Grade12FinalCertificate { get; set; }

        //[Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = ATTACHMENT_VALIDATION_MESSAGE_KEY)]
        public HttpPostedFileBase CertificateOfGoodConduct { get; set; }

        //[Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = ATTACHMENT_VALIDATION_MESSAGE_KEY)]
        public HttpPostedFileBase PoliceCertificate { get; set; }

        //[Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = ATTACHMENT_VALIDATION_MESSAGE_KEY)]
        public HttpPostedFileBase PeopleOfDeterminationCard { get; set; }

        //[Required]
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = ATTACHMENT_VALIDATION_MESSAGE_KEY)]
        public HttpPostedFileBase UniversityTranscript { get; set; }

        public Attachment ExistingAcademicCertificate { get; set; }
        public Attachment ExistingGrade12FinalCertificate { get; set; }
        public Attachment ExistingCertificateOfGoodConduct { get; set; }
        public Attachment ExistingPoliceCertificate { get; set; }
        public Attachment ExistingPODCard { get; set; }
        public Attachment ExistingUniversityTranscript { get; set; }

        public List<SelectListItem> Programs { get; set; }
        public List<SelectListItem> Grades { get; set; }

        private List<SelectListItem> _universities = new List<SelectListItem>();
        private List<SelectListItem> _majors = new List<SelectListItem>();
        private List<SelectListItem> _majors1 = new List<SelectListItem>();
        private List<SelectListItem> _levels = new List<SelectListItem>();
        private List<SelectListItem> _countries = new List<SelectListItem>();
        private List<SelectListItem> _regions = new List<SelectListItem>();

        public List<SelectListItem> Universities
        {
            get { return this._universities; }
            set { this._universities = value; }
        }

        public List<SelectListItem> Majors
        {
            get
            {
                return _majors;
            }

            set
            {
                _majors = value;
            }
        }

        public List<SelectListItem> Majors1
        {
            get
            {
                return _majors1;
            }

            set
            {
                _majors1 = value;
            }
        }

        public List<SelectListItem> Levels
        {
            get
            {
                return _levels;
            }

            set
            {
                _levels = value;
            }
        }

        public List<SelectListItem> Countries
        {
            get
            {
                return _countries;
            }

            set
            {
                _countries = value;
            }
        }

        public List<SelectListItem> Regions
        {
            get
            {
                return _regions;
            }

            set
            {
                _regions = value;
            }
        }

        public string GetGradeLabel(string levelID)
        {
            return this.Grades.Where(x => x.Value.Trim().Equals(levelID.Trim())).FirstOrDefault().Text;

            /* var level = this.Levels.Where(x => x.Value.Trim().Contains(levelID.Trim()+":")).FirstOrDefault();
             if (level != null)
             {
                 string[] keys = level.Value.Split(':');
                 return this.Grades.Where(x => x.Value.Trim() == keys[1].Trim()).FirstOrDefault().Text;
             }
             return "-";*/
        }

        public string GetGradeValue(string gradeText)
        {
            return this.Grades.Where(x => x.Text.Trim().Equals(gradeText.Trim())).FirstOrDefault().Value;
            /*var level = this.Levels.Where(x => x.Value.Trim().Contains(":"+gradeID.Trim() )).FirstOrDefault();
            if (level != null)
            {
                string[] keys = level.Value.Split(':');
                return keys[0];
            }
            return "-";*/
        }
    }

    public class Eduction
    {
        //public string ID { get; set; }
        public string School { get; set; }

        public string Major { get; set; }
        public string Level { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string GradeOrPercentage { get; set; }
    }
}