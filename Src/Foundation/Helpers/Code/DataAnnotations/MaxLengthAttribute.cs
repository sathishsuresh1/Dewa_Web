using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Sitecore.Globalization;

namespace DEWAXP.Foundation.DataAnnotations
{
	public class MaxLengthAttribute: System.ComponentModel.DataAnnotations.MaxLengthAttribute, ISitecoreDictionaryDataAnnotation
	{
		public MaxLengthAttribute(int otherProperty)
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