// <copyright file="DRRGApproverRegister.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    using DEWAXP.Foundation.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="DRRGApproverRegister" />
    /// </summary>
    public class DRRGApproverRegister
    {
        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Password
        /// </summary>
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the Efolderid
        /// </summary>
        public string Efolderid { get; set; }

        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Approvedorrejected
        /// </summary>
        public bool Approvedorrejected { get; set; }
    }
}
