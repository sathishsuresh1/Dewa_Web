// <copyright file="EFormRESTSoapClient.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Impl.EFormSvc
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.Requests;
    using DEWAXP.Foundation.Integration.Responses;
    using RestSharp;
    using DEWAXP.Foundation.DI;

    [Service(typeof(IEFormRESTServiceClient), Lifetime = Lifetime.Transient)]
    /// <summary>
    /// Defines the <see cref="EFormRESTSoapClient" />
    /// </summary>
    public class EFormRESTSoapClient : BaseDewaGateway, IEFormRESTServiceClient
    {

        /// <summary>
        /// The Query_Ework_DB
        /// </summary>
        /// <param name="SQL">The SQL<see cref="string"/></param>
        /// <returns>The <see cref="ServiceResponse{RestServiceResponse}"/></returns>
        public ServiceResponse<WebApiRestResponseEpass> Query_Ework_DB(string SQL)
        {
            try
            {
                RestRequest request = null;

                RestClient client = CreateRESTClient();

                request = new RestRequest("eform.api/queryeworkdb/", Method.POST);

                request = CreateHeader(request);

                //SQL
                var resquestBody = new
                {
                    SqlQuery = SQL,
                    DB_Username = DewaWebApi.DBUserName,
                    DB_Password = DewaWebApi.DBPassword
                };

                request.AddBody(resquestBody);

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    WebApiRestResponseEpass _eformResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<WebApiRestResponseEpass>(response.Content);

                    return new ServiceResponse<WebApiRestResponseEpass>(_eformResponse);
                }
                else
                {
                    return new ServiceResponse<WebApiRestResponseEpass>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {

                LogService.Fatal(ex, this);
                return new ServiceResponse<WebApiRestResponseEpass>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<WebApiRestResponseEpass> Query_Ework_DB(RestServiceRequest apiRequest)
        {
            try
            {
                RestRequest request = null;

                RestClient client = CreateRESTClient();

                request = new RestRequest("eform.api/queryeworkdb/", Method.POST);

                request = CreateHeader(request);


                apiRequest.DB_Username = DewaWebApi.DBUserName;
                apiRequest.DB_Password = DewaWebApi.DBPassword;

                request.AddBody(apiRequest);

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    WebApiRestResponseEpass _eformResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<WebApiRestResponseEpass>(response.Content);

                    return new ServiceResponse<WebApiRestResponseEpass>(_eformResponse);
                }
                else
                {
                    return new ServiceResponse<WebApiRestResponseEpass>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {

                LogService.Fatal(ex, this);
                return new ServiceResponse<WebApiRestResponseEpass>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        /// <summary>
        /// The SubmitNewForm
        /// </summary>
        /// <param name="formXml">The formXml<see cref="string"/></param>
        /// <param name="mapName">The mapName<see cref="string"/></param>
        /// <param name="mapAction">The mapAction<see cref="string"/></param>
        /// <returns>The <see cref="ServiceResponse{RestServiceResponse}"/></returns>
        public ServiceResponse<WebApiRestResponseEpass> SubmitNewForm(string formXml, string mapName, string mapAction)
        {
            try
            {
                RestRequest request = null;

                RestClient client = CreateRESTClient();

                request = new RestRequest("eform.api/submitform/", Method.POST);

                request = CreateHeader(request);

                //SQL
                var resquestBody = new
                {
                    Xml_FormValues = formXml,
                    MapName = mapName,
                    MapAction = mapAction,
                };

                request.AddBody(resquestBody);

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    WebApiRestResponseEpass _eformResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<WebApiRestResponseEpass>(response.Content);
                    _eformResponse.eFolderId = _eformResponse.ResponseData;
                    return new ServiceResponse<WebApiRestResponseEpass>(_eformResponse);
                }
                else
                {
                    return new ServiceResponse<WebApiRestResponseEpass>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<WebApiRestResponseEpass>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        /// <summary>
        /// The UpdateForm
        /// </summary>
        /// <param name="eFolderID">The eFolderID<see cref="string"/></param>
        /// <param name="formXml">The formXml<see cref="string"/></param>
        /// <param name="mapName">The mapName<see cref="string"/></param>
        /// <param name="mapAction">The mapAction<see cref="string"/></param>
        /// <returns>The <see cref="ServiceResponse{RestServiceResponse}"/></returns>
        public ServiceResponse<WebApiRestResponseEpass> UpdateForm(string eFolderID, string formXml, string mapName, string mapAction)
        {
            try
            {
                RestRequest request = null;

                RestClient client = CreateRESTClient();

                request = new RestRequest("eform.api/updateform/", Method.POST);

                request = CreateHeader(request);

                //SQL
                var resquestBody = new
                {
                    EFolderId = eFolderID,
                    Xml_FormValues = formXml,
                    MapName = mapName,
                    MapAction = mapAction,
                };

                request.AddBody(resquestBody);

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    WebApiRestResponseEpass _eformResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<WebApiRestResponseEpass>(response.Content);
                    _eformResponse.eFolderId = _eformResponse.ResponseData;
                    return new ServiceResponse<WebApiRestResponseEpass>(_eformResponse);
                }
                else
                {
                    return new ServiceResponse<WebApiRestResponseEpass>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {

                LogService.Fatal(ex, this);
                return new ServiceResponse<WebApiRestResponseEpass>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        /// <summary>
        /// The CreateHeader
        /// </summary>
        /// <param name="request">The request<see cref="RestRequest"/></param>
        /// <returns>The <see cref="RestRequest"/></returns>
        private RestRequest CreateHeader(RestRequest request)
        {
            request.AddHeader("password", DewaWebApi.RESTClientPassword);
            request.AddHeader("username", DewaWebApi.RESTClientUserName);
            request.RequestFormat = DataFormat.Json;

            return request;
        }

        /// <summary>
        /// The CreateRESTClient
        /// </summary>
        /// <returns>The <see cref="RestClient"/></returns>
        private RestClient CreateRESTClient()
        {
            return new RestClient(DewaWebApi.RESTClientURL);
        }
    }
}
