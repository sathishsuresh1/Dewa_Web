using DEWAXP.Foundation.Integration.Enums;
using System;

namespace DEWAXP.Foundation.Helpers.Extensions
{
    public class RequestLanguageExtension
    {
        public SupportedLanguage RequestLanguage
        {
            get
            {
                return "en".Equals(Sitecore.Context.Language.CultureInfo.TwoLetterISOLanguageName, StringComparison.InvariantCultureIgnoreCase) ?
                    SupportedLanguage.English : SupportedLanguage.Arabic;
            }
        }
    }
}