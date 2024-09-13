using DEWAXP.Foundation.DataAnnotations;
using DEWAXP.Foundation.Integration.JobSeekerSvc;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.HR.Models.CareerPortal
{
    public class CandidateProfile
    {
        #region Personal Information

        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Religion { get; set; }
        public List<SelectListItem> ReligionList { get; set; }
        public string PeopleOfDetermination { get; set; }
        public string NatureOfWorkType { get; set; }
        public string ExperienceType { get; set; }

        public string MaritalStatus { get; set; }
        public string DOB { get; set; }
        public string Nationality { get; set; }
        public string InterestGroup { get; set; }
        public string HierarachyLevel { get; set; }
        public string ApplicationSourceType { get; set; }
        public string ApplicationSource { get; set; }
        public string FunctionalAreas { get; set; }
        public string CoverLetterDescription { get; set; }
        public List<SelectListItem> NationalityList { get; set; }
        public List<SelectListItem> InterestGroupsList { get; set; }
        public List<SelectListItem> HierarachyLevelList { get; set; }
        public List<applicationSource> ApplicationSourceTypeList { get; set; }
        public List<applicationSource> ApplicationSourceList { get; set; }

        public string PassportNo { get; set; }
        public string IsUAEResident { get; set; }

        [EmiratesID(ValidationMessageKey = "emiratesid validation message")]
        public string EmiratesID { get; set; }

        [EmiratesID(ValidationMessageKey = "emiratesid validation message")]
        public string ImportEmiratesID { get; set; }

        public string DisabilityId { get; set; }
        public string YearsOfExperience { get; set; }
        public string HighestQualificationLevel { get; set; }
        public ProfielRelease ProfileRelease { get; set; }
        public string RegistrationType { get; set; }

        #endregion Personal Information

        #region Address & Contact Information

        public string Country { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public string Emirates { get; set; }
        public List<SelectListItem> EmiratesList { get; set; }
        public string City { get; set; }
        public string permantAddress { get; set; }
        public string MobileNo { get; set; }
        public string PostalCode { get; set; }
        public string ProfilePic { get; set; }
        public string ProfilePicId { get; set; }

        #endregion Address & Contact Information

        #region Educational & Professional Qualifications

        #region Education

        public List<Educations> CandidateEducation { get; set; }

        //public List<SelectListItem> Grades { get; set; }
        public List<SelectListItem> EducationLevel { get; set; }

        public List<SelectListItem> EducationType { get; set; }
        public List<SelectListItem> FieldOfStudy { get; set; }
        public string postedEducations { get; set; }

        #endregion Education

        #region Qualification

        public List<Qualification> CandidateQualification { get; set; }
        public List<Qualification> CandidateCertification { get; set; }
        public List<SelectListItem> QualificationGroup { get; set; }
        public List<SelectListItem> QualificationList { get; set; }
        public List<SelectListItem> CertificationList { get; set; }

        public List<SelectListItem> ProficiencyList { get; set; }
        public string postedQualifications { get; set; }

        #endregion Qualification

        #region Certification

        public List<Certification> CandiadateCeritification { get; set; }
        public string postedCeritificates { get; set; }

        #endregion Certification

        #region Work experience

        public List<WorkExperience> CandidateWorkExperience { get; set; }
        public List<SelectListItem> DesignationList { get; set; }
        public List<SelectListItem> JobLocation { get; set; }
        public string postedWorkExperience { get; set; }

        #endregion Work experience

        #endregion Educational & Professional Qualifications

        public HttpPostedFileBase PassportFile { get; set; }
        public HttpPostedFileBase EmiratesIDFile { get; set; }
        public List<SelectListItem> AttachementTypes { get; set; }
        public List<UploadedDocuments> CandidateAttchments { get; set; }

        #region Common

        public string Section { get; set; }
        public string JobId { get; set; }
        public string strMessage { get; set; }
        public bool Success { get; set; } = true;
        public string errorCode { get; set; }
        public string RegistrationId { get; set; }
        public bool SaveandContinueButtonClicked { get; set; }
        public string sequenceNo { get; set; }
        public string objectid { get; set; }
        public string objecttype { get; set; }
        public string planversion { get; set; }
        public bool IsSessionCheck { get; set; } = true;
        public string LastLogin { get; set; }
        public int ProfileProgress { get; set; }
        public bool IsSubmit { get; set; }

        #endregion Common
    }

    public class Educations
    {
        public string EducationLevel { get; set; }
        public string UniversityName { get; set; }
        public string FieldOfStudy { get; set; }
        public string strStartDate { get; set; }
        public string strEndDate { get; set; }
        public string sequenceNo { get; set; }
        public string objectid { get; set; }
        public string objectversion { get; set; }
        public string planversion { get; set; }
    }

    public class Qualification
    {
        public string IssuingOrganization { get; set; }
        public string QualificationGrp { get; set; }
        public string QualificationGrpId { get; set; }
        public string ProficiencyName { get; set; }
        public string ProficiencyId { get; set; }
        public string QualificationId { get; set; }
        public string QualificationName { get; set; }
        public string sequenceNo { get; set; }
        public string objectid { get; set; }
        public string objectversion { get; set; }
        public string planversion { get; set; }
    }

    public class Certification
    {
        public string CertificationName { get; set; }
        public string OrganizationName { get; set; }
    }

    public class WorkExperience
    {
        public string EmployerName { get; set; }
        public string Designation { get; set; }
        public string JobLocation { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool CurrentEmployer { get; set; }
        public string sequenceNo { get; set; }
        public string objectid { get; set; }
        public string objectversion { get; set; }
        public string planversion { get; set; }
    }

    public class UploadedDocuments
    {
        public HttpPostedFileBase DocumentType { get; set; }
        public byte[] content { get; set; }

        public string attachment { get; set; }

        public string attachmentguid { get; set; }

        public string attachmentheader { get; set; }

        public string attachmenttype { get; set; }

        public string attachmenttypetext { get; set; }

        public string attachmenturl { get; set; }

        public string contenttype { get; set; }

        public string languagekey { get; set; }

        public string languagetxt { get; set; }

        public string objectid { get; set; }

        public string objecttype { get; set; }

        public string planversion { get; set; }

        public string refernceguid { get; set; }

        public string sequencenumber { get; set; }

        public string ContentBase64 { get; set; }
    }

    public class ProfielRelease
    {
        public string PrivacyAccepted { get; set; }
        public string TermsCondition { get; set; }
        public string PrivacyUrl { get; set; }
        public string ProfileStatus { get; set; }
        public string ProfileStatustext { get; set; }
        public bool Success { get; set; } = true;
        public string strMessage { get; set; }
        public string errorCode { get; set; }
    }

    public class QualificationValues
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public string Group { get; set; }
    }
}