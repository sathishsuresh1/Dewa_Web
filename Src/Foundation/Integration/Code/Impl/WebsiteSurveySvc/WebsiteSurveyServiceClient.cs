using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using Sitecore.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.Integration.Requests;
using System.Configuration;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.Impl.WebsiteSurveySvc
{
    [Service(typeof(IWebsiteSurveyServiceClient),Lifetime =Lifetime.Transient)]
    public class WebsiteSurveyServiceClient : BaseDewaGateway, IWebsiteSurveyServiceClient
    {
        public ServiceResponse<WebsiteSurveyResponse> SaveWebsiteSurvey(QuestionAnswers questionAnswers)
        {
            string message = string.Empty;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (questionAnswers.SuggestionText == null)
                        questionAnswers.SuggestionText = "";
                    var request = GetRequest("happiness/eventssurvey");

                    StringContent paramContent = new StringContent(JsonConvert.SerializeObject(questionAnswers), Encoding.UTF8, "application/json");

                    request.Content = paramContent;

                    var response = httpClient.SendAsync(request).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        LogService.Error(new Exception(response.ToString()), this);
                        message = ErrorMessages.FRONTEND_ERROR_MESSAGE;
                        //return null;
                    }
                    else
                    {
                        var ResultData = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = JsonConvert.DeserializeObject<WebsiteSurveyResponse>(ResultData);
                        if (result != null && result.ResponseCode.Equals(200))
                        {
                            return new ServiceResponse<WebsiteSurveyResponse>(result);
                        }
                        else
                        {
                            return new ServiceResponse<WebsiteSurveyResponse>(null, false, "Error Response");
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                message = ErrorMessages.FRONTEND_ERROR_MESSAGE;
            }
            return new ServiceResponse<WebsiteSurveyResponse>(null, false, message);
        }

        #region Get Request
        public HttpRequestMessage GetRequest(string method)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Smart_Customer] + method);
            request.Headers.Add("Authorization", "Bearer " + OAuthToken.GetAccessToken());
            request.Headers.Add("apikey", ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Client_Id]);
            return request;
        }
        #endregion
    }

    public class SurveyData
    {
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
        public string RemarksText { get; set; }
    }

    public class QuestionAnswers
    {
        public List<SurveyData> SurveyData { get; set; }
        public string LanguageCode { get; set; }
        public int Condition { get; set; }
        public int ChannelId { get; set; }
        public string SuggestionText { get; set; }

    }
}
