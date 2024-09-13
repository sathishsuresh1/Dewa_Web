// <copyright file="MeterTestingProject.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

using DEWAXP.Foundation.DataAnnotations;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.Miscellaneous
{
    /// <summary>
    /// Defines the <see cref="MeterTestingNewconnection" />
    /// </summary>
    public class JointerTesting : BaseMiscellanous
    {
        public IEnumerable<SelectListItem> ContractorList { get; set; }
        /// <summary>
        /// Gets or sets the Approved Contractors List
        /// </summary>
        public string Contractor { get; set; }
        ///// <summary>
        ///// Gets or sets the isApproved
        ///// </summary>
        //public bool isApproved { get; set; }

        /// <summary>
        /// Gets or sets the JointerName
        /// </summary>
        public string JointerName { get; set; }

        /// <summary>
        /// Gets or sets the AsstJointerName
        /// </summary>
        public string AsstJointerName { get; set; }

        /// <summary>
        /// Gets or sets the HelperName
        /// </summary>
        public string HelperName { get; set; }

        /// <summary>
        /// Gets or sets the Attachment1
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase Attachment2 { get; set; }

        /// <summary>
        /// Gets or sets the Attachment1
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase Attachment3 { get; set; }

    }
}
