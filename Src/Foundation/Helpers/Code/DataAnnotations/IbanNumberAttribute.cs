using DEWAXP.Foundation.Integration.Helpers;
using Sitecore.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DEWAXP.Foundation.DataAnnotations
{
    /// <summary>
    /// Validates IBAN numbers according to the rules stipulated at: https://www.alpha.gr/files/personalbanking/iban_check_digit_En.pdf
    /// An example IBAN number for the Emirates is AE070331234567890123456
    /// </summary>
    public class IbanNumberAttribute : ValidationAttribute, ISitecoreDictionaryDataAnnotation
    {
        private readonly string _ibanPrefix;

        public IbanNumberAttribute(string ibanPrefix = "AE")
        {
            _ibanPrefix = ibanPrefix;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;

            var s = value.ToString();
            if (string.IsNullOrWhiteSpace(s)) return true;

            if (!s.StartsWith(_ibanPrefix))
            {
                s = string.Concat(_ibanPrefix, s);
            }
            return IbanNumberValidator.IsValid(s);
        }

        public string ValidationMessageKey { get; set; }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, Translate.Text(this.ValidationMessageKey), name);
        }
    }
}