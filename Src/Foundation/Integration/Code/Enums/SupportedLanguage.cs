using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Enums
{
    public enum SupportedLanguage
    {
        English = 0,
        Arabic = 1
    }

    public static class SupportedLanguageExtensions
    {
        public static string Code(this SupportedLanguage language)
        {
            switch (language)
            {
                case SupportedLanguage.Arabic:
                    return "AR";
                default:
                    return "EN";
            }
        }
	}
}
