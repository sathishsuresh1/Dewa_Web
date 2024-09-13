using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Alexa;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IAlexaClient), Lifetime = Lifetime.Transient)]
    public class Alexaclient : IAlexaClient
    {
        public ServiceResponse<Login_Res> ValidateCredential(string username, string password, string lang)
        {
            Login_Res res;
            try
            {
                var client = new RestClient(ConfigurationManager.AppSettings[ConfigKeys.ALEXA_LOGIN_URL])
                {
                    Timeout = -1
                };
                var request = new RestRequest(Method.POST);
                request.AddHeader("apikey", ConfigurationManager.AppSettings[ConfigKeys.ALEXA_API_KEY]);
                request.AddHeader("Content-Type", "application/json");
                var apiRequest = new
                {
                    userid = username.Trim(),
                    password = password.Trim(),
                    lang = lang
                };
                request.RequestFormat = DataFormat.Json;
                request.AddBody(apiRequest);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    res = JsonConvert.DeserializeObject<Login_Res>(response.Content);
                    if (res.developerStatus.Equals("000"))
                    {
                        return new ServiceResponse<Login_Res>(res);
                    }
                    else
                    {
                        return new ServiceResponse<Login_Res>(null, false, res.developerMessage);
                    }
                }
                else
                {
                    return new ServiceResponse<Login_Res>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return new ServiceResponse<Login_Res>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}