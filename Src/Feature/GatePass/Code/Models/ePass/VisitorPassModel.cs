// <copyright file="VisitorPassModel.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Models.ePass
{
    using DEWAXP.Foundation.DataAnnotations;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Defines the <see cref="VisitorPassModel" />.
    /// </summary>
    public class VisitorPassModel
    {
        /// <summary>
        /// Gets or sets the VisitorEmailid.
        /// </summary>
        public string VisitorEmailid { get; set; }

        /// <summary>
        /// Gets or sets the Visitorname.
        /// </summary>
        public string Visitorname { get; set; }

        /// <summary>
        /// Gets or sets the Projectname.
        /// </summary>
        public string Projectname { get; set; }

        /// <summary>
        /// Gets or sets the ProjectID.
        /// </summary>
        public string ProjectID { get; set; }

        /// <summary>
        /// Gets or sets the Designation.
        /// </summary>
        public string Designation { get; set; }

        /// <summary>
        /// Gets or sets the MobileNumber.
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the Nationality.
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        public IEnumerable<string> Location { get; set; }

        /// <summary>
        /// Gets or sets the LocationList.
        /// </summary>
        public List<SelectListItem> LocationList { get; set; }

        /// <summary>
        /// Gets or sets the Entrydate.
        /// </summary>
        public string Entrydate { get; set; }

        /// <summary>
        /// Gets or sets the Entryintime.
        /// </summary>
        public string Entryintime { get; set; }

        /// <summary>
        /// Gets or sets the Entryouttime.
        /// </summary>
        public string Entryouttime { get; set; }

        /// <summary>
        /// Gets or sets the EmiratesID.
        /// </summary>
        public string EmiratesID { get; set; }

        /// <summary>
        /// Gets or sets the EmiratesIDExpirydate.
        /// </summary>
        public string EmiratesIDExpirydate { get; set; }

        /// <summary>
        /// Gets or sets the Passport.
        /// </summary>
        public string Passport { get; set; }

        /// <summary>
        /// Gets or sets the PassportExpirydate.
        /// </summary>
        public string PassportExpirydate { get; set; }

        /// <summary>
        /// Gets or sets the VisaNumber.
        /// </summary>
        public string VisaNumber { get; set; }

        /// <summary>
        /// Gets or sets the VisaNumberExpirydate.
        /// </summary>
        public string VisaNumberExpirydate { get; set; }

        /// <summary>
        /// Gets or sets the EmiratesIDCard_attachment.
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase EmiratesIDCard_attachment { get; set; }

        /// <summary>
        /// Gets or sets the EmiratesIDCard_attachmentName.
        /// </summary>
        public string EmiratesIDCard_attachmentName { get; set; }

        /// <summary>
        /// Gets or sets the EmiratesIDCard_contentType.
        /// </summary>
        public string EmiratesIDCard_contentType { get; set; }

        /// <summary>
        /// Gets or sets the EmiratesIDCard_fileExtension.
        /// </summary>
        public string EmiratesIDCard_fileExtension { get; set; }

        /// <summary>
        /// Gets or sets the EmiratesIDCard_filetype.
        /// </summary>
        public string EmiratesIDCard_filetype { get; set; }

        /// <summary>
        /// Gets or sets the EmiratesIDCard_attachmentData.
        /// </summary>
        public byte[] EmiratesIDCard_attachmentData { get; set; }

        /// <summary>
        /// Gets or sets the EmiratesIDCard_attachmentOrigSize.
        /// </summary>
        public int EmiratesIDCard_attachmentOrigSize { get; set; }

        /// <summary>
        /// Gets or sets the PassportCard_attachment.
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase PassportCard_attachment { get; set; }

        /// <summary>
        /// Gets or sets the PassportCard_attachmentName.
        /// </summary>
        public string PassportCard_attachmentName { get; set; }

        /// <summary>
        /// Gets or sets the PassportCard_contentType.
        /// </summary>
        public string PassportCard_contentType { get; set; }

        /// <summary>
        /// Gets or sets the PassportCard_fileExtension.
        /// </summary>
        public string PassportCard_fileExtension { get; set; }

        /// <summary>
        /// Gets or sets the PassportCard_filetype.
        /// </summary>
        public string PassportCard_filetype { get; set; }

        /// <summary>
        /// Gets or sets the PassportCard_attachmentData.
        /// </summary>
        public byte[] PassportCard_attachmentData { get; set; }

        /// <summary>
        /// Gets or sets the PassportCard_attachmentOrigSize.
        /// </summary>
        public int PassportCard_attachmentOrigSize { get; set; }

        /// <summary>
        /// Gets or sets the VisaNumberCard_attachment.
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase VisaNumberCard_attachment { get; set; }

        /// <summary>
        /// Gets or sets the VisaNumberCard_attachmentName.
        /// </summary>
        public string VisaNumberCard_attachmentName { get; set; }

        /// <summary>
        /// Gets or sets the VisaNumberCard_contentType.
        /// </summary>
        public string VisaNumberCard_contentType { get; set; }

        /// <summary>
        /// Gets or sets the VisaNumberCard_fileExtension.
        /// </summary>
        public string VisaNumberCard_fileExtension { get; set; }

        /// <summary>
        /// Gets or sets the VisaNumberCard_filetype.
        /// </summary>
        public string VisaNumberCard_filetype { get; set; }

        /// <summary>
        /// Gets or sets the VisaNumberCard_attachmentData.
        /// </summary>
        public byte[] VisaNumberCard_attachmentData { get; set; }

        /// <summary>
        /// Gets or sets the VisaNumberCard_attachmentOrigSize.
        /// </summary>
        public int VisaNumberCard_attachmentOrigSize { get; set; }

        /// <summary>
        /// Gets or sets the ApplicatePhoto_attachment.
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase ApplicatePhoto_attachment { get; set; }

        /// <summary>
        /// Gets or sets the ApplicatePhoto_attachmentName.
        /// </summary>
        public string ApplicatePhoto_attachmentName { get; set; }

        /// <summary>
        /// Gets or sets the ApplicatePhoto_contentType.
        /// </summary>
        public string ApplicatePhoto_contentType { get; set; }

        /// <summary>
        /// Gets or sets the ApplicatePhoto_fileExtension.
        /// </summary>
        public string ApplicatePhoto_fileExtension { get; set; }

        /// <summary>
        /// Gets or sets the ApplicatePhoto_filetype.
        /// </summary>
        public string ApplicatePhoto_filetype { get; set; }

        /// <summary>
        /// Gets or sets the ApplicatePhoto_attachmentData.
        /// </summary>
        public byte[] ApplicatePhoto_attachmentData { get; set; }

        /// <summary>
        /// Gets or sets the ApplicatePhoto_attachmentOrigSize.
        /// </summary>
        public int ApplicatePhoto_attachmentOrigSize { get; set; }

        /// <summary>
        /// Gets or sets the Entry_In.
        /// </summary>
        public string Entry_In { get; set; }

        /// <summary>
        /// Gets or sets the Exit_Out.
        /// </summary>
        public string Exit_Out { get; set; }

        /// <summary>
        /// Gets or sets the Comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the CardNo.
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// Gets or sets the Flag.
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// Gets or sets the ReferenceNumber.
        /// </summary>
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the WID.
        /// </summary>
        public string WID { get; set; }

        /// <summary>
        /// Gets or sets the GID.
        /// </summary>
        public string GID { get; set; }
    }
}
