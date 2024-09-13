// <copyright file="BookTrainingModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Mayur Prajapati</author>

using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.ShamsDubai.Models.ShamsDubaiTraining
{
    [Serializable]
    /// <summary>
    /// Defines the <see cref="BookTrainingModel" />.
    /// </summary>
    public class BookTrainingModel
    {
        public BookTrainingModel()
        {
            companyDetails = new CompanyDetails();
            ReasonforEnrollmentList = new List<SelectListItem>();
            DesignationList = new List<SelectListItem>();
        }
        public long TrainingId { get; set; }
        public DateTime? TraningStartDate { get; set; }
        public DateTime? TraningEndDate { get; set; }
        public string TrainingName { get; set; }
        public string TrainingDuration { get; set; }
        public string EmiratesID { get; set; }
        public string ApplicationName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string PVCCCertificateNumber { get; set; }
        public string ReasonforEnrollment { get; set; }
        public string Nationality { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string SolarPVExpert { get; set; }
        public string VisaNumber { get; set; }
        public DateTime VisaIssueDate { get; set; }
        public DateTime VisaExpiryDate { get; set; }
        public string PassportNumber { get; set; }
        public DateTime PassportIssueDate { get; set; }
        public DateTime PassportExpiryDate { get; set; }
        public string ElectricalSystem { get; set; }
        public string PVDesign { get; set; }
        public string DescforShamsProject { get; set; }
        public string DescforAdditional { get; set; }
        public CompanyDetails companyDetails { get; set; }
        public bool IsSuccess { get; set; }
        public string RequestNumber { get; set; }
        public bool IsCheckedCertificate { get; set; }

        #region DropdownlistItems
        public List<SelectListItem> ReasonforEnrollmentList { get; set; }
        public List<SelectListItem> DesignationList { get; set; }
        public List<SelectListItem> ElectricalSystemList { get; set; }
        public List<SelectListItem> PVDesignList { get; set; }
        public List<SelectListItem> SolarPVExpertList { get; set; }
        #endregion

        #region Attachment
        // Applicant’s Experience Certificates (attested)
        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase PV1_ExperienceCertificate { get; set; }
        public byte[] PV1_ExperienceCertificate_FileBinary { get; set; }
        public string PV1_ExperienceCertificate_FileType { get; set; }

        

        // PVCC Enrollment form with Company Seal
        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase PV2_CompanySeal { get; set; }
        public byte[] PV2_CompanySeal_FileBinary { get; set; }
        public string PV2_CompanySeal_FileType { get; set; }


        // Company Trade License
        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase PV3_TradeLicense { get; set; }
        public byte[] PV3_TradeLicense_FileBinary { get; set; }
        public string PV3_TradeLicense_FileType { get; set; }


        // Applicant’s degree (attested)
        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase PV4_ApplicantDegree { get; set; }
        public byte[] PV4_ApplicantDegree_FileBinary { get; set; }
        public string PV4_ApplicantDegree_FileType { get; set; }


        // Applicant’s Passport (attested)
        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase PV5_ApplicantPassport { get; set; }
        public byte[] PV5_ApplicantPassport_FileBinary { get; set; }
        public string PV5_ApplicantPassport_FileType { get; set; }


        // Applicant’s Visa (attested)
        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase PV6_ApplicantEmiratesID { get; set; }
        public byte[] PV6_ApplicantEmiratesID_FileBinary { get; set; }
        public string PV6_ApplicantEmiratesID_FileType { get; set; }


        // Others (if any)
        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase PV7_Others { get; set; }
        public byte[] PV7_Others_FileBinary { get; set; }
        public string PV7_Others_FileType { get; set; }

        // Applicant's Photo
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase PV8_ApplicantPhoto { get; set; }
        public byte[] PV8_ApplicantPhoto_FileBinary { get; set; }
        public string PV8_ApplicantPhoto_FileType { get; set; }
        #endregion
    }
    public class CompanyDetails
    {
        public string CompanyName { get; set; }
        public string CompanyTradeLicenceNumber { get; set; }
        public DateTime LicenseIssueDate { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public string VATRegistrationNumber { get; set; }
        public string CompanyActivityDescription { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyMobile { get; set; }
        public string ContactPersonName { get; set; }

    }
}
