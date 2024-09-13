// <copyright file="DRRGFileUpload.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    using System.Web;

    /// <summary>
    /// Defines the <see cref="DRRGFileUpload" />
    /// </summary>
    public class DRRGFileUpload
    {
        /// <summary>
        /// Gets or sets the file
        /// </summary>
        public HttpPostedFileBase file { get; set; }

        /// <summary>
        /// Gets or sets the validityDate
        /// </summary>
        public string validityDate { get; set; }

        /// <summary>
        /// Gets or sets the fileStandard
        /// </summary>
        public string fileStandard { get; set; }
    }
}
