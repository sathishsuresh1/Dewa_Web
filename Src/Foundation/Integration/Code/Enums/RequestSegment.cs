using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Enums
{
	public enum RequestSegment
	{
		Desktop,
		Mobile
	}

	internal static class RequestSegmentExtensions
	{
		internal static string Identifier(this RequestSegment segment)
		{
			if (segment == RequestSegment.Mobile)
			{
				return "m";
			}
			return "d";
		}
	}
}
