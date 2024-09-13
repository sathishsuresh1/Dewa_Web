// <copyright file="DRRGApplicationStatus.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    /// <summary>
    /// Defines the <see cref="DRRGApplicationStatus" />
    /// </summary>
    public class DRRGApplicationStatus
    {
        /// <summary>
        /// Gets or sets the Efolderid
        /// </summary>
        public string Efolderid { get; set; }

        /// <summary>
        /// Gets or sets the UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the ModelName
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Gets or sets the DateSubmitted
        /// </summary>
        public string DateSubmitted { get; set; }

        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Remarks
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the count
        /// </summary>
        public string count { get; set; }

        /// <summary>
        /// Gets or sets the Type
        /// </summary>
        public string Type { get; set; }
    }
}
