// <copyright file="SecurityPassViewModel.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Models.ePass
{
    using DEWAXP.Foundation.Integration.Responses.SmartVendor.WorkPermit;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="SecurityPassViewModel" />.
    /// </summary>
    public class SecurityPassViewModel
    {
        /// <summary>
        /// Gets or sets the eFolderId.
        /// </summary>
        public string eFolderId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public SecurityPassStatus status { get; set; }

        /// <summary>
        /// Gets or sets the SeachbyNumber.
        /// </summary>
        public string SeachbyNumber { get; set; }

        /// <summary>
        /// Gets or sets the passNumber.
        /// </summary>
        public string passNumber { get; set; }

        /// <summary>
        /// Gets or sets the mainpassNumber.
        /// </summary>
        public string mainpassNumber { get; set; }
        public string grouppassid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Subpass.
        /// </summary>
        public bool Subpass { get; set; }

        /// <summary>
        /// Gets or sets the passExpiryDate.
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? passExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the passIssueDate.
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? passIssueDate { get; set; }

        /// <summary>
        /// Gets or sets the CreatedDate.
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the ChangedDate.
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? ChangedDate { get; set; }

        /// <summary>
        /// Gets or sets the passType.
        /// </summary>
        public string passType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether wppass.
        /// </summary>
        public bool wppass { get; set; }
        public bool grouppass { get; set; }

        /// <summary>
        /// Gets or sets the passTypeText.
        /// </summary>
        public string passTypeText { get; set; }

        /// <summary>
        /// Gets or sets the nationality.
        /// </summary>
        public string nationality { get; set; }

        /// <summary>
        /// Gets or sets the profession.
        /// </summary>
        public string profession { get; set; }

        /// <summary>
        /// Gets or sets the visaNumber.
        /// </summary>
        public string visaNumber { get; set; }

        /// <summary>
        /// Gets or sets the visaExpiryDate.
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? visaExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the passportNumber.
        /// </summary>
        public string passportNumber { get; set; }

        /// <summary>
        /// Gets or sets the passportExpiryDate.
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? passportExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the emiratesId.
        /// </summary>
        public string emiratesId { get; set; }

        /// <summary>
        /// Gets or sets the strstatus.
        /// </summary>
        public string strstatus { get; set; }

        /// <summary>
        /// Gets or sets the strclass.
        /// </summary>
        public string strclass { get; set; }

        /// <summary>
        /// Gets or sets the emiratesExpiryDate.
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? emiratesExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the strpassExpiryDate.
        /// </summary>
        public string strpassExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the mobile.
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Gets or sets the visitor email.......
        /// </summary>
        public string VisitorEmail { get; set; }

        /// <summary>
        /// Gets or sets the SeniorManagerEmail
        /// Gets or Sets the senior manager email.......
        /// </summary>
        public string SeniorManagerEmail { get; set; }

        /// <summary>
        /// Gets or sets the link URL.......
        /// </summary>
        public string LinkURL { get; set; }

        /// <summary>
        /// Gets or sets the linkExpiryDate.
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? linkExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the smsLimit.
        /// </summary>
        public int smsLimit { get; set; }

        /// <summary>
        /// Gets or sets the emailLimit.
        /// </summary>
        public int emailLimit { get; set; }

        /// <summary>
        /// Gets or sets the downloadLimit.
        /// </summary>
        public int downloadLimit { get; set; }

        /// <summary>
        /// Gets or sets the passAttachements.
        /// </summary>
        public List<SecurityPassAttachement> passAttachements { get; set; }

        /// <summary>
        /// Gets or sets the coor_user.
        /// </summary>
        public string coor_user { get; set; }

        /// <summary>
        /// Gets or sets the security_user.
        /// </summary>
        public string security_user { get; set; }

        /// <summary>
        /// Gets or sets the pendingwith.
        /// </summary>
        public string pendingwith { get; set; }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        public List<string> Location { get; set; }
        public List<Grouppasslocationlistres> WpLocation { get; set; }
        public List<string> SeletecedLocation { get; set; }

        /// <summary>
        /// Gets or sets the RejectRemarks.
        /// </summary>
        public string RejectRemarks { get; set; }
        public string Remarks { get; set; }
        public string Purpose { get; set; }

        /// <summary>
        /// Gets or sets the Subcontractor.
        /// </summary>
        public string Subcontractor { get; set; }

        /// <summary>
        /// Gets or sets the profilepic.
        /// </summary>
        public string profilepic { get; set; }

        /// <summary>
        /// Gets or sets the fromTime.
        /// </summary>
        public string fromTime { get; set; }

        /// <summary>
        /// Gets or sets the toTime.
        /// </summary>
        public string toTime { get; set; }

        /// <summary>
        /// Gets or sets the projectName.
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// Gets or sets the projectId.
        /// </summary>
        public string projectId { get; set; }

        /// <summary>
        /// Gets or sets the wpprojectcoordinatorname.
        /// </summary>
        public string wpprojectcoordinatorname { get; set; }

        /// <summary>
        /// Gets or sets the wpprojectcoordinatoremail.
        /// </summary>
        public string wpprojectcoordinatoremail { get; set; }

        /// <summary>
        /// Gets or sets the wpprojectcoordinatormobile.
        /// </summary>
        public string wpprojectcoordinatormobile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enablerenewbutton.
        /// </summary>
        public bool enablerenewbutton { get; set; }

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
        /// Gets or sets the companyName.
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsBlocked
        /// Gets or sets the IsBlocked.......
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsUserBlocked
        /// Gets or sets the IsUserBlocked.......
        /// </summary>
        public bool IsUserBlocked { get; set; }

        /// <summary>
        /// Gets or sets the Errormessage.
        /// </summary>
        public string Errormessage { get; set; }

        /// <summary>
        /// Gets or sets the Designation
        /// Designation or Position......
        /// </summary>
        public string Designation { get; set; }

        /// <summary>
        /// Gets or sets the SubPasses.
        /// </summary>
        public List<SubPasses.SubpassDetails> SubPasses { get; set; }

        /// <summary>
        /// Gets or sets the MySubPasses.
        /// </summary>
        public List<BasePassViewModel> MySubPasses { get; set; }

        public bool isPassportattachment { get; set; }
        public bool isVisaattachment { get; set; }
        public bool isTrafficfilecodeattachment { get; set; }
        public bool isDrivingLicenseattachment { get; set; }
        public bool isEidattachment { get; set; }
        /// <summary>
        /// Gets or sets the DEWAID.
        /// </summary>
        public string DEWAID { get; set; }

        /// <summary>
        /// Gets or sets the DEWAdesignation.
        /// </summary>
        public string DEWAdesignation { get; set; }

        /// <summary>
        /// Gets or sets the VehicleRegNumber.
        /// </summary>
        public string VehicleRegNumber { get; set; }

        /// <summary>
        /// Gets or sets the strpassVehicleRegDate.
        /// </summary>
        public string strpassVehicleRegDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? VehRegistrationDate { get; set; }

        /// <summary>
        /// Gets or sets the Blockcomments.
        /// </summary>
        public string Blockcomments { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="SecurityPassAttachement" />.
    /// </summary>
    public class SecurityPassAttachement
    {
        /// <summary>
        /// Gets or sets the dms_objectId.
        /// </summary>
        public string dms_objectId { get; set; }

        /// <summary>
        /// Gets or sets the docType.
        /// </summary>
        public string docType { get; set; }

        /// <summary>
        /// Gets or sets the fileName.
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// Gets or sets the fileCategory.
        /// </summary>
        public string fileCategory { get; set; }

        /// <summary>
        /// Gets or sets the fileContent.
        /// </summary>
        public string fileContent { get; set; }

        /// <summary>
        /// Gets or sets the fileContentType.
        /// </summary>
        public string fileContentType { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="SecurityApproveRejectPassViewModel" />.
    /// </summary>
    public class SecurityApproveRejectPassViewModel
    {
        /// <summary>
        /// Gets or sets the eFolderID.
        /// </summary>
        public string eFolderID { get; set; }

        /// <summary>
        /// Gets or sets the PassNumber.
        /// </summary>
        public string PassNumber { get; set; }

        /// <summary>
        /// Gets or sets the visaExpiryDate.
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime? visaExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the PassIssueDate.
        /// </summary>
        public DateTime? PassIssueDate { get; set; }

        /// <summary>
        /// Gets or sets the PassExpiryDate.
        /// </summary>
        public DateTime? PassExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the PassDays.
        /// </summary>
        public int PassDays { get; set; }

        /// <summary>
        /// Gets or sets the FromTime.
        /// </summary>
        public string FromTime { get; set; }

        /// <summary>
        /// Gets or sets the ToTime.
        /// </summary>
        public string ToTime { get; set; }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        public List<string> Location { get; set; }

        /// <summary>
        /// Gets or sets the Comments.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the PassType
        /// Get or sets pass type.......
        /// </summary>
        public string PassType { get; set; }

        /// <summary>
        /// Gets or sets the PassType
        /// Get or sets pass type.......
        /// </summary>
        public string PassStatus { get; set; }

        /// <summary>
        /// Gets or sets the SelectedLocation
        /// selected locations.......
        /// </summary>
        public List<string> SelectedLocation { get; set; }

        /// <summary>
        /// Gets or sets the StrLocation.
        /// </summary>
        public string StrLocation { get; set; }

        /// <summary>
        /// Gets or sets the ApprovalType.
        /// </summary>
        public string ApprovalType { get; set; }

        /// <summary>
        /// Gets or sets the OfficeLocations
        /// This method is added to get locations from sitecore instead of eform,
        /// Also, this will replace Location property once all bindings will be updated..
        /// </summary>
        public List<System.Web.Mvc.SelectListItem> OfficeLocations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsOneDayPass.
        /// </summary>
        public bool IsOneDayPass { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="SecurityBlockedUserViewModel" />.
    /// </summary>
    public class SecurityBlockedUserViewModel
    {
        /// <summary>
        /// Gets or sets the eFolderId.
        /// </summary>
        public string eFolderId { get; set; }

        /// <summary>
        /// Gets or sets the passNo.
        /// </summary>
        public string passNo { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the emiratesID.
        /// </summary>
        public string emiratesID { get; set; }

        /// <summary>
        /// Gets or sets the visaNumber.
        /// </summary>
        public string visaNumber { get; set; }

        /// <summary>
        /// Gets or sets the passportNumber.
        /// </summary>
        public string passportNumber { get; set; }

        /// <summary>
        /// Gets or sets the isBlocked.
        /// </summary>
        public string isBlocked { get; set; }

        /// <summary>
        /// Gets or sets the efid.
        /// </summary>
        public string efid { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string status { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="SecurityPassWorkLogModel" />.
    /// </summary>
    public class SecurityPassWorkLogModel
    {
        /// <summary>
        /// Gets or sets the PassNo.
        /// </summary>
        public string PassNo { get; set; }

        /// <summary>
        /// Gets or sets the CheckIn.
        /// </summary>
        public DateTime? CheckIn { get; set; }

        /// <summary>
        /// Gets or sets the CheckOut.
        /// </summary>
        public DateTime? CheckOut { get; set; }

        /// <summary>
        /// Gets or sets the AccessDate.
        /// </summary>
        public DateTime? AccessDate { get; set; }

        /// <summary>
        /// Gets or sets the Hours.
        /// </summary>
        public double Hours { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="SecurityPassWorkLogFilterModel" />.
    /// </summary>
    public class SecurityPassWorkLogFilterModel
    {
        /// <summary>
        /// Gets or sets the ListWorkLogs.
        /// </summary>
        public List<SecurityPassWorkLogModel> ListWorkLogs { get; set; }

        /// <summary>
        /// Gets or sets the totalpage.
        /// </summary>
        public int totalpage { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pagination.
        /// </summary>
        public bool pagination { get; set; }

        /// <summary>
        /// Gets or sets the pagenumbers.
        /// </summary>
        public IEnumerable<int> pagenumbers { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="SecurityPassActivityLogModel" />.
    /// </summary>
    public class SecurityPassActivityLogModel
    {
        /// <summary>
        /// Gets or sets the eventId.
        /// </summary>
        public string eventId { get; set; }

        /// <summary>
        /// Gets or sets the activityName.
        /// </summary>
        public string activityName { get; set; }

        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        public string activity { get; set; }

        /// <summary>
        /// Gets or sets the eventTime.
        /// </summary>
        public DateTime eventTime { get; set; }

        /// <summary>
        /// Gets or sets the Notes.
        /// </summary>
        public string Notes { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="SecurityPassFilterViewModel" />.
    /// </summary>
    public class SecurityPassFilterViewModel
    {
        /// <summary>
        /// Gets or sets the lstpasses.
        /// </summary>
        public string lstpasses { get; set; }

        /// <summary>
        /// Gets or sets the lstsubcontractors.
        /// </summary>
        public string lstsubcontractors { get; set; }

        /// <summary>
        /// Gets or sets the totalpage.
        /// </summary>
        public int totalpage { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pagination.
        /// </summary>
        public bool pagination { get; set; }

        /// <summary>
        /// Gets or sets the pagenumbers.
        /// </summary>
        public IEnumerable<int> pagenumbers { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        public string keywords { get; set; }

        /// <summary>
        /// Gets or sets the namesort.
        /// </summary>
        public string namesort { get; set; }

        /// <summary>
        /// Gets or sets the passtypefilter.
        /// </summary>
        public string passtypefilter { get; set; }

        /// <summary>
        /// Gets or sets the strdataindex.
        /// </summary>
        public string strdataindex { get; set; }
    }

    /// <summary>
    /// Defines the SecurityPassStatus
    /// </summary>
    public enum SecurityPassStatus
    {
        /// <summary>
        /// Defines the PendingApprovalwithCoordinator
        /// </summary>
        PendingApprovalwithCoordinator = 0,
        /// <summary>
        /// Defines the PendingApprovalwithSecurity
        /// </summary>
        PendingApprovalwithSecurity = 1,
        /// <summary>
        /// Defines the Expired
        /// </summary>
        Expired = 2,
        /// <summary>
        /// Defines the SoontoExpire
        /// </summary>
        SoontoExpire = 3,
        /// <summary>
        /// Defines the Active
        /// </summary>
        Active = 4,
        /// <summary>
        /// Defines the Rejected
        /// </summary>
        Rejected = 5,
        /// <summary>
        /// Defines the Notapplicable
        /// </summary>
        Notapplicable = 6,
        /// <summary>
        /// Defines the Cancelled
        /// </summary>
        Cancelled = 7,
        /// <summary>
        /// Defines the Blocked
        /// </summary>
        Blocked = 8,
        /// <summary>
        /// Defines one day pass initiated
        /// </summary>
        Initiated = 9,
        /// <summary>
        /// Defines the UnderApprovalinWorkPermit.
        /// </summary>
        UnderApprovalinWorkPermit = 10,
        /// <summary>
        /// Defines the Approved.
        /// </summary>
        Approved = 11,
    }

    /// <summary>
    /// Defines the EntryStatus
    /// </summary>
    public enum EntryStatus
    {
        /// <summary>
        /// Defines the Checkin
        /// </summary>
        Checkin = 0,
        /// <summary>
        /// Defines the Checkout
        /// </summary>
        Checkout = 1
    }

    /// <summary>
    /// Defines the PassType.
    /// </summary>
    public enum PassType
    {
        /// <summary>
        /// Defines the Single.
        /// </summary>
        [Display(Name = "Single")]
        Single = 0,
        /// <summary>
        /// Defines the LongTerm.
        /// </summary>
        [Display(Name = "Long Term")]
        LongTerm = 1,
        /// <summary>
        /// Defines the ShortTerm.
        /// </summary>
        [Display(Name = "Short Term")]
        ShortTerm = 2,
        /// <summary>
        /// Defines the OnedayPass.
        /// </summary>
        [Display(Name = "OnedayPass")]
        OnedayPass = 3
    }

    /// <summary>
    /// Defines the PassRole.
    /// </summary>
    public enum PassRole
    {
        /// <summary>
        /// Defines the Individual.
        /// </summary>
        [Display(Name = "Individual")]
        Individual = 0,
        /// <summary>
        /// Defines the Dptmgr.
        /// </summary>
        [Display(Name = "Dptmgr")]
        Dptmgr = 1,
        /// <summary>
        /// Defines the SecurityAdmin.
        /// </summary>
        [Display(Name = "SecurityAdmin")]
        SecurityAdmin = 2,
    }
}
