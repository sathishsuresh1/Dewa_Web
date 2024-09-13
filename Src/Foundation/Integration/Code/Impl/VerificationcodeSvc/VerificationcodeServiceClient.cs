using System;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using Sitecore.Diagnostics;
using DEWAXP.Foundation.Integration.VerificationcodeSvc;
using System.Net.Http;
using System.Net;
using System.Web.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.Impl.VerificationcodeSvc
{
    [Service(typeof(IVerificationcodeServiceClient), Lifetime = Lifetime.Transient)]
    public class VerificationcodeServiceClient : BaseDewaGateway, IVerificationcodeServiceClient
    {
        #region Methods
        public ServiceResponse<QRCodeResponse> getQRCodeVerified(string certificatetype, string referencenumber, string pinnumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                //ICredentials credentials = new NetworkCredential(WebConfigurationManager.AppSettings["PROXYUSER"], WebConfigurationManager.AppSettings["PROXYPASSWORD"]);
                //HttpClientHandler handler = new HttpClientHandler()
                //{
                //    Proxy = new WebProxy(WebConfigurationManager.AppSettings["PROXYURL"], true, null, credentials),
                //    UseProxy = true
                //};

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(WebConfigurationManager.AppSettings["QRCodeURL"]);
                    // var uri = string.Format("?certificatetype={0}&lang={1}&referencenumber={2}&ts=" + DateTime.Now.Ticks, certificatetype, language, referencenumber);
                    var uri = string.Format("?certificatetype={0}&lang={1}&referencenumber={2}", certificatetype, language, referencenumber);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store, max-age=0, must-revalidate");
                    httpClient.DefaultRequestHeaders.Add("pinnumber", pinnumber);
                    httpClient.DefaultRequestHeaders.Add("apikey", WebConfigurationManager.AppSettings["RestAPI_Client_Id"]);
                    httpClient.DefaultRequestHeaders.Authorization= new AuthenticationHeaderValue("Bearer", OAuthToken.GetAccessToken());// "Bearer " + OAuthToken.GetAccessToken());
                    HttpResponseMessage response = httpClient.GetAsync(uri).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ServiceResponse<QRCodeResponse>(null, false, "Response Error");
                    }
                    else
                    {
                        var VerifyData = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = JsonConvert.DeserializeObject<QRCodeResponse>(VerifyData);
                        if (result != null && !string.IsNullOrWhiteSpace(result.responsecode) && result.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<QRCodeResponse>(result);
                        }
                        else
                        {
                            return new ServiceResponse<QRCodeResponse>(result, false, "Error Response");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Info(ex.Message + "---" + ex.InnerException.ToString(), this);
                return new ServiceResponse<QRCodeResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion
    }
}
