// <copyright file="SetNewPassword.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    using DEWAXP.Foundation.DataAnnotations;
    using System.Security;

    /// <summary>
    /// Defines the <see cref="SetNewPassword" />.
    /// </summary>
    public class SetNewPassword
    {
        /// <summary>
        /// Gets or sets the Username.
        /// </summary>
        public string Username { get; set; }
        public string CodeUnchanged { get; set; }
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the Password.
        /// </summary>
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        [RegularExpression("^(?=.*\\d)(?=.*[\\D])[0-9\\D]{8,}$", ValidationMessageKey = "login password validation message alphanumeric")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the ConfirmPassword.
        /// </summary>
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Compare("Password", ValidationMessageKey = "Password mismatch error")]
        public string ConfirmPassword { get; set; }
    }
}
