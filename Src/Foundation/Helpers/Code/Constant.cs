using System.Configuration;

namespace DEWAXP.Foundation.Helpers
{
    internal class SitecoreItemIdentifiers
    {
        public const string RECAPTCHA_CONFIG = "{156F9379-983D-47E8-89FA-398650C5AD82}";
        public const string YOUTUBE_CONFIG = "{FCD459E7-4897-4981-BA3A-45EB754C0831}";
    }

    internal class RecaptchaKeys
    {
        internal static string Proxyuser => ConfigurationManager.AppSettings["PROXYUSER"];

        internal static string Proxypassword => ConfigurationManager.AppSettings["PROXYPASSWORD"];

        internal static string Proxydomain => ConfigurationManager.AppSettings["PROXYDOMAIN"];

        internal static string Proxyurl => ConfigurationManager.AppSettings["PROXYURL"];
    }
}