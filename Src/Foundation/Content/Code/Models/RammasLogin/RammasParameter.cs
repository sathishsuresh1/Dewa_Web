using DEWAXP.Foundation.DataAnnotations;
using Sitecore.Diagnostics;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Models.RammasLogin
{
    public class RammasParameter
    {
        public string ChannelId { get; set; }
        public string ConversationId { get; set; }
        public string surl { get; set; }
        public string ActivityId { get; set; }
        public string FromId { get; set; }
        public string FromName { get; set; }
        public string ToId { get; set; }
        public string ToName { get; set; }
        public string LoginType { get; set; }
        public string LanguageCode { get; set; }

        public static bool PostLoginCredentialsToBot(string userName, string sessionId, string channelId, string conversationId, string surl, string activityId, string fromId, string fromName, string toId, string toName, string languageCode, out string error, int magicNumber, string loginType = "authentication")
        {
            try
            {
                //Log.Info("postlogin", true);

                bool IsSuccess = false;
                if (string.IsNullOrEmpty(channelId))
                { channelId = string.Empty; }

                ICredentials credentials = new NetworkCredential(WebConfigurationManager.AppSettings["PROXYUSER"], WebConfigurationManager.AppSettings["PROXYPASSWORD"]);
                HttpClientHandler handler = new HttpClientHandler()
                {
                    Proxy = new WebProxy(WebConfigurationManager.AppSettings["PROXYURL"], true, null, credentials),
                    UseProxy = true
                };
                //After login sucess need to post call for bot controller i.e; below code
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient(handler))
                {
                    var baseAddress = WebConfigurationManager.AppSettings["BotUrl"]; //TODO: Set main bot url here 
                    httpClient.BaseAddress = new Uri(baseAddress);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // var jwtToken = new JwtManager().GenerateToken(userName, sessionId, channelId, conversationId, surl, activityId, fromId, fromName, toId, toName, languageCode, magicNumber, loginType, 60);
                    var jwtToken = new JwtManager().GenerateToken(userName, sessionId, channelId, conversationId, languageCode, magicNumber, loginType, 60);
                    HttpResponseMessage response = httpClient.PostAsJsonAsync(new Uri(baseAddress), jwtToken).Result;
                    //Log.Info("postlogin response", response);

                    IsSuccess = response.IsSuccessStatusCode;
                    error = response.ReasonPhrase;

                }
                return IsSuccess;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Log.Info(error + "Customer login --", "");
                return false;
            }
        }

        public async Task<RammasGateWayRequest> FetchBotState(string ConversationId)
        {
            try
            {
                ICredentials credentials = new NetworkCredential(WebConfigurationManager.AppSettings["PROXYUSER"], WebConfigurationManager.AppSettings["PROXYPASSWORD"]);
                HttpClientHandler handler = new HttpClientHandler()
                {
                    Proxy = new WebProxy(WebConfigurationManager.AppSettings["PROXYURL"], true, null, credentials),
                    UseProxy = true
                };
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient(handler))
                {
                    //var baseAddress = "https://rammamsbottestdewa.azurewebsites.net/api/PaymentInitiate/FetchState?conversationId="+ ConversationId; //TODO: Set main bot url here 
                    var baseAddress = WebConfigurationManager.AppSettings["Rammas_FetchBotState"] + "?conversationId=" + ConversationId;
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = httpClient.GetAsync(new Uri(baseAddress)).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }
                    else
                    {
                        var paymentData = await response.Content.ReadAsStringAsync();
                        return System.Web.Helpers.Json.Decode<RammasGateWayRequest>(paymentData.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Info(ex.Message + "Fetch state --", ex.InnerException.ToString());
                return null;
            }
        }

        public static bool EpayRedirect(RammasGateWayResponse rammasGateWayResponse, out string error)
        {
            try
            {
                ICredentials credentials = new NetworkCredential(WebConfigurationManager.AppSettings["PROXYUSER"], WebConfigurationManager.AppSettings["PROXYPASSWORD"]);
                HttpClientHandler handler = new HttpClientHandler()
                {
                    Proxy = new WebProxy(WebConfigurationManager.AppSettings["PROXYURL"], true, null, credentials),
                    UseProxy = true
                };
                //After login sucess need to post call for bot controller i.e; below code
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient(handler))
                {
                    //var baseAddress = "https://rammamsbottestdewa.azurewebsites.net/api/EPay/EPayResponse"; //// TODO: Set main bot url here 
                    var baseAddress = WebConfigurationManager.AppSettings["Rammas_EpayRedirect"]; //// TODO: Set main bot url here 
                    httpClient.BaseAddress = new Uri(baseAddress);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = httpClient.PostAsJsonAsync(new Uri(baseAddress), rammasGateWayResponse).Result;

                    error = response.ReasonPhrase;
                    if (!response.IsSuccessStatusCode)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Log.Info(error + "Epay Response --", ex.InnerException.ToString());
                return false;
            }
        }
        #region GenerateRandomNumber
        public static int GenerateRandomNumber()
        {
            RNGCryptoServiceProvider service = new RNGCryptoServiceProvider();

            int number = 0;
            byte[] randomNumber = new byte[1];

            do
            {
                service.GetBytes(randomNumber);
                var digit = randomNumber[0] % 10;
                number = (number * 10) + digit;
            }
            while (number.ToString().Length < 6);

            return number;
        }
        #endregion
    }
    public class RammasExportChat
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid email address")]
        public string EmailAddress { get; set; }
        [Required]
        public string Subject { get; set; }

        [Required]
        [AllowHtml]
        public string Chat { get; set; }

        public string captcha { get; set; }
    }
}