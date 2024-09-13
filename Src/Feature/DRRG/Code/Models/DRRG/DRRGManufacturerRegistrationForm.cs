// <copyright file="DRRGManufacturerRegistrationForm.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    using DEWAXP.Foundation.DataAnnotations;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Defines the <see cref="ManufacturerRegistration" />
    /// </summary>
    public class ManufacturerRegistration
    {
        /// <summary>
        /// Defines the _manufacturerName
        /// </summary>
        private string _manufacturerName;

        /// <summary>
        /// Gets or sets the ManufacturerName
        /// </summary>
        public string ManufacturerName
        {
            get { return _manufacturerName; }
            set { _manufacturerName = value.Trim(); }
        }

        /// <summary>
        /// Gets or sets the Referencepersondetails
        /// </summary>
        public string Referencepersondetails { get; set; }

        /// <summary>
        /// Gets or sets the lstReferencepersondetails
        /// </summary>
        public List<ReferencePersonDetail> lstReferencepersondetails { get; set; }

        /// <summary>
        /// Gets or sets the Website
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets the Nationality
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// Gets or sets the Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the ReferenceAddress
        /// </summary>
        public string ReferenceAddress { get; set; }

        /// <summary>
        /// Gets or sets the objidEnvironmentalManagementSystem
        /// </summary>
        public string objidEnvironmentalManagementSystem { get; set; }

        /// <summary>
        /// Gets or sets the EnvironmentalManagementSystem
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase EnvironmentalManagementSystem { get; set; }

        /// <summary>
        /// Gets or sets the objidUploadQualityManagementSystem
        /// </summary>
        public string objidUploadQualityManagementSystem { get; set; }

        /// <summary>
        /// Gets or sets the UploadQualityManagementSystem
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase UploadQualityManagementSystem { get; set; }

        /// <summary>
        /// Gets or sets the objidDistributorAuthorizationletter
        /// </summary>
        public string objidDistributorAuthorizationletter { get; set; }

        /// <summary>
        /// Gets or sets the DistributorAuthorizationletter
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase DistributorAuthorizationletter { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ReferencePersonDetail" />
    /// </summary>
    public class ReferencePersonDetail
    {
        /// <summary>
        /// Defines the _email
        /// </summary>
        private string _email;

        /// <summary>
        /// Defines the _referenceperson
        /// </summary>
        private string _referenceperson;

        /// <summary>
        /// Gets or sets the ReferencePerson
        /// </summary>
        public string ReferencePerson
        {
            get { return _referenceperson; }
            set { _referenceperson = value.Trim(); }
        }

        /// <summary>
        /// Gets or sets the Email
        /// </summary>
        public string Email
        {
            get { return _email; }
            set { _email = value.Trim(); }
        }

        /// <summary>
        /// Gets or sets the MobileNumber
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PrimaryContact
        /// </summary>
        public bool PrimaryContact { get; set; }
    }
}
