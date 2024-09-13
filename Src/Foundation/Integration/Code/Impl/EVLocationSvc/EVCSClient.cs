// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Impl.EVLocationSvc
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.Helpers;
    using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.EVLocationSvc;
    using RestSharp;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Configuration;
    using DEWAXP.Foundation.DI;

    [Service(typeof(IEVCSClient), Lifetime = Lifetime.Transient)]
    /// <summary>
    /// Defines the <see cref="SmartConsultantClient" />.
    /// </summary>
    public class EVCSClient : BaseDewaGateway, IEVCSClient
    {
        /// <summary>
        /// Gets or sets the DEWAEVLOCATIONSURL.
        /// </summary>
        internal string DEWAEVLOCATIONSURL { get; set; } = WebConfigurationManager.AppSettings["DEWAEVLOCATIONS_URL"];

        /// <summary>
        /// The SmartConsultantSubmit.
        /// </summary>
        /// <returns>The <see cref="ServiceResponse{Devicelist}"/>.</returns>
        public ServiceResponse<Devicelist> GetLocations(string data= EVCSConstant.BASICDATA,string HubeleonID="")
        {
            try
            {
                Dictionary<string, string> querystringValue = null;
                if (data.Equals(EVCSConstant.BASICDATA))
                {
                    querystringValue = new Dictionary<string, string>
                {
                    {EVCSConstant.DATA, EVCSConstant.BASICDATA }
                };
                }
                else if (data.Equals(EVCSConstant.FULLDATA))
                {
                    querystringValue = new Dictionary<string, string>
                {
                    {EVCSConstant.DATA, EVCSConstant.FULLDATA },
                    {EVCSConstant.HubeleonID, HubeleonID }
                };
                }
                IRestResponse  response = Submit(EVCSConstant.DEVICELIST, null, Method.GET, querystringValue);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Devicelist _Response = CustomJsonConvertor.DeserializeObject<Devicelist>(response.Content);
                    if (_Response != null && _Response.Devices != null && _Response.Devices.Length > 0)
                    {
                        return new ServiceResponse<Devicelist>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<Devicelist>(null, false, "No Response");
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<Devicelist>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<Devicelist>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        /// <summary>
        /// The SmartConsultantSubmit.
        /// </summary>
        /// <param name="methodname">The methodname<see cref="string"/>.</param>
        /// <param name="requestbody">The requestbody<see cref="object"/>.</param>
        /// <param name="method">The method<see cref="Method"/>.</param>
        /// <param name="Querystring_Array">The Querystring_Array<see cref="Dictionary{string, string}"/>.</param>
        /// <returns>The <see cref="RestResponse"/>.</returns>
        public IRestResponse  Submit(string methodname, object requestbody, Method method = Method.POST, Dictionary<string, string> Querystring_Array = null)
        {
            RestRequest request = null;
            RestClient client = CreateClient();
            request = new RestRequest(methodname, method);
            request = CreateHeader(request);
            if (Querystring_Array != null)
            {
                request = CreateQueryString(request, Querystring_Array);
            }
            if (requestbody != null)
            {
                request.AddBody(requestbody);
            }
            IRestResponse  response = client.Execute(request);
            return response;
        }

        /// <summary>
        /// The CreateClient.
        /// </summary>
        /// <returns>The <see cref="RestClient"/>.</returns>
        private RestClient CreateClient()
        {
            return new RestClient(DEWAEVLOCATIONSURL);
        }

        /// <summary>
        /// The CreateHeader.
        /// </summary>
        /// <param name="request">The request<see cref="RestRequest"/>.</param>
        /// <returns>The <see cref="RestRequest"/>.</returns>
        private RestRequest CreateHeader(RestRequest request)
        {
            request.AddHeader("Authorization", "Bearer " + OAuthToken.GetAccessToken());
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json");
            return request;
        }

        /// <summary>
        /// The CreateQueryString.
        /// </summary>
        /// <param name="request">The request<see cref="RestRequest"/>.</param>
        /// <param name="Querystring_Array">The Querystring_Array<see cref="Dictionary{string, string}"/>.</param>
        /// <returns>The <see cref="RestRequest"/>.</returns>
        private RestRequest CreateQueryString(RestRequest request, Dictionary<string, string> Querystring_Array)
        {
            Querystring_Array.ToList().ForEach
            (
                pair =>
                {
                    request.AddQueryParameter(pair.Key, pair.Value);
                }
            );
            return request;
        }
    }
}
