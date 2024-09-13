// <copyright file="EmiratesIDValidator.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Helpers
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines the <see cref="EmailValidator" />
    /// </summary>
    public class EmailValidator
    {
        /// <summary>
        /// The IsValid
        /// </summary>
        /// <param name="emiratesid">The emiratesid<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            email = email.Trim();
            Regex regex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            Match match = regex.Match(email);
            if (match.Success)
            {
                return true;
            }
            return false;
        }
    }
}
