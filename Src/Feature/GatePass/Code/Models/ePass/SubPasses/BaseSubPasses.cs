// <copyright file="BaseSubPasses.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Models.ePass.SubPasses
{
    using System.Web;

    /// <summary>
    /// Defines the <see cref="BaseSubPass" />.
    /// </summary>
    public class BaseSubPass
    {
        /// <summary>
        /// Gets or sets the PassNumber.
        /// </summary>
        public string PassNumber { get; set; }

        /// <summary>
        /// Gets or sets the SubPassNumber.
        /// </summary>
        public string SubPassNumber { get; set; }

        /// <summary>
        /// Gets or sets the PassType.
        /// </summary>
        public SubPassType PassType { get; set; }

        /// <summary>
        /// Gets or sets the StartDate.
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Gets or sets the FromTime.
        /// </summary>
        public string FromTime { get; set; }

        /// <summary>
        /// Gets or sets the EndDate.
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Gets or sets the ToTime.
        /// </summary>
        public string ToTime { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the ManagerEmail.
        /// </summary>
        public string ManagerEmail { get; set; }

        /// <summary>
        /// Gets or sets the SecurityEmail.
        /// </summary>
        public string SecurityEmail { get; set; }

        /// <summary>
        /// Gets or sets the RequesterEmail.
        /// </summary>
        public string RequesterEmail { get; set; }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        public string Location { get; set; }
    }

    /// <summary>
    /// Defines the SubPassType.
    /// </summary>
    public enum SubPassType
    {
        /// <summary>
        /// Defines the Night.
        /// </summary>
        Night = 0,
        /// <summary>
        /// Defines the Holiday.
        /// </summary>
        Holiday = 1,
        /// <summary>
        /// Defines the Device.
        /// </summary>
        Device = 2,
        /// <summary>
        /// Defines the Material.
        /// </summary>
        Material = 3
    }

    /// <summary>
    /// Defines the <see cref="ElectronicPass" />.
    /// </summary>
    public class ElectronicPass : BaseSubPass
    {
        /// <summary>
        /// Gets or sets the NameoftheDevice.
        /// </summary>
        public string NameoftheDevice { get; set; }

        /// <summary>
        /// Gets or sets the ModelName.
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Gets or sets the SerialNumber.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the Purpose.
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// Gets or sets the DevicePic.
        /// </summary>
        public HttpPostedFileBase DevicePic { get; set; }

        /// <summary>
        /// Gets or sets the DevicePicbytes.
        /// </summary>
        public byte[] DevicePicbytes { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="MaterialPass" />.
    /// </summary>
    public class MaterialPass : BaseSubPass
    {
        /// <summary>
        /// Gets or sets the Contractorname.
        /// </summary>
        public string Contractorname { get; set; }

        /// <summary>
        /// Gets or sets the ProjectDetails.
        /// </summary>
        public string ProjectDetails { get; set; }

        /// <summary>
        /// Gets or sets the MaterialInformation.
        /// </summary>
        public string MaterialInformation { get; set; }

        /// <summary>
        /// Gets or sets the DeliveryNoteNumber.
        /// </summary>
        public string DeliveryNoteNumber { get; set; }

        /// <summary>
        /// Gets or sets the Lponumber.
        /// </summary>
        public string Lponumber { get; set; }

        /// <summary>
        /// Gets or sets the MaterialMode.
        /// </summary>
        public string MaterialMode { get; set; }

        /// <summary>
        /// Gets or sets the Inwardmaterialcategory.
        /// </summary>
        public string Inwardmaterialcategory { get; set; }

        /// <summary>
        /// Gets or sets the storelocation.
        /// </summary>
        public string Storelocation { get; set; }

        /// <summary>
        /// Gets or sets the LPOAttachment.
        /// </summary>
        public HttpPostedFileBase LPOAttachment { get; set; }

        /// <summary>
        /// Gets or sets the LPOAttachmentbytes.
        /// </summary>
        public byte[] LPOAttachmentbytes { get; set; }

        /// <summary>
        /// Gets or sets the DeliveryNoteattachment.
        /// </summary>
        public HttpPostedFileBase DeliveryNoteattachment { get; set; }

        /// <summary>
        /// Gets or sets the DeliveryNoteattachmentbytes.
        /// </summary>
        public byte[] DeliveryNoteattachmentbytes { get; set; }

        /// <summary>
        /// Gets or sets the ContractorSiteAttachment.
        /// </summary>
        public HttpPostedFileBase ContractorSiteAttachment { get; set; }

        /// <summary>
        /// Gets or sets the ContractorSiteAttachmentbytes.
        /// </summary>
        public byte[] ContractorSiteAttachmentbytes { get; set; }
    }
}
