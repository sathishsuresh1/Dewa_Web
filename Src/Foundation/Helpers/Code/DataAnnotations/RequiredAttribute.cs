using System.Globalization;
using Sitecore.Globalization;

namespace DEWAXP.Foundation.DataAnnotations
{
	public class RequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute, ISitecoreDictionaryDataAnnotation
	{
		public string ValidationMessageKey { get; set; }

		public override string FormatErrorMessage(string name)
		{
			if (this.ValidationMessageKey != null)
			{
				return string.Format(CultureInfo.CurrentCulture, Translate.Text(this.ValidationMessageKey), name);
			}
            return string.Empty;
		}
	}
}