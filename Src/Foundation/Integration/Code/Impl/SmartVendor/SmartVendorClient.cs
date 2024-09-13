// <copyright file="SmartVendorClient.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Impl.SmartVendor
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Helpers;
    using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
    using DEWAXP.Foundation.Integration.Requests.SmartVendor;
    using DEWAXP.Foundation.Integration.Requests.SmartVendor.WorkPermit;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.SmartVendor.WorkPermit;
    using RestSharp;
    using System.Collections.Generic;
    using System.Linq;
    using DEWAXP.Foundation.DI;

    [Service(typeof(ISmartVendorClient), Lifetime = Lifetime.Transient)]
    /// <summary>
    /// Defines the <see cref="SmartVendorClient" />.
    /// </summary>
    public class SmartVendorClient : BaseDewaGateway, ISmartVendorClient
    {
        /// <summary>
        /// The SubmitWorkPermitPass.
        /// </summary>
        /// <param name="model">The model<see cref="GroupPemitPassRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{GroupPassPemitResponse}"/>.</returns>
        public ServiceResponse<GroupPassPemitResponse> SubmitWorkPermitPass(GroupPemitPassRequest model, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                model.grouppassinput.lang = language.Code();
                model.grouppassinput.vendorid = GetVendorId(segment);
                model.grouppassinput.appidentifier = segment.Identifier();
                model.grouppassinput.appversion = AppVersion;
                model.grouppassinput.mobileosversion = AppVersion;
                IRestResponse  response = SmartVendorSubmit(SmartVendorConstant.GROUPPERMITPASS, model, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    GroupPassPemitResponse _Response = CustomJsonConvertor.DeserializeObject<GroupPassPemitResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<GroupPassPemitResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<GroupPassPemitResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<GroupPassPemitResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<GroupPassPemitResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<UserDetailsResponse> GetICADetails(UserDetailsRequest model, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                model.inquiryinput.lang = language.Code();
                model.inquiryinput.vendorid = GetVendorId(segment);
                model.inquiryinput.appidentifier = segment.Identifier();
                model.inquiryinput.appversion = AppVersion;
                model.inquiryinput.mobileosversion = AppVersion;
                IRestResponse  response = SmartVendorSubmit(SmartVendorConstant.ICAUSERDETAILS, model, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UserDetailsResponse _Response = CustomJsonConvertor.DeserializeObject<UserDetailsResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<UserDetailsResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<UserDetailsResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<UserDetailsResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<UserDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<TradeLicenseResponse> GetTradeLicenseDetails(TradeLicenseRequest model, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                model.tradedetailsinput.lang = language.Code();
                model.tradedetailsinput.vendorid = GetVendorId(segment);
                model.tradedetailsinput.appidentifier = segment.Identifier();
                model.tradedetailsinput.appversion = AppVersion;
                model.tradedetailsinput.mobileosversion = AppVersion;
                IRestResponse  response = SmartVendorSubmit(SmartVendorConstant.GETTRADELICENSEDETAILS, model, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TradeLicenseResponse _Response = CustomJsonConvertor.DeserializeObject<TradeLicenseResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000") && !string.IsNullOrWhiteSpace(_Response.Tradeexistflag) && (_Response.Tradeexistflag.Equals("X")))
                    {
                        return new ServiceResponse<TradeLicenseResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<TradeLicenseResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<TradeLicenseResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<TradeLicenseResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        /// <summary>
        /// The SmartVendorSubmit.
        /// </summary>
        /// <param name="methodname">The methodname<see cref="string"/>.</param>
        /// <param name="requestbody">The requestbody<see cref="object"/>.</param>
        /// <param name="method">The method<see cref="Method"/>.</param>
        /// <param name="Querystring_Array">The Querystring_Array<see cref="Dictionary{string, string}"/>.</param>
        /// <returns>The <see cref="IRestResponse"/>.</returns>
        public IRestResponse  SmartVendorSubmit(string methodname, object requestbody, Method method = Method.POST, Dictionary<string, string> Querystring_Array = null)
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
            return new RestClient(DEWASMARTVENDORURL);
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
