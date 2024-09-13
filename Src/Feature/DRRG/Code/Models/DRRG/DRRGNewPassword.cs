// <copyright file="DRRGNewPassword.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    using DEWAXP.Foundation.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="NewPassword" />
    /// </summary>
    public class NewPassword
    {
        /// <summary>
        /// Gets or sets the NewPasswordstring
        /// </summary>
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter New Password")]
        public string NewPasswordstring { get; set; }

        /// <summary>
        /// Gets or sets the ConfirmPassword
        /// </summary>
        [Compare("NewPasswordstring", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the manufacturerEmailid
        /// </summary>
        public string manufacturerEmailid { get; set; }
    }
}
