// <copyright file="ForgotPasswordRequest.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword
{
    /// <summary>
    /// Defines the <see cref="passwordinput" />.
    /// </summary>
    public class passwordinput
    {
        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        public string mode { get; set; }

        /// <summary>
        /// Gets or sets the lang.
        /// </summary>
        public string lang { get; set; }

        /// <summary>
        /// Gets or sets the sessionid.
        /// </summary>
        public string sessionid { get; set; }

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        public string reference { get; set; }

        /// <summary>
        /// Gets or sets the prtype.
        /// </summary>
        public string prtype { get; set; }

        /// <summary>
        /// Gets or sets the mobile.
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Gets or sets the contractaccountnumber.
        /// </summary>
        public string contractaccountnumber { get; set; }

        /// <summary>
        /// Gets or sets the businesspartner.
        /// </summary>
        public string businesspartner { get; set; }

        /// <summary>
        /// Gets or sets the otp.
        /// </summary>
        public string otp { get; set; }

        /// <summary>
        /// Gets or sets the userid.
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// Gets or sets the confirmpassword.
        /// </summary>
        public string confirmpassword { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ForgotPasswordRequest" />.
    /// </summary>
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// Gets or sets the passwordinput.
        /// </summary>
        public passwordinput passwordinput { get; set; }
    }
}
