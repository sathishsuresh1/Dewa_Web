using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.GatePass.Models.ePass
{
    public class BasePassViewModel
    {
        public string ePassCreatedBy { get; set; }
        public string ePassCreatedOn { get; set; }
        public string ePassVehicleRegDate { get; set; }
        public string ePassVehicleNo { get; set; }
        public string ePassVisitingTimeTo { get; set; }
        public string ePassVistingTimeFrom { get; set; }
        public string ePassVisitingDate { get; set; }
        public string ePassJobTiltle { get; set; }
        public string ePassDEWAID { get; set; }
        public string ePassVisitorEmailID { get; set; }
        public string ePassLocation { get; set; }
        public string ePassDepartmentName { get; set; }
        public string ePassProjectStatus { get; set; }
        public string ePassProjectID { get; set; }
        public string ePassIsBlocked { get; set; }
        public string ePassToDateTime { get; set; }
        public string ePassFromDateTime { get; set; }
        public string ePassPassExpiryDate { get; set; }
        public string ePassPassIssueDate { get; set; }
        public string ePassProjectEndDate { get; set; }
        public string ePassProjectStartName { get; set; }
        public string ePassCompanyName { get; set; }
        public string ePassProjectName { get; set; }
        public string ePassPONumber { get; set; }
        public string ePassDownloadLimit { get; set; }
        public string ePassEmailLimit { get; set; }
        public string ePassSMSLimit { get; set; }
        public string ePassEmailAddress { get; set; }
        public string ePassSecurityApprovers { get; set; }
        public string ePassMobileNumber { get; set; }
        public string ePassSubmitterEmail { get; set; }
        public string ePassProjectDeptApprovers { get; set; }
        public string ePassCoordinatorNames { get; set; }
        public string ePassVendorID { get; set; }
        public string ePassSubContractorID { get; set; }
        public string ePassPassportExpiryDate { get; set; }
        public string ePassPassportNumber { get; set; }
        public string ePassVisaExpiryDate { get; set; }
        public string ePassVisaNumber { get; set; }
        public string ePassEmiratesiDExpiry { get; set; }
        public string ePassEmiratesID { get; set; }
        public string ePassDesignation { get; set; }
        public string ePassProfession { get; set; }
        public string ePassNationality { get; set; }
        public string ePassVisitorName { get; set; }
        public string ePassPassStatus { get; set; }
        public string ePassPassType { get; set; }
        public string ePassPassNo { get; set; }
        public string ePassLinkURL { get; set; }
        public string ePassLinkExpiry { get; set; }

        public string subPassVisitorEmail { get; set; }
        public string subPassDeptApprovalDate { get; set; }
        public string subPassSecApprovedBy { get; set; }
        public string subPassSecurityApprovers { get; set; }
        public string subPassDepartmentApprovalRemarks { get; set; }
        public string subPassSecurityApprovalRemarks { get; set; }
        public string subPassValidTo { get; set; }
        public string subPassDepartmentApprover { get; set; }
        public string subPassSecApprovalDate { get; set; }
        public string subPassMainPassValidity { get; set; }
        public string subPassValidFrom { get; set; }
        public int ID { get; set; }
        public string subPassCreatedOn { get; set; }
        public string subPassJustification { get; set; }
        public string subPassDeptApprovedBy { get; set; }
        public string subPassDeviceModel { get; set; }
        public string subPassCreatedByEmail { get; set; }
        public string subPassDeviceSerialNo { get; set; }
        public string subPassDeviceType { get; set; }
        public string subPassStatus { get; set; }
        public string subPassNewPassType { get; set; }
        public string subPassRequestID { get; set; }
        public string subPassDeviceImage { get; set; }
        public string subPassMainPassNo { get; set; }
        public string subPassLocation { get; set; }
        public List<BasePassAttachment> Attachments { get; set; }
    }
    public class BasePassAttachment
    {
        public string ReqID { get; set; }
        public string SupportingFile { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
        public string FileContentType { get; set; }
        public string FileCategory { get; set; }
    }
}