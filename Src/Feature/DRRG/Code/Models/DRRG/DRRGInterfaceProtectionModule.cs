// <copyright file="DRRGInterfaceProtectionModule.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    using DEWAXP.Foundation.DataAnnotations;
    using System.Web;

    /// <summary>
    /// Defines the <see cref="DRRGInterfaceProtectionModule" />
    /// </summary>
    public class DRRGInterfaceProtectionModule
    {
        /// <summary>
        /// Gets or sets the Efolderid
        /// </summary>
        public string Efolderid { get; set; }

        /// <summary>
        /// Gets or sets the Manufactureruserid
        /// </summary>
        public string Manufactureruserid { get; set; }

        /// <summary>
        /// Gets or sets the ManufacturerName
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// Gets or sets the Reject_Remarks
        /// </summary>
        public string Reject_Remarks { get; set; }

        /// <summary>
        /// Gets or sets the ModelofInterfaceProtectionModule
        /// </summary>
        public string ModelofInterfaceProtectionModule { get; set; }

        /// <summary>
        /// Gets or sets the Application
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the CommunicationProtocol
        /// </summary>
        public string CommunicationProtocol { get; set; }

        /// <summary>
        /// Gets or sets the NationalGridConnection
        /// </summary>
        public string NationalGridConnection { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC61850
        /// </summary>
        public string objidIEC61850 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61850
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC61850 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61850Date
        /// </summary>
        public string IEC61850Date { get; set; }

        /// <summary>
        /// Gets or sets the objiddewadrrg
        /// </summary>
        public string objiddewadrrg { get; set; }

        /// <summary>
        /// Gets or sets the dewadrrg
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase dewadrrg { get; set; }

        /// <summary>
        /// Gets or sets the DewaDRRGDate
        /// </summary>
        public string DewaDRRGDate { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC610101
        /// </summary>
        public string objidIEC610101 { get; set; }

        /// <summary>
        /// Gets or sets the IEC610101
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC610101 { get; set; }

        /// <summary>
        /// Gets or sets the IEC610101Date
        /// </summary>
        public string IEC610101Date { get; set; }
    }
}
