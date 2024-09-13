// <copyright file="LoginModel.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    using DEWAXP.Foundation.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="LoginModel" />.
    /// </summary>
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
        [RegularExpression(Foundation.Content.Utils.RegexUtil.PasswordRegexWithSmallLetterSymbolAndNumber, ValidationMessageKey = "Please enter the password as per policy")]
        public string Password { get; set; }
    }
}
