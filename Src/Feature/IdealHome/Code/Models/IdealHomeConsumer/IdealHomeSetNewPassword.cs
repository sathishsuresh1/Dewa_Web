// <copyright file="IdealHomeSetNewPassword.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>


using DEWAXP.Foundation.DataAnnotations;

namespace DEWAXP.Feature.IdealHome.Models.IdealHomeConsumer
{
    /// <summary>
    /// Defines the <see cref="IdealHomeSetNewPassword" />.
    /// </summary>
    public class IdealHomeSetNewPassword
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public string Id { get; set; }

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
