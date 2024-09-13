// <copyright file="DRRGForgotPassword.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

using DEWAXP.Foundation.DataAnnotations;

namespace DEWAXP.Feature.DRRG.Models
{
    /// <summary>
    /// Defines the <see cref="ForgotPassword" />
    /// </summary>
    public class ForgotPassword
    {
        /// <summary>
        /// Gets or sets the EmailId
        /// </summary>
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter Email Id")]
        public string EmailId { get; set; }
    }
}
