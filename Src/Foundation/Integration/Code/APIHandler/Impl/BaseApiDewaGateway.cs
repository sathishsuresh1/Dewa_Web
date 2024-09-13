using DEWAXP.Foundation.Integration.Impl;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    public class BaseApiDewaGateway : BaseDewaGateway
    {
        /// <summary>
        /// DewaApiExecute
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="methodname"></param>
        /// <param name="requestbody"></param>
        /// <param name="method"></param>
        /// <param name="Querystring_Array"></param>
        /// <returns></returns>
        protected internal IRestResponse  DewaApiExecute(string baseUrl, string methodname, object requestbody, Method method = Method.POST, Dictionary<string, string> Querystring_Array = null, Dictionary<string, string> requetHeader = null)
        {
            RestClient client = CreateClient(baseUrl);
            RestRequest request = new RestRequest(methodname, method);

            if(requetHeader!=null)
            {
                foreach (KeyValuePair<string,string> item in requetHeader)
                {
                    request.AddHeader(item.Key, item.Value);
                }
            }
            request = CreateAuthorizationHeader(request);
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

        protected internal async Task<IRestResponse> DewaApiExecuteAsync(string baseUrl, string methodname, object requestbody, Method method = Method.POST, Dictionary<string, string> Querystring_Array = null, Dictionary<string, string> requetHeader = null)
        {
            RestClient client = CreateClient(baseUrl);
            RestRequest request = new RestRequest(methodname, method);
            if (requetHeader != null)
            {
                foreach (KeyValuePair<string, string> item in requetHeader)
                {
                    request.AddHeader(item.Key, item.Value);
                }
            }
            request = CreateAuthorizationHeader(request);
            if (Querystring_Array != null)
            {
                request = CreateQueryString(request, Querystring_Array);
            }
            if (requestbody != null)
            {
                request.AddBody(requestbody);
            }
            IRestResponse  response = await client.ExecuteAsync(request);
            return response;
        }

        /// <summary>
        /// CreateClient
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private RestClient CreateClient(string baseUrl)
        {
            return new RestClient(baseUrl);
        }

        /// <summary>
        /// CreateHeader
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private RestRequest CreateAuthorizationHeader(RestRequest request)
        {
            request.AddHeader("Authorization", "Bearer " + OAuthToken.GetAccessToken());
            request.RequestFormat = DataFormat.Json;
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
