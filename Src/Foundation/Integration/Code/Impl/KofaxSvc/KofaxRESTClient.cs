// <copyright file="KofaxRESTServiceClient.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Impl.KofaxSvc
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.KofaxRest;
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using System.Net;
    using System.Web.Configuration;
    using DEWAXP.Foundation.DI;

    [Service(typeof(IKofaxRESTServiceClient), Lifetime = Lifetime.Transient)]
    /// <summary>
    /// Defines the <see cref="KofaxRESTClient" />.
    /// </summary>
    public class KofaxRESTClient : BaseDewaGateway, IKofaxRESTServiceClient
    {
        /// <summary>
        /// Defines the RESTClientURL.
        /// </summary>
        internal string RESTClientURL = Sitecore.Context.Database.GetItem(SitecoreItemId.KofaxconfigItem) != null ? Sitecore.Context.Database.GetItem(SitecoreItemId.KofaxconfigItem)["URL"].ToString() : string.Empty;

        /// <summary>
        /// Defines the RESTClientUserName.
        /// </summary>
        internal string RESTClientUserName = Sitecore.Context.Database.GetItem(SitecoreItemId.KofaxconfigItem) != null ? Sitecore.Context.Database.GetItem(SitecoreItemId.KofaxconfigItem)["Username"].ToString() : string.Empty;

        /// <summary>
        /// Defines the RESTClientPassword.
        /// </summary>
        internal string RESTClientPassword = Sitecore.Context.Database.GetItem(SitecoreItemId.KofaxconfigItem) != null ? Sitecore.Context.Database.GetItem(SitecoreItemId.KofaxconfigItem)["Password"].ToString() : string.Empty;

        /// <summary>
        /// The SubmitKofax.
        /// </summary>
        /// <param name="methodname">The methodname<see cref="string"/>.</param>
        /// <param name="modelJson">The modelJson<see cref="string"/>.</param>
        /// <returns>.</returns>
        public ServiceResponse<KofaxRestResponse> SubmitKofax(string methodname, string modelJson)
        {
            try
            {
                RestClient client = GetRestClient(methodname);

                JToken val = JValue.Parse(modelJson);
                RestRequest request = GetRestRequest(val);

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    KofaxRestResponse resp = Newtonsoft.Json.JsonConvert.DeserializeObject<KofaxRestResponse>(response.Content);

                    return new ServiceResponse<KofaxRestResponse>(resp);
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<KofaxRestResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<KofaxRestResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        /// <summary>
        /// The GetRestClient.
        /// </summary>
        /// <param name="resource">The resource<see cref="string"/>.</param>
        /// <returns>The <see cref="RestClient"/>.</returns>
        private RestClient GetRestClient(string resource)
        {
            RestClient client = new RestClient(RESTClientURL + resource);

            return client;
        }

        /// <summary>
        /// The GetRestRequest.
        /// </summary>
        /// <param name="json">The json<see cref="JToken"/>.</param>
        /// <returns>The <see cref="RestRequest"/>.</returns>
        private RestRequest GetRestRequest(JToken json)
        {
            RestRequest request = new RestRequest()
            {
                Method=Method.POST,
                RequestFormat = DataFormat.Json
            };
            const string jsontype = "application/json";
            //request.Parameters.Clear();
            request.AlwaysMultipartFormData = false;

            request.AddHeader("Accept", jsontype);
            request.AddHeader("Content-Type", jsontype);
            request.AddHeader("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(RESTClientUserName + ":" + RESTClientPassword)));
            request.AddParameter(jsontype, json, ParameterType.RequestBody);
            //request.AddJsonBody(json);

            return request;
        }


        // Epass Kofax
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqId">e.g. 12</param>
        /// <param name="status">Dept Approved or Security Approved</param>
        /// <param name="approverType">Dept or Sec</param>
        /// <param name="remarks">needed only in case of Rejection</param>
        /// <returns></returns>
        public ServiceResponse<KofaxRestResponse> UpdateApproval(string reqId, string status, string approverType, string currentUserEmail, string remarks = "")
        {
            try
            {
                RestClient client = GetRestClient("UpdateApproval.robot");

                RestRequest request = GetRestRequest(GetApproveJson(reqId, status, approverType, currentUserEmail, remarks));

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    KofaxRestResponse resp = Newtonsoft.Json.JsonConvert.DeserializeObject<KofaxRestResponse>(response.Content);

                    return new ServiceResponse<KofaxRestResponse>(resp);
                }
                else
                {
                    return new ServiceResponse<KofaxRestResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<KofaxRestResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<KofaxRestResponse> GetMyPasses(string currentUserEmail)
        {
            try
            {
                RestClient client = GetRestClient("GetRequestsByMe.robot");

                RestRequest request = GetRestRequest(GetMyPassesJson(currentUserEmail));

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    KofaxRestResponse resp = Newtonsoft.Json.JsonConvert.DeserializeObject<KofaxRestResponse>(response.Content);

                    return new ServiceResponse<KofaxRestResponse>(resp);
                }
                else
                {
                    return new ServiceResponse<KofaxRestResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<KofaxRestResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        public ServiceResponse<KofaxRestResponse> GetDepartmentPendingPasses()
        {
            try
            {
                RestClient client = GetRestClient("GetPendingApproval.robot");

                RestRequest request = GetRestRequest(GetPendingPassesRequestJson("Initiated"));

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    KofaxRestResponse resp = Newtonsoft.Json.JsonConvert.DeserializeObject<KofaxRestResponse>(response.Content);

                    return new ServiceResponse<KofaxRestResponse>(resp);
                }
                else
                {
                    return new ServiceResponse<KofaxRestResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<KofaxRestResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        public ServiceResponse<KofaxRestResponse> GetSecurityPendingPasses()
        {
            try
            {
                RestClient client = GetRestClient("GetPendingApproval.robot");

                RestRequest request = GetRestRequest(GetPendingPassesRequestJson("Dept Approved"));

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    KofaxRestResponse resp = Newtonsoft.Json.JsonConvert.DeserializeObject<KofaxRestResponse>(response.Content);

                    return new ServiceResponse<KofaxRestResponse>(resp);
                }
                else
                {
                    return new ServiceResponse<KofaxRestResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<KofaxRestResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        public ServiceResponse<KofaxRestResponse> CreateSubPass(string modelJson)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                RestClient client = GetRestClient("RequestSubmission.robot");

                JToken val = JValue.Parse(modelJson);
                RestRequest request = GetRestRequest(val);

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    KofaxRestResponse resp = Newtonsoft.Json.JsonConvert.DeserializeObject<KofaxRestResponse>(response.Content);

                    return new ServiceResponse<KofaxRestResponse>(resp);
                }
                else
                {
                    return new ServiceResponse<KofaxRestResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<KofaxRestResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status">Dept Approved or Initiated</param>
        /// <returns></returns>
        private string GetPendingPassesRequestJson(string status)
        {
            return "{\"parameters\":[{\"variableName\":\"t000121PendingApproval\",\"attribute\":[{\"type\":\"text\",\"name\":\"Status\",\"value\":\"" + status + "\"}]}]}";
        }
        private string GetMyPassesJson(string currentUserEmail)
        {
            return "{\"parameters\":[{\"variableName\":\"t000121CurrentUser\",\"attribute\":[{\"type\":\"text\",\"name\":\"Email\",\"value\":\"" + currentUserEmail + "\"},{\"type\":\"text\",\"name\":\"Name\",\"value\":\"" + currentUserEmail + "\"}]}]}";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqId">e.g. 12</param>
        /// <param name="status">Dept Approved or Security Approved</param>
        /// <param name="approverType">Dept or Sec</param>
        /// <returns></returns>
        private string GetApproveJson(string reqId, string status, string approverType, string approvedByEmail, string remarks = "")
        {
            return "{\"parameters\":[{\"variableName\":\"t000121UpdateStatus\",\"attribute\":[{\"type\":\"integer\",\"name\":\"ReqID\",\"value\":\"" + reqId + "\"},{\"type\":\"text\",\"name\":\"Status\",\"value\":\"" + status + "\"},{\"type\":\"text\",\"name\":\"ApprovedByEmail\",\"value\":\"" + approvedByEmail + "\"},{\"type\":\"text\",\"name\":\"ApproverType\",\"value\":\"" + approverType + "\"},{\"type\":\"text\",\"name\":\"Remarks\",\"value\":\"" + remarks + "\"}]}]}";
        }
    }
}
