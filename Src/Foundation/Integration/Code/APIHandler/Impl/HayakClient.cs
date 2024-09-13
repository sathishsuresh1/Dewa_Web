using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Hayak;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using RestSharp;
using System;
using System.Configuration;
using System.Net;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IHayakClient), Lifetime = Lifetime.Transient)]
    public class HayakClient : IHayakClient
    {
        public ServiceResponse<string> GetContextId(string req)
        {
            try
            {
                RestClient client = new RestClient(ConfigurationManager.AppSettings["chatInitUrl"]);

                RestRequest request = new RestRequest(Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
                const string jsontype = "application/json";
                request.Parameters.Clear();
                request.AlwaysMultipartFormData = false;

                request.AddHeader("Accept", jsontype);
                request.AddHeader("Content-Type", jsontype);
                request.AddQueryParameter("journeyElement", "ChatService");
                request.AddParameter(jsontype, Newtonsoft.Json.Linq.JValue.Parse(req), ParameterType.RequestBody);

                //Remove the below line in production
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    InitResponseRoot robj = CustomJsonConvertor.DeserializeObject<InitResponseRoot>(response.Content);
                    if (robj != null && !string.IsNullOrEmpty(robj.data?.contextId))
                    {
                        return new ServiceResponse<string>(robj.data.contextId);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return new ServiceResponse<string>(string.Empty, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}