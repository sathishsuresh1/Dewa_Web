using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Helpers.Common
{
    public class BaseSiteHelper
    {
        public static string GetSecureLocalURLWithoutPort(string urlPaintext)
        {
            if(!string.IsNullOrWhiteSpace(urlPaintext))
            {
            Uri uri = new Uri(urlPaintext);
                return $"https://{HttpContext.Current.Request.ServerVariables["SERVER_NAME"]}{uri.LocalPath}{uri.Query}";   
            }
            return "";
        }
    }
}