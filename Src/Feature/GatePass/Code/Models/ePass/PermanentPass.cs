// <copyright file="PermanentPass.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Models.ePass
{
    using FileHelpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Defines the <see cref="PermanentPass" />.
    /// </summary>
    public class PermanentPass
    {
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
        /// Gets or sets the SinglePass_DrivingLicense_Bytes.
        /// </summary>
        public byte[] SinglePass_DrivingLicense_Bytes { get; set; } = new byte[0];

        /// <summary>
        /// Gets or sets the SinglePass_EID_Bytes.
        /// </summary>
        public byte[] SinglePass_EID_Bytes { get; set; } = new byte[0];

        /// <summary>
        /// Gets or sets the Vehicle registration bytes....
        /// </summary>
        public byte[] SinglePass_VehicleRegistration_Bytes { get; set; } = new byte[0];

        /// <summary>
        /// Gets or sets the SinglePass_DewaID_Bytes.
        /// </summary>
        public byte[] SinglePass_DewaID_Bytes { get; set; } = new byte[0];

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
        /// Gets or sets the SinglePass_DrivingLicense.
        /// </summary>
        public HttpPostedFileBase SinglePass_DrivingLicense { get; set; }

        /// <summary>
        /// Gets or sets the SinglePass_Visa.
        /// </summary>
        public HttpPostedFileBase SinglePass_Visa { get; set; }

        /// <summary>
        /// Gets or sets the SinglePass_Passport.
        /// </summary>
        public HttpPostedFileBase SinglePass_Passport { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Registration....
        /// </summary>
        public HttpPostedFileBase SinglePass_VehicleRegistration { get; set; }

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
        /// Gets or sets the PassNumber.
        /// </summary>
        public string PassNumber { get; set; }

        /// <summary>
        /// Gets or sets the PassExpiry.
        /// </summary>
        public string PassExpiry { get; set; }

        /// <summary>
        /// Gets or sets the PassIssue.
        /// </summary>
        public string PassIssue { get; set; }

        /// <summary>
        /// Gets or sets the DateOfVisit
        /// Gets of sets the Date of visit...
        /// </summary>
        public string DateOfVisit { get; set; }

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
        /// Gets or sets the PassSubmitType.
        /// </summary>
        public string PassSubmitType { get; set; }

        /// <summary>
        /// Gets or sets the PassStatus.
        /// </summary>
        public string PassStatus { get; set; }

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
        /// Gets or sets the VisaNumber.
        /// </summary>
        [MaxLength(15)]
        public string VisaNumber { get; set; }

        /// <summary>
        /// Gets or sets the VisaExpiry.
        /// </summary>
        public string VisaExpiry { get; set; }

        /// <summary>
        /// Gets or sets the PassportNumber.
        /// </summary>
        public string PassportNumber { get; set; }

        /// <summary>
        /// Gets or sets the VehicleNumber
        /// Vehicle Number of Code...
        /// </summary>
        public string VehicleNumber { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Registration or Insurance date...
        /// </summary>
        public string VehicleRegistrationDate { get; set; }

        /// <summary>
        /// Gets or sets the PassportExpiry.
        /// </summary>
        public string PassportExpiry { get; set; }

        /// <summary>
        /// Gets or sets the SubContractorID.
        /// </summary>
        public string SubContractorID { get; set; }

        /// <summary>
        /// Gets or sets the VendorID.
        /// </summary>
        public string VendorID { get; set; }

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
        /// Gets or sets the SecurityApproversEmail.
        /// </summary>
        public string SecurityApproversEmail { get; set; }

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
        /// Gets or sets the SubContractList.
        /// </summary>
        public List<ePassSubContractor> SubContractList { get; set; }

        /// <summary>
        /// Gets or sets the eFolderId.
        /// </summary>
        public string eFolderId { get; set; }

        /// <summary>
        /// Gets or sets the errorlist.
        /// </summary>
        public List<ErrorInfo> errorlist { get; set; }

        /// <summary>
        /// Gets or sets the errorDuplicatelist.
        /// </summary>
        public List<ErrorDuplicateInfo> errorDuplicatelist { get; set; }

        /// <summary>
        /// Gets or sets the GroupPass_Applicants.
        /// </summary>
        public HttpPostedFileBase GroupPass_Applicants { get; set; }

        /// <summary>
        /// Gets or sets the GroupPass_Photo.
        /// </summary>
        public HttpPostedFileBase GroupPass_Photo { get; set; }

        /// <summary>
        /// Gets or sets the GroupPass_EmiratesID.
        /// </summary>
        public HttpPostedFileBase GroupPass_EmiratesID { get; set; }

        /// <summary>
        /// Gets or sets the GroupPass_Visa.
        /// </summary>
        public HttpPostedFileBase GroupPass_Visa { get; set; }

        /// <summary>
        /// Gets or sets the GroupPass_Passport.
        /// </summary>
        public HttpPostedFileBase GroupPass_Passport { get; set; }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        public List<SelectListItem> Location { get; set; }

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
        /// Gets or sets the PlateCode.
        /// </summary>
        public string PlateCode { get; set; }

        /// <summary>
        /// Gets or sets the PlateNumber.
        /// </summary>
        public string PlateNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether withcar.
        /// </summary>
        public bool withcar { get; set; }

        /// <summary>
        /// Gets or sets the SelectedLocation.
        /// </summary>
        public IEnumerable<string> SelectedLocation { get; set; }

        /// <summary>
        /// Gets or sets the op_projectName - purpose of visit or project name...
        /// </summary>
        public string op_projectName { get; set; }

        /// <summary>
        /// Gets or sets the op_projectID. Tander document or PO...
        /// </summary>
        public string op_projectID { get; set; }

        /// <summary>
        /// Gets or sets the op_visitorEmailid.
        /// </summary>
        public string op_visitorEmailid { get; set; }

        /// <summary>
        /// Gets or sets the op_seniormanagerEmailid.
        /// </summary>
        public string op_seniormanagerEmailid { get; set; }

        /// <summary>
        /// Gets or sets the op_visitorname.
        /// </summary>
        public string op_visitorname { get; set; }

        /// <summary>
        /// Gets or sets the op_dewantid.
        /// </summary>
        public string op_dewantid { get; set; }

        /// <summary>
        /// Gets or sets the op_code.
        /// </summary>
        public string op_code { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether op_dewaemployee.
        /// </summary>
        public bool op_dewaemployee { get; set; }

        /// <summary>
        /// Gets or sets the op_DEWAId.
        /// </summary>
        public HttpPostedFileBase op_DEWAId { get; set; }

        /// <summary>
        /// Gets or sets the OfficeLocations.
        /// </summary>
        public List<SelectListItem> OfficeLocations { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ErrorDuplicateInfo" />.
    /// </summary>
    public class ErrorDuplicateInfo
    {
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the EmiratesID.
        /// </summary>
        public string EmiratesID { get; set; }

        /// <summary>
        /// Gets or sets the VisaNumber.
        /// </summary>
        public string VisaNumber { get; set; }

        /// <summary>
        /// Gets or sets the PassportNumber.
        /// </summary>
        public string PassportNumber { get; set; }

        /// <summary>
        /// Gets or sets the Errormessage.
        /// </summary>
        public string Errormessage { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ePassLogin" />.
    /// </summary>
    public class ePassLogin
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
        /// Gets or sets the lstpassess.
        /// </summary>
        public List<SecurityPassViewModel> lstpassess { get; set; }

        /// <summary>
        /// Gets or sets the Userid.
        /// </summary>
        public string Userid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether onedayinitiate.
        /// </summary>
        public bool onedayinitiate { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ePassSubContractor" />.
    /// </summary>
    public class ePassSubContractor
    {
        /// <summary>
        /// Gets or sets the TradelicenseDoc.
        /// </summary>
        public HttpPostedFileBase TradelicenseDoc { get; set; }

        /// <summary>
        /// Gets or sets the TradelicenseDoc_Bytes.
        /// </summary>
        public byte[] TradelicenseDoc_Bytes { get; set; } = new byte[0];

        /// <summary>
        /// Gets or sets the TradelicenseDocFileName.
        /// </summary>
        public string TradelicenseDocFileName { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Vendor_Account_Category.
        /// </summary>
        public string Vendor_Account_Category { get; set; }

        /// <summary>
        /// Gets or sets the Vendor_Name.
        /// </summary>
        public string Vendor_Name { get; set; }

        /// <summary>
        /// Gets or sets the Address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the City.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the CountryKey.
        /// </summary>
        public string CountryKey { get; set; }

        /// <summary>
        /// Gets or sets the PO_Box.
        /// </summary>
        public string PO_Box { get; set; }

        /// <summary>
        /// Gets or sets the Telephone_Number.
        /// </summary>
        public string Telephone_Number { get; set; }

        /// <summary>
        /// Gets or sets the Email_Address.
        /// </summary>
        public string Email_Address { get; set; }

        /// <summary>
        /// Gets or sets the CountryList.
        /// </summary>
        public IEnumerable<SelectListItem> CountryList { get; set; }

        /// <summary>
        /// Gets or sets the Trade_License_Number.
        /// </summary>
        public string Trade_License_Number { get; set; }

        /// <summary>
        /// Gets or sets the Trade_License_Issue_Date.
        /// </summary>
        public string Trade_License_Issue_Date { get; set; }

        /// <summary>
        /// Gets or sets the Trade_License_Expiry_Date.
        /// </summary>
        public string Trade_License_Expiry_Date { get; set; }

        /// <summary>
        /// Gets or sets the SubcontractorID.
        /// </summary>
        public string SubcontractorID { get; set; }

        /// <summary>
        /// Gets or sets the displaysubcontractorid.
        /// </summary>
        public string displaysubcontractorid { get; set; }

        /// <summary>
        /// Gets or sets the displaysubcontractorname.
        /// </summary>
        public string displaysubcontractorname { get; set; }

        /// <summary>
        /// Gets or sets the displaycountry.
        /// </summary>
        public string displaycountry { get; set; }

        /// <summary>
        /// Gets or sets the displaytradelicensenumber.
        /// </summary>
        public string displaytradelicensenumber { get; set; }

        /// <summary>
        /// Gets or sets the displaytradeissuedate.
        /// </summary>
        public string displaytradeissuedate { get; set; }

        /// <summary>
        /// Gets or sets the displaytradeenddate.
        /// </summary>
        public string displaytradeenddate { get; set; }

        /// <summary>
        /// Gets or sets the ReferenceNumber.
        /// </summary>
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the Successmessage.
        /// </summary>
        public string Successmessage { get; set; }

        /// <summary>
        /// Gets the DT_Trade_License_Issue_Date.
        /// </summary>
        public DateTime? DT_Trade_License_Issue_Date => Converttodate(Trade_License_Issue_Date);

        /// <summary>
        /// Gets the DT_Trade_License_Expiry_Date.
        /// </summary>
        public DateTime? DT_Trade_License_Expiry_Date => Converttodate(Trade_License_Expiry_Date);

        /// <summary>
        /// The converttodate.
        /// </summary>
        /// <param name="strdate">The strdate<see cref="string"/>.</param>
        /// <returns>The <see cref="DateTime?"/>.</returns>
        public DateTime? Converttodate(string strdate)
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
    }

    /// <summary>
    /// Defines the <see cref="ListTableContractor" />.
    /// </summary>
    public class ListTableContractor
    {
        /// <summary>
        /// Gets or sets the Table.
        /// </summary>
        public List<TABLE_SUB_CONTRACTOR> Table { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="TABLE_SUB_CONTRACTOR" />.
    /// </summary>
    public class TABLE_SUB_CONTRACTOR
    {
        /// <summary>
        /// Gets or sets the SUB_CONT_NAME.
        /// </summary>
        public string SUB_CONT_NAME { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ListTableResponseData{T}" />.
    /// </summary>
    /// <typeparam name="T">.</typeparam>
    public class ListTableResponseData<T>
    {
        /// <summary>
        /// Gets or sets the Table.
        /// </summary>
        public List<T> Table { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListTableResponseData{T}"/> class.
        /// </summary>
        /// <param name="table">The table<see cref="List{T}"/>.</param>
        /// <param name="succeeded">The succeeded<see cref="bool"/>.</param>
        /// <param name="message">The message<see cref="string"/>.</param>
        public ListTableResponseData(List<T> table, bool succeeded = true, string message = "Success")
        {
            Table = table;
        }
    }
}
