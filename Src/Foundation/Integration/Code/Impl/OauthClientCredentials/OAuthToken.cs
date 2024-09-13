// <copyright file="OAuthToken.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Impl.OauthClientCredentials
{
    using DEWAXP.Foundation.Logger;
    using DotNetOpenAuth.OAuth2;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Defines the <see cref="OAuthToken" />
    /// </summary>
    public class OAuthToken : BaseDewaGateway
    {
        /// <summary>
        /// The GetAccessToken
        /// </summary>
        /// <param name="tokenEndPoint">The tokenEndPoint<see cref="string"/></param>
        /// <param name="clientid">The clientid<see cref="string"/></param>
        /// <param name="clientsecret">The clientsecret<see cref="string"/></param>
        /// <param name="devid">The devid<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetAccessToken(string tokenEndPoint = "", string clientid = "", string clientsecret = "", string devid = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tokenEndPoint))
                {
                    tokenEndPoint = ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Oauth_URL];
                }
                if (string.IsNullOrWhiteSpace(clientid))
                {
                    clientid = ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Client_Id];
                }
                if (string.IsNullOrWhiteSpace(clientsecret))
                {
                    clientsecret = ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Client_Secret];
                }
                if (string.IsNullOrWhiteSpace(devid))
                {
                    devid = ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Dev_Id];
                }
                object session = HttpContext.Current?.Session["OAuthToken"];
                object sessiontime = HttpContext.Current?.Session["OAuthTokenExp"];

                bool isExpiry = DateTime.Parse(sessiontime != null ? Convert.ToString(sessiontime) : DateTime.MinValue.ToString()) < DateTime.Now;

                if (session == null || sessiontime == null || isExpiry)
                {
                    AuthorizationServerDescription authorizationServerDescription = new AuthorizationServerDescription
                    {
                        TokenEndpoint = new Uri(tokenEndPoint)
                    };
                    WebServerClient _webserverclient = new WebServerClient(authorizationServerDescription, clientid, clientsecret);
                    _webserverclient.JsonReaderQuotas.MaxDepth = 10;
                    IAuthorizationState response = _webserverclient.GetClientAccessToken(new[] { "devid", devid });
                    if (response != null && response.AccessToken != null && response.AccessTokenExpirationUtc != null)
                    {
                        HttpContext.Current.Session["OAuthToken"] = "";
                        HttpContext.Current.Session["OAuthTokenExp"] = "";
                        HttpContext.Current.Session["OAuthToken"] = response.AccessToken;
                        HttpContext.Current.Session["OAuthTokenExp"] = response.AccessTokenExpirationUtc.Value.ToLocalTime();
                    }
                }
                return HttpContext.Current?.Session?["OAuthToken"] != null ? Convert.ToString(HttpContext.Current.Session["OAuthToken"]) : "";
            }
            catch (Exception ex)
            {
                string id = LogService.Fatal(ex, "AccessToken");
                return "";
            }
        }
        public static string GetApplicationAccessToken()
        {
            try
            {
                AuthorizationServerDescription authorizationServerDescription = new AuthorizationServerDescription
                {
                    TokenEndpoint = new Uri(ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Oauth_URL])
                };
                WebServerClient _webserverclient = new WebServerClient(authorizationServerDescription, ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Client_Id], ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Client_Secret]);
                _webserverclient.JsonReaderQuotas.MaxDepth = 10;
                var response = _webserverclient.GetClientAccessToken(new[] { "devid", ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Dev_Id] });
                if (response != null && response.AccessToken != null && response.AccessTokenExpirationUtc != null)
                {
                    return response.AccessToken;
                }
                //return HttpContext.Current?.Session?["OAuthToken"] != null ? Convert.ToString(HttpContext.Current.Session["OAuthToken"]) : "";
            }
            catch (Exception ex)
            {
                var id = LogService.Fatal(ex, "AccessToken");
            }
            return "";
        }

        /// <summary>
        /// The ConvertParameterString
        /// </summary>
        /// <param name="parameter">The parameter<see cref="Dictionary{string, string}"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string ConvertParameterString(Dictionary<string, string> parameter)
        {
            StringBuilder postData = new StringBuilder();
            if (parameter != null && parameter.Count() > 0)
            {
                int cnt = 0;
                foreach (KeyValuePair<string, string> item in parameter)
                {
                    cnt++;
                    if (parameter.Count() != cnt)
                    {
                        postData.Append(string.Format("{0}={1}&", HttpUtility.HtmlEncode(item.Key), HttpUtility.HtmlEncode(item.Value)));
                    }
                    else
                    {
                        postData.Append(string.Format("{0}={1}", HttpUtility.HtmlEncode(item.Key), HttpUtility.HtmlEncode(item.Value)));
                    }
                }
            }
            return postData.ToString();
        }
    }
}
