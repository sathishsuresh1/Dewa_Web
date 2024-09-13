using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Sitecore.Globalization;

namespace DEWAXP.Foundation.DataAnnotations
{
	public class RegularExpressionAttribute : System.ComponentModel.DataAnnotations.RegularExpressionAttribute, ISitecoreDictionaryDataAnnotation
	{
		public bool AllowEmptyStrings { get; set; }

		public RegularExpressionAttribute(string pattern) 
			: base(pattern)
		{
		}

		public string ValidationMessageKey { get; set; }

		public override bool IsValid(object value)
		{
			var s = value != null ? value.ToString() : null;
			if (AllowEmptyStrings && string.IsNullOrWhiteSpace(s))
			{
				return true;
			}
			return base.IsValid(value);
		}

		public override string FormatErrorMessage(string name)
		{
			return string.Format(CultureInfo.CurrentCulture, Translate.Text(this.ValidationMessageKey), name);
		}
	}
}