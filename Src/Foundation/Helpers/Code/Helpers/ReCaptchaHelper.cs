using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecorex = Sitecore.Context;
using System.Web;
using Sitecore.Data.Items;
using System.Net;
using Newtonsoft.Json.Linq;
using DEWAXP.Foundation.Logger;

namespace DEWAXP.Foundation.Helpers
{
    public static class ReCaptchaHelper
    {
       public static Item recaptchaItem = Sitecorex.Database.GetItem(SitecoreItemIdentifiers.RECAPTCHA_CONFIG);
        public static bool Recaptchasetting()
        {
            return recaptchaItem != null && recaptchaItem.Fields["IsRecaptcha"] != null && recaptchaItem.Fields["IsRecaptcha"].Value.Equals("1") ? true : false;
        }
        public static string RecaptchaSiteKey()
        {
           return recaptchaItem != null && recaptchaItem.Fields["Recaptcha Site key"] != null ? recaptchaItem.Fields["Recaptcha Site key"].Value : string.Empty;
        }
        public static bool RecaptchaResponse(string response)
        {
            bool status;
            try
            {
                string secretKey = recaptchaItem != null && recaptchaItem.Fields["Recaptcha Secret key"] != null ? recaptchaItem.Fields["Recaptcha Secret key"].Value : string.Empty;
                if (!string.IsNullOrWhiteSpace(secretKey))
                {
                    var client = new WebClient();
                    var url = string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response);
                    NetworkCredential netcredit = new NetworkCredential(RecaptchaKeys.Proxyuser, RecaptchaKeys.Proxypassword, RecaptchaKeys.Proxydomain);
                    client.Credentials = netcredit;
                    WebProxy proxyObject = new WebProxy(RecaptchaKeys.Proxyurl);
                    proxyObject.Credentials = new NetworkCredential(RecaptchaKeys.Proxyuser, RecaptchaKeys.Proxypassword, RecaptchaKeys.Proxydomain);
                    client.Proxy = proxyObject;
                    var result = client.DownloadString(url);
                    var captchaResponse = CustomJsonConvertor.DeserializeObject<ReCaptchaClass>(result);
                    status = captchaResponse.Success.Equals("true") ? true : false;
                }
                else
                {
                    status = false;
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, null);
                status = false;
            }
            return status;
        }
    }

    public class ReCaptchaClass
    {
        [JsonProperty("success")]
        public string Success
        {
            get { return m_Success; }
            set { m_Success = value; }
        }

        private string m_Success;
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes
        {
            get { return m_ErrorCodes; }
            set { m_ErrorCodes = value; }
        }
        private List<string> m_ErrorCodes;
    }
}