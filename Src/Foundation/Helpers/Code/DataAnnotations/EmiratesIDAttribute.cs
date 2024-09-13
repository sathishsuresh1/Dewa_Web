// <copyright file="EmiratesIDAttribute.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.DataAnnotations
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using Integration.Helpers;

    /// <summary>
    /// Emirates ID is the main identification or resident card of all UAE citizens and residents
	/// It starts with 784 followed by 12 digits.
    /// </summary>
    public class EmiratesIDAttribute : ValidationAttribute
    {
        /// <summary>
        /// Defines the _emiratesid
        /// </summary>
        private readonly string _emiratesid;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmiratesIDAttribute"/> class.
        /// </summary>
        /// <param name="emiratesid">The emiratesid<see cref="string"/></param>
        public EmiratesIDAttribute(string emiratesid = "784")
        {
            _emiratesid = emiratesid;
        }

        /// <summary>
        /// The IsValid
        /// </summary>
        /// <param name="value">The value<see cref="object"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            string s = value.ToString();
            if (string.IsNullOrWhiteSpace(s))
            {
                return true;
            }

            return EmiratesIDValidator.IsValid(s);
        }

        /// <summary>
        /// Gets or sets the ValidationMessageKey
        /// </summary>
        public string ValidationMessageKey { get; set; }

        /// <summary>
        /// The FormatErrorMessage
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public override string FormatErrorMessage(string name) => string.Format(CultureInfo.CurrentCulture, ValidationMessageKey, name);
    }
}
