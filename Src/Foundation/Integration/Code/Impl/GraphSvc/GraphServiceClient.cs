using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.GraphSvc;
using System.Net.Http;
using System.Configuration;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.Impl.GraphSvc
{
    [Service(typeof(IGraphServiceClient), Lifetime = Lifetime.Transient)]
    public class GraphServiceClient : BaseDewaGateway, IGraphServiceClient
    {
        public ServiceResponse<RootObject> GetGraph(string contractAccountNumber, string date, string month, string year, string usagetype,string userid,string session,SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string query = new System.Uri(ConfigurationManager.AppSettings["GraphAPI_Smart_Meter"]) +
                    string.Format("/meterreading?contractaccountnumber={0}&date={1}&month={2}&year={3}&usagetype={4}&userid={5}&vendorid={6}&appversion={7}&appidentifier={8}&sessionid={9}&lang={10}", contractAccountNumber, date, month, year, usagetype,userid,GetVendorId(segment),AppVersion,segment.Identifier(),session,language.Code());

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", OAuthToken.GetAccessToken());

                    var response = httpClient.PostAsync(query,null).Result;

#if DEBUG
                    System.Diagnostics.Debug.WriteLine(response);
#endif

                    if (!response.IsSuccessStatusCode)
                    {
                        LogService.Debug("Meterreading response failed: " + response.ReasonPhrase);
                        return new ServiceResponse<RootObject>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                    }
                    else
                    {
                        var content = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(content);

                        //var result = JsonConvert.DeserializeObject<EVCardResponse>(ResultData);
                        if (result != null && result.ReplyMessage?.Reply?.replyCode == 0)
                        {
                            return new ServiceResponse<RootObject>(result);
                        }
                        else
                        {
                            LogService.Error(new System.Exception("Unable to Parse Response from MeterReading Service" + System.Environment.NewLine + content), this);
                            return new ServiceResponse<RootObject>(result, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<RootObject>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }

        }

        #region private methods
        public HttpRequestMessage GetRequest(string method)
        {
            //https://api.qa.dewa.gov.ae/v1/smartmeter/meterreading
            //var request = new HttpRequestMessage(HttpMethod.Post, ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Smart_Customer] + method);
            var request = new HttpRequestMessage(HttpMethod.Get, ConfigurationManager.AppSettings["GraphAPI_Smart_Meter"] + method);
            request.Headers.Add("Authorization", "Bearer " + OAuthToken.GetAccessToken());
            return request;
        }
        #endregion
    }
}
