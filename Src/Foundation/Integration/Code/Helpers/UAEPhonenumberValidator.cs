// <copyright file="EmiratesIDValidator.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Helpers
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines the <see cref="UAEPhonenumberValidator" />
    /// </summary>
    public class UAEPhonenumberValidator
    {
        /// <summary>
        /// The IsValid
        /// </summary>
        /// <param name="emiratesid">The emiratesid<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsValid(string phonenumber)
        {
            if (string.IsNullOrWhiteSpace(phonenumber)) return false;
            phonenumber = phonenumber.Trim();
            if (phonenumber.StartsWith("+"))
            {
                phonenumber = phonenumber.Substring(1);
            }
            if (phonenumber.StartsWith("971"))
            {
                phonenumber = phonenumber.Substring(3);
            }
            if (phonenumber.StartsWith("0"))
            {
                phonenumber = phonenumber.Substring(1);
            }
            Regex regex = new Regex(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$");
            Match match = regex.Match(phonenumber);
            if (match.Success)
            {
                return true;
            }
            return false;
        }
    }
}
