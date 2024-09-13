using System.Globalization;
using Sitecore.Globalization;

namespace DEWAXP.Foundation.DataAnnotations
{
	public class CompareAttribute : System.ComponentModel.DataAnnotations.CompareAttribute, ISitecoreDictionaryDataAnnotation
	{
		public CompareAttribute(string otherProperty)
			: base(otherProperty)
		{
		}

		public string ValidationMessageKey { get; set; }

		public override string FormatErrorMessage(string name)
		{
			return string.Format(CultureInfo.CurrentCulture, Translate.Text(this.ValidationMessageKey), name);
		}
	}
}