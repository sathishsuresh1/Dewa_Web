using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Helpers
{
	public class IbanNumberValidator
	{
		public static bool IsValid(string ibanNumber)
		{
			if (string.IsNullOrWhiteSpace(ibanNumber)) return false;

			ibanNumber = ibanNumber.Trim();

			var aligned = string.Concat(ibanNumber.Substring(4), ibanNumber.Substring(0, 4));
			var numeric = ConvertToNumbericRepresentation(aligned);
			var challenge = BigInteger.Parse(numeric);

			return challenge%97 == 1;
		}

		private static string ConvertToNumbericRepresentation(string aligned)
		{
			var rebuild = new List<string>();
			foreach (var c in aligned)
			{
				rebuild.Add(GetNumericString(c));
			}
			return string.Join(string.Empty, rebuild);
		}

		private static string GetNumericString(char c)
		{
			switch (c)
			{
				case 'A':
					return "10";
				case 'B':
					return "11";
				case 'C':
					return "12";
				case 'D':
					return "13";
				case 'E':
					return "14";
				case 'F':
					return "15";
				case 'G':
					return "16";
				case 'H':
					return "17";
				case 'I':
					return "18";
				case 'J':
					return "19";
				case 'K':
					return "20";
				case 'L':
					return "21";
				case 'M':
					return "22";
				case 'N':
					return "23";
				case 'O':
					return "24";
				case 'P':
					return "25";
				case 'Q':
					return "26";
				case 'R':
					return "27";
				case 'S':
					return "28";
				case 'T':
					return "29";
				case 'U':
					return "30";
				case 'V':
					return "31";
				case 'W':
					return "32";
				case 'X':
					return "33";
				case 'Y':
					return "34";
				case 'Z':
					return "35";
				default:
					return new string(new[] { c });
			}
		}
	}
}
