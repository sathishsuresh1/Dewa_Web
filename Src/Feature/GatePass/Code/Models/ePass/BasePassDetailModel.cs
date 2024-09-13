// <copyright file="BasePassDetailModel.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Models.ePass
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="BasePassDetailModel" />.
    /// </summary>
    public class BasePassDetailModel
    {
        /// <summary>
        /// Gets or sets the MainPass.
        /// </summary>
        public List<BaseMainPassDetailModel> MainPass { get; set; }

        /// <summary>
        /// Gets or sets the SubPasses.
        /// </summary>
        public List<BaseSubPassDetailModel> SubPasses { get; set; }

        /// <summary>
        /// Gets or sets the Attachments.
        /// </summary>
        public List<BasePassDetailAttachment> Attachments { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="BaseMainPassDetailModel" />.
    /// </summary>
    public class BaseMainPassDetailModel
    {
        /// <summary>
        /// Gets or sets the ePassCreatedBy.
        /// </summary>
        public string ePassCreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the ePassCreatedOn.
        /// </summary>
        public string ePassCreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the ePassVehicleRegDate.
        /// </summary>
        public string ePassVehicleRegDate { get; set; }

        /// <summary>
        /// Gets or sets the ePassVehicleNo.
        /// </summary>
        public string ePassVehicleNo { get; set; }

        /// <summary>
        /// Gets or sets the ePassVisitingTimeTo.
        /// </summary>
        public string ePassVisitingTimeTo { get; set; }

        /// <summary>
        /// Gets or sets the ePassVistingTimeFrom.
        /// </summary>
        public string ePassVistingTimeFrom { get; set; }

        /// <summary>
        /// Gets or sets the ePassVisitingDate.
        /// </summary>
        public string ePassVisitingDate { get; set; }

        /// <summary>
        /// Gets or sets the ePassJobTiltle.
        /// </summary>
        public string ePassJobTiltle { get; set; }

        /// <summary>
        /// Gets or sets the ePassDEWAID.
        /// </summary>
        public string ePassDEWAID { get; set; }

        /// <summary>
        /// Gets or sets the ePassVisitorEmailID.
        /// </summary>
        public string ePassVisitorEmailID { get; set; }

        /// <summary>
        /// Gets or sets the ePassLocation.
        /// </summary>
        public string ePassLocation { get; set; }

        /// <summary>
        /// Gets or sets the ePassDepartmentName.
        /// </summary>
        public string ePassDepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the ePassProjectStatus.
        /// </summary>
        public string ePassProjectStatus { get; set; }

        /// <summary>
        /// Gets or sets the ePassProjectID.
        /// </summary>
        public string ePassProjectID { get; set; }

        /// <summary>
        /// Gets or sets the ePassIsBlocked.
        /// </summary>
        public string ePassIsBlocked { get; set; }

        /// <summary>
        /// Gets or sets the ePassToDateTime.
        /// </summary>
        public string ePassToDateTime { get; set; }

        /// <summary>
        /// Gets or sets the ePassFromDateTime.
        /// </summary>
        public string ePassFromDateTime { get; set; }

        /// <summary>
        /// Gets or sets the ePassPassExpiryDate.
        /// </summary>
        public string ePassPassExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the ePassPassIssueDate.
        /// </summary>
        public string ePassPassIssueDate { get; set; }

        /// <summary>
        /// Gets or sets the ePassProjectEndDate.
        /// </summary>
        public string ePassProjectEndDate { get; set; }

        /// <summary>
        /// Gets or sets the ePassProjectStartName.
        /// </summary>
        public string ePassProjectStartName { get; set; }

        /// <summary>
        /// Gets or sets the ePassCompanyName.
        /// </summary>
        public string ePassCompanyName { get; set; }

        /// <summary>
        /// Gets or sets the ePassProjectName.
        /// </summary>
        public string ePassProjectName { get; set; }

        /// <summary>
        /// Gets or sets the ePassPONumber.
        /// </summary>
        public string ePassPONumber { get; set; }

        /// <summary>
        /// Gets or sets the ePassDownloadLimit.
        /// </summary>
        public string ePassDownloadLimit { get; set; }

        /// <summary>
        /// Gets or sets the ePassEmailLimit.
        /// </summary>
        public string ePassEmailLimit { get; set; }

        /// <summary>
        /// Gets or sets the ePassSMSLimit.
        /// </summary>
        public string ePassSMSLimit { get; set; }

        /// <summary>
        /// Gets or sets the ePassEmailAddress.
        /// </summary>
        public string ePassEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the ePassSecurityApprovers.
        /// </summary>
        public string ePassSecurityApprovers { get; set; }

        /// <summary>
        /// Gets or sets the ePassMobileNumber.
        /// </summary>
        public string ePassMobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the ePassSubmitterEmail.
        /// </summary>
        public string ePassSubmitterEmail { get; set; }

        /// <summary>
        /// Gets or sets the ePassProjectDeptApprovers.
        /// </summary>
        public string ePassProjectDeptApprovers { get; set; }

        /// <summary>
        /// Gets or sets the ePassCoordinatorNames.
        /// </summary>
        public string ePassCoordinatorNames { get; set; }

        /// <summary>
        /// Gets or sets the ePassVendorID.
        /// </summary>
        public string ePassVendorID { get; set; }

        /// <summary>
        /// Gets or sets the ePassSubContractorID.
        /// </summary>
        public string ePassSubContractorID { get; set; }

        /// <summary>
        /// Gets or sets the ePassPassportExpiryDate.
        /// </summary>
        public string ePassPassportExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the ePassPassportNumber.
        /// </summary>
        public string ePassPassportNumber { get; set; }

        /// <summary>
        /// Gets or sets the ePassVisaExpiryDate.
        /// </summary>
        public string ePassVisaExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the ePassVisaNumber.
        /// </summary>
        public string ePassVisaNumber { get; set; }

        /// <summary>
        /// Gets or sets the ePassEmiratesiDExpiry.
        /// </summary>
        public string ePassEmiratesiDExpiry { get; set; }

        /// <summary>
        /// Gets or sets the ePassEmiratesID.
        /// </summary>
        public string ePassEmiratesID { get; set; }

        /// <summary>
        /// Gets or sets the ePassDesignation.
        /// </summary>
        public string ePassDesignation { get; set; }

        /// <summary>
        /// Gets or sets the ePassProfession.
        /// </summary>
        public string ePassProfession { get; set; }

        /// <summary>
        /// Gets or sets the ePassNationality.
        /// </summary>
        public string ePassNationality { get; set; }

        /// <summary>
        /// Gets or sets the ePassVisitorName.
        /// </summary>
        public string ePassVisitorName { get; set; }

        /// <summary>
        /// Gets or sets the ePassPassStatus.
        /// </summary>
        public string ePassPassStatus { get; set; }

        /// <summary>
        /// Gets or sets the ePassPassType.
        /// </summary>
        public string ePassPassType { get; set; }

        /// <summary>
        /// Gets or sets the ePassPassNo.
        /// </summary>
        public string ePassPassNo { get; set; }

        /// <summary>
        /// Gets or sets the ePassLinkURL.
        /// </summary>
        public string ePassLinkURL { get; set; }

        /// <summary>
        /// Gets or sets the ePassLinkExpiry.
        /// </summary>
        public string ePassLinkExpiry { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="BaseSubPassDetailModel" />.
    /// </summary>
    public class BaseSubPassDetailModel
    {
        /// <summary>
        /// Gets or sets the subPassVisitorEmail.
        /// </summary>
        public string subPassVisitorEmail { get; set; }

        /// <summary>
        /// Gets or sets the subPassDeptApprovalDate.
        /// </summary>
        public string subPassDeptApprovalDate { get; set; }

        /// <summary>
        /// Gets or sets the subPassSecApprovedBy.
        /// </summary>
        public string subPassSecApprovedBy { get; set; }

        /// <summary>
        /// Gets or sets the subPassSecurityApprovers.
        /// </summary>
        public string subPassSecurityApprovers { get; set; }

        /// <summary>
        /// Gets or sets the subPassDepartmentApprovalRemarks.
        /// </summary>
        public string subPassDepartmentApprovalRemarks { get; set; }

        /// <summary>
        /// Gets or sets the subPassSecurityApprovalRemarks.
        /// </summary>
        public string subPassSecurityApprovalRemarks { get; set; }

        /// <summary>
        /// Gets or sets the subPassValidTo.
        /// </summary>
        public string subPassValidTo { get; set; }

        /// <summary>
        /// Gets or sets the subPassDepartmentApprover.
        /// </summary>
        public string subPassDepartmentApprover { get; set; }

        /// <summary>
        /// Gets or sets the subPassSecApprovalDate.
        /// </summary>
        public string subPassSecApprovalDate { get; set; }

        /// <summary>
        /// Gets or sets the subPassMainPassValidity.
        /// </summary>
        public string subPassMainPassValidity { get; set; }

        /// <summary>
        /// Gets or sets the subPassValidFrom.
        /// </summary>
        public string subPassValidFrom { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the subPassCreatedOn.
        /// </summary>
        public string subPassCreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the subPassJustification.
        /// </summary>
        public string subPassJustification { get; set; }

        /// <summary>
        /// Gets or sets the subPassDeptApprovedBy.
        /// </summary>
        public string subPassDeptApprovedBy { get; set; }

        /// <summary>
        /// Gets or sets the subPassDeviceModel.
        /// </summary>
        public string subPassDeviceModel { get; set; }

        /// <summary>
        /// Gets or sets the subPassCreatedByEmail.
        /// </summary>
        public string subPassCreatedByEmail { get; set; }

        /// <summary>
        /// Gets or sets the subPassDeviceSerialNo.
        /// </summary>
        public string subPassDeviceSerialNo { get; set; }

        /// <summary>
        /// Gets or sets the subPassDeviceType.
        /// </summary>
        public string subPassDeviceType { get; set; }

        /// <summary>
        /// Gets or sets the subPassStatus.
        /// </summary>
        public string subPassStatus { get; set; }

        /// <summary>
        /// Gets or sets the subPassNewPassType.
        /// </summary>
        public string subPassNewPassType { get; set; }

        /// <summary>
        /// Gets or sets the subPassRequestID.
        /// </summary>
        public string subPassRequestID { get; set; }

        /// <summary>
        /// Gets or sets the subPassDeviceImage.
        /// </summary>
        public string subPassDeviceImage { get; set; }

        /// <summary>
        /// Gets or sets the subPassMainPassNo.
        /// </summary>
        public string subPassMainPassNo { get; set; }

        /// <summary>
        /// Gets or sets the subPassLocation.
        /// </summary>
        public string subPassLocation { get; set; }

        /// <summary>
        /// Gets or sets the subPassDeliveryNoteNo.
        /// </summary>
        public string subPassDeliveryNoteNo { get; set; }

        /// <summary>
        /// Gets or sets the subPassLPONo.
        /// </summary>
        public string subPassLPONo { get; set; }

        /// <summary>
        /// Gets or sets the subPassMaterialPassType.
        /// </summary>
        public string subPassMaterialPassType { get; set; }

        /// <summary>
        /// Gets or sets the subPassMaterialList.
        /// </summary>
        public MaterialList[] subPassMaterialList { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="BasePassDetailAttachment" />.
    /// </summary>
    public class BasePassDetailAttachment
    {
        /// <summary>
        /// Gets or sets the MainPassNo.
        /// </summary>
        public string MainPassNo { get; set; }

        /// <summary>
        /// Gets or sets the ReqID.
        /// </summary>
        public string ReqID { get; set; }

        /// <summary>
        /// Gets or sets the SupportingFile.
        /// </summary>
        public string SupportingFile { get; set; }

        /// <summary>
        /// Gets or sets the FileName.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the FileType.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets the FileExtension.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the FileContentType.
        /// </summary>
        public string FileContentType { get; set; }

        /// <summary>
        /// Gets or sets the FileCategory.
        /// </summary>
        public string FileCategory { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="KofaxSubContractor" />.
    /// </summary>
    public class KofaxSubContractor
    {
        /// <summary>
        /// Gets or sets the VendorID.
        /// </summary>
        public string VendorID { get; set; }

        /// <summary>
        /// Gets or sets the VendorName.
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// Gets or sets the Address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the POBOX.
        /// </summary>
        public string POBOX { get; set; }

        /// <summary>
        /// Gets or sets the SubContractID.
        /// </summary>
        public int SubContractID { get; set; }

        /// <summary>
        /// Gets or sets the SubcontractName.
        /// </summary>
        public string SubcontractName { get; set; }

        /// <summary>
        /// Gets or sets the TelephoneNumber.
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the TradeLicenseNumber.
        /// </summary>
        public string TradeLicenseNumber { get; set; }

        /// <summary>
        /// Gets or sets the TradeLicenseIssueDate.
        /// </summary>
        public string TradeLicenseIssueDate { get; set; }

        /// <summary>
        /// Gets or sets the TradeLicenseExpiryDate.
        /// </summary>
        public string TradeLicenseExpiryDate { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="MaterialList" />.
    /// </summary>
    public class MaterialList
    {
        /// <summary>
        /// Gets or sets the SubpassNo.
        /// </summary>
        public string SubpassNo { get; set; }

        /// <summary>
        /// Gets or sets the MaterialName.
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the SerialNo.
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// Gets or sets the Quantity.
        /// </summary>
        public string Quantity { get; set; }
    }

    public class KofaxePassBlockedUsers
    {
        /// <summary>
        /// Gets or sets the VendorID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the VendorName.
        /// </summary>
        public string PassNo { get; set; }

        /// <summary>
        /// Gets or sets the Address.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the POBOX.
        /// </summary>
        public string EmiratesID { get; set; }

        /// <summary>
        /// Gets or sets the SubContractID.
        /// </summary>
        public string VisaNo { get; set; }

        /// <summary>
        /// Gets or sets the SubcontractName.
        /// </summary>
        public string PassportNo { get; set; }

        /// <summary>
        /// Gets or sets the TelephoneNumber.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the TradeLicenseNumber.
        /// </summary>
        public string StatusDate { get; set; }

        /// <summary>
        /// Gets or sets the TradeLicenseIssueDate.
        /// </summary>
        public string StatusComments { get; set; }

        /// <summary>
        /// Gets or sets the TradeLicenseExpiryDate.
        /// </summary>
        public string BlockedBy { get; set; }
    }

}
