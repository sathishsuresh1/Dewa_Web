using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Sitecore.Globalization;

namespace DEWAXP.Foundation.DataAnnotations
{
	public class MinLengthAttribute: System.ComponentModel.DataAnnotations.MinLengthAttribute, ISitecoreDictionaryDataAnnotation
	{
		public MinLengthAttribute(int otherProperty)
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