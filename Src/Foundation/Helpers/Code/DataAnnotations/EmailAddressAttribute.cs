using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using Sitecore.Globalization;

namespace DEWAXP.Foundation.DataAnnotations
{
	public class EmailAddressAttribute : RegularExpressionAttribute, ISitecoreDictionaryDataAnnotation
	{
        private const string EMAIL_REGEX = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
		
		public EmailAddressAttribute()
			: base(EMAIL_REGEX)
		{
		}
	}
}