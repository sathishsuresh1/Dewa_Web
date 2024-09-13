using DEWAXP.Foundation.Integration.Enums;
using Sitecore.Globalization;

namespace DEWAXP.Foundation.Helpers.Extensions
{
    public static class SupportedLanguageExtensions
    {
        public static string DisplayText(this SupportedLanguage lang)
        {
            switch (lang)
            {
                case SupportedLanguage.Arabic:
                    return Translate.Text("global.PreferredLanguage.Arabic");

                default:
                    return Translate.Text("global.PreferredLanguage.English");
            }
        }
    }
}