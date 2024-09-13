// <copyright file="LoginModel.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

using DEWAXP.Foundation.DataAnnotations;

namespace DEWAXP.Feature.GatePass.Models.ePass
{

    /// <summary>
    /// Defines the <see cref="VendorLoginModel" />
    /// </summary>
    public class VendorLoginModel
    {
        /// <summary>
        /// Gets or sets the VendorId
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// Gets or sets the Emailaddress
        /// </summary>
        public string Emailaddress { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="UserRegistration" />
    /// </summary>
    public class UserRegistration
    {
        /// <summary>
        /// Gets or sets the VendorId
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// Gets or sets the CompanyName
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the Fullname
        /// </summary>
        public string Fullname { get; set; }

        /// <summary>
        /// Gets or sets the Emailid
        /// </summary>
        public string Emailid { get; set; }

        /// <summary>
        /// Gets or sets the MobilePhone
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the ConfirmationPassword
        /// </summary>
        public string ConfirmationPassword { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="SetNewPassword" />
    /// </summary>
    public class SetNewPassword
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// Gets or sets the userid
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// Gets or sets the supplierid
        /// </summary>
        public string supplierid { get; set; }

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
