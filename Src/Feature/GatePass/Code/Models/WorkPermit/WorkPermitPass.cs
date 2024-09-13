// <copyright file="WorkPermitPass.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Models.WorkPermit
{
    using DEWAXP.Foundation.Integration.SmartVendorSvc;
    using DEWAXP.Feature.GatePass.Models.ePass;
    using FileHelpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Defines the <see cref="WorkPermitPass" />.
    /// </summary>
    public class WorkPermitPass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkPermitPass"/> class.
        /// </summary>
        public WorkPermitPass()
        {
            SubcontractorList = new List<subContractorDetails>();
        }

        /// <summary>
        /// Gets or sets the GroupPass_Applicants.
        /// </summary>
        public HttpPostedFileBase GroupPass_Applicants { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allpasssubmitted.
        /// </summary>
        public bool allpasssubmitted { get; set; }

        /// <summary>
        /// Gets or sets the errorlist.
        /// </summary>
        public List<ErrorInfo> errorlist { get; set; }

        /// <summary>
        /// Gets or sets the PassSubmitType.
        /// </summary>
        public string PassSubmitType { get; set; }

        /// <summary>
        /// Gets or sets the errorDuplicatelist.
        /// </summary>
        public List<ErrorDuplicateInfo> errorDuplicatelist { get; set; }

        /// <summary>
        /// Gets or sets the SinglePass_Photo_Bytes.
        /// </summary>
        public byte[] SinglePass_Photo_Bytes { get; set; } = new byte[0];

        /// <summary>
        /// Gets or sets the SinglePass_Passport_Bytes.
        /// </summary>
        public byte[] SinglePass_Passport_Bytes { get; set; } = new byte[0];

        /// <summary>
        /// Gets or sets the SinglePass_Visa_Bytes.
        /// </summary>
        public byte[] SinglePass_Visa_Bytes { get; set; } = new byte[0];

        /// <summary>
        /// Gets or sets the SinglePass_EID_Bytes.
        /// </summary>
        public byte[] SinglePass_EID_Bytes { get; set; } = new byte[0];

        /// <summary>
        /// Gets or sets the ReferenceNumber.
        /// </summary>
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the PageNo.
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// Gets or sets the SinglePass_Photo.
        /// </summary>
        public HttpPostedFileBase SinglePass_Photo { get; set; }

        /// <summary>
        /// Gets or sets the SinglePass_EmiratesID.
        /// </summary>
        public HttpPostedFileBase SinglePass_EmiratesID { get; set; }

        /// <summary>
        /// Gets or sets the SinglePass_Visa.
        /// </summary>
        public HttpPostedFileBase SinglePass_Visa { get; set; }

        /// <summary>
        /// Gets or sets the SinglePass_Passport.
        /// </summary>
        public HttpPostedFileBase SinglePass_Passport { get; set; }

        /// <summary>
        /// Gets or sets the DrivingLicense.
        /// </summary>
        public HttpPostedFileBase DrivingLicense { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsApplicantphoto.
        /// </summary>
        public bool IsApplicantphoto { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsEmiratesid.
        /// </summary>
        public bool IsEmiratesid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Ispassportdocument.
        /// </summary>
        public bool Ispassportdocument { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Isvisadocument.
        /// </summary>
        public bool Isvisadocument { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDrivingLicense.
        /// </summary>
        public bool IsDrivingLicense { get; set; }

        /// <summary>
        /// Gets or sets the Serialnumber.
        /// </summary>
        public string Serialnumber { get; set; }

        /// <summary>
        /// Gets or sets the PONumber.
        /// </summary>
        public string PONumber { get; set; }

        /// <summary>
        /// Gets or sets the POName.
        /// </summary>
        public string POName { get; set; }

        /// <summary>
        /// Gets or sets the Mobilenumber.
        /// </summary>
        public string Mobilenumber { get; set; }

        /// <summary>
        /// Gets or sets the Emailaddress.
        /// </summary>
        public string Emailaddress { get; set; }

        /// <summary>
        /// Gets or sets the ProjectCoordinatorEmailaddress.
        /// </summary>
        public string ProjectCoordinatorEmailaddress { get; set; }

        /// <summary>
        /// Gets or sets the ProjectCoordinatorMobile.
        /// </summary>
        public string ProjectCoordinatorMobile { get; set; }

        /// <summary>
        /// Gets or sets the PassNumber.
        /// </summary>
        public string PassNumber { get; set; }

        /// <summary>
        /// Gets or sets the PassIssue.
        /// </summary>
        public string PassIssue { get; set; }

        /// <summary>
        /// Gets the PassIssueDate.
        /// </summary>
        public DateTime? PassIssueDate => converttodate(PassIssue);

        /// <summary>
        /// Gets or sets the PassExpiry.
        /// </summary>
        public string PassExpiry { get; set; }

        /// <summary>
        /// Gets the PassExpiryDate.
        /// </summary>
        public DateTime? PassExpiryDate => converttodate(PassExpiry);

        /// <summary>
        /// Gets or sets the FromTime.
        /// </summary>
        public string FromTime { get; set; }

        /// <summary>
        /// Gets or sets the ToTime.
        /// </summary>
        public string ToTime { get; set; }

        /// <summary>
        /// Gets or sets the PassType.
        /// </summary>
        public string PassType { get; set; }

        /// <summary>
        /// Gets or sets the FullName.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the Nationality.
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// Gets or sets the ProfessionLevel.
        /// </summary>
        public string ProfessionLevel { get; set; }

        /// <summary>
        /// Gets or sets the Designation.
        /// </summary>
        public string Designation { get; set; }

        /// <summary>
        /// Gets or sets the EmiratesID.
        /// </summary>
        public string EmiratesID { get; set; }

        /// <summary>
        /// Gets or sets the EmiratesIDExpiry.
        /// </summary>
        public string EmiratesIDExpiry { get; set; }

        /// <summary>
        /// Gets the EmiratesIDExpiryDate.
        /// </summary>
        public DateTime? EmiratesIDExpiryDate => converttodate(EmiratesIDExpiry);

        /// <summary>
        /// Gets or sets the VisaNumber.
        /// </summary>
        [MaxLength(15)]
        public string VisaNumber { get; set; }

        /// <summary>
        /// Gets or sets the VisaExpiry.
        /// </summary>
        public string VisaExpiry { get; set; }

        /// <summary>
        /// Gets the VisaExpiryDate.
        /// </summary>
        public DateTime? VisaExpiryDate => converttodate(VisaExpiry);

        /// <summary>
        /// Gets or sets the PassportNumber.
        /// </summary>
        public string PassportNumber { get; set; }

        /// <summary>
        /// Gets or sets the PassportExpiry.
        /// </summary>
        public string PassportExpiry { get; set; }

        /// <summary>
        /// Gets the PassportExpiryDate.
        /// </summary>
        public DateTime? PassportExpiryDate => converttodate(PassportExpiry);

        /// <summary>
        /// Gets or sets the SubContractorID.
        /// </summary>
        public string SubContractorID { get; set; }

        /// <summary>
        /// Gets or sets the VendorID.
        /// </summary>
        public string VendorID { get; set; }

        /// <summary>
        /// Gets or sets the PurposeofVisit.
        /// </summary>
        public string PurposeofVisit { get; set; }

        /// <summary>
        /// Gets or sets the GroupPurposeofVisit.
        /// </summary>
        public string GroupPurposeofVisit { get; set; }

        /// <summary>
        /// Gets or sets the Remarks.
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the Projectcoordinatorname.
        /// </summary>
        public string Projectcoordinatorname { get; set; }

        /// <summary>
        /// Gets or sets the Coor_Username_List.
        /// </summary>
        public string Coor_Username_List { get; set; }

        /// <summary>
        /// Gets or sets the Submitter_Email_ID.
        /// </summary>
        public string Submitter_Email_ID { get; set; }

        /// <summary>
        /// Gets or sets the Coor_eMail_IDs.
        /// </summary>
        public string Coor_eMail_IDs { get; set; }

        /// <summary>
        /// Gets or sets the CompanyName.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the projectId.
        /// </summary>
        public string projectId { get; set; }

        /// <summary>
        /// Gets or sets the projectStatus.
        /// </summary>
        public string projectStatus { get; set; }

        /// <summary>
        /// Gets or sets the departmentName.
        /// </summary>
        public string departmentName { get; set; }

        /// <summary>
        /// Gets or sets the projectStartDate.
        /// </summary>
        public DateTime? projectStartDate { get; set; }

        /// <summary>
        /// Gets or sets the projectEndDate.
        /// </summary>
        public DateTime? projectEndDate { get; set; }

        /// <summary>
        /// Gets or sets the eFolderId.
        /// </summary>
        public string eFolderId { get; set; }

        /// <summary>
        /// Gets or sets the PlateCode.
        /// </summary>
        public string PlateCode { get; set; }

        /// <summary>
        /// Gets or sets the PlateNumber.
        /// </summary>
        public string PlateNumber { get; set; }

        /// <summary>
        /// Gets or sets the Emirates.
        /// </summary>
        public List<SelectListItem> Emirates { get; set; }

        /// <summary>
        /// Gets or sets the PlateCategory.
        /// </summary>
        public List<SelectListItem> PlateCategory { get; set; }

        /// <summary>
        /// Gets or sets the EmirateOrCountry.
        /// </summary>
        public string EmirateOrCountry { get; set; }

        /// <summary>
        /// Gets or sets the SelectedPlateCategory.
        /// </summary>
        public string SelectedPlateCategory { get; set; }

        /// <summary>
        /// Gets or sets the MulkiyaNumber.
        /// </summary>
        public string MulkiyaNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether withcar.
        /// </summary>
        public bool withcar { get; set; }

        /// <summary>
        /// Gets or sets the LocationList.
        /// </summary>
        public IEnumerable<SelectListItem> LocationList { get; set; }

        /// <summary>
        /// Gets or sets the NationalityList.
        /// </summary>
        public IEnumerable<SelectListItem> NationalityList { get; set; }

        /// <summary>
        /// Gets or sets the SubcontractorList.
        /// </summary>
        public List<subContractorDetails> SubcontractorList { get; set; }

        /// <summary>
        /// Gets or sets the SelectedLocation.
        /// </summary>
        public IEnumerable<string> SelectedLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enablerenewbutton.
        /// </summary>
        public bool enablerenewbutton { get; set; }

        /// <summary>
        /// The converttodate.
        /// </summary>
        /// <param name="strdate">The strdate<see cref="string"/>.</param>
        /// <returns>The <see cref="DateTime?"/>.</returns>
        public DateTime? converttodate(string strdate)
        {
            if (!string.IsNullOrWhiteSpace(strdate))
            {
                CultureInfo culture = global::Sitecore.Context.Culture;
                if (culture.ToString().Equals("ar-AE"))
                {
                    strdate = strdate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                }
                DateTime.TryParse(strdate, out DateTime fromdateTime);
                return fromdateTime;
            }
            return null;
        }

        /// <summary>
        /// Gets or sets the GroupSelectedLocation.
        /// </summary>
        public IEnumerable<string> GroupSelectedLocation { get; set; }

        /// <summary>
        /// Gets or sets the GroupSubcontractorID.
        /// </summary>
        public string GroupSubcontractorID { get; set; }

        /// <summary>
        /// Gets or sets the GroupProjectcoordinatorname.
        /// </summary>
        public string GroupProjectcoordinatorname { get; set; }

        /// <summary>
        /// Gets or sets the GroupProjectCoordinatorEmailaddress.
        /// </summary>
        public string GroupProjectCoordinatorEmailaddress { get; set; }

        /// <summary>
        /// Gets or sets the GroupProjectCoordinatorMobile.
        /// </summary>
        public string GroupProjectCoordinatorMobile { get; set; }

        /// <summary>
        /// Gets or sets the GroupPassIssue.
        /// </summary>
        public string GroupPassIssue { get; set; }

        /// <summary>
        /// Gets or sets the GroupPassid.
        /// </summary>
        public string GroupPassid { get; set; }

        /// <summary>
        /// Gets the GroupPassIssueDate.
        /// </summary>
        public DateTime? GroupPassIssueDate => converttodate(GroupPassIssue);

        /// <summary>
        /// Gets or sets the GroupPassExpiry.
        /// </summary>
        public string GroupPassExpiry { get; set; }

        /// <summary>
        /// Gets the GroupPassExpiryDate.
        /// </summary>
        public DateTime? GroupPassExpiryDate => converttodate(GroupPassExpiry);

        /// <summary>
        /// Gets or sets the GroupFromTime.
        /// </summary>
        public string GroupFromTime { get; set; }

        /// <summary>
        /// Gets or sets the GroupToTime.
        /// </summary>
        public string GroupToTime { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ePassLogin" />.
    /// </summary>
    public class WorkPermitLogin
    {
        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the UserPassword.
        /// </summary>
        public string UserPassword { get; set; }

        /// <summary>
        /// Gets or sets the UserType.
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// Gets or sets the Userid.
        /// </summary>
        public string Userid { get; set; }

        /// <summary>
        /// Gets or sets the LstworkPermitPassDetails.
        /// </summary>
        public List<workPermitPassDetails> LstworkPermitPassDetails { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="AttachmentTypes" />.
    /// </summary>
    internal static class AttachmentTypes
    {
        /// <summary>
        /// Defines the Applicant_Photo.
        /// </summary>
        public const string Applicant_Photo = "4";

        /// <summary>
        /// Defines the EmiratesID.
        /// </summary>
        public const string EmiratesID = "6";

        /// <summary>
        /// Defines the PassportDocument.
        /// </summary>
        public const string PassportDocument = "8";

        /// <summary>
        /// Defines the VisaDocument.
        /// </summary>
        public const string VisaDocument = "10";

        /// <summary>
        /// Defines the DrivingLicense.
        /// </summary>
        public const string DrivingLicense = "12";

        /// <summary>
        /// Defines the TradeLicense.
        /// </summary>
        public const string TradeLicense = "14";
    }
}
