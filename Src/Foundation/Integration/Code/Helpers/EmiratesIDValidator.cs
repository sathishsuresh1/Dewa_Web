// <copyright file="EmiratesIDValidator.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Helpers
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines the <see cref="EmiratesIDValidator" />
    /// </summary>
    public class EmiratesIDValidator
    {
        /// <summary>
        /// The IsValid
        /// </summary>
        /// <param name="emiratesid">The emiratesid<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsValid(string emiratesid)
        {
            if (string.IsNullOrWhiteSpace(emiratesid)) return false;
            emiratesid = emiratesid.Trim();
            Regex regex = new Regex(@"^(?:784)\d{12}$");
            Match match = regex.Match(emiratesid);
            if (match.Success)
            {
                return true;
            }
            return false;
        }
    }
}
