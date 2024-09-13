// <copyright file="DashboardUserDetails.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    /// <summary>
    /// Defines the <see cref="DashboardUserDetails" />.
    /// </summary>
    public class DashboardUserDetails
    {
        /// <summary>
        /// Gets or sets the Firstname.
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        /// Gets or sets the Lastname.
        /// </summary>
        public string Lastname { get; set; }

        /// <summary>
        /// Gets or sets the Manufacturername.
        /// </summary>
        public string Manufacturername { get; set; }

        /// <summary>
        /// Gets or sets the Logofilename.
        /// </summary>
        public string Logofilename { get; set; }

        /// <summary>
        /// Gets or sets the Logofilecontenttype.
        /// </summary>
        public string Logofilecontenttype { get; set; }

        /// <summary>
        /// Gets or sets the Logofileextension.
        /// </summary>
        public string Logofileextension { get; set; }

        /// <summary>
        /// Gets or sets the Logofile.
        /// </summary>
        public string Logofile { get; set; }
        public string ManufacturerCode { get; set; }
        public string Address { get; set; }
        public string MobileNumber { get; set; }
    }
}
