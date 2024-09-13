// <copyright file="LoginModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.Partner.Models.CorporatePartnership
{
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.DataAnnotations;
    using System;

    /// <summary>
    /// Defines the <see cref="LoginModel" />.
    /// </summary>
    [Serializable]
    public class LoginModel
    {
        /// <summary>
        /// Gets or sets the Username.
        /// </summary>
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Password.
        /// </summary>
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        [RegularExpression(RegexConstant.PasswordRegexWithSmallLetterSymbolAndNumber, ValidationMessageKey = "Please enter the password as per policy")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the CP_PartnerID.
        /// </summary>
        public string CP_PartnerID { get; set; }

        /// <summary>
        /// Gets or sets the CP_PartnerName.
        /// </summary>
        public string CP_PartnerName { get; set; }

        /// <summary>
        /// Gets or sets the CP_CoorName.
        /// </summary>
        public string CP_CoorName { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="SetNewPassword" />
    /// </summary>
    public class SetNewPassword
    {
        /// <summary>
        /// Gets or sets the OldPassword
        /// </summary>
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        [RegularExpression("^(?=.*\\d)(?=.*[\\D])[0-9\\D]{8,}$", ValidationMessageKey = "login password validation message alphanumeric")]
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the Password
        /// </summary>
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        [RegularExpression("^(?=.*\\d)(?=.*[\\D])[0-9\\D]{8,}$", ValidationMessageKey = "login password validation message alphanumeric")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the ConfirmPassword
        /// </summary>
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Compare("Password", ValidationMessageKey = "Password mismatch error")]
        public string ConfirmPassword { get; set; }
    }
}
