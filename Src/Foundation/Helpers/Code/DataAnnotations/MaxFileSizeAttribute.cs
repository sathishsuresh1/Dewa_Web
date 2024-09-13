using Sitecore.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web;

namespace DEWAXP.Foundation.DataAnnotations
{
    public class MaxFileSizeAttribute : ValidationAttribute, ISitecoreDictionaryDataAnnotation
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return true;
            }
            return file.ContentLength <= _maxFileSize;
        }

        public string ValidationMessageKey { get; set; }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, Translate.Text(this.ValidationMessageKey), name);
        }
    }
}
